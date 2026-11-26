// Four-neighbor grid. A cell's value is the cost of entering it; zero is a wall.
export function searchGrid(grid,start,goal,algorithm='astar'){
 const height=grid.length,width=grid[0]?.length;
 if(!width||width>32||height>32||grid.some(row=>row.length!==width||row.some(v=>!Number.isInteger(v)||v<0||v>9))||![start,goal].every(v=>Number.isInteger(v)&&v>=0&&v<width*height)||!['astar','dijkstra','bfs'].includes(algorithm))throw new RangeError('Invalid grid, endpoints or algorithm');
 const cells=grid.flat(),empty={path:[],expanded:[],cost:null,steps:null,frontierPeak:0};
 if(!cells[start]||!cells[goal])return empty;
 const minimum=Math.min(...cells.filter(v=>v>0));
 const h=id=>(Math.abs(id%width-goal%width)+Math.abs(Math.floor(id/width)-Math.floor(goal/width)))*minimum;
 const g=new Map([[start,0]]),depth=new Map([[start,0]]),parent=new Map(),open=new Set([start]),closed=new Set(),expanded=[];let peak=1;
 while(open.size){
  const score=id=>algorithm==='bfs'?depth.get(id):g.get(id)+(algorithm==='astar'?h(id):0);
  const current=[...open].sort((a,b)=>score(a)-score(b)||(algorithm==='astar'?h(a)-h(b):0)||a-b)[0];
  open.delete(current);closed.add(current);expanded.push(current);
  if(current===goal){const path=[goal];while(path[0]!==start)path.unshift(parent.get(path[0]));return {path,expanded,cost:g.get(goal),steps:path.length-1,frontierPeak:peak};}
  const x=current%width,y=Math.floor(current/width);
  for(const [nx,ny] of [[x+1,y],[x,y+1],[x-1,y],[x,y-1]]){
   if(nx<0||nx>=width||ny<0||ny>=height)continue;
   const next=ny*width+nx;if(!cells[next]||closed.has(next))continue;
   const cost=g.get(current)+cells[next];
   if(algorithm==='bfs'?g.has(next):cost>=(g.get(next)??Infinity))continue;
   g.set(next,cost);depth.set(next,depth.get(current)+1);parent.set(next,current);open.add(next);
  }
  peak=Math.max(peak,open.size);
 }
 return {...empty,expanded,frontierPeak:peak};
}
export function pathfindingScenario(mode=0){
 if(![0,1,2].includes(mode))throw new RangeError('Invalid scenario');
 const grid=Array.from({length:8},()=>Array(10).fill(1));
 if(mode===0){for(let y=0;y<7;y++)if(y!==5)grid[y][4]=0;for(let x=5;x<8;x++)grid[3][x]=0;}
 if(mode===1){for(let x=2;x<8;x++)grid[3][x]=5;}
 if(mode===2){for(let y=0;y<8;y++)grid[y][5]=0;grid[3][5]=1;grid[6][5]=1;}
 return {grid,start:31,goal:38};
}

// Deliberately simplified game hearing: weighted graph distance, not wave acoustics.
// Rough tiles absorb 3 units; ordinary tiles 1. Walls transmit nothing.
// A heard event includes an idealized localized source coordinate.
export function hearSound(grid,source,listener,budget){
 if(!Number.isFinite(budget)||budget<0||budget>100)throw new RangeError('Invalid hearing budget');
 const acoustic=grid.map(row=>row.map(v=>v===0?0:v>1?3:1));
 const result=searchGrid(acoustic,source,listener,'dijkstra');
 const heard=result.cost!==null&&result.cost<=budget;
 return {heard,attenuation:result.cost,location:heard?source:null};
}
