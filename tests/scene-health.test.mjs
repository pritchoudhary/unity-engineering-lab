import test from 'node:test';
import assert from 'node:assert/strict';
import {inspectScene} from '../lib/scene-health.mjs';
import {sceneHealthLessons as lessons} from '../lib/scene-health-fixtures.mjs';
const scene = index => structuredClone(lessons[index].scene);

test('migration identifies one missing slot; optional null is not an error',()=>{
 const result=inspectScene(scene(0));assert.equal(result.findings.length,1);
 assert.equal(result.findings[0].rule,'missing-script');assert.equal(result.findings[0].componentId,'slot-1');
 assert.equal(result.findings[0].path,'Migration scene/Migrated door');
});
test('handoff distinguishes required stale links, optional stale links and runtime assignment',()=>{
 const result=inspectScene(scene(1));assert.deepEqual(result.detectedCounts,{error:1,warning:1,info:0});
 assert.deepEqual(result.findings.map(f=>f.field),['spawn','optionalSound']);
});
test('required empty reference is an error; inactive hierarchy is only informational',()=>{
 const result=inspectScene(scene(2));assert.deepEqual(result.detectedCounts,{error:1,warning:0,info:2});
 const enemy=result.findings.find(f=>f.rule==='inactive-object'&&f.objectId==='enemy');
 assert.match(enemy.evidence,/Pool \(pool\)/);assert.equal(result.findings[0].rule,'required-reference');
});
test('active-only scan reports reduced coverage rather than implying no issues exist',()=>{
 const result=inspectScene(scene(2),{includeInactive:false});assert.equal(result.findings.length,0);
 assert.equal(result.scannedObjects,2);assert.equal(result.excludedInactive,2);assert.equal(result.totalObjects,4);
});
test('disabling inactive review still scans inactive scripts and references',()=>{
 const result=inspectScene(scene(2),{reviewInactive:false});assert.equal(result.scannedObjects,4);
 assert.equal(result.findings.length,1);assert.equal(result.findings[0].rule,'required-reference');
});
test('severity filter does not hide total detected counts',()=>{
 const result=inspectScene(scene(1),{minimumSeverity:'error'});assert.equal(result.findings.length,1);
 assert.equal(result.hiddenBySeverity,1);assert.equal(result.detectedCounts.warning,1);
});
test('subtree references resolve against the full manifest, not just scanned objects',()=>{
 const result=inspectScene(scene(1),{rootId:'player'});assert.equal(result.scannedObjects,1);
 assert.equal(result.findings.length,2);assert.ok(result.findings.every(f=>f.field!=='camera'));
});
test('selected subtree inherits inactive state from parents outside the scope',()=>{
 const result=inspectScene(scene(2),{rootId:'enemy',includeInactive:false});assert.equal(result.scopedObjects,1);
 assert.equal(result.scannedObjects,0);assert.equal(result.excludedInactive,1);
});
test('restoring the modeled script removes its finding without modifying other objects',()=>{
 const original=scene(0), repaired=structuredClone(original);repaired.objects[1].components[0].type='DoorController';
 assert.equal(inspectScene(repaired).findings.length,0);assert.equal(inspectScene(original).findings.length,1);
});
test('reordering input preserves finding identity and order; scanning never mutates input',()=>{
 const original=scene(1),before=JSON.stringify(original),first=inspectScene(original);
 assert.equal(JSON.stringify(original),before);original.objects.reverse();
 for(const o of original.objects)for(const c of o.components)c.references.reverse();
 assert.deepEqual(inspectScene(original).findings,first.findings);
});
test('ambiguous or malformed hierarchy is rejected instead of producing misleading findings',()=>{
 const duplicate=scene(0);duplicate.objects.push(duplicate.objects[0]);assert.throws(()=>inspectScene(duplicate),/unique/);
 const dangling=scene(0);dangling.objects[0].parent='absent';assert.throws(()=>inspectScene(dangling),/unknown parent/);
 const cycle=scene(0);cycle.objects[0].parent='door';assert.throws(()=>inspectScene(cycle),/cycle/);
 assert.throws(()=>inspectScene(scene(0),{rootId:'absent'}),/scan root/);
 assert.throws(()=>inspectScene(scene(0),{minimumSeverity:'fatal'}),/severity/);
});
test('object and asset ID spaces remain distinct',()=>{
 const input=scene(1);input.assets.push('deleted-spawn');
 assert.ok(inspectScene(input).findings.some(f=>f.field==='spawn'));
});
