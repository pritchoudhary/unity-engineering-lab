// Seeded compatibility generator adapted from .NET Random.CompatImpl (MIT).
// Source and notice: docs/third-party-notices/dotnet-random.md.
const integerLimit = 2147483647;
const f32 = Math.fround;

function integer(value, min, max, name) {
  if (!Number.isInteger(value) || value < min || value > max) throw new RangeError(name);
}
function positive(value, name) {
  if (!Number.isFinite(value) || value <= 0) throw new RangeError(name);
}
function validate(count, spacing) {
  integer(count, 0, 200, 'count');
  positive(spacing, 'spacing');
  if (spacing > 100) throw new RangeError('spacing');
}

export function seededRandom(seed) {
  integer(seed, -2147483648, integerLimit, 'seed');
  const state = new Int32Array(56);
  let previous = 161803398 - (seed === -2147483648 ? integerLimit : Math.abs(seed));
  let next = 1;
  state[55] = previous;
  for (let index = 1; index < 55; index++) {
    const slot = (21 * index) % 55;
    state[slot] = next;
    next = (previous - next) | 0;
    if (next < 0) next += integerLimit;
    previous = state[slot];
  }
  for (let pass = 0; pass < 4; pass++) {
    for (let index = 1; index < 56; index++) {
      state[index] = (state[index] - state[1 + (index + 30) % 55]) | 0;
      if (state[index] < 0) state[index] += integerLimit;
    }
  }
  let current = 0, offset = 21;
  return () => {
    current = current === 55 ? 1 : current + 1;
    offset = offset === 55 ? 1 : offset + 1;
    let sample = (state[current] - state[offset]) | 0;
    if (sample === integerLimit) sample--;
    if (sample < 0) sample += integerLimit;
    state[current] = sample;
    return sample * (1 / integerLimit);
  };
}

export function grid(count = 36, columns = 6, spacing = 2) {
  spacing = f32(spacing);
  validate(count, spacing);
  integer(columns, 1, integerLimit, 'columns');
  return Array.from({ length: count }, (_, index) =>
    [f32((index % columns) * spacing), f32(Math.floor(index / columns) * spacing)]);
}

export function scatter(count = 36, width = 16, spacing = 2, seed = 42) {
  spacing = f32(spacing); width = f32(width);
  validate(count, spacing);
  positive(width, 'width');
  const random = seededRandom(seed);
  const points = [];
  const threshold = f32(spacing * spacing);
  let attempts = 0;
  while (attempts < count * 40 && points.length < count) {
    attempts++;
    const candidate = [f32(f32(random()) * width), f32(f32(random()) * width)];
    const clear = points.every(point => {
      const dx = f32(point[0] - candidate[0]), dz = f32(point[1] - candidate[1]);
      return f32(f32(dx * dx) + f32(dz * dz)) >= threshold;
    });
    if (clear) points.push(candidate);
  }
  return { points, attempts, budget: count * 40 };
}

export function footprint(points) {
  if (!points.length) return [0, 0];
  const x = points.map(point => point[0]), z = points.map(point => point[1]);
  return [Math.max(...x) - Math.min(...x), Math.max(...z) - Math.min(...z)];
}

export function minimumDistance(points) {
  if (points.length < 2) return null;
  let minimum = Infinity;
  for (let i = 0; i < points.length; i++)
    for (let j = 0; j < i; j++)
      minimum = Math.min(minimum, Math.hypot(points[i][0] - points[j][0], points[i][1] - points[j][1]));
  return minimum;
}

// Ideal planar surface. Native Unity projects candidates using collider raycasts.
export function surfaceLayout(angle = 30, slopeLimit = 45, aligned = true) {
  if (!Number.isFinite(angle) || angle < 0 || angle > 60 ||
      !Number.isFinite(slopeLimit) || slopeLimit < 0 || slopeLimit > 85) throw new RangeError('slope');
  const radians = angle * Math.PI / 180;
  const normal = [0, Math.cos(radians), Math.sin(radians)];
  const points = angle > slopeLimit ? [] : grid(9, 3, 2).map(([x, z]) => {
    const surfaceZ = z - 2;
    return {
      position: [x - 2, -Math.tan(radians) * surfaceZ + normal[1] * .5, surfaceZ + normal[2] * .5],
      up: aligned ? normal : [0, 1, 0]
    };
  });
  return { points, normal, alignmentError: aligned ? 0 : angle };
}
