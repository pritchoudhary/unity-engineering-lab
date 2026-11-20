// Same constant-gravity equations and positive-root isolation as BallisticModel.cs.
const empty = () => ({ found: false, x: 0, y: 0, time: 0, speed: 0, angle: 0 });
function validate(x, y, speed, gravity) {
  if (![x, y, speed, gravity].every(Number.isFinite) || Math.abs(x) > 10000 ||
      Math.abs(y) > 10000 || speed < .1 || speed > 1000 || gravity < .1 || gravity > 1000)
    throw new RangeError('Ballistic inputs');
}
function velocity(dx, dy, gravity, time, limit) {
  const x = dx / time, y = dy / time + .5 * gravity * time, speed = Math.hypot(x, y);
  return { found: speed <= limit + 1e-6, x, y, time, speed, angle: Math.atan2(y, x) * 180 / Math.PI };
}
function evaluate(c, t) { let v = 0; for (let i = c.length - 1; i >= 0; i--) v = v * t + c[i]; return v; }
function nearZero(c, t) {
  let scale = 0, power = 1;
  for (const v of c) { scale += Math.abs(v) * power; power *= t; }
  return Math.abs(evaluate(c, t)) <= 1e-10 * Math.max(1e-30, scale);
}
function roots(c, low, high) {
  let degree = c.length - 1;
  while (degree > 0 && c[degree] === 0) degree--;
  if (!degree) return [];
  if (degree === 1) { const t = -c[0] / c[1]; return t >= low && t <= high ? [t] : []; }
  const derivative = c.slice(1, degree + 1).map((v, i) => v * (i + 1));
  const cuts = [low, ...roots(derivative, low, high), high], result = [];
  const add = t => { if (result.every(other => Math.abs(other - t) > 1e-6)) result.push(t); };
  for (const t of cuts) if (nearZero(c, t)) add(t);
  for (let i = 1; i < cuts.length; i++) {
    let left = cuts[i - 1], right = cuts[i], a = evaluate(c, left);
    if (nearZero(c, left) || nearZero(c, right) || Math.sign(a) === Math.sign(evaluate(c, right))) continue;
    for (let step = 0; step < 70; step++) {
      const middle = (left + right) * .5, value = evaluate(c, middle);
      if (Math.sign(value) === Math.sign(a)) { left = middle; a = value; } else right = middle;
    }
    add((left + right) * .5);
  }
  return result.sort((a, b) => a - b);
}
export function aim(dx, dy, speed, gravity, high = false) {
  validate(dx, dy, speed, gravity);
  if (dx === 0 && dy === 0) return velocity(0, 0, gravity, 2 * speed / gravity, speed);
  const maximum = (speed + Math.sqrt(speed * speed + 2 * gravity * Math.abs(dy))) / gravity + 1;
  const times = roots([dx * dx + dy * dy, 0, gravity * dy - speed * speed, 0, .25 * gravity * gravity], 1e-6, maximum);
  return times.length ? velocity(dx, dy, gravity, times[high ? times.length - 1 : 0], speed) : empty();
}
export function timed(dx, dy, limit, gravity, time) {
  validate(dx, dy, limit, gravity);
  if (!Number.isFinite(time) || time <= 0 || time > 60) throw new RangeError('Flight time');
  return velocity(dx, dy, gravity, time, limit);
}
export function intercept(dx, dy, ux, uy, speed, gravity, horizon = 8) {
  validate(dx, dy, speed, gravity);
  // Keep coefficients bounded as well as finite. UI velocities stay within ±8 m/s.
  if (![ux, uy, horizon].every(Number.isFinite) || Math.abs(ux) > 1000 || Math.abs(uy) > 1000 || horizon <= 0 || horizon > 60)
    throw new RangeError('Target velocity or horizon');
  const times = roots([dx * dx + dy * dy, 2 * (dx * ux + dy * uy),
    ux * ux + uy * uy + gravity * dy - speed * speed, gravity * uy, .25 * gravity * gravity], 1e-6, horizon);
  return times.length ? velocity(dx + ux * times[0], dy + uy * times[0], gravity, times[0], speed) : empty();
}
export const heightAt = (shot, gravity, t) => shot.y * t - .5 * gravity * t * t;
export function clearance(shot, gravity, wallX, wallHeight) {
  if (shot.time <= 0 || shot.x === 0) return null;
  const t = wallX / shot.x;
  return t < 0 || t > shot.time ? null : heightAt(shot, gravity, t) - wallHeight;
}
export const defaults = Object.freeze({ distance: 18, height: 0, speed: 20, gravity: 9.81, fuse: 1, targetSpeed: 4, high: false });
export function study(mode, p) {
  const { distance: dx, height: dy, speed, gravity, fuse, targetSpeed, high } = p;
  if (![0, 1, 2].includes(mode)) throw new RangeError('Study');
  const shot = mode === 0 ? aim(dx, dy, speed, gravity, high) : mode === 1 ? timed(dx, dy, speed, gravity, fuse)
    : intercept(dx, dy, targetSpeed, 0, speed, gravity);
  const reference = mode === 0 ? aim(dx, dy, 20, gravity) : mode === 1 ? timed(dx, dy, speed, gravity, 1)
    : aim(dx, dy, speed, gravity);
  const gap = mode === 1 ? clearance(shot, gravity, dx * .45, 3) : null;
  const blocked = mode === 1 && gap !== null && gap < 0;
  const impactTime = blocked && shot.x > 0 ? Math.min(shot.time, dx * .45 / shot.x) : shot.time;
  const residual = shot.time > 0 ? Math.hypot(shot.x * shot.time - dx - (mode === 2 ? targetSpeed * shot.time : 0),
    heightAt(shot, gravity, shot.time) - dy) : null;
  return { shot, reference, gap, blocked, impactTime, residual,
    duration: Math.max(1, shot.time, reference.time),
    referenceMiss: mode === 2 && reference.found ? Math.abs(targetSpeed * reference.time) : null };
}
export function challengeMet(mode, p) {
  const result = study(mode, p);
  const fixed = p.distance === 18 && p.height === 0 && p.speed === 20 && p.gravity === 9.81;
  return fixed && result.shot.found && (mode === 0 ? p.high && result.shot.time > 3.5
    : mode === 1 ? !result.blocked && result.gap >= 1 : p.targetSpeed === 6 && result.referenceMiss > 5 && result.residual < .001);
}
export function pointAt(result, p, mode, time) {
  const sample = Math.min(Math.max(0, time), result.impactTime);
  return { projectile: result.shot.found ? [result.shot.x * sample, heightAt(result.shot, p.gravity, sample)] : null,
    target: [p.distance + (mode === 2 ? p.targetSpeed * time : 0), p.height] };
}
