// Visibility-graph planning for an axis-aligned square footprint in a bounded 2D world.
const EPS=1e-8;
export const WORLD={width:20,height:12};
export const warehouse=[{x:6,y:0,w:1,h:4.8},{x:6,y:7.2,w:1,h:4.8},{x:11,y:3,w:3,h:2},{x:11,y:7,w:3,h:2}];
// Closed doorway is one solid wall: touching rectangle edges must not form a zero-width passage.
export const closedWarehouse=[{x:6,y:0,w:1,h:12},...warehouse.slice(2)];
export const length=(a,b)=>Math.hypot(a[0]-b[0],a[1]-b[1]);
const inside=(p,r)=>p[0]>r.x+EPS&&p[0]<r.x+r.w-EPS&&p[1]>r.y+EPS&&p[1]<r.y+r.h-EPS;
export function segmentInterior(a,b,r){
 let lo=0,hi=1;
 for(let axis=0;axis<2;axis++){
  const min=(axis===0?r.x:r.y)+EPS,max=(axis===0?r.x+r.w:r.y+r.h)-EPS,d=b[axis]-a[axis];
  if(Math.abs(d)<EPS){if(a[axis]<=min||a[axis]>=max)return false;continue;}
  const t1=(min-a[axis])/d,t2=(max-a[axis])/d;lo=Math.max(lo,Math.min(t1,t2));hi=Math.min(hi,Math.max(t1,t2));if(hi<=lo)return false;
 }
 return hi>lo&&hi>0&&lo<1;
}
export function planRoute(start,goal,obstacles=warehouse,clearance=.35,algorithm='astar'){
 if(![start,goal].every(p=>Array.isArray(p)&&p.length===2&&p.every(Number.isFinite))||!Number.isFinite(clearance)||clearance<0||clearance>2||!['astar','dijkstra'].includes(algorithm)||obstacles.some(r=>![r.x,r.y,r.w,r.h].every(Number.isFinite)||r.w<=0||r.h<=0))throw new RangeError('Invalid navigation scene');
 const inflated=obstacles.map(r=>({x:r.x-clearance,y:r.y-clearance,w:r.w+2*clearance,h:r.h+2*clearance}));
 const legal=p=>p[0]>clearance+EPS&&p[0]<WORLD.width-clearance-EPS&&p[1]>clearance+EPS&&p[1]<WORLD.height-clearance-EPS&&!inflated.some(r=>inside(p,r));
 const empty={path:[],distance:null,expanded:[],nodes:[],edges:[],inflated};
 if(!legal(start)||!legal(goal))return {...empty,reason:'An endpoint has insufficient clearance.'};
 const nodes=[start,goal];
 for(const r of inflated)for(const p of [[r.x,r.y],[r.x+r.w,r.y],[r.x+r.w,r.y+r.h],[r.x,r.y+r.h]])if(legal(p)&&!nodes.some(n=>length(n,p)<EPS))nodes.push(p);
 const edges=[];for(let a=0;a<nodes.length;a++)for(let b=a+1;b<nodes.length;b++)if(!inflated.some(r=>segmentInterior(nodes[a],nodes[b],r)))edges.push({a,b,cost:length(nodes[a],nodes[b])});
 const open=new Set([0]),closed=new Set(),g=new Map([[0,0]]),parent=new Map(),expanded=[];
 while(open.size){const h=i=>algorithm==='astar'?length(nodes[i],goal):0;const current=[...open].sort((a,b)=>(g.get(a)+h(a))-(g.get(b)+h(b))||a-b)[0];open.delete(current);closed.add(current);expanded.push(current);
  if(current===1){const indices=[1];while(indices[0]!==0)indices.unshift(parent.get(indices[0]));return {path:indices.map(i=>nodes[i]),distance:g.get(1),expanded,nodes,edges,inflated,reason:'Route found.'};}
  for(const edge of edges){const next=edge.a===current?edge.b:edge.b===current?edge.a:-1;if(next<0||closed.has(next))continue;const cost=g.get(current)+edge.cost;if(cost<(g.get(next)??Infinity)){g.set(next,cost);parent.set(next,current);open.add(next);}}
 }
 return {...empty,expanded,nodes,edges,reason:'No clearance-safe connection between these regions.'};
}
export function pointAlong(path,fraction){if(!path.length)return null;if(path.length===1)return path[0];const lengths=path.slice(1).map((p,i)=>length(path[i],p)),total=lengths.reduce((a,b)=>a+b,0);let remain=Math.max(0,Math.min(1,fraction))*total;for(let i=0;i<lengths.length;i++){if(remain<=lengths[i]||i===lengths.length-1){const t=lengths[i]?remain/lengths[i]:0;return [path[i][0]+(path[i+1][0]-path[i][0])*t,path[i][1]+(path[i+1][1]-path[i][1])*t];}remain-=lengths[i];}return path.at(-1);}
export function hearInWorld(source,listener,obstacles,budget){if(!Number.isFinite(budget)||budget<0)throw new RangeError('Invalid sound budget');const wave=planRoute(source,listener,obstacles,0);const heard=wave.distance!==null&&wave.distance<=budget;return {heard,distance:wave.distance,location:heard?[...source]:null,path:wave.path};}
