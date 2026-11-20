import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import {aim,timed,intercept,clearance,heightAt,study,defaults,challengeMet,pointAt} from '../lib/ballistics.mjs';
const close=(a,b,tolerance=1e-7)=>assert.ok(Math.abs(a-b)<=tolerance,`${a} != ${b}`);
const residual=(s,x,y,g)=>Math.hypot(s.x*s.time-x,heightAt(s,g,s.time)-y);

test('browser ballistics matches 900 independently executed C# cases',()=>{
  const fixtures=JSON.parse(fs.readFileSync(new URL('./fixtures/ballistics-csharp.json',import.meta.url)));
  assert.equal(fixtures.length,900);
  for(const [op,args,expected] of fixtures) {
    const shot=op==='aim'?aim(...args.slice(0,4),Boolean(args[4])):op==='timed'?timed(...args):intercept(...args);
    assert.equal(shot.found,Boolean(expected[0]),`${op} ${args}`);
    [shot.x,shot.y,shot.time].forEach((v,i)=>close(v,expected[i+1],1e-7));
  }
});
test('fixed-target reachability agrees with closed form, including tangent and vertical roots',()=>{
  let count=0;
  for(const x of [4,18,30]) for(const y of [0,3,6]) for(const speed of [5,15,25]) for(const g of [2,9.81,20])
    for(const high of [false,true]) {
      const shot=aim(x,y,speed,g,high),disc=speed**4-g*(g*x*x+2*y*speed*speed);
      assert.equal(shot.found,disc>=0);
      if(shot.found){count++;close(shot.speed,speed);assert.ok(residual(shot,x,y,g)<1e-7);}
    }
  assert.equal(count,86);
  const tangent=aim(400/9.81,0,20,9.81);
  assert.ok(tangent.found);close(tangent.angle,45,1e-6);
  for(const [x,y] of [[0,3],[-12,2],[0,0]])assert.ok(residual(aim(x,y,20,9.81),x,y,9.81)<1e-7);
});
test('timed fuse, budget, and cover remain different constraints',()=>{
  const short=study(1,defaults),long=study(1,{...defaults,fuse:2}),over=study(1,{...defaults,speed:5});
  assert.ok(short.shot.found&&short.blocked&&short.gap<0);
  assert.ok(long.shot.found&&!long.blocked&&long.gap>1);
  assert.ok(long.shot.speed<short.shot.speed);
  assert.ok(!over.shot.found&&over.shot.time===1&&over.shot.speed>5);
  assert.equal(pointAt(over,{...defaults,speed:5},1,1).projectile,null);
  const stopped=pointAt(short,defaults,1,short.duration).projectile;
  close(stopped[0],18*.45);assert.ok(stopped[1]<3);
  assert.equal(clearance(aim(0,3,20,9.81),9.81,8,3),null);
});
test('moving interception beats current-position aiming; impossible and horizon cases reject',()=>{
  const result=study(2,defaults),point=pointAt(result,defaults,2,result.shot.time);
  assert.ok(result.shot.found&&result.referenceMiss>3);
  close(point.projectile[0],point.target[0]);close(point.projectile[1],point.target[1]);
  assert.ok(!intercept(18,0,8,0,8,9.81).found);
  assert.ok(!intercept(18,0,4,0,20,9.81,.1).found);
  assert.equal(study(2,{...defaults,speed:8,targetSpeed:8}).referenceMiss,null);
});
test('constructed moving-target trajectories provide 1000 independent feasible interception checks',()=>{
  let seed=3271;
  const random=()=>{seed=(Math.imul(seed,1664525)+1013904223)>>>0;return seed/4294967296;};
  for(let i=0;i<1000;i++){
    const t=.3+random()*3,g=2+random()*18,vx=8+random()*12,vy=g*t/2+random()*3;
    const ux=-4+random()*8,uy=-2+random()*4;
    const dx=(vx-ux)*t,dy=(vy-uy)*t-.5*g*t*t,speed=Math.hypot(vx,vy);
    const shot=intercept(dx,dy,ux,uy,speed,g,4);
    assert.ok(shot.found);assert.ok(shot.time<=t+1e-6);
    assert.ok(residual(shot,dx+ux*shot.time,dy+uy*shot.time,g)<1e-6);close(shot.speed,speed,1e-6);
  }
});
test('each learning challenge requires a meaningful change and keeps its controlled inputs fixed',()=>{
  for(let mode=0;mode<3;mode++)assert.ok(!challengeMet(mode,defaults));
  assert.ok(challengeMet(0,{...defaults,high:true}));
  assert.ok(challengeMet(1,{...defaults,fuse:2}));
  assert.ok(challengeMet(2,{...defaults,targetSpeed:6}));
  for(let mode=0;mode<3;mode++)assert.ok(!challengeMet(mode,{...defaults,high:true,fuse:2,targetSpeed:6,distance:10}));
});
test('supported control corners produce finite measurements and bounded inspection positions',()=>{
  for(const distance of [4,30])for(const height of [0,6])for(const speed of [5,25])for(const gravity of [2,20])
    for(const fuse of [.75,3])for(const targetSpeed of [-8,8])for(const high of [false,true])for(let mode=0;mode<3;mode++){
      const p={distance,height,speed,gravity,fuse,targetSpeed,high},r=study(mode,p);
      for(const value of [r.duration,r.impactTime,r.residual,r.gap,r.referenceMiss,...Object.values(r.shot)])
        if(typeof value==='number')assert.ok(Number.isFinite(value));
      for(const t of [0,r.duration]){
        const point=pointAt(r,p,mode,t);assert.ok([...point.target,...point.projectile??[]].every(Number.isFinite));
      }
    }
});
test('invalid inputs fail explicitly before entering the root solver',()=>{
  for(const bad of [NaN,Infinity,-Infinity,0,-1,1001])assert.throws(()=>aim(18,0,bad,9.81),RangeError);
  assert.throws(()=>timed(18,0,20,9.81,0),RangeError);
  assert.throws(()=>intercept(18,0,Infinity,0,20,9.81),RangeError);
  assert.throws(()=>intercept(18,0,4,0,20,9.81,61),RangeError);
  assert.throws(()=>study(9,defaults),RangeError);
});
