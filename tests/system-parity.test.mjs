import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import {systemStudies} from '../lib/system-study-config.mjs';
// Exported by StudyValidation inside Unity 6000.5.2f1, November 24, 2026.
const fixture=JSON.parse(fs.readFileSync(new URL('./fixtures/systems-csharp.json',import.meta.url),'utf8'));
// Dense 64-agent schooling amplifies floating-point differences: observed max 0.000050 m.
// Its sampled coordinates use a 0.1 mm tolerance; all other coordinates and all metrics use 1e-7.
test('27 actual Unity model runs match browser metrics and inspected coordinates',()=>{
 assert.equal(fixture.length,27);
 for(const sample of fixture){const cfg=Object.values(systemStudies)[sample.kind],c=cfg.cases[sample.mode],p={...cfg.base};if(sample.preset===1)p[c.key]=c.balanced;if(sample.preset===2)Object.assign(p,c.stress);const r=cfg.run(sample.mode,p);const coordinateTolerance=sample.kind===0&&sample.mode===1&&sample.preset===2?1e-4:1e-7;for(const [key,value]of Object.entries(sample.metrics))assert.ok(Math.abs(r.metrics[key]-value)<1e-7,`${sample.kind}/${sample.mode}/${sample.preset} metric ${key}`);sample.frames.forEach((f,i)=>{const actual=r.frames[i*180];assert.equal(actual.dots.length,f.dots.length);f.dots.forEach(([x,y],j)=>{assert.ok(Math.abs(actual.dots[j].x-x)<coordinateTolerance,'x coordinate');assert.ok(Math.abs(actual.dots[j].y-y)<coordinateTolerance,'y coordinate');});});}
});
