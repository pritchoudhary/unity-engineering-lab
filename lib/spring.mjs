// SI units. Semi-implicit Euler, with an explicit bounded timestep.
export const STEP = 1 / 240;
export function validate(p) {
  if (!Number.isFinite(p.mass) || p.mass < 0.25 || p.mass > 5 || !Number.isFinite(p.stiffness) || p.stiffness < 1 || p.stiffness > 100 || !Number.isFinite(p.damping) || p.damping < 0 || p.damping > 50) throw new RangeError('Spring parameters outside supported range');
}
export function step(s, p, dt = STEP) {
  validate(p);
  if (!Number.isFinite(dt) || dt <= 0 || dt > STEP || !Number.isFinite(s.x) || !Number.isFinite(s.v)) throw new RangeError('Invalid timestep or state');
  const v = s.v + (-p.stiffness * s.x - p.damping * s.v) / p.mass * dt;
  return { x: s.x + v * dt, v };
}
export function energy(s,p) { return 0.5 * p.mass * s.v ** 2 + 0.5 * p.stiffness * s.x ** 2; }
export function ratio(p) { return p.damping / (2 * Math.sqrt(p.mass * p.stiffness)); }
export function trajectory(p, x = 1.5, seconds = 8) {
  let s = {x,v:0}; const points = [{t:0,x}];
  for (let i=1;i<=Math.round(seconds/STEP);i++) { s=step(s,p); if(i%8===0) points.push({t:i*STEP,x:s.x}); }
  return points;
}
// Follow a destination while preserving world-space position and velocity.
export function follow(s, target, p, dt = STEP) {
  if (!Number.isFinite(target)) throw new RangeError('Invalid target');
  const next = step({x:s.x-target,v:s.v},p,dt);
  return {x:next.x+target,v:next.v};
}
