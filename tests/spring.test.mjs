import {test} from 'node:test';
import assert from 'node:assert/strict';
import {step,STEP,energy} from '../lib/spring.mjs';
const evolve=(p,seconds,x=1,dt=STEP)=>{let s={x,v:0};for(let i=0;i<Math.round(seconds/dt);i++)s=step(s,p,dt);return s;};
test('equilibrium remains at rest',()=>assert.deepEqual(evolve({mass:1,stiffness:20,damping:2},10,0),{x:0,v:0}));
test('undamped motion matches analytic cosine within integration error',()=>{const p={mass:1,stiffness:20,damping:0};assert.ok(Math.abs(evolve(p,2).x-Math.cos(Math.sqrt(20)*2))<.011);});
test('halving timestep reduces analytic error',()=>{const p={mass:1,stiffness:20,damping:0},exact=Math.cos(Math.sqrt(20)*2);assert.ok(Math.abs(evolve(p,2,1,STEP/2).x-exact)<Math.abs(evolve(p,2).x-exact));});
test('damped energy decays toward equilibrium',()=>{const p={mass:1,stiffness:20,damping:2};assert.ok(energy(evolve(p,8),p)<1e-5);});
test('critical damping matches analytic response and has no overshoot',()=>{const p={mass:1,stiffness:20,damping:2*Math.sqrt(20)},t=1,exact=(1+Math.sqrt(20)*t)*Math.exp(-Math.sqrt(20)*t);assert.ok(Math.abs(evolve(p,t).x-exact)<.003);let s={x:1,v:0};for(let i=0;i<2400;i++){s=step(s,p);assert.ok(s.x>=0);}});
test('overdamping settles slower than critical response',()=>{const p={mass:1,stiffness:20,damping:20};assert.ok(evolve(p,1).x>evolve({...p,damping:2*Math.sqrt(20)},1).x);});
test('all UI parameter corners remain finite and bounded for 30 seconds',()=>{for(const mass of [.25,5])for(const stiffness of [1,100])for(const damping of [0,50]){let s={x:2,v:0};for(let i=0;i<7200;i++){s=step(s,{mass,stiffness,damping});assert.ok(Number.isFinite(s.x)&&Math.abs(s.x)<2.1);}}});
test('reject invalid parameters, states and timesteps',()=>{const p={mass:1,stiffness:20,damping:2};for(const bad of [{...p,mass:0},{...p,damping:-1},{...p,stiffness:NaN}])assert.throws(()=>step({x:1,v:0},bad));assert.throws(()=>step({x:Infinity,v:0},p));assert.throws(()=>step({x:1,v:0},p,1));});

test('target following converges without snapping at a target change',async()=>{const {follow}=await import('../lib/spring.mjs');const p={mass:1,stiffness:70,damping:2*Math.sqrt(70)};let s={x:0,v:0};const first=follow(s,1,p);assert.ok(first.x>0&&first.x<.01);s=first;for(let i=0;i<2400;i++)s=follow(s,1,p);assert.ok(Math.abs(s.x-1)<1e-4);});
test('retargeting preserves inertial continuity',async()=>{const {follow}=await import('../lib/spring.mjs');const p={mass:1,stiffness:30,damping:2};let s={x:0,v:0};for(let i=0;i<30;i++)s=follow(s,1,p);const next=follow(s,-1,p);assert.ok(next.v>0,'direction does not reverse instantly');assert.ok(Math.abs(next.x-s.x)<.03,'position stays continuous');});
test('cosmetic impulse moves away from rest and then recovers',async()=>{const {follow}=await import('../lib/spring.mjs');const p={mass:1,stiffness:60,damping:Math.sqrt(60)};let s={x:0,v:5};s=follow(s,0,p);assert.ok(s.x>0);for(let i=0;i<2400;i++)s=follow(s,0,p);assert.ok(Math.abs(s.x)<1e-4&&Math.abs(s.v)<1e-4);assert.throws(()=>follow(s,NaN,p));});
