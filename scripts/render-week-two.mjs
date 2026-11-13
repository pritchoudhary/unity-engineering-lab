// Run from the repository root with Node 22, installed web dependencies and ffmpeg on PATH.
import sharp from 'sharp';
import {spawn} from 'node:child_process';
import {once} from 'node:events';
import {mkdir} from 'node:fs/promises';
import {drive} from '../lib/half-car.mjs';
import {rail,initialRail,point,offsets,patrol} from '../lib/rail.mjs';
const fps=24, width=960,height=540,dir='docs/video-previews';
const ink='#25372d',orange='#a53616',green='#297054';
const label=(x,y,s,size=22,color=ink)=>'<text x="'+x+'" y="'+y+'" font-family="Helvetica,Arial,sans-serif" font-size="'+size+'" fill="'+color+'">'+s+'</text>';
const line=(x,y,X,Y,c,w=3)=>'<line x1="'+x+'" y1="'+y+'" x2="'+X+'" y2="'+Y+'" stroke="'+c+'" stroke-width="'+w+'"/>';
const dot=(x,y,r,c)=>'<circle cx="'+x+'" cy="'+y+'" r="'+r+'" fill="'+c+'"/>';
const poly=(coords,c,w)=>'<polyline points="'+coords.map(p=>p.join(',')).join(' ')+'" fill="none" stroke="'+c+'" stroke-width="'+w+'"/>';
const svg=(body)=>'<svg xmlns="http://www.w3.org/2000/svg" width="960" height="540"><rect width="960" height="540" fill="#f7f6f0"/>'+body+'</svg>';
const modes=['brake','rough','dip'], runs=modes.map(m=>[drive(m),drive(m,22000,2400)]);
function vehicle(i,t){
 const titles=['Does stronger damping reduce brake dive?','What happens over three road strips?','Can the tires stay loaded in a dip?'];
 const names=['Peak nose pitch','Body acceleration RMS','Time either tire is unloaded'],units=['°',' m/s²',' s'];
 let s=label(36,40,'VEHICLE SUSPENSION  /  '+(i+1)+' OF 3',17)+label(36,91,titles[i],36);
 for(let j=0;j<2;j++){const run=runs[i][j],f=run.frames[Math.round(t*60)],x=110+j*455,c=j?orange:green;
  s+=label(x-45,146,'Damper: '+(j?'2,400':'1,800')+' N·s/m',24,c);
  const rear=225-370*(f.z-1.2*f.p),front=225-370*(f.z+1.2*f.p);
  s+=line(x,rear,x+240,front,c,12);
  for(const [xx,z,r,b] of [[x,f.rear,f.roadR,rear],[x+240,f.front,f.roadF,front]]){
   const y=287-370*z;s+=line(xx,b,xx,y,c,3)+dot(xx,y,13,ink)+line(xx-39,300-370*r,xx+39,300-370*r,ink,3);
  }
  const v=[run.pitch,run.rms,run.air][i];s+=label(x-45,375,names[i],19)+label(x-45,415,v.toFixed(3)+units[i],34,c);
 }
 return svg(s+label(36,467,['Less dive on the same braking pulse. Try keeping that setting for the road test.','The stiffer damper setting transmits a larger body acceleration.','The higher setting leaves a tire unloaded for longer in this model.'][i],21)+label(36,511,'Model animation • 22,000 N/m springs • same input on both sides • motion exaggerated',16));
}
const curve=rail(initialRail),world=a=>[105+68*a[0],310-35*a[1]],curvePts=curve.samples.map(p=>world(p.pos));
function spline(i,t){
 const names=['Camera rail: compare timing dots','Road: widen the ribbon, keep the centerline','Patrol: wait longer, move faster'];
 let s=label(36,40,'MODERN SPLINE  /  '+(i+1)+' OF 3',17)+label(36,91,names[i],36);
 if(i===1){for(const [wid,c] of [[3,orange],[1.5,green]])for(const side of [0,1])s+=poly(curve.samples.map(p=>world(offsets(initialRail,p.t,wid)[side])),c,3);}
 s+=poly(curvePts,ink,3);
 if(i===0){for(let j=0;j<=8;j++){for(const [tt,c,dy] of [[j/8,green,-10],[curve.at(j/8),orange,10]]){const [x,y]=world(point(initialRail,tt));s+=dot(x,y+dy,4,c)}}}
 for(let j=0;j<2;j++){const param=i===2?curve.at(patrol(t,j?1:.5).fraction):i===0?(j?curve.at(t/4):t/4):t/4;
 const [x,y]=world(i===1?offsets(initialRail,param,j?3:1.5)[0]:point(initialRail,param));s+=dot(x,y+(i===1?0:j?10:-10),7,j?orange:green);}
 return svg(s+label(36,378,['Green: equal parameter steps','Green: 1.5 m edge separation','Green: 0.5 s wait → 7.101 m/s while moving'][i],23,green)+label(36,418,['Orange: equal distance steps','Orange: 3.0 m edge separation','Orange: 1.0 s wait → 8.284 m/s while moving'][i],23,orange)+label(36,469,['Same path and trip time. Equal changes in the parameter do not mean equal distances.','Width is measured normal to the path in world space. Centerline length stays 12.427 m.','Both guards finish the out-and-back trip in four seconds. Watch the far endpoint.'][i],20)+label(36,511,'Model animation • one cubic Bézier • original control points • diagram not to scale',16));
}
await mkdir(dir,{recursive:true});
for(const [name,frame] of [['vehicle-suspension',vehicle],['spline-editor',spline]]){
 const ff=spawn('ffmpeg',['-hide_banner','-loglevel','error','-y','-f','rawvideo','-pixel_format','rgb24','-video_size',width+'x'+height,'-framerate',String(fps),'-i','pipe:0','-an','-c:v','libx264','-crf','20','-pix_fmt','yuv420p','-movflags','+faststart',dir+'/'+name+'.mp4'],{stdio:['pipe','ignore','inherit']});
 const done=once(ff,'close');for(let n=0;n<12*fps;n++){const pixels=await sharp(Buffer.from(frame(Math.floor(n/(4*fps)),n/fps%4))).removeAlpha().raw().toBuffer();if(!ff.stdin.write(pixels))await once(ff.stdin,'drain');}
 ff.stdin.end();const [code]=await done;if(code!==0)throw Error('ffmpeg exit '+code);console.log('Rendered '+name);
}
