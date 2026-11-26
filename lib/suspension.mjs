// One sprung mass; road position is prescribed. Coordinates are relative to static equilibrium.
export const DT = 1 / 240;
export function road(t,duration=.5) {
 const u=(t-1)/duration;
 return t<=1||t>=1+duration ? {y:0,v:0} : {y:.08*Math.sin(Math.PI*u)**2,v:.08*Math.PI/duration*Math.sin(2*Math.PI*u)};
}
export function acceleration(t,z,v,c,duration=.5) { const r=road(t,duration);return (-18000*(z-r.y)-c*(v-r.v))/300; }
export function simulate(c,dt=DT,duration=.5) {
 if(!Number.isFinite(c)||c<300||c>8000||!Number.isFinite(dt)||dt<=0||dt>DT||!Number.isFinite(duration)||duration<.25||duration>1)throw new RangeError('Invalid damping, timestep or crossing duration');
 const points=[];let z=0,v=0;
 const count=Math.round(8/dt);dt=8/count;
 for(let i=0;i<=count;i++){
  const t=i*dt,r=road(t,duration);points.push({t,z,v,road:r.y,a:acceleration(t,z,v,c,duration),travel:z-r.y});
  const a=acceleration(t,z,v,c,duration), b=acceleration(t+dt/2,z+v*dt/2,v+a*dt/2,c,duration), d=acceleration(t+dt/2,z+(v+a*dt/2)*dt/2,v+b*dt/2,c,duration), e=acceleration(t+dt,z+(v+b*dt/2)*dt,v+d*dt,c,duration);
  z+=dt/6*(v+2*(v+a*dt/2)+2*(v+b*dt/2)+(v+d*dt));v+=dt/6*(a+2*b+2*d+e);
 }
 let last=points.findIndex(p=>p.t>=1+duration);
 for(let i=last;i<points.length;i++)if(Math.abs(points[i].z)>.002||Math.abs(points[i].v)>.005)last=i+1;
 return {points,peak:Math.max(...points.map(p=>Math.abs(p.a))),travel:Math.max(...points.map(p=>Math.abs(p.travel)))*1000,settle:last>=points.length?null:Math.max(0,points[last].t-(1+duration))};
}
