import test from 'node:test';
import assert from 'node:assert/strict';
import {searchGrid,pathfindingScenario} from '../lib/pathfinding.mjs';
// Independent repeated-relaxation oracle, with no frontier or heuristic.
function oracle(grid,start,goal){const w=grid[0].length,cells=grid.flat(),d=cells.map(()=>Infinity);if(!cells[start]||!cells[goal])return null;d[start]=0;for(let k=0;k<cells.length;k++){let changed=false;for(let a=0;a<cells.length;a++)if(cells[a])for(let b=0;b<cells.length;b++)if(cells[b]&&Math.abs(a%w-b%w)+Math.abs(Math.floor(a/w)-Math.floor(b/w))===1&&d[a]+cells[b]<d[b]){d[b]=d[a]+cells[b];changed=true;}if(!changed)break;}return Number.isFinite(d[goal])?d[goal]:null;}
test('A* and Dijkstra match an independent weighted shortest-path oracle',()=>{let seed=32;const random=()=>{seed=(1664525*seed+1013904223)>>>0;return seed/2**32;};for(let i=0;i<30;i++){const grid=Array.from({length:6},()=>Array.from({length:6},()=>random()<.2?0:1+Math.floor(random()*9)));grid[0][0]=grid[5][5]=1;for(const algorithm of ['astar','dijkstra']){const r=searchGrid(grid,0,35,algorithm);assert.equal(r.cost,oracle(grid,0,35));if(r.cost!==null){assert.equal(r.path[0],0);assert.equal(r.path.at(-1),35);assert.equal(r.cost,r.path.slice(1).reduce((sum,id)=>sum+grid[Math.floor(id/6)][id%6],0));for(let j=1;j<r.path.length;j++)assert.equal(Math.abs(r.path[j]%6-r.path[j-1]%6)+Math.abs(Math.floor(r.path[j]/6)-Math.floor(r.path[j-1]/6)),1);}}}});
test('terrain lesson distinguishes cheapest cost from fewest steps',()=>{const {grid,start,goal}=pathfindingScenario(1),a=searchGrid(grid,start,goal),b=searchGrid(grid,start,goal,'bfs');assert.ok(a.cost<b.cost);assert.ok(a.steps>b.steps);assert.equal(a.cost,9);assert.equal(b.cost,31);});
test('gate closure replans or explicitly reports no route without mutating the map',()=>{const {grid,start,goal}=pathfindingScenario(2),initial=structuredClone(grid),a=searchGrid(grid,start,goal);assert.deepEqual(grid,initial);grid[3][5]=0;const detour=searchGrid(grid,start,goal);assert.ok(detour.cost>a.cost);grid[6][5]=0;const closed=searchGrid(grid,start,goal);assert.equal(closed.cost,null);assert.deepEqual(closed.path,[]);assert.ok(closed.expanded.length>0);});
test('endpoints, deterministic ties, bounds and start-equals-goal are explicit',()=>{assert.deepEqual(searchGrid([[1]],0,0).path,[0]);assert.equal(searchGrid([[1]],0,0).cost,0);assert.equal(searchGrid([[0,1]],0,1).cost,null);const s=pathfindingScenario(0);assert.deepEqual(searchGrid(s.grid,s.start,s.goal),searchGrid(s.grid,s.start,s.goal));for(const bad of [[],[[1],[1,2]],[[NaN]],[[10]],[[-1]]])assert.throws(()=>searchGrid(bad,0,0),RangeError);assert.throws(()=>searchGrid([[1]],2,0),RangeError);assert.throws(()=>searchGrid([[1]],0,0,'greedy'),RangeError);});
test('unit-cost A* preserves shortest steps and explores no more than the baseline in the curated maze',()=>{const {grid,start,goal}=pathfindingScenario(0),a=searchGrid(grid,start,goal),b=searchGrid(grid,start,goal,'dijkstra');assert.equal(a.cost,b.cost);assert.equal(a.cost,a.steps);assert.ok(a.expanded.length<=b.expanded.length);});

import {hearSound} from '../lib/pathfinding.mjs';
test('hearing respects walls, a finite budget and absorption before reporting a location',()=>{
 assert.deepEqual(hearSound([[1,1,1]],2,0,2),{heard:true,attenuation:2,location:2});
 assert.deepEqual(hearSound([[1,1,1]],2,0,1),{heard:false,attenuation:2,location:null});
 assert.equal(hearSound([[1,0,1]],2,0,100).heard,false);
 assert.equal(hearSound([[1,5,1]],2,0,2).heard,false);
 assert.equal(hearSound([[1,5,1]],2,0,4).heard,true);
 const map=[[1,0,1],[1,1,1]];
 assert.equal(hearSound(map,2,0,3).heard,false);
 assert.equal(hearSound(map,2,0,4).heard,true);
 assert.throws(()=>hearSound(map,2,0,-1),RangeError);
});

test("every starter map has walkable endpoints and a solvable route",()=>{for(const mode of [0,1,2]){const {grid,start,goal}=pathfindingScenario(mode);assert.ok(searchGrid(grid,start,goal).path.length>0);}});
