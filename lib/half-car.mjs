// SI units, displacements from loaded equilibrium. Nose-up pitch is positive.
const mass=600, wheel=35, arm=1.2, inertia=720, tyre=160000, load=(mass/2+wheel)*9.81;
const bump=(phase,height)=>phase<0||phase>1?0:height*Math.sin(Math.PI*phase)**2;
function surface(mode,t){
  if(mode==='rough')return [0,.3].map(delay=>[.6,1.3,2].reduce((sum,start)=>sum+bump((t-start-delay)/.32,.05),0));
  if(mode==='dip')return [0,.3].map(delay=>bump((t-.6-delay)/.16,-.09));
  return [0,0];
}
export function drive(mode,k=22000,c=1800,hz=480){
  if(!['rough','brake','dip','flat'].includes(mode)||!Number.isFinite(k)||k<12000||k>40000||!Number.isFinite(c)||c<600||c>4000||![480,960].includes(hz))throw new RangeError('Outside experiment bounds');
  const dt=1/hz, frames=[];
  let z=0,v=0,p=0,w=0,y=[0,0],u=[0,0],squares=0,pitch=0,air=0;
  for(let i=0;i<=4*hz;i++){
    const t=i/hz,r=surface(mode,t);
    const f=y.map((a,j)=>{const x=j===0?arm:-arm;return k*(a-z-x*p)+c*(u[j]-v-x*w)});
    const n=y.map((a,j)=>Math.max(0,load+tyre*(r[j]-a)));
    const acc=(f[0]+f[1])/mass;
    const torque=mode==='brake'?-mass*.52*5*bump((t-.6)/1.2,1):0;
    pitch=Math.max(pitch,Math.abs(p)*180/Math.PI);
    if(i<4*hz){squares+=acc*acc*dt;if(n.some(a=>a===0))air+=dt}
    if(i%(hz/60)===0)frames.push({t,z,p,front:y[0],rear:y[1],roadF:r[0],roadR:r[1],loadF:n[0],loadR:n[1]});
    v+=acc*dt;w+=(arm*(f[0]-f[1])+torque)/inertia*dt;z+=v*dt;p+=w*dt;
    for(let j=0;j<2;j++){u[j]+=(n[j]-load-f[j])/wheel*dt;y[j]+=u[j]*dt}
  }
  return {frames,rms:Math.sqrt(squares/4),pitch,air};
}
