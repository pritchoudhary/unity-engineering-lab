'use client';
import MixingDesk from './MixingDesk';

import { useId, useMemo, useState } from 'react';
import { grid, scatter, footprint, minimumDistance, surfaceLayout } from '../lib/placement.mjs';

const names = ['Exhibit grid', 'Hillside signs', 'Vegetation scatter'];
const tasks = [
  'Arrange 36 exhibit pivots within a six-metre square. Compare a tighter layout with the fixed two-metre reference.',
  'A sign should stand away from the hillside along its surface normal. Compare slope filtering and alignment as separate decisions.',
  'Fill a patch with vegetation while keeping useful space between pivots. Compare the same candidate stream at different minimum distances.'
];
const guidance = [
  'Use a grid for regular exhibits, modular rooms and shelving. Choose path-based placement for curved streets. Pivot spacing alone does not account for the size of each exhibit.',
  'Use surface alignment for signs, rocks and ground props. Buildings often need level foundations. Real scenes also need collider selection, pivot offsets and checks for overhangs.',
  'Use constrained scatter for vegetation and irregular decor. Bounded rejection can leave a dense request partly filled. Navigation, species rules and mesh clearance need additional constraints.'
];
const mappings = [
  'Unity generates row and column positions, instantiates primitives or prefab instances, and groups the operation for Undo.',
  'Unity casts downward onto colliders, filters hit normals by slope, and rotates local up onto the accepted normal. This browser exercise uses an ideal plane.',
  'Unity uses System.Random with the supplied seed and tries at most 40 candidates per requested object. The browser reproduces that seeded stream and float-position arithmetic.'
];

function Range({ label, value, min, max, step = 1, unit = '', onChange }: {
  label: string; value: number; min: number; max: number; step?: number; unit?: string;
  onChange: (value: number) => void;
}) {
  const id = useId();
  return <label htmlFor={id}>{label}<output htmlFor={id}>{value}{unit}</output>
    <input id={id} type="range" min={min} max={max} step={step} value={value}
      onChange={event => onChange(Number(event.target.value))} />
  </label>;
}

export default function PlacementStudy() {
  const [mode, setMode] = useState(0);
  return <section>
    <nav className="health-tabs" aria-label="Placement applications">
      {names.map((name, index) => <button key={name} aria-pressed={mode === index}
        onClick={() => setMode(index)}>{name}</button>)}
    </nav>
    <Study key={mode} mode={mode} />
  </section>;
}

function Study({ mode }: { mode: number }) {
  const [spacing, setSpacing] = useState(mode === 2 ? 1 : 2);
  const [columns, setColumns] = useState(6);
  const [angle, setAngle] = useState(30);
  const [limit, setLimit] = useState(45);
  const [aligned, setAligned] = useState(false);
  const [count, setCount] = useState(36);
  const [width, setWidth] = useState(16);
  const [seed, setSeed] = useState(42);
  const [shown, setShown] = useState(200);

  const currentScatter = useMemo(() => scatter(count, width, spacing, seed), [count, width, spacing, seed]);
  const referenceScatter = useMemo(() => scatter(count, width, 1, seed), [count, width, seed]);
  const surface = surfaceLayout(angle, limit, aligned);
  const current = mode === 0 ? grid(36, columns, spacing) : currentScatter.points;
  const reference = mode === 0 ? grid(36, columns, 2) : referenceScatter.points;
  const points = mode === 1 ? surface.points.map(point => point.position) : current.map(([x, z]) => [x, 0, z]);
  const referencePoints = mode === 1 ? surfaceLayout(angle, 15, false).points.map(point => point.position)
    : reference.map(([x, z]) => [x, 0, z]);
  const size = footprint(current);
  const nearest = minimumDistance(current);
  const requested = mode === 1 ? 9 : mode === 0 ? 36 : count;
  const visible = Math.min(shown, points.length);
  const achieved = mode === 0 ? size.every(side => side <= 6) && spacing >= 1
    : mode === 1 ? angle === 30 && limit <= 45 && points.length === 9 && aligned
    : count === 36 && width === 16 && seed === 42 && spacing >= 2 && points.length === 36;
  const goal = mode === 0 ? 'Fit 36 pivots into 6 × 6 m with at least 1 m spacing.'
    : mode === 1 ? 'At a 30° slope, accept all nine signs and align their up axes. Keep the slope limit at or below 45°.'
    : 'Accept 36 trees in a 16 m square with at least 2 m requested spacing. Keep seed 42 for a fair comparison.';

  function reset() {
    setSpacing(mode === 2 ? 1 : 2); setColumns(6); setAngle(30); setLimit(45);
    setAligned(false); setCount(36); setWidth(16); setSeed(42); setShown(200);
  }

  const metrics = mode === 0 ? [
    ['Pivot footprint', `${size[0].toFixed(1)} × ${size[1].toFixed(1)} m`, `Two-metre reference: ${footprint(reference).map(v => v.toFixed(1)).join(' × ')} m`],
    ['Nearest pivots', `${nearest?.toFixed(2)} m`, 'Measured Euclidean separation in the ground plane'],
    ['Requested objects', '36', `${columns} columns · ${Math.ceil(36 / columns)} rows`]
  ] : mode === 1 ? [
    ['Accepted signs', `${points.length} / 9`, `Fixed 15° reference accepts ${referencePoints.length}`],
    ['Up-axis error', points.length ? `${surface.alignmentError.toFixed(1)}°` : '—', 'Accepted signs · 0° means aligned to the surface'],
    ['Surface / limit', `${angle}° / ${limit}°`, 'Change the limit while keeping the slope fixed']
  ] : [
    ['Accepted trees', `${points.length} / ${count}`, `One-metre reference accepts ${referencePoints.length}`],
    ['Nearest pivots', nearest === null ? '—' : `${nearest.toFixed(2)} m`, `Requested minimum: ${spacing.toFixed(1)} m`],
    ['Candidate attempts', `${currentScatter.attempts} / ${currentScatter.budget}`, 'Budget is 40 attempts per requested object']
  ];

  return <section className="practical placement-study">
    <header className="lesson-header"><h2>{names[mode]}</h2><p>{tasks[mode]}</p></header>
    <MixingDesk preview={<><LayoutView mode={mode} points={points.slice(0, visible)} allPoints={points}
          reference={referencePoints} angle={angle} aligned={aligned} width={width} /></>} playback={<><Range label="Reveal placements" value={visible} min={0} max={points.length}
          onChange={setShown} unit={` / ${points.length}`} />
<div className="feel-presets">
          <button onClick={() => setShown(0)}>Inspect from start</button>
          <button onClick={() => setShown(200)}>Show all</button>
        </div></>} controls={<div className="mix-channels">{mode === 0 && <>
          <Range label="Spacing" value={spacing} min={.5} max={3} step={.25} unit=" m" onChange={setSpacing} />
          <Range label="Columns" value={columns} min={3} max={12} onChange={setColumns} />
          <div className="feel-presets">
            <button onClick={() => setSpacing(1)}>Compact · 1 m</button>
            <button onClick={() => setSpacing(2)}>Standard · 2 m</button>
          </div>
        </>}
{mode === 1 && <>
          <Range label="Surface slope" value={angle} min={0} max={60} step={5} unit="°" onChange={setAngle} />
          <Range label="Maximum slope" value={limit} min={0} max={60} step={5} unit="°" onChange={setLimit} />
          <label className="instant-option"><input type="checkbox" checked={aligned}
            onChange={event => setAligned(event.target.checked)} /> Align local up to the surface</label>
          <div className="feel-presets">
            <button onClick={() => setLimit(15)}>Strict · 15°</button>
            <button onClick={() => setLimit(45)}>Allow · 45°</button>
          </div>
        </>}
{mode === 2 && <>
          <Range label="Minimum spacing" value={spacing} min={.5} max={4} step={.25} unit=" m" onChange={setSpacing} />
          <Range label="Patch width" value={width} min={2} max={20} unit=" m" onChange={setWidth} />
          <Range label="Requested trees" value={count} min={1} max={200} onChange={setCount} />
          <Range label="Random seed" value={seed} min={0} max={100} onChange={setSeed} />
          <div className="feel-presets">
            <button onClick={() => { setCount(200); setWidth(2); setSpacing(2); setSeed(42); setShown(200); }}>
              Crowded patch
            </button>
            <button onClick={() => { setCount(36); setWidth(16); setSpacing(2); setSeed(42); setShown(200); }}>
              Spaced · 2 m
            </button>
          </div>
        </>}
<button className="button dark" onClick={reset}>Reset lesson</button></div>} details={<><p className="chart-note">{mode === 1 ? `Surface ${angle}°. ` : mode === 2 ? `${width} × ${width} m patch. ` : '36 exhibit pivots. '}Orange marks the current layout. Grey marks the fixed reference.
          Both share one scale, fitted to their combined extent. Symbols identify pivots.</p><div className="mix-context"><h3>Change one thing</h3></div></>}/>
    <div className="tuning-brief">
      <h3>Read the result</h3>
      <div className="metric-grid">{metrics.map(([label, value, note]) =>
        <div className="metric" key={label}><span>{label}</span><strong>{value}</strong><small>{note}</small></div>)}</div>
      {mode === 2 && points.length < count && <p role="status">
        The request exceeded what this bounded sampler could fit. Reduce spacing, enlarge the patch, or request fewer trees.
        A shortfall describes this attempt budget and seed; it does not prove an optimal packing limit.
      </p>}
      {mode === 1 && points.length === 0 && <p role="status">
        All candidates were rejected by the slope limit. Raise the limit while keeping the surface angle fixed.
      </p>}
      <div className="placement-challenge"><h3>Try it</h3><p>{goal}</p>
        <p role="status">{achieved ? 'Challenge met. Explain which constraint changed the outcome.' : 'Adjust the controls and compare the measurements.'}</p></div>
      <h3>Where it helps</h3><p>{guidance[mode]}</p>
      <h3>Unity mapping</h3><p>{mappings[mode]}</p>
      <p className="chart-note">Open Assets/Scenes/ScenePlacement.unity in the repository’s Unity project.
        The native Editor workflow passed 18 checks covering prefab links, placement transforms, slope filtering and grouped Undo/Redo.
        This page is the browser teaching model. Native mouse routing and player exports remain unverified.</p>
      <details className="placement-coordinates"><summary>Inspect accepted pivot coordinates</summary>
        <div><table><caption>Current layout in world metres</caption><thead><tr><th>Object</th><th>X</th><th>Y</th><th>Z</th></tr></thead>
          <tbody>{points.map((point, index) => <tr key={index}><td>{index + 1}</td>
            {point.map((coordinate, axis) => <td key={axis}>{coordinate.toFixed(3)}</td>)}</tr>)}</tbody></table></div>
      </details>
    </div>
  </section>;
}

function LayoutView({ mode, points, allPoints, reference, angle, aligned, width }: {
  mode: number; points: number[][]; allPoints: number[][]; reference: number[][];
  angle: number; aligned: boolean; width: number;
}) {
  const radians = angle * Math.PI / 180;
  const boundary = mode === 1 ? [[-4, 0, -4], [4, 0, -4], [4, 0, 4], [-4, 0, 4]]
    .map(([x, , z]) => [x, -Math.tan(radians) * z, z])
    : mode === 2 ? [[0, 0, 0], [width, 0, 0], [width, 0, width], [0, 0, width]] : [];
  const raw = ([x, y, z]: number[]) => [x - z, (x + z) * .45 - y];
  const bounds = [...allPoints, ...reference, ...boundary, [0, 0, 0]].map(raw);
  const xs = bounds.map(point => point[0]), ys = bounds.map(point => point[1]);
  const minX = Math.min(...xs), maxX = Math.max(...xs), minY = Math.min(...ys), maxY = Math.max(...ys);
  const scale = Math.min(500 / Math.max(1, maxX - minX), 225 / Math.max(1, maxY - minY));
  function project(point: number[]) {
    const [x, y] = raw(point);
    return [320 + (x - (minX + maxX) / 2) * scale, 180 + (y - (minY + maxY) / 2) * scale];
  }
  const polygon = (vertices: number[][]) => vertices.map(point => project(point).join(',')).join(' ');
  return <svg viewBox="0 0 640 380" role="img" aria-label={`${names[mode]}, ${points.length} visible current pivots, ${reference.length} reference pivots`}>
    <rect width="640" height="380" fill="#e9ede2" />
    {boundary.length > 0 && <polygon points={polygon(boundary)} fill="#cfdbc7" stroke="#77846f" />}
    {reference.map((point, index) => {
      const [x, y] = project(point);
      return <path key={index} d={`M${x - 4},${y}h8 M${x},${y - 4}v8`} stroke="#687665" strokeWidth="1.5" />;
    })}
    {points.map((point, index) => {
      const [x, y] = project(point);
      if (mode === 1) {
        const up = aligned ? [0, Math.cos(radians), Math.sin(radians)] : [0, 1, 0];
        const top = point.map((value, axis) => value + up[axis] * .65);
        const board = [[-.3, -.2], [.3, -.2], [.3, .2], [-.3, .2]].map(([dx, dy]) =>
          top.map((value, axis) => value + (axis === 0 ? dx : up[axis] * dy)));
        const [tx, ty] = project(top);
        return <g key={index}><path d={`M${x},${y}L${tx},${ty}`} stroke="#25372d" strokeWidth="3" />
          <polygon points={polygon(board)} fill="#c44825" /></g>;
      }
      return mode === 2 ? <g key={index}><path d={`M${x},${y}v-7`} stroke="#25372d" strokeWidth="2" />
        <circle cx={x} cy={y - 10} r="5" fill="#c44825" /></g>
        : <g key={index}><path d={`M${x},${y - 4}l6,4 -6,4 -6,-4z`} fill="#c44825" /></g>;
    })}
  </svg>;
}
