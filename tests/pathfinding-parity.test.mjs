import test from 'node:test';
import assert from 'node:assert/strict';
import {readFileSync} from 'node:fs';
import {planRoute,warehouse,closedWarehouse} from '../lib/spatial-navigation.mjs';
const cases=JSON.parse(readFileSync(new URL('./fixtures/pathfinding-csharp.json',import.meta.url),'utf8'));
test('browser route lengths agree with 24 compiled C# spatial planner cases',()=>{
 assert.equal(cases.length,24);
 for(const c of cases){const r=planRoute([2,2],[18,10],c.open?warehouse:closedWarehouse,c.radius,c.astar?'astar':'dijkstra');if(c.distance===null)assert.equal(r.distance,null);else assert.ok(r.distance!==null&&Math.abs(r.distance-c.distance)<1e-7,JSON.stringify(c));}
});
