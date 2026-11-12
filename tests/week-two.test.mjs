import test from 'node:test';
import assert from 'node:assert/strict';
import {drive} from '../lib/half-car.mjs';
import {rail,initialRail,point,offsets,tangent} from '../lib/rail.mjs';
test('loaded chassis stays at rest on flat ground and braking drives nose-down pitch without net heave',()=>{
 const flat=drive('flat');assert.equal(flat.rms,0);assert.equal(flat.pitch,0);
 assert.ok(flat.frames.every(f=>f.loadF===f.loadR&&f.loadF>0));
 const brake=drive('brake');assert.ok(brake.frames.some(f=>f.p<-.02));assert.ok(brake.rms<1e-9);
});
test('front and rear see the same terrain with a delay; loss and recovery of contact are observable',()=>{
 const {frames,air}=drive('dip');assert.ok(air>0);
 for(let i=18;i<frames.length;i++)assert.ok(Math.abs(frames[i].roadR-frames[i-18].roadF)<1e-12);
 assert.ok(frames.some(f=>f.loadF===0));assert.ok(frames.at(-1).loadF>0);
});
test('halving the integration step preserves target metrics and all presets have actionable trade-offs',()=>{
 const soft=[14000,1200],firm=[36000,2800];assert.ok(drive('rough',...soft).rms<drive('rough').rms);
 assert.ok(drive('brake',...firm).pitch<drive('brake').pitch);assert.ok(drive('dip',...soft).air<drive('dip').air);
 for(const mode of ['rough','brake','dip'])for(const k of [12000,22000,40000])for(const d of [600,1800,4000]){
  const a=drive(mode,k,d),b=drive(mode,k,d,960);assert.ok(Math.abs(a.rms-b.rms)<.06);assert.ok(Math.abs(a.pitch-b.pitch)<.06);assert.ok(Math.abs(a.air-b.air)<.015);
  assert.ok(a.frames.every(f=>Object.values(f).every(Number.isFinite)&&f.loadF>=0&&f.loadR>=0));
 }
});
test('distance lookup compensates for non-uniform parameter speed even on a straight path',()=>{
 const p=[[0,0],[.5,0],[5.5,0],[10,0]],r=rail(p);assert.equal(r.length,10);assert.ok(Math.abs(point(p,.5)[0]-5)>1);
 for(let j=0;j<=10;j++)assert.ok(Math.abs(point(p,r.at(j/10))[0]-j)<.001);
});
test('road edges maintain requested separation and are normal to the centerline',()=>{
 for(const t of [0,.1,.5,.9,1]){const [a,b]=offsets(initialRail,t,2.25),d=tangent(initialRail,t),dx=a[0]-b[0],dy=a[1]-b[1];assert.ok(Math.abs(Math.hypot(dx,dy)-2.25)<1e-10);assert.ok(Math.abs(dx*d[0]+dy*d[1])<1e-10)}
});
test('patrol preserves the four-second round trip while spending the requested time at the endpoint',async()=>{
 const {patrol}=await import('../lib/rail.mjs');for(const pause of [0,.5,1,2]){const leg=(4-pause)/2;assert.equal(patrol(0,pause).fraction,0);assert.equal(patrol(leg,pause).fraction,1);assert.equal(patrol(leg+pause,pause).fraction,1);assert.equal(patrol(4,pause).fraction,0);if(pause)assert.equal(patrol(2,pause).phase,'Waiting');assert.ok(patrol(4-.1,pause).fraction<.11)}
});
