import test from 'node:test';
import assert from 'node:assert/strict';
import {runFlock,runCamera,runVisibility,segmentDisk,clearSegment,projectPoint,inspectRun} from '../lib/engineering-batch.mjs';
test('finite segment/disk intersections respect tangent, behind, beyond, start-inside and zero-length cases',()=>{
 assert.equal(segmentDisk(0,0,10,0,5,0,1),.4);
 assert.equal(segmentDisk(0,0,10,0,5,1,1),.5);
 assert.equal(segmentDisk(0,0,10,0,-5,0,1),null);
 assert.equal(segmentDisk(0,0,10,0,15,0,1),null);
 assert.equal(segmentDisk(0,0,0,0,5,0,1),null);
 assert.equal(segmentDisk(0,0,0,0,0,0,1),0);
 assert.equal(clearSegment({x:0,y:0},{x:10,y:0},[{x:5,y:1.2,r:1}]),true);
 assert.equal(clearSegment({x:0,y:0},{x:10,y:0},[{x:5,y:1.2,r:1}],.3),false);
});
test('perspective projection preserves center and rejects points behind the camera',()=>{
 assert.deepEqual(projectPoint(0,0,0,0,6,70),{x:10,y:6,visible:true,depth:6});
 assert.equal(projectPoint(0,0,-7,0,6,70).visible,false);
 const narrow=projectPoint(3,0,0,0,6,35),wide=projectPoint(3,0,0,0,6,70);
 assert.ok(Math.abs(wide.x-10)<Math.abs(narrow.x-10));
});
test('snapshot flock is deterministic, bounded and respects speed/acceleration caps in all three jobs',()=>{
 for(let mode=0;mode<3;mode++){
  const run=runFlock(mode);assert.deepEqual(run,runFlock(mode));
  assert.ok(run.metrics.maxSpeed<=2.5+1e-10);assert.ok(run.metrics.maxAcceleration<=4+1e-10);
  for(const f of run.frames)for(const d of f.dots.filter(d=>d.tone==='agent')){assert.ok(d.x>=.2&&d.x<=19.8);assert.ok(d.y>=.2&&d.y<=11.8);}
 }
});
test('flock use cases have distinct, reachable measured compromises and reveal density failure',()=>{
 assert.ok(runFlock(0,{separation:2.5}).metrics.overlaps<.12);
 const school=runFlock(1,{cohesion:.9}).metrics;assert.ok(school.spread<2&&school.overlaps<.5);
 assert.ok(runFlock(1,{cohesion:2}).metrics.overlaps>school.overlaps);
 const gate=runFlock(2,{separation:3}).metrics;assert.equal(gate.passed,100);assert.equal(gate.overlaps,0);
 assert.ok(runFlock(2,{count:64,separation:0}).metrics.overlaps>gate.overlaps);
});
test('camera controls change composition, not the underlying actor input',()=>{
 const base=runCamera(0),dead=runCamera(0,{deadZone:2});assert.ok(dead.metrics.travel<28&&dead.metrics.travel<base.metrics.travel);assert.equal(dead.metrics.visible,100);
 const straight=runCamera(1),ahead=runCamera(1,{lookAhead:.5}),stress=runCamera(1,{lookAhead:2});assert.ok(ahead.metrics.preview>1.6&&ahead.metrics.preview>straight.metrics.preview);assert.equal(ahead.metrics.visible,100);assert.ok(stress.metrics.visible<100);
 const narrow=runCamera(2),fit=runCamera(2,{fov:70});assert.equal(fit.metrics.visible,100);assert.ok(fit.metrics.margin>5);assert.ok(narrow.metrics.visible<fit.metrics.visible);
});
test('visibility applications expose volume clearance, stale state and candidate eligibility separately',()=>{
 const ray=runVisibility(0),volume=runVisibility(0,{clearance:.35});assert.ok(ray.metrics.clipping>20);assert.equal(volume.metrics.clipping,0);assert.ok(volume.metrics.minArm>1.5&&volume.metrics.minArm<ray.metrics.minArm);
 const raw=runVisibility(1),remember=runVisibility(1,{memory:1.2});assert.equal(raw.metrics.stale,0);assert.ok(remember.metrics.transitions<10&&remember.metrics.transitions<raw.metrics.transitions);assert.ok(remember.metrics.maxStale<=1.21&&remember.metrics.stale>0);
 const distance=runVisibility(2),eligible=runVisibility(2,{requireLOS:1});assert.ok(distance.metrics.wrong>10);assert.equal(eligible.metrics.wrong,0);assert.ok(eligible.metrics.available>80&&eligible.metrics.available<distance.metrics.available);
});
test('all nine studies hold initial/final frames and reject non-finite or out-of-range parameters',()=>{
 for(const fn of [runFlock,runCamera,runVisibility])for(let mode=0;mode<3;mode++){
  const run=fn(mode);assert.equal(run.frames.length,361);assert.equal(inspectRun(run,-1),run.frames[0]);assert.equal(inspectRun(run,99),run.frames[360]);
  assert.equal(inspectRun(run,12),run.frames[360]);for(const value of Object.values(run.metrics))assert.ok(Number.isFinite(value));
  assert.throws(()=>fn(mode,{bad:NaN}));
 }
 assert.throws(()=>runFlock(3));assert.throws(()=>runFlock(0,{count:3}));assert.throws(()=>runCamera(2,{fov:0}));assert.throws(()=>runVisibility(0,{clearance:-1}));
});
