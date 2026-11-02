import test from 'node:test';
import assert from 'node:assert/strict';
import {simulate,road,DT} from '../lib/suspension.mjs';
test('smooth bump endpoints and peak',()=>{assert.deepEqual(road(1),{y:0,v:0});assert.deepEqual(road(1.5),{y:0,v:0});assert.equal(road(1.25).y,.08);assert.ok(Math.abs(road(1.25).v)<1e-12)});
test('rest before bump; finite and converged across allowed damping range',()=>{for(const c of [300,500,1800,4650,8000]){const coarse=simulate(c),fine=simulate(c,DT/2);assert.ok(coarse.points.filter(p=>p.t<1).every(p=>p.z===0&&p.v===0));assert.ok(coarse.points.every(p=>Number.isFinite(p.z)&&Number.isFinite(p.a)));assert.ok(Math.abs(coarse.peak-fine.peak)<.003);assert.ok(Math.max(...coarse.points.map((p,i)=>Math.abs(p.z-fine.points[2*i].z)))<1e-6)}});
test('both design briefs are achievable and have different trade-offs',()=>{const a=simulate(1800),b=simulate(4650);assert.ok(a.peak<=5.6&&a.settle<=1.6&&a.travel<=65);assert.ok(b.peak<=6.2&&b.settle<=1&&b.travel<=35);assert.ok(a.travel>35&&b.peak>5.6);assert.ok(simulate(8000).peak>simulate(1800).peak)});
test('settling means every subsequent sample satisfies both tolerances',()=>{for(const c of [300,1800,8000]){const r=simulate(c);if(r.settle!==null)assert.ok(r.points.filter(p=>p.t>=1.5+r.settle+1e-9).every(p=>Math.abs(p.z)<=.002&&Math.abs(p.v)<=.005))}});
test('invalid parameters rejected',()=>{for(const c of [NaN,Infinity,0,9000])assert.throws(()=>simulate(c),RangeError);assert.throws(()=>simulate(500,0),RangeError)});
