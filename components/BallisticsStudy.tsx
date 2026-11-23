'use client';

import { useEffect, useId, useMemo, useState } from 'react';
import { defaults, study, challengeMet, pointAt, heightAt, type Parameters, type Shot, type StudyResult } from '../lib/ballistics.mjs';

const names = ['Projectile aiming', 'Timed grenade', 'Moving target'];
const tasks = [
  'Aim an artillery indicator or preview a jump landing. Hold launch speed fixed and compare the two arcs that can reach the same point.',
  'Plan a throwable that arrives when its fuse expires. A small change in fuse duration can clear cover while requiring less launch speed.',
  'Lead a steadily moving enemy. Aim where the target will be at arrival, then compare with aiming at its current position.'
];
const references = ['20 m/s low arc', '1.00 s fuse', 'Aim at current position'];
const goals = [
  'Keep the study at 18 m distance, 0 m height, 20 m/s and 9.81 m/s². Find the arc that stays airborne for more than 3.5 seconds.',
  'Keep distance 18 m, height 0 m, speed budget 20 m/s and gravity 9.81 m/s². Change only the fuse to clear the 3 m cover by at least 1 m.',
  'Keep distance 18 m, height 0 m, launch speed 20 m/s and gravity 9.81 m/s². Lead a target at +6 m/s. Make the tuned error less than 1 mm while the current-position reference misses by more than 5 m.'
];
const guidance = [
  'Use for ideal artillery aiming, launcher indicators and jump landing previews. Avoid this model when drag, wind or changing gravity dominates. A landing preview also needs checks for reachable surfaces and the character’s collision shape.',
  'Use for fuse-controlled throwables or timed delivery previews. The thin cover plane illustrates a constraint. Real grenades need swept collision, radius, bounce, detonation timing and damage logic; this model implements none of those.',
  'Use for a lead indicator against steadily moving enemies. Avoid treating constant-velocity prediction as reliable for turning, accelerating or network-delayed targets. The solver searches only the next eight seconds.'
];

function Range({label,value,min,max,step=1,unit='',onChange}:{label:string;value:number;min:number;max:number;step?:number;unit?:string;onChange:(v:number)=>void}) {
  const id=useId();
  return <label htmlFor={id}>{label}<output htmlFor={id}>{value.toFixed(step < .1 ? 2 : step < 1 ? 1 : 0)}{unit}</output>
    <input id={id} type="range" value={value} min={min} max={max} step={step} onChange={e=>onChange(Number(e.target.value))}/></label>;
}

export default function BallisticsStudy() {
  const [mode,setMode]=useState(0);
  return <section className="ballistics-suite">
    <nav className="health-tabs" aria-label="Ballistics applications">{names.map((name,i)=><button key={name} aria-pressed={mode===i} onClick={()=>setMode(i)}>{name}</button>)}</nav>
    <Lesson key={mode} mode={mode}/>
  </section>;
}

function Lesson({mode}:{mode:number}) {
  const [p,setP]=useState<Parameters>({...defaults});
  const [time,setTime]=useState(0),[playing,setPlaying]=useState(false),[manual,setManual]=useState(false);
  const result=useMemo(()=>study(mode,p),[mode,p]);
  const timeId=useId();
  useEffect(()=>{
    const media=matchMedia('(prefers-reduced-motion: reduce)');
    const sync=()=>{setManual(media.matches);if(media.matches)setPlaying(false);};
    sync();media.addEventListener('change',sync);return()=>media.removeEventListener('change',sync);
  },[]);
  useEffect(()=>{
    if(!playing) return;
    let frame=0, previous=performance.now(), elapsed=time>=result.duration?0:time;
    const tick=(now:number)=>{
      elapsed=Math.min(result.duration,elapsed+Math.min(.05,(now-previous)/1000));previous=now;setTime(elapsed);
      if(elapsed>=result.duration)setPlaying(false);else frame=requestAnimationFrame(tick);
    };
    frame=requestAnimationFrame(tick);return()=>cancelAnimationFrame(frame);
    // The clock owns elapsed time while playing; user edits pause it explicitly.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  },[playing,result.duration]);
  function change(values:Partial<Parameters>) {setPlaying(false);setTime(0);setP(old=>({...old,...values}));}
  function reset(){setPlaying(false);setTime(0);setP({...defaults});}
  const {shot,reference,gap,blocked}=result;
  const status=!shot.found ? mode===1 ? 'Speed budget exceeded' : 'No feasible shot'
    : blocked ? 'Cover blocks this throw' : mode===2 ? 'Interception found' : 'Target reachable';
  const explanation=!shot.found ? mode===1 ? `This fuse needs ${shot.speed.toFixed(2)} m/s, above the ${p.speed.toFixed(2)} m/s budget. The dashed orange path is a candidate only; no projectile is launched.`
    : mode===2 ? 'No positive interception exists within the eight-second horizon at this speed. Change speed, distance or target velocity.'
    : 'The chosen speed cannot reach this point against gravity. Increase speed or bring the target closer.'
    : blocked ? `At ${(p.distance*.45).toFixed(2)} m, the throw passes ${Math.abs(gap).toFixed(2)} m below the cover top. The projectile stops there.`
    : mode===1 ? 'The throw reaches the target at the fuse time, stays within budget and clears the analytic cover plane.'
    : mode===2 ? 'At the tuned arrival time, the projectile and constant-velocity target meet. The grey reference aims at where the target started.'
    : 'Both horizontal travel and vertical fall satisfy the target position. Compare the same launch speed on low and high arcs.';
  const achieved=challengeMet(mode,p);
  const metrics=mode===0 ? [
    ['Flight time',shot.found?`${shot.time.toFixed(2)} s`:'—',`Fixed 20 m/s low arc: ${reference.found?reference.time.toFixed(2)+' s':'unreachable'}`],
    ['Launch angle',shot.found?`${shot.angle.toFixed(1)}°`:'—','Measured from horizontal; high arc takes longer'],
    ['Ideal endpoint error',shot.found?`${(result.residual*1000).toFixed(4)} mm`:'—','Equation residual, without obstacles or integration error']
  ] : mode===1 ? [
    ['Cover clearance',gap===null?'—':`${gap>=0?'+':''}${gap.toFixed(2)} m`,'Height above the 3 m cover; negative means blocked'],
    ['Required launch speed',`${shot.speed.toFixed(2)} m/s`,`Budget: ${p.speed.toFixed(2)} m/s · reference: ${reference.speed.toFixed(2)} m/s`],
    ['Arrival / fuse',`${shot.time.toFixed(2)} s`,'Unobstructed arrival time; a blocked throw stops earlier']
  ] : [
    ['Ideal tuned error',shot.found?`${(result.residual*1000).toFixed(4)} mm`:'—','Separation from the moving target at tuned arrival'],
    ['Current-position miss',result.referenceMiss===null?'—':`${result.referenceMiss.toFixed(2)} m`,'Target separation at the reference’s own arrival time'],
    ['Lead distance',shot.found?`${(p.targetSpeed*shot.time).toFixed(2)} m`:'—','Signed travel of the target before tuned arrival']
  ];
  return <section className="practical ballistics-study">
    <header className="lesson-header"><h2>{names[mode]}</h2><p>{tasks[mode]}</p></header>
    <div className="scenario-grid">
      <div className="scenario-demo">
        <div className="ballistics-legend"><span>Orange · tuned {mode===1&&!shot.found?'candidate':'shot'}</span><span>Dashed grey · {references[mode]}</span><span>Teal · target</span></div>
        <Trajectory mode={mode} p={p} result={result} time={time}/>
        <p className="chart-note">World metres. Both paths share one scale fitted to their combined extent.
          {mode===1?' Grey is an unobstructed ghost; the orange projectile stops at cover.':''}
          {mode===2?' The teal outline marks the initial target position. Playback stops at the tuned impact, with the target and projectile together.':''}</p>
        <label className="scrubber" htmlFor={timeId}>Inspect time <output htmlFor={timeId}>{time.toFixed(2)} s</output>
          <input id={timeId} type="range" min={0} max={result.duration} step="any" value={time} onChange={e=>{setPlaying(false);const next=Number(e.target.value);setTime(next>=result.duration-1e-8?result.duration:next);}}/></label>
        <div className="scenario-actions">
          <button className="button dark" disabled={manual} onClick={()=>setPlaying(v=>!v)}>{playing?'Pause':time>=result.duration?'Replay':'Play'}</button>
          <button className="button outline" onClick={()=>{setPlaying(false);setTime(0);}}>Rewind</button>
          <button className="button outline" disabled={!shot.found} onClick={()=>{setPlaying(false);setTime(blocked?result.impactTime:shot.time);}}>{blocked?'Inspect cover impact':'Inspect tuned arrival'}</button>
        </div>
        <p className="chart-note">Starts paused. Tuning or scrubbing pauses and rewinds as appropriate.{manual?' Reduced motion is enabled: use the manual timeline and arrival button.':''}</p>
        <div className="ballistics-state" aria-live="polite"><h3>{status}</h3><p>{explanation}</p></div>
      </div>
      <aside className="scenario-tuning"><h3>Change one thing</h3>
        <p>The grey reference keeps its named rule. Other target and gravity inputs stay shared. Reset before each controlled comparison.</p>
        <Range label="Target distance" value={p.distance} min={4} max={30} unit=" m" onChange={distance=>change({distance})}/>
        <Range label="Target height" value={p.height} min={0} max={6} step={.5} unit=" m" onChange={height=>change({height})}/>
        <Range label={mode===1?'Speed budget':'Launch speed'} value={p.speed} min={5} max={25} unit=" m/s" onChange={speed=>change({speed})}/>
        <Range label="Gravity" value={p.gravity} min={2} max={20} step={.01} unit=" m/s²" onChange={gravity=>change({gravity})}/>
        {mode===0&&<label className="instant-option"><input type="checkbox" checked={p.high} onChange={e=>change({high:e.target.checked})}/> Use high arc</label>}
        {mode===1&&<Range label="Fuse duration" value={p.fuse} min={.75} max={3} step={.05} unit=" s" onChange={fuse=>change({fuse})}/>}
        {mode===2&&<Range label="Target velocity" value={p.targetSpeed} min={-8} max={8} unit=" m/s" onChange={targetSpeed=>change({targetSpeed})}/>}
        <h3>Repeat a comparison</h3>
        <div className="feel-presets">
          {mode===0?<><button onClick={()=>change({high:false})}>Low arc</button><button onClick={()=>change({high:true})}>High arc</button></>:
          mode===1?<><button onClick={()=>change({fuse:1})}>Short fuse · 1 s</button><button onClick={()=>change({fuse:2})}>Long fuse · 2 s</button></>:
          <><button onClick={()=>change({targetSpeed:6})}>Receding · +6 m/s</button><button onClick={()=>change({targetSpeed:-8})}>Closing · −8 m/s</button></>}
        </div>
        <button className="button outline" onClick={()=>change(mode===0?{...defaults,speed:5}:mode===1?{...defaults,speed:5}:{...defaults,speed:8,targetSpeed:8})}>Run failure case</button>
        <button className="button dark" onClick={reset}>Reset lesson</button>
      </aside>
    </div>
    <div className="tuning-brief"><h3>Read the result</h3>
      <div className="metric-grid">{metrics.map(([label,value,note])=><div className="metric" key={label}><span>{label}</span><strong>{value}</strong><small>{note}</small></div>)}</div>
      <div className="placement-challenge"><h3>Try it</h3><p>{goals[mode]}</p><p role="status">{achieved?'Challenge met. Explain what changed while the comparison stayed fixed.':'Adjust the controls and compare the measurements.'}</p></div>
      <h3>Where it helps / where to avoid it</h3><p>{guidance[mode]}</p>
      <h3>One model, different constraints</h3><p>{mode===0?'A chosen launch speed leaves arrival time unknown. Gravity permits a low and a high arc when the point is reachable.':mode===1?'A chosen arrival time determines launch velocity. We then ask whether it fits the budget and passes above cover.':'Target position changes with arrival time. The solver finds the earliest positive time that also satisfies the projectile’s launch speed.'}</p>
      <h3>Unity mapping</h3><p>Open <code>Assets/Scenes/BallisticsLab.unity</code>. Its native lesson uses the same equations and starts paused.
        For gameplay, map solved X/Y velocity to <code>Rigidbody.linearVelocity</code> with matching gravity and appropriate swept collision.
        The lesson itself evaluates ideal positions directly; it does not test PhysX integration.</p>
      <p className="chart-note">This page is a JavaScript teaching model. Native Unity 6000.5.2f1 passed 16 grouped model checks and 16 scripted Play checks.
        Hardware input routing, actual Rigidbody comparison and exported Unity/WebGL players remain unverified.</p>
      <details className="ballistics-method"><summary>Inspect equations, reference rules and test limits</summary>
        <p>Horizontal position is vx × time. Vertical position is vy × time − ½ × gravity × time².
          Fixed-speed and interception constraints produce a polynomial in time. Derivative roots split it into monotone intervals; checking the boundaries also preserves the tangent solution at maximum range.</p>
        <p>The grey reference uses {references[mode].toLowerCase()}. It shares target distance, height and gravity{mode===0?'':', and the current speed constraint'}.
          A missing reference is reported rather than shown as zero error. Endpoint residuals measure the ideal equation result and cannot establish a collision-free trajectory or a real gameplay hit.</p>
        <p>Repeat the failure case, inspect its measurements, then reset. The regression suite compares a deterministic parameter sweep with the closed-form fixed-target discriminant and independently executed C# fixtures. Cover is a plane at 45% of target distance, with height 3 m. Projectile radius, air resistance, bounce and target acceleration are excluded.</p>
      </details>
    </div>
  </section>;
}

function Trajectory({mode,p,result,time}:{mode:number;p:Parameters;result:StudyResult;time:number}) {
  function samples(shot:Shot,end=shot.time) {return shot.time>0?Array.from({length:101},(_,i)=>{const t=end*i/100;return [shot.x*t,heightAt(shot,p.gravity,t)];}):[];}
  const tuned=samples(result.shot,result.impactTime), ghost=samples(result.reference);
  const current=pointAt(result,p,mode,time), future=[p.distance+(mode===2?p.targetSpeed*result.duration:0),p.height];
  const bounds=[...tuned,...ghost,[0,0],[p.distance,p.height],future,...(mode===1?[[p.distance*.45,3]]:[])];
  const minX=Math.min(0,...bounds.map(v=>v[0])), maxX=Math.max(4,...bounds.map(v=>v[0]));
  const minY=Math.min(0,...bounds.map(v=>v[1])), maxY=Math.max(4,...bounds.map(v=>v[1]));
  const scale=Math.min(520/(maxX-minX),245/(maxY-minY));
  const wideAxis=(maxX-minX)*scale>180;
  const originX=60+(520-(maxX-minX)*scale)/2, originY=45+(245-(maxY-minY)*scale)/2;
  const project=([x,y]:number[])=>[originX+(x-minX)*scale,originY+(maxY-y)*scale];
  const path=(points:number[][])=>points.map((pt,i)=>`${i?'L':'M'}${project(pt).join(',')}`).join(' ');
  const [launchX,launchY]=project([0,0]),[tx,ty]=project(current.target),[ix,iy]=project([p.distance,p.height]);
  const ball=current.projectile?project(current.projectile):null;
  const refTime=Math.min(time,result.reference.time);
  const refBall=result.reference.found?project([result.reference.x*refTime,heightAt(result.reference,p.gravity,refTime)]):null;
  const [wx,wy]=project([p.distance*.45,3]),[,ground]=project([0,0]);
  return <svg viewBox="0 0 640 350" role="img" aria-label={`${names[mode]} at ${time.toFixed(2)} seconds; ${result.shot.found?result.blocked?'throw blocked by cover':'feasible trajectory':'no feasible launch'}`}>
    <rect width="640" height="350" rx="8" fill="#e9ede2"/>
    {(wideAxis?[0,.5,1]:[]).map(f=>{const x=minX+(maxX-minX)*f,[px]=project([x,0]);return <g key={f}><path d={`M${px},40V295`} stroke="#d0d8c8"/><text x={px} y="325" textAnchor="middle">{x.toFixed(1)}</text></g>;})}
    {!wideAxis&&<text x="320" y="325" textAnchor="middle">Horizontal span {minX.toFixed(1)} to {maxX.toFixed(1)} m</text>}
    <path d={`M35,${ground}H605`} stroke="#778971"/>
    <text x="30" y="25">Height up · horizontal distance in m</text>
    {mode===2&&<><path d={`M${ix},${iy}L${project(future).join(',')}`} stroke="#327773" strokeDasharray="3 5"/><circle cx={ix} cy={iy} r="8" fill="none" stroke="#327773" strokeWidth="2"/></>}
    {mode===1&&<g><rect x={wx-4} y={wy} width="8" height={ground-wy} fill="#657b67"/><text x={wx+10} y={wy-12}>3 m cover</text></g>}
    <path d={path(ghost)} className="reference-line"/>
    <path d={path(tuned)} fill="none" stroke="#c44825" strokeWidth="3" strokeDasharray={result.shot.found?undefined:'6 5'}/>
    <rect x={launchX-10} y={launchY-4} width="20" height="9" fill="#25372d"/>
    {refBall&&<circle cx={refBall[0]} cy={refBall[1]} r="6" fill="#e9ede2" stroke="#657b67" strokeWidth="2"/>}
    <circle cx={tx} cy={ty} r="9" fill="#327773"/>
    <path d={`M${tx-15},${ty}h30 M${tx},${ty-15}v30`} stroke="#327773"/>
    {ball&&<circle cx={ball[0]} cy={ball[1]} r="6" fill="#c44825" stroke="#f7f6f0" strokeWidth="2"/>}
  </svg>;
}
