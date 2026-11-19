import test from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { grid, scatter, seededRandom, footprint, minimumDistance, surfaceLayout } from '../lib/placement.mjs';

const reference = JSON.parse(readFileSync(new URL('./fixtures/placement-csharp.json', import.meta.url), 'utf8'));

test('seeded browser streams match independent C# execution, including signed seed boundaries', () => {
  for (const fixture of reference.streams) {
    const random = seededRandom(fixture.seed);
    assert.deepEqual(fixture.values.map(() => random()), fixture.values);
  }
});

test('scatter points match the actual C# placement model for regular, crowded and negative-seed requests', () => {
  for (const fixture of reference.scatter) {
    const result = scatter(fixture.count, fixture.width, fixture.spacing, fixture.seed);
    assert.deepEqual(result.points, fixture.points);
    assert.ok(result.attempts <= result.budget);
  }
});

test('grid challenge is reachable and partial rows preserve the requested count', () => {
  assert.deepEqual(footprint(grid(36, 6, 2)), [10, 10]);
  assert.deepEqual(footprint(grid(36, 6, 1)), [5, 5]);
  assert.equal(minimumDistance(grid(36, 6, 1)), 1);
  assert.deepEqual(grid(7, 3, 2).at(-1), [0, 4]);
  assert.deepEqual(grid(0, 3, 2), []);
  assert.equal(minimumDistance([]), null);
});

test('scatter preserves bounds, useful spacing and a bounded attempt budget across exposed controls', () => {
  for (const count of [1, 36, 200]) for (const width of [2, 16, 20])
    for (const spacing of [.5, 1, 2, 4]) for (const seed of [0, 42, 100]) {
      const result = scatter(count, width, spacing, seed);
      assert.ok(result.points.length <= count);
      assert.ok(result.points.every(([x, z]) => x >= 0 && x <= width && z >= 0 && z <= width));
      const nearest = minimumDistance(result.points);
      assert.ok(nearest === null || nearest >= spacing - 1e-6);
      assert.ok(result.attempts <= count * 40);
    }
});

test('crowded stress case reports a shortfall and the spaced-tree challenge remains reachable', () => {
  const crowded = scatter(200, 2, 2, 42);
  assert.equal(crowded.points.length, 2);
  assert.equal(crowded.attempts, 8000);
  const spaced = scatter(36, 16, 2, 42);
  assert.equal(spaced.points.length, 36);
  assert.ok(minimumDistance(spaced.points) >= 2);
  assert.deepEqual(scatter(36, 16, 2, 42), spaced);
  assert.notDeepEqual(scatter(36, 16, 2, 43).points, spaced.points);
});

test('surface acceptance and orientation can be changed independently on the same slope', () => {
  assert.equal(surfaceLayout(30, 15).points.length, 0);
  assert.equal(surfaceLayout(30, 45).points.length, 9);
  assert.equal(surfaceLayout(30, 30).points.length, 9);
  const upright = surfaceLayout(30, 45, false), aligned = surfaceLayout(30, 45, true);
  assert.deepEqual(upright.points.map(p => p.position), aligned.points.map(p => p.position));
  assert.equal(upright.alignmentError, 30);
  assert.equal(aligned.alignmentError, 0);
  for (const angle of [0, 15, 30, 45, 60]) {
    const result = surfaceLayout(angle, 60);
    assert.ok(Math.abs(Math.hypot(...result.normal) - 1) < 1e-12);
    assert.ok(result.points.every(p => p.position.every(Number.isFinite)));
  }
});

test('invalid or unrepresentable placement inputs fail before generating a layout', () => {
  for (const spacing of [0, -1, NaN, Infinity, 101, 1e-60]) {
    assert.throws(() => grid(36, 6, spacing), RangeError);
    assert.throws(() => scatter(36, 16, spacing, 42), RangeError);
  }
  for (const count of [-1, 201, 1.5]) assert.throws(() => grid(count, 6, 2), RangeError);
  for (const width of [0, -1, NaN, Infinity, Number.MAX_VALUE]) assert.throws(() => scatter(36, width), RangeError);
  assert.throws(() => seededRandom(2147483648), RangeError);
  assert.throws(() => surfaceLayout(90, 45), RangeError);
});
