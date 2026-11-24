import test from 'node:test';import assert from 'node:assert/strict';
import {springPosition,peakOvershoot} from '../lib/spring-intro.mjs';
import {createStudyComputer} from '../lib/study-compute.mjs';
import {StudyQueue} from '../lib/study-queue.mjs';
import {systemStudies} from '../lib/system-study-config.mjs';
test('intro exact response matches an independently integrated ODE across all feels',()=>{
 for(const z of [.25,1,1.8]){let x=0,v=0;const dt=1/20000;for(let i=0;i<=60000;i++){if(i%10000===0)assert.ok(Math.abs(x-springPosition(i*dt,z))<.00015);const a=36*(1-x)-12*z*v;v+=a*dt;x+=v*dt;}assert.equal(springPosition(0,z),0);}
 assert.ok(peakOvershoot(.25)>44&&peakOvershoot(.25)<45);assert.equal(peakOvershoot(1),0);assert.equal(peakOvershoot(1.8),0);assert.ok(springPosition(1,1)>springPosition(1,1.8));assert.throws(()=>springPosition(Infinity,1),RangeError);
});
test('worker calculations preserve all nine briefs and reuse fixed reference when only the tunable changes',()=>{
 const compute=createStudyComputer();for(const[kind,cfg]of Object.entries(systemStudies))for(let mode=0;mode<3;mode++){let c=cfg.cases[mode],base=compute({kind,mode,p:{...cfg.base}}),p={...cfg.base,[c.key]:c.balanced},tuned=compute({kind,mode,p});assert.ok(c.pass(tuned.run.metrics,p));assert.strictEqual(base.reference,tuned.reference);assert.deepEqual(structuredClone(tuned).run.metrics,tuned.run.metrics);}
 assert.throws(()=>compute({kind:'toString',mode:0,p:{}}),RangeError);assert.throws(()=>compute({kind:'flock',mode:4,p:{}}),RangeError);
});
test('rapid inputs keep only the latest pending job and never accept stale results or late disposal messages',()=>{
 const sent=[],accepted=[];const q=new StudyQueue(j=>sent.push(j.id),m=>accepted.push(m.id));q.offer({id:1});q.offer({id:2});q.offer({id:3});assert.deepEqual(sent,[1]);q.resolve({id:999});assert.deepEqual(sent,[1]);q.resolve({id:1});assert.deepEqual(sent,[1,3]);assert.deepEqual(accepted,[]);q.resolve({id:1});q.resolve({id:3});assert.deepEqual(accepted,[3]);q.offer({id:4});q.dispose();q.resolve({id:4});q.offer({id:5});assert.deepEqual(sent,[1,3,4]);assert.deepEqual(accepted,[3]);
});

import {playbackTime} from '../lib/playback-time.mjs';
test('frame timestamps before the registration time hold the initial pose, with bounded resume and terminal hold',()=>{
 assert.equal(playbackTime(0,1000.5,1000,3),0);assert.equal(playbackTime(1.2,1000.5,1000,3),1.2);assert.equal(playbackTime(1.2,1000,2000,3),2.2);assert.equal(playbackTime(0,1000,9000,3),3);assert.throws(()=>playbackTime(0,0,Infinity,3),RangeError);
});
