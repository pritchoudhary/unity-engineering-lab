'use client';
import {playbackTime} from '../lib/playback-time.mjs';
import Link from 'next/link';
import {useEffect,useMemo,useRef,useState,useId} from 'react';
import {springPosition,peakOvershoot} from '../lib/spring-intro.mjs';
const setups=[{name:'Bouncy',zeta:.25},{name:'Critical',zeta:1},{name:'Heavy',zeta:1.8}];
export default function SpringIntro(){
 const [zeta,setZeta]=useState(.25),[t,setT]=useState(0),[playing,setPlaying]=useState(false),[guess,setGuess]=useState(''),[revealed,setRevealed]=useState(false),[reduced,setReduced]=useState(false);
 const root=useRef<HTMLElement>(null),current=useRef(t),id=useId();current.current=t;
 useEffect(()=>{const media=matchMedia('(prefers-reduced-motion: reduce)');const change=()=>{setReduced(media.matches);setPlaying(false);};change();media.addEventListener('change',change);const hidden=()=>{if(document.hidden)setPlaying(false);};document.addEventListener('visibilitychange',hidden);const observer=new IntersectionObserver(entries=>{if(!entries[0].isIntersecting)setPlaying(false);},{threshold:.05});if(root.current)observer.observe(root.current);return()=>{media.removeEventListener('change',change);document.removeEventListener('visibilitychange',hidden);observer.disconnect();};},[]);
 useEffect(()=>{if(!playing)return;const start=performance.now(),initial=current.current;let animation=0;const tick=(now:number)=>{const next=playbackTime(initial,start,now,3);setT(next);if(next===3)setPlaying(false);else animation=requestAnimationFrame(tick);};animation=requestAnimationFrame(tick);return()=>cancelAnimationFrame(animation);},[playing]);
 const paths=useMemo(()=>[zeta,1].map(z=>Array.from({length:181},(_,i)=>`${i?'L':'M'}${40+i/180*480},${278-springPosition(i/60,z)*76}`).join(' ')),[zeta]);
 const position=springPosition(t,zeta),reference=springPosition(t,1),overshoot=peakOvershoot(zeta),crosses=overshoot>0;
 function run(){setRevealed(true);setT(reduced?3:0);setPlaying(!reduced);}
 return <aside ref={root} className="spring-intro" aria-labelledby={id+'title'}>
  <div className="intro-heading"><span className="eyebrow">A SMALL EXPERIMENT. A REAL DESIGN DECISION.</span><span className="intro-chip">Try it here</span></div>
  <h2 id={id+'title'}>Make it land.<br/><em>Lose the wobble.</em></h2>
  <p>A menu highlight moves to its new tab. Will it cross the target before it settles?</p>
  <div className="intro-options" aria-label="Choose a damping setup">{setups.map(s=><button key={s.name} aria-pressed={zeta===s.zeta} onClick={()=>{setZeta(s.zeta);setT(0);setPlaying(false);setRevealed(false);}}>{s.name}<small>ζ {s.zeta.toFixed(2)}</small></button>)}</div>
  <svg className="intro-scene" viewBox="0 0 560 166" role="img" aria-label={`Menu highlight at ${position.toFixed(2)} normalized units; target 1; time ${t.toFixed(2)} seconds`}>
   <defs><pattern id={id+'grid'} width="30" height="30" patternUnits="userSpaceOnUse"><path d="M30 0H0V30" fill="none" stroke="#314547" strokeWidth=".5"/></pattern></defs>
   <rect width="560" height="166" rx="12" fill={'url(#'+id+'grid)'}/>
   <text x="25" y="32" fill="#e1eddf" fontSize="15">YOUR MENU HIGHLIGHT</text><text x="25" y="111" fill="#9cbdb1" fontSize="14">Critical reference · ζ 1.00</text>
   <path d="M70 63H490 M70 137H490" stroke="#45605a"/><path d="M370 41V156" stroke="#a7c4ac" strokeDasharray="4 5"/>
   <rect x={46+position*300} y="46" width="48" height="32" rx="7" fill="#ec9865"/><rect x={46+reference*300} y="120" width="48" height="26" rx="6" fill="none" stroke="#a7c4ac" strokeWidth="2"/>
   <text x="380" y="92" fill="#d5e5d6" fontSize="14">Settings target</text>

  </svg>
  <div className="intro-predict" aria-label="Make a prediction"><div>{['Cross target','Stay this side'].map(choice=><button key={choice} aria-pressed={guess===choice} onClick={()=>{setGuess(choice);setRevealed(false);}}>{choice}</button>)}</div></div>
  <div className="intro-actions"><button className="intro-run" onClick={run}>{revealed?'Run again':'Test this setup'}</button><button onClick={()=>{if(t===3)setT(0);setPlaying(p=>!p);}} disabled={reduced||!revealed}>{playing?'Pause':'Play'}</button><button onClick={()=>{setT(0);setPlaying(false);}}>Reset time</button></div>

  <div className="intro-answer" role="status">{revealed?<><strong>{overshoot.toFixed(1)}% peak beyond the target.</strong><p>{guess?(guess===(crosses?'Cross target':'Stay this side')?'Prediction matched. ':'Compare that with your prediction. '):''}{zeta<1?'Low damping preserves a bounce. Try Critical to remove it.':zeta===1?'Critical damping removes overshoot for this ideal step. Try Heavy: no bounce, but a slower arrival.':'More damping removes the bounce but slows the approach. More is not always better.'}</p></>:<p>Choose a feel, predict what happens, then test it. No animation starts on its own.</p>}</div>
  <details className="intro-detail"><summary>Inspect the timing and model</summary>
  <label className="intro-time" htmlFor={id+'time'}>Inspect the motion <output>{t.toFixed(2)} s</output><input id={id+'time'} type="range" min="0" max="3" step="any" value={t} onChange={e=>{setT(+e.target.value);setPlaying(false);}}/></label>
   <svg className="intro-scene" viewBox="0 160 560 150" role="img" aria-label="Predicted position over three seconds, with a fixed critical reference">   <path d="M40 202H520 M40 278H520" stroke="#45605a" strokeDasharray="3 4"/>
   {revealed&&<><path d={paths[1]} fill="none" stroke="#a7c4ac" strokeWidth="2" strokeDasharray="5 5"/><path d={paths[0]} fill="none" stroke="#ec9865" strokeWidth="2.5"/><path d={`M${40+t/3*480} 170V282`} stroke="#dce8df" opacity=".55"/></>}
   <text x="40" y="301" fill="#b2c6bd" fontSize="14">0 s</text><text x="491" y="301" fill="#b2c6bd" fontSize="14">3 s</text><text x="40" y="183" fill="#b2c6bd" fontSize="14">{revealed?'Position over time · target = 1':'Run to reveal the two response curves'}</text></svg>
  <p className="intro-limits">Normalized UI motion, fixed natural frequency 6 rad/s. This ideal step has no interruptions or frame-step error. Real interfaces need both tested.</p>
  </details>
  <Link prefetch={false} className="intro-next" href="/projects/spring-physics-lab/">Explore suspension, interfaces, cameras and impacts</Link>
 </aside>;
}
