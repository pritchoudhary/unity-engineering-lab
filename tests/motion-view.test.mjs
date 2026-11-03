import test from 'node:test';
import assert from 'node:assert/strict';
import {experiment} from '../lib/motion-experiments.mjs';
import {plotY,highlightX,cameraX,inspectionTime,MOTION_DOMAIN} from '../lib/motion-view.mjs';

test('every slider combination keeps full response and stage geometry in view',()=>{
 for(const mode of ['ui','camera','impact'])for(let k=10;k<=100;k++)for(let zi=3;zi<=30;zi++){
  const r=experiment(mode,k,zi/20);
  for(const p of r.points){
   assert.ok(Number.isFinite(p.x)&&Number.isFinite(p.v));
   assert.ok(p.x>=MOTION_DOMAIN.min&&p.x<=MOTION_DOMAIN.max,`${mode}: k=${k}, damping=${zi/20}, x=${p.x}`);
   assert.ok(plotY(p.x)>=20&&plotY(p.x)<=180);
   if(mode==='ui')assert.ok(highlightX(p.x)>=0&&highlightX(p.x)+100<=600);
   if(mode==='camera')assert.ok(cameraX(p.x)>=0&&cameraX(p.x)+160<=600);
   if(mode==='impact')assert.ok(85+p.x*50>=0&&100-p.x*40>0&&185+p.x*10<=260);
  }
 }
});

test('manual inspection selects the actual reversal or second impact',()=>{
 assert.equal(inspectionTime('camera'),1.5);
 assert.equal(inspectionTime('ui'),.75);
 assert.equal(inspectionTime('impact'),.75);
});
