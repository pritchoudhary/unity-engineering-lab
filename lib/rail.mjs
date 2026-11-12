// A single editable cubic with a chord-length lookup table for distance-based traversal.
export const initialRail=[[0,0],[1,4],[8,4],[10,0]];
export function point(p,t){let u=1-t;return [0,1].map(d=>u*u*u*p[0][d]+3*u*u*t*p[1][d]+3*u*t*t*p[2][d]+t*t*t*p[3][d])}
export function tangent(p,t){let u=1-t;return [0,1].map(d=>3*(u*u*(p[1][d]-p[0][d])+2*u*t*(p[2][d]-p[1][d])+t*t*(p[3][d]-p[2][d])))}
export function rail(p,steps=200){
  const samples=[{t:0,pos:point(p,0),s:0}];let length=0;
  for(let i=1;i<=steps;i++){const pos=point(p,i/steps),last=samples[i-1].pos;length+=Math.hypot(pos[0]-last[0],pos[1]-last[1]);samples.push({t:i/steps,pos,s:length})}
  function at(f){const distance=Math.max(0,Math.min(1,f))*length;const i=samples.findIndex(a=>a.s>=distance);if(i<=0)return 0;const a=samples[i-1],b=samples[i];return a.t+(b.t-a.t)*(distance-a.s)/(b.s-a.s)}
  return {samples,length,at};
}
export function offsets(p,t,width){const a=point(p,t),d=tangent(p,t),mag=Math.hypot(...d);if(!mag)return [a,a];const x=-d[1]*width/(2*mag),y=d[0]*width/(2*mag);return [[a[0]+x,a[1]+y],[a[0]-x,a[1]-y]]}
export function patrol(time,pause){
 const leg=(4-pause)/2;
 if(time<leg)return {fraction:time/leg,phase:'Outbound',leg};
 if(time<leg+pause)return {fraction:1,phase:'Waiting',leg};
 return {fraction:(4-time)/leg,phase:'Returning',leg};
}
