'use client';
import MixingDesk from './MixingDesk';
import {learningPaths} from '../lib/learning-paths';
import {useId,useMemo,useState} from 'react';
import ExperimentTimeline from './ExperimentTimeline';
import {defaults,comparison,inspect,obstacle,agentRadius,type SteeringParameters,type SteeringRun} from '../lib/steering.mjs';
const names=['Dock at a loading bay','Avoid a storage drum','Pursue a moving courier'];
const tasks=[
 'A delivery robot must slow down before a 12 m loading bay. Compare overshoot and settling time while changing only the slowing radius.',
 'A service robot must get around one storage drum. Compare late detection with an earlier look ahead, using the same acceleration limit.',
 'A guard drone must catch a courier moving across its view. Compare steering toward the current position with a short velocity prediction.'
];
const rules=['Slowing radius · 0.5 m','Look ahead · 0.2 s','Prediction horizon · 0 s'];
const goals=[
 'Keep speed 3 m/s and acceleration 4 m/s². Settle with less than 0.05 m overshoot in under 5.5 seconds.',
 'Keep speed 3 m/s and acceleration 4 m/s². Reach the destination with at least 0.3 m measured edge clearance.',
 'Keep speed 3 m/s and acceleration 4 m/s². Capture within 3.45 seconds and save at least 0.15 seconds against direct pursuit.'
];
const uses=[
 'Use arrival steering for docking robots, NPC destinations and smooth movement toward waypoints. Avoid it for guaranteed stopping distances, vehicle tire dynamics or collision-constrained navigation. A large slowing radius can eliminate overshoot while making arrival unnecessarily slow.',
 'Use look-ahead steering for a known local obstacle with a free bypass lane. This bounded lesson chooses a fixed bypass marker at (10, 2.8) after detecting the drum. It is not a global path planner: dead ends, moving obstacles and multiple blocked lanes need navigation and broader collision handling.',
 'Use pursuit for guards, companion drones or moving pickups when target velocity is reasonably steady. Avoid assuming a constant-velocity estimate will predict turns, network delay or evasive opponents. This example captures contact, not combat or adversarial behavior.'
];
function Range({label,value,min,max,step=1,unit,onChange}:{label:string;value:number;min:number;max:number;step?:number;unit:string;onChange:(v:number)=>void}){
 const id=useId();return <label htmlFor={id}>{label}<output htmlFor={id}>{value.toFixed(step===.25?2:step<1?1:0)} {unit}</output><input id={id} type="range" min={min} max={max} step={step} value={value} onChange={e=>onChange(Number(e.target.value))}/></label>;
}
export default function SteeringStudy(){const [mode,setMode]=useState(0);return <section className="steering-suite"><nav className="health-tabs" aria-label="Steering applications">{names.map((name,i)=><button key={name} aria-pressed={mode===i} onClick={()=>setMode(i)}>{name}</button>)}</nav><Lesson key={mode} mode={mode}/></section>;}
function Lesson({mode}:{mode:number}){
 const [p,setP]=useState<SteeringParameters>({...defaults}),[time,setTime]=useState(0);
 const {current,reference}=useMemo(()=>comparison(mode,p),[mode,p]);const duration=Math.max(current.duration,reference.duration);
 const frame=inspect(current,time);const fixed=p.speed===3&&p.acceleration===4;
 const met=fixed&&current.completed&&(mode===0?current.overshoot<.05&&current.duration<5.5:mode===1?current.closest>=.3:current.duration<3.45&&reference.duration-current.duration>=.15);
 function change(values:Partial<SteeringParameters>){setTime(0);setP(old=>({...old,...values}));}
 const metrics=mode===0?[
  ['Overshoot',`${current.overshoot.toFixed(2)} m`,`0.5 m reference: ${reference.overshoot.toFixed(2)} m beyond the destination`],
  ['Settling time',current.completed?`${current.duration.toFixed(2)} s`:'Not settled',`Reference: ${reference.completed?reference.duration.toFixed(2)+' s':'not settled in 12 s'}`],
  ['Final position error',`${current.error.toFixed(3)} m`,'Settled means distance <0.08 m and speed <0.1 m/s']
 ]:mode===1?[
  ['Minimum edge clearance',`${Math.max(0,current.closest).toFixed(2)} m`,current.collision?'Swept contact detected; run stops at the drum':'Measured along every movement segment, with the robot radius included'],
  ['Path travelled',`${current.path.toFixed(2)} m`,`Reference: ${reference.path.toFixed(2)} m ${reference.collision?'before collision':'to its last frame'}`],
  ['Destination time',current.completed?`${current.duration.toFixed(2)} s`:current.collision?'Collision':'Not reached',`Late-look reference: ${reference.collision?'collision':reference.completed?reference.duration.toFixed(2)+' s':'not reached'}`]
 ]:[
  ['Capture time',current.completed?`${current.duration.toFixed(2)} s`:'Not captured',`Direct pursuit: ${reference.completed?reference.duration.toFixed(2)+' s':'not captured in 8 s'}`],
  ['Time saved',current.completed&&reference.completed?`${(reference.duration-current.duration).toFixed(2)} s`:'—','Positive means prediction captured sooner'],
  ['Centre separation',`${current.error.toFixed(2)} m`,'Contact is 0.4 m: robot radius 0.3 m + courier radius 0.1 m']
 ];
 return <section className="practical steering-study"><header className="lesson-header"><h2>{names[mode]}</h2><p>{tasks[mode]}</p></header>
 <MixingDesk guide={learningPaths.steering[mode]} evidence={<>{current.collision?"Collision":current.completed?"Destination reached":"Time budget exhausted"} · {metrics[0][0]}: {metrics[0][1]}</>} preview={<><div className="steering-legend"><span>Orange · tuned robot</span><span>Dashed grey · reference</span><span>Teal · destination / courier</span></div>
<Map mode={mode} current={current} reference={reference} time={time} p={p}/></>} playback={<><ExperimentTimeline key={JSON.stringify(p)} t={time} setT={setTime} duration={duration} step="any"/></>} controls={<div className="mix-channels"><Range label="Speed limit" value={p.speed} min={1} max={5} step={.5} unit="m/s" onChange={speed=>change({speed})}/>
<Range label="Acceleration limit" value={p.acceleration} min={1} max={8} step={.5} unit="m/s²" onChange={acceleration=>change({acceleration})}/>
{mode===0?<Range label="Slowing radius" value={p.slowRadius} min={.25} max={6} step={.25} unit="m" onChange={slowRadius=>change({slowRadius})}/>:mode===1?<Range label="Look ahead" value={p.lookAhead} min={.1} max={3} step={.1} unit="s" onChange={lookAhead=>change({lookAhead})}/>:<Range label="Prediction horizon" value={p.prediction} min={0} max={3} step={.25} unit="s" onChange={prediction=>change({prediction})}/>}
<div className="feel-presets">{mode===0?<><button onClick={()=>change({slowRadius:.5})}>Late brake · 0.5 m</button><button onClick={()=>change({slowRadius:2})}>Balanced · 2 m</button><button onClick={()=>change({slowRadius:6})}>Cautious · 6 m</button></>:mode===1?<><button onClick={()=>change({lookAhead:.2})}>Late look · 0.2 s</button><button onClick={()=>change({lookAhead:2})}>Early look · 2 s</button></>:<><button onClick={()=>change({prediction:0})}>Direct pursuit</button><button onClick={()=>change({prediction:2})}>Predict · 2 s</button></>}</div>
<button className="button outline" onClick={()=>change(mode===0?{...defaults,speed:5,acceleration:1,slowRadius:.25}:mode===1?{...defaults,speed:5,acceleration:1,lookAhead:.1}:{...defaults,speed:1,prediction:2})}>Run stress case</button>
<button className="button dark" onClick={()=>change({...defaults})}>Reset lesson</button></div>} details={<><p className="chart-note">Top-down world metres; equal scale on both axes. Each run holds its whole frame at settlement, collision or capture. The reference keeps its own target and terminal frame.</p>
<p className="chart-note">Starts paused. Tuning restarts and pauses the lesson. Reduced-motion preference keeps manual inspection available.</p>
<div className="steering-state" aria-live="polite"><h3>Run outcome: {current.collision?'Collision detected':current.completed?(mode===2?'Courier captured':'Destination reached'):'Time budget exhausted'}</h3><p>{mode===0?`The robot passes the bay by ${current.overshoot.toFixed(2)} m. Start slowing earlier, then compare how long it takes to settle.`:mode===1?current.collision?'The look-ahead trigger came too late for the available turning acceleration. Try an earlier look ahead with the speed unchanged.':'Earlier detection gives the robot space to steer toward the fixed bypass marker, then return to the destination.':current.completed?'Prediction changes the aim point. The robot and courier stop at edge contact, so their centres remain 0.4 m apart.':'The courier was not captured within eight seconds. Prediction cannot compensate for an insufficient speed budget.'}</p></div><div className="mix-context"><h3>Change one thing</h3>
<p>Reference: {rules[mode]}. Other inputs stay shared between the two runs.</p>
<p className="chart-note">At this frame: speed {Math.hypot(...frame.velocity).toFixed(2)} m/s. Run peak acceleration {current.peakAcceleration.toFixed(2)} m/s².</p></div></>}/>
 <div className="tuning-brief"><h3>Read the run</h3><div className="metric-grid">{metrics.map(([name,value,note])=><div className="metric" key={name}><span>{name}</span><strong>{value}</strong><small>{note}</small></div>)}</div>
 <div className="placement-challenge"><h3>Try it</h3><p>{goals[mode]}</p><p role="status">{met?'Challenge met. Explain the trade-off against the fixed reference.':'Adjust the controls and compare the measured run.'}</p></div>
 <h3>Where it helps / where to avoid it</h3><p>{uses[mode]}</p>
 <h3>The shared idea</h3><p>Choose a desired velocity, turn the difference from current velocity into acceleration, cap that acceleration, then integrate motion. Arrival reduces desired speed near the goal. Avoidance changes the aim point after a local look-ahead trigger. Pursuit estimates a future aim point from target velocity.</p>
 <h3>Unity mapping</h3><p>Open <code>Assets/Scenes/SteeringLab.unity</code> or use Tools → Engineering Lab → Open Steering Behaviors. The matching C# model uses horizontal X/Z motion. It computes a deterministic teaching trajectory, independent of PhysX or NavMesh. A production controller needs collision handling, navigation, animation and agent-specific motion limits.</p><p className="chart-note">Steering passed ten grouped native model checks and thirteen scripted Play checks in Unity 6000.5.2f1. Hardware input, PhysX/NavMesh integration and exported player builds remain unverified.</p>
 <details className="steering-method"><summary>Repeat the experiment and inspect its limits</summary><p>Integration uses a fixed 1/120 s step, with desired velocity corrected over a 0.25 s response time. The model caps acceleration before applying a speed limit. Docking/avoidance runs stop after 12 seconds; pursuit after 8. Terminal poses hold for manual inspection. Swept circle checks find first contact between frames.</p><p>The obstacle centre is (8, 0) m with radius 1.3 m; robot radius is 0.3 m. A local detection switches to the fixed bypass marker (10, 2.8) m, retaining it until X reaches 10 m. This deliberately bounded rule exposes late detection and acceleration limits without claiming general route planning. The courier begins at (6, 3) m and moves at 1.2 m/s along the second axis. Prediction is capped by both the selected horizon and distance / speed.</p><p>Repeat the named presets, inspect terminal frames, then run the stress case and Reset. Tests compare C#-executed runs, halve the timestep and sweep all exposed parameter corners. Browser animation, C# source compilation, native Play checks and exported players are recorded separately in the repository’s progress log.</p></details>
 </div></section>;
}
function Map({mode,current,reference,time,p}:{mode:number;current:SteeringRun;reference:SteeringRun;time:number;p:SteeringParameters}){
 const a=inspect(current,time),b=inspect(reference,time);
 const points=[...current.frames,...reference.frames].flatMap(f=>[f.position,f.target,f.aim]);
 if(mode===0)points.push([12-p.slowRadius,-p.slowRadius],[12+p.slowRadius,p.slowRadius]);
 const minX=Math.min(-1,...points.map(v=>v[0])),maxX=Math.max(13,...points.map(v=>v[0]));
 const minY=Math.min(-2,...points.map(v=>v[1])),maxY=Math.max(4,...points.map(v=>v[1]));
 const scale=Math.min(520/(maxX-minX),235/(maxY-minY));
 const project=([x,y]:readonly number[])=>[60+(520-(maxX-minX)*scale)/2+(x-minX)*scale,50+(235-(maxY-minY)*scale)/2+(maxY-y)*scale];
 const path=(r:SteeringRun)=>r.frames.map((f,i)=>`${i?'L':'M'}${project(f.position).join(',')}`).join(' ');
 const [x,y]=project(a.position),[rx,ry]=project(b.position),[tx,ty]=project(a.target),[rtx,rty]=project(b.target),[ox,oy]=project(obstacle);
 return <svg viewBox="0 0 640 350" role="img" aria-label={`${names[mode]} at ${time.toFixed(2)} seconds; ${current.collision?'tuned run collides':current.completed?'tuned run completes':'tuned run times out'}`}>
 <rect width="640" height="350" rx="8" fill="#e9ede2"/><text x="25" y="25">Top-down motion · metres</text>
 {[0,.5,1].map(f=>{const wx=minX+(maxX-minX)*f,[px]=project([wx,0]);return <g key={f}><path d={`M${px},45V290`} stroke="#cdd7c5"/><text x={px} y="325" textAnchor="middle">{wx.toFixed(1)}</text></g>;})}
 {mode===0&&<circle cx={tx} cy={ty} r={p.slowRadius*scale} fill="#327773" fillOpacity=".07" stroke="#8ca79a" strokeDasharray="3 6"/>}
 {mode===1&&<><circle cx={ox} cy={oy} r={(obstacle[2]+agentRadius)*scale} fill="none" stroke="#657b67" strokeDasharray="4 4"/><circle cx={ox} cy={oy} r={obstacle[2]*scale} fill="#82927c"/><text x={ox} y={oy+5} textAnchor="middle">Drum</text></>}
 <path d={path(reference)} className="reference-line"/><path d={path(current)} fill="none" stroke="#c44825" strokeWidth="2.5"/>
 {mode===2?<><circle cx={rtx} cy={rty} r={.1*scale+2} fill="none" stroke="#657b67"/><circle cx={tx} cy={ty} r={.1*scale} fill="#327773"/><path d={`M${tx-9},${ty}h18 M${tx},${ty-9}v18`} stroke="#327773"/></>:<><circle cx={tx} cy={ty} r="7" fill="none" stroke="#327773" strokeWidth="2"/><path d={`M${tx-12},${ty}h24 M${tx},${ty-12}v24`} stroke="#327773"/></>}
 <circle cx={rx} cy={ry} r={agentRadius*scale} fill="#e9ede2" stroke="#657b67" strokeWidth="2"/>
 <circle cx={x} cy={y} r={agentRadius*scale} fill="#c44825" stroke="#f7f6f0" strokeWidth="1.5"/>
 </svg>;
}
