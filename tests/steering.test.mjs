import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import {defaults,simulate,comparison,inspect,segmentDistance,segmentHit} from '../lib/steering.mjs';
const close=(a,b,t=1e-6)=>assert.ok(Math.abs(a-b)<t,`${a} != ${b}`);
test('steering runs and 864 inspected frames agree with independently executed C#',()=>{
 const rows=JSON.parse(fs.readFileSync(new URL('./fixtures/steering-csharp.json',import.meta.url)));assert.equal(rows.length,96);
 for(const [mode,values,m,samples] of rows){const [speed,acceleration,slowRadius,lookAhead,prediction]=values,r=simulate(mode,{speed,acceleration,slowRadius,lookAhead,prediction});
  assert.equal(r.completed,!!m[0]);assert.equal(r.collision,!!m[1]);
  [r.duration,r.path,r.overshoot,r.closest,r.error].forEach((v,i)=>v===null?assert.equal(m[i+2],null):close(v,m[i+2]));
  for(const [t,pos,vel,target] of samples){const f=inspect(r,t);for(const [a,b] of [[f.position,pos],[f.velocity,vel],[f.target,target]])a.forEach((v,i)=>close(v,b[i]));}
 }
});
test('arrival radius changes overshoot and settling instead of only changing the drawing',()=>{
 const {current,reference}=comparison(0,{...defaults,slowRadius:2});
 assert.ok(current.completed&&reference.completed);assert.ok(reference.overshoot>.7&&current.overshoot<.03);
 assert.ok(current.duration<reference.duration);assert.ok(current.error<.08);
 const cautious=simulate(0,{slowRadius:6});assert.equal(cautious.overshoot,0);assert.ok(cautious.duration>current.duration);
});
test('earlier obstacle detection clears the obstacle while a short lookahead collides',()=>{
 const {current,reference}=comparison(1,{...defaults,lookAhead:2});
 assert.ok(current.completed&&!current.collision&&current.closest>.4);assert.ok(reference.collision&&!reference.completed);
 close(reference.closest,0);assert.ok(current.path>16);
});
test('prediction reduces pursuit time and contact stays fixed after capture',()=>{
 const {current,reference}=comparison(2,{...defaults,prediction:2});
 assert.ok(current.completed&&reference.completed);assert.ok(reference.duration-current.duration>.18);close(current.error,.4);
 const end=inspect(current,current.duration),later=inspect(current,current.duration+2);
 assert.deepEqual(end,later);close(Math.hypot(end.position[0]-end.target[0],end.position[1]-end.target[1]),.4);
 assert.ok(!simulate(2,{speed:1}).completed);
});
test('swept circle checks detect tunnelling, tangency and initial overlap',()=>{
 close(segmentHit([-5,0],[5,0],1),.4);close(segmentHit([-5,1],[5,1],1),.5);
 assert.equal(segmentHit([0,0],[0,0],1),0);assert.equal(segmentHit([-5,2],[5,2],1),null);
 close(segmentDistance([-5,2],[5,2],[0,0]),2);
});
test('halving the timestep preserves the practical conclusions and metrics',()=>{
 for(let mode=0;mode<3;mode++){
  const p={...defaults,slowRadius:2,lookAhead:2,prediction:2},a=simulate(mode,p),b=simulate(mode,p,1/240);
  assert.equal(a.completed,b.completed);assert.equal(a.collision,b.collision);close(a.duration,b.duration,.04);close(a.path,b.path,.05);
  if(mode===1)close(a.closest,b.closest,.025);
 }
});
test('control corners respect acceleration/speed caps and terminal holds with finite bounded runs',()=>{
 for(let mode=0;mode<3;mode++)for(let mask=0;mask<32;mask++){
  const p={speed:mask&1?5:1,acceleration:mask&2?8:1,slowRadius:mask&4?6:.25,lookAhead:mask&8?3:.1,prediction:mask&16?3:0},r=simulate(mode,p);
  assert.ok(r.peakSpeed<=p.speed+1e-8&&r.peakAcceleration<=p.acceleration+1e-8);
  assert.ok(r.frames.length<=1441);assert.ok(r.frames.every(f=>[f.time,...f.position,...f.velocity,...f.target].every(Number.isFinite)));
  assert.deepEqual(inspect(r,r.duration+1),r.frames.at(-1));
 }
});
test('invalid parameters fail before simulation',()=>{
 for(const p of [{speed:NaN},{speed:0},{acceleration:9},{slowRadius:0},{lookAhead:4},{prediction:-1}])assert.throws(()=>simulate(0,p),RangeError);
 assert.throws(()=>simulate(3),RangeError);assert.throws(()=>simulate(0,{},1),RangeError);
 assert.throws(()=>inspect(simulate(0),NaN),RangeError);
});
