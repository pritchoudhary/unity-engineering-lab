// Original bounded teaching models. No PhysX, NavMesh or Cinemachine dependency.
const clamp=(x,a,b)=>Math.max(a,Math.min(b,x));
// Bounded coordinates; keep the same operation order as the native double model.
const length=(x,y)=>Math.sqrt(x*x+y*y);
const cap=(x,y,max)=>{const l=length(x,y);return l>max?[x*max/l,y*max/l]:[x,y];};
const dot=(x,y,r=.18,label='',tone='agent')=>({x,y,r,label,tone});
const box=(x,y,w,h)=>({x,y,w,h});
const line=(x1,y1,x2,y2,tone='agent')=>({x1,y1,x2,y2,tone});
export const duration=12;
export function validateInput(mode,p){
 if(!Number.isInteger(mode)||mode<0||mode>2)throw new RangeError('Application must be 0, 1 or 2');
 for(const [k,v] of Object.entries(p))if(typeof v!=='number'||!Number.isFinite(v))throw new RangeError('Finite parameter required: '+k);
}
export function inspectRun(run,t){if(!Number.isFinite(t))throw new RangeError('Finite time required');return run.frames[Math.min(run.frames.length-1,Math.max(0,Math.round(clamp(t,0,duration)*30)))];}
// All agents read the previous frame. Stable IDs and deterministic input remove update-order noise.
export function runFlock(mode,options={}){
 const p={count:24,separation:1,alignment:1,cohesion:.45,...options};validateInput(mode,p);
 if(p.count<4||p.count>64||!Number.isInteger(p.count)||p.separation<0||p.separation>5||p.alignment<0||p.alignment>4||p.cohesion<0||p.cohesion>2)throw new RangeError('Flock bounds');
 let agents=Array.from({length:p.count},(_,i)=>({x:2+(i%6)*.48,y:3+Math.floor(i/6)*.6,vx:.3+Math.cos(i*2.4)*.3,vy:Math.sin(i*2.4)*.5,id:i}));
 const frames=[];let pairs=0,safe=0,passed=0,totalHeading=0,totalThreat=0,maxSpeed=0,maxAcceleration=0;
 for(let step=0;step<=360;step++){
  const t=step/30,threat={x:10+2*Math.sin(t*.6),y:6+2*Math.cos(t*.7)};
  let near=0,heading=0,threatened=0;
  for(let i=0;i<agents.length;i++){let a=agents[i];maxSpeed=Math.max(maxSpeed,length(a.vx,a.vy));heading+=a.vx/(length(a.vx,a.vy)||1);if(length(a.x-threat.x,a.y-threat.y)<2)threatened++;for(let j=0;j<i;j++)if(length(a.x-agents[j].x,a.y-agents[j].y)<.36)near++;}
  pairs+=near;totalHeading+=heading/agents.length;totalThreat+=threatened;
  if(near===0)safe++;
  const dots=agents.map(a=>dot(a.x,a.y,.18,'Agent '+a.id));if(mode===1)dots.push(dot(threat.x,threat.y,.65,'Predator','danger'));
  const obstacles=mode===2?[box(10,0,.35,4.5),box(10,7.5,.35,4.5)]:[];
  frames.push({t,dots,boxes:obstacles,lines:[],state:near?`${near} overlapping pairs`:'No overlaps at this sample'});
  if(step===360)break;
  agents=agents.map(a=>{
   let sx=0,sy=0,avx=0,avy=0,cx=0,cy=0,n=0;
   for(const b of agents){if(a.id===b.id)continue;let dx=a.x-b.x,dy=a.y-b.y,d=length(dx,dy);if(d<3){n++;avx+=b.vx;avy+=b.vy;cx+=b.x;cy+=b.y;}if(d<1.2){const d2=Math.max(d*d,.02);sx+=dx/d2;sy+=dy/d2;}}
   let ax=sx*p.separation,ay=sy*p.separation;
   if(n){ax+=(avx/n-a.vx)*p.alignment+(cx/n-a.x)*p.cohesion;ay+=(avy/n-a.vy)*p.alignment+(cy/n-a.y)*p.cohesion;}
   if(mode===0){ax+=(2-a.vx)*.8;ay+=Math.sin(t*1.7+a.id*.15)*.45;}
   if(mode===1){ax+=(10-a.x)*.35;ay+=(6-a.y)*.35;let dx=a.x-threat.x,dy=a.y-threat.y,d=length(dx,dy);if(d<4){const v=5*(4-d)/Math.max(d,.1);ax+=dx*v;ay+=dy*v;}}
   if(mode===2){const gx=a.x<10.5?10.8:18,gy=6;let dx=gx-a.x,dy=gy-a.y,d=length(dx,dy)||1;ax+=(dx/d*1.8-a.vx)*1.6;ay+=(dy/d*1.8-a.vy)*1.6;}
   [ax,ay]=cap(ax,ay,4);maxAcceleration=Math.max(maxAcceleration,length(ax,ay));let [vx,vy]=cap(a.vx+ax/30,a.vy+ay/30,2.5);let x=a.x+vx/30,y=a.y+vy/30;
   if(mode===2&&x>=9.82&&x<=10.53&&(y<4.68||y>7.32)){x=a.x;vx=0;}
   if(x<.2||x>19.8){x=clamp(x,.2,19.8);vx=0;}if(y<.2||y>11.8){y=clamp(y,.2,11.8);vy=0;}
   return {...a,x,y,vx,vy};
  });
 }
 const meanX=agents.reduce((s,a)=>s+a.x,0)/agents.length,meanY=agents.reduce((s,a)=>s+a.y,0)/agents.length;
 passed=agents.filter(a=>a.x>11).length;
 return {frames,metrics:{overlaps:pairs/361,heading:totalHeading/361,threat:totalThreat/(361*p.count)*100,passed:passed/p.count*100,spread:Math.sqrt(agents.reduce((s,a)=>s+(a.x-meanX)**2+(a.y-meanY)**2,0)/agents.length),safe:safe/361*100,maxSpeed,maxAcceleration},parameters:p};
}
export function projectPoint(x,y,z,yaw,distance,fov){
 const co=Math.cos(yaw),si=Math.sin(yaw),depth=distance+z*co-x*si;
 if(depth<=.05)return {x:10,y:6,visible:false,depth};
 const half=depth*Math.tan(fov*Math.PI/360),nx=(x*co+z*si)/half,ny=y/(half/(16/9));
 return {x:10+8*nx,y:6-4.5*ny,visible:Math.abs(nx)<=1&&Math.abs(ny)<=1,depth};
}
export function runCamera(mode,options={}){
 const p={deadZone:0,lookAhead:0,fov:35,distance:6,...options};validateInput(mode,p);
 if(p.deadZone<0||p.deadZone>5||p.lookAhead<0||p.lookAhead>2||p.fov<20||p.fov>100||p.distance<3||p.distance>15)throw new RangeError('Camera bounds');
 let cx=0,cy=0,travel=0,visible=0,preview=0,margin=1;
 const frames=[];const specimens=[[-3,0,-1],[3,0,1],[-1,1.8,0],[1,-1.8,0],[0,0,2.5]];
 for(let step=0;step<=360;step++){
  const t=step/30;let dots=[],lines=[],allVisible=true;
  if(mode<2){
   const x=mode===0?2*t+1.2*Math.sin(t*3):6*t,y=mode===0?1.5*Math.max(0,Math.sin(t*1.4)):0,vx=mode===0?2+3.6*Math.cos(t*3):6;
   if(step===0){cx=x;cy=y;}else{let aim=x;if(mode===0)aim=x>cx+p.deadZone?x-p.deadZone:x<cx-p.deadZone?x+p.deadZone:cx;else aim=x+vx*p.lookAhead;let nx=cx+(aim-cx)*(1-Math.exp(-1/(30*.12))),ny=cy+(y-cy)*(1-Math.exp(-1/(30*.2)));travel+=length(nx-cx,ny-cy);cx=nx;cy=ny;}
   const half=mode===0?6:8,h=half*9/16,px=10+(x-cx)/half*8,py=6-(y-cy)/h*4.5;allVisible=px>=2&&px<=18&&py>=1.5&&py<=10.5;
   dots=[dot(clamp(px,.5,19.5),clamp(py,.5,11.5),.25,allVisible?'Player':'Player outside viewport',allVisible?'agent':'danger')];
   if(mode===1){const hx=10+(x+9-cx)/half*8;dots.push(dot(clamp(hx,.5,19.5),6,.2,'Hazard 9 m ahead',hx>18?'danger':'target'));preview+=Math.max(0,(cx+half-x)/6);}
   else lines=[line(10-p.deadZone/half*8,1.5,10-p.deadZone/half*8,10.5,'target'),line(10+p.deadZone/half*8,1.5,10+p.deadZone/half*8,10.5,'target')];
  }else{
   const points=specimens.map(([x,y,z])=>projectPoint(x,y,z,t*Math.PI/6,p.distance,p.fov));allVisible=points.every(q=>q.visible);
   margin=Math.min(margin,...points.map(q=>q.depth<=.05?-1:Math.min((q.x-2)/8,(18-q.x)/8,(q.y-1.5)/4.5,(10.5-q.y)/4.5)));
   dots=points.map((q,i)=>dot(clamp(q.x,.5,19.5),clamp(q.y,.5,11.5),.25,'Specimen '+(i+1),q.visible?'agent':'danger'));
  }
  if(allVisible)visible++;
  frames.push({t,dots,boxes:[box(2,1.5,16,9)],lines,state:allVisible?'All subjects in frame':'Subject outside viewport'});
 }
 return {frames,metrics:{travel,visible:visible/361*100,preview:preview/361,margin:margin*100},parameters:p};
}
// Earliest contact on a finite segment, including a start already inside the inflated disk.
export function segmentDisk(ax,ay,bx,by,cx,cy,r){
 const dx=bx-ax,dy=by-ay,ox=ax-cx,oy=ay-cy,A=dx*dx+dy*dy,C=ox*ox+oy*oy-r*r;
 if(C<=0)return 0;if(A<1e-15)return null;
 const B=2*(ox*dx+oy*dy),D=B*B-4*A*C;if(D<0)return null;
 const t=(-B-Math.sqrt(D))/(2*A);return t>=0&&t<=1?t:null;
}
export function clearSegment(a,b,obstacles,radius=0){return obstacles.every(o=>segmentDisk(a.x,a.y,b.x,b.y,o.x,o.y,o.r+radius)===null);}
export function runVisibility(mode,options={}){
 const p={clearance:0,memory:0,requireLOS:0,reach:4,...options};validateInput(mode,p);
 if(p.clearance<0||p.clearance>1||p.memory<0||p.memory>3||![0,1].includes(p.requireLOS)||p.reach<2||p.reach>8)throw new RangeError('Visibility bounds');
 const frames=[];let clipping=0,minArm=6,transitions=0,stale=0,staleRun=0,maxStale=0,last=false,lastSeen=-100,wrong=0,available=0;
 for(let step=0;step<=360;step++){
  const t=step/30;let dots=[],lines=[],obstacles=[],state='';
  if(mode===0){
   const a={x:10,y:6},b={x:10+6*Math.cos(t*.6),y:6+4*Math.sin(t*.6)};obstacles=[{x:13,y:6,r:.9},{x:7,y:8,r:.8}];
   let hit=1;for(const o of obstacles){let h=segmentDisk(a.x,a.y,b.x,b.y,o.x,o.y,o.r+p.clearance);if(h!==null)hit=Math.min(hit,h);}
   const factor=Math.max(0,hit-1e-4),camera={x:a.x+(b.x-a.x)*factor,y:a.y+(b.y-a.y)*factor};
   const overlap=obstacles.some(o=>length(camera.x-o.x,camera.y-o.y)<o.r+.3-1e-6);if(overlap)clipping++;
   minArm=Math.min(minArm,length(camera.x-a.x,camera.y-a.y));dots=[dot(a.x,a.y,.25,'Subject','target'),dot(b.x,b.y,.12,'Desired camera','reference'),dot(camera.x,camera.y,.3,'Camera',overlap?'danger':'agent')];lines=[line(a.x,a.y,b.x,b.y,'reference'),line(a.x,a.y,camera.x,camera.y)];state=overlap?'Camera body clips an obstacle':hit<1?'Arm shortened; camera body clear':'Desired arm clear';
  }
  if(mode===1){
   const a={x:2,y:6},b={x:17,y:6+3*Math.sin(t*1.3)};obstacles=[{x:9,y:6,r:.9},{x:12,y:8,r:.65}];
   const raw=clearSegment(a,b,obstacles);if(raw)lastSeen=t;const alert=raw||t-lastSeen<=p.memory+1e-9;
   if(step&&last!==alert)transitions++;last=alert;
   if(alert&&!raw){stale++;staleRun+=1/30;maxStale=Math.max(maxStale,staleRun);}else staleRun=0;
   dots=[dot(a.x,a.y,.3,'Observer','target'),dot(b.x,b.y,.3,raw?'Target visible':'Target occluded',raw?'agent':'danger')];lines=[line(a.x,a.y,b.x,b.y,raw?'agent':'reference')];state=(raw?'Line of sight clear':'Line of sight blocked')+' / '+(alert?'Alert':'No contact');
  }
  if(mode===2){
   const a={x:4+8*t/12,y:6+1.2*Math.sin(t*.5)},props=[{x:7,y:6},{x:8,y:9},{x:12,y:6},{x:15,y:9}];obstacles=[{x:8,y:6,r:.8},{x:12,y:8,r:.6}];
   const candidates=props.map((b,id)=>({...b,id,d:length(b.x-a.x,b.y-a.y),clear:clearSegment(a,b,obstacles)})).filter(b=>b.d<=p.reach&&b.x>=a.x&&(!p.requireLOS||b.clear)).sort((a,b)=>a.d-b.d||a.id-b.id),chosen=candidates[0];
   if(chosen){available++;if(!chosen.clear)wrong++;lines.push(line(a.x,a.y,chosen.x,chosen.y,chosen.clear?'agent':'danger'));}
   dots=[dot(a.x,a.y,.25,'Player','target'),...props.map((b,id)=>dot(b.x,b.y,.22,'Prop '+(id+1),chosen?.id===id?'agent':'reference'))];state=chosen?(chosen.clear?'Visible prop selected':'Wrong prompt: prop hidden by wall'):'No eligible prop';
  }
  frames.push({t,dots:[...dots,...obstacles.map(o=>dot(o.x,o.y,o.r,'Occluder','wall'))],boxes:[],lines,state});
 }
 return {frames,metrics:{clipping:clipping/361*100,minArm,transitions,stale:stale/361*100,maxStale,wrong:wrong/361*100,available:available/361*100},parameters:p};
}
