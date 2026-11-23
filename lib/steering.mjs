// Original bounded teaching model. Coordinates are a horizontal plane in metres.
export const defaults = Object.freeze({ speed: 3, acceleration: 4, slowRadius: .5, lookAhead: .2, prediction: 0 });
export const agentRadius = .3;
export const obstacle = Object.freeze([8, 0, 1.3]);
const add=(a,b)=>[a[0]+b[0],a[1]+b[1]], sub=(a,b)=>[a[0]-b[0],a[1]-b[1]], mul=(a,n)=>[a[0]*n,a[1]*n];
const length=a=>Math.hypot(...a), unit=a=>length(a)>1e-10?mul(a,1/length(a)):[0,0];
const limit=(a,n)=>length(a)>n?mul(unit(a),n):a;
export function segmentDistance(a,b,c) {
  const d=sub(b,a),den=d[0]*d[0]+d[1]*d[1];
  const t=den?Math.max(0,Math.min(1,((c[0]-a[0])*d[0]+(c[1]-a[1])*d[1])/den)):0;
  return length(sub(add(a,mul(d,t)),c));
}
export function segmentHit(a,b,radius) {
  const d=sub(b,a),aa=d[0]*d[0]+d[1]*d[1],bb=2*(a[0]*d[0]+a[1]*d[1]),cc=a[0]*a[0]+a[1]*a[1]-radius*radius;
  if(cc<=0)return 0;
  if(aa<1e-20)return null;
  const disc=bb*bb-4*aa*cc;
  if(disc<0)return null;
  const root=(-bb-Math.sqrt(disc))/(2*aa);
  return root>=0&&root<=1?root:null;
}
function validate(mode,p,dt) {
  if(![0,1,2].includes(mode)||!Number.isFinite(dt)||dt<1/1000||dt>1/30)throw new RangeError('Mode or timestep');
  for(const [key,min,max] of [['speed',1,5],['acceleration',1,8],['slowRadius',.25,6],['lookAhead',.1,3],['prediction',0,3]])
    if(!Number.isFinite(p[key])||p[key]<min||p[key]>max)throw new RangeError(key);
}
export function simulate(mode, parameters={},dt=1/120) {
  const p={...defaults,...parameters};validate(mode,p,dt);
  let position=[0,0],velocity=[0,0],path=0,overshoot=0,closest=Infinity,peakAcceleration=0,peakSpeed=0,done=false,collision=false,eventTime=null,detour=false;
  const frames=[];const duration=mode===2?8:12;
  function target(t){return mode===2?[6,3+1.2*t]:[mode===0?12:16,0];}
  function frame(t,aim){return {time:t,position:[...position],velocity:[...velocity],target:target(t),aim:[...aim]};}
  frames.push(frame(0,target(0)));
  for(let i=1;i<=Math.round(duration/dt);i++) {
    const t=i*dt,previousTime=(i-1)*dt,goal=target(previousTime);
    let aim=goal,desired=[0,0];
    if(mode===2){const prediction=Math.min(p.prediction,length(sub(goal,position))/p.speed);aim=add(goal,[0,1.2*prediction]);desired=mul(unit(sub(aim,position)),p.speed);}
    else {
      if(mode===1&&!done){
        const forward=unit(length(velocity)>.05?velocity:sub(goal,position));
        const tip=add(position,mul(forward,Math.max(.1,length(velocity))*p.lookAhead));
        if(!detour&&position[0]<obstacle[0]&&segmentDistance(position,tip,obstacle)<obstacle[2]+agentRadius+.4)detour=true;
        if(detour&&position[0]<10)aim=[10,2.8];
      }
      const distance=length(sub(aim,position));
      desired=mul(unit(sub(aim,position)),p.speed*Math.min(1,distance/(mode===0?p.slowRadius:2)));
    }
    if(!done){
      const acceleration=limit(mul(sub(desired,velocity),1/.25),p.acceleration);
      peakAcceleration=Math.max(peakAcceleration,length(acceleration));
      velocity=limit(add(velocity,mul(acceleration,dt)),p.speed);
      peakSpeed=Math.max(peakSpeed,length(velocity));
      let next=add(position,mul(velocity,dt)),sampleTime=t;
      if(mode===1){
        const hit=segmentHit(sub(position,obstacle),sub(next,obstacle),obstacle[2]+agentRadius);
        if(hit!==null){next=add(position,mul(sub(next,position),hit));sampleTime=previousTime+dt*hit;collision=true;done=true;eventTime=sampleTime;}
        closest=Math.min(closest,segmentDistance(position,next,obstacle)-obstacle[2]-agentRadius);
      }
      if(mode===2){
        const hit=segmentHit(sub(position,goal),sub(next,target(t)),.4);
        if(hit!==null){next=add(position,mul(sub(next,position),hit));sampleTime=previousTime+dt*hit;done=true;eventTime=sampleTime;}
      }
      path+=length(sub(next,position));
      if(mode===0)overshoot=Math.max(overshoot,next[0]-12);
      if(mode!==2&&!done&&length(sub(next,goal))<.08&&length(velocity)<.1){done=true;eventTime=t;}
      position=next;
      if(done){velocity=[0,0];frames.push(frame(sampleTime,aim));break;}
    }
    frames.push(frame(t,aim));
    if(done)break;
  }
  const final=frames.at(-1),error=length(sub(final.position,final.target));
  return { frames, duration:final.time, completed:done&&!collision, collision,eventTime,path,overshoot,
    closest:mode===1?closest:null,error,peakAcceleration,peakSpeed,parameters:p };
}
export function comparison(mode,p) {
  const reference={...p,...(mode===0?{slowRadius:.5}:mode===1?{lookAhead:.2}:{prediction:0})};
  return {current:simulate(mode,p),reference:simulate(mode,reference)};
}
export function inspect(run,time) {
  if(!Number.isFinite(time))throw new RangeError('Time');
  const frames=run.frames;
  if(time<=0)return frames[0];
  if(time>=run.duration)return frames.at(-1);
  let low=0,high=frames.length-1;
  while(high-low>1){const mid=(low+high)>>1;if(frames[mid].time<=time)low=mid;else high=mid;}
  const a=frames[low],b=frames[high],f=(time-a.time)/(b.time-a.time);
  const mix=(x,y)=>add(x,mul(sub(y,x),f));
  return {time,position:mix(a.position,b.position),velocity:mix(a.velocity,b.velocity),target:mix(a.target,b.target),aim:mix(a.aim,b.aim)};
}
