'use client';
import {type ReactNode,useId,useState,useRef,useEffect} from 'react';
import type {LearningPath} from '../lib/learning-paths';

type Props={preview:ReactNode;playback?:ReactNode;controls:ReactNode;details?:ReactNode;guide?:LearningPath;evidence?:ReactNode;onSuspend?:()=>void;initialPinned?:boolean};
/** One workspace for the scene, controls and measured evidence. */
export default function MixingDesk({preview,playback,controls,details,guide,evidence,onSuspend,initialPinned=true}:Props){
 const id=useId(),[pinned,setPinned]=useState(initialPinned),[prediction,setPrediction]=useState(''),[revealed,setRevealed]=useState(false);
 const monitor=useRef<HTMLDivElement>(null),desk=useRef<HTMLElement>(null),suspend=useRef(onSuspend);suspend.current=onSuspend;
 useEffect(()=>{setPrediction('');setRevealed(false)},[guide?.question]);
 useEffect(()=>{const element=monitor.current;if(!element)return;const observer=new ResizeObserver(()=>{desk.current?.style.setProperty('--monitor-height',`${element.getBoundingClientRect().height}px`)});observer.observe(element);const visibility=()=>{if(document.hidden)suspend.current?.()};const intersection=new IntersectionObserver(([entry])=>{if(!entry.isIntersecting)suspend.current?.()});intersection.observe(element);document.addEventListener('visibilitychange',visibility);return()=>{observer.disconnect();intersection.disconnect();document.removeEventListener('visibilitychange',visibility)}},[]);
 return <section ref={desk} className={`mixing-desk${pinned?' mix-pinned':''}`} aria-label="Experiment workspace">
  {guide&&<details className="experiment-prediction"><summary><span>01 / Predict</span> {guide.question}</summary><div><p>{guide.try}</p><div className="prediction-choices" aria-label="Your prediction">{guide.choices.map(choice=><button key={choice} type="button" aria-pressed={prediction===choice} onClick={()=>{setPrediction(choice);setRevealed(false)}}>{choice}</button>)}</div><p className="prediction-note">{prediction?`Your prediction: ${prediction}. Change one input, then compare the evidence.`:'Make a prediction, or explore directly. Your choice is not graded.'}</p></div></details>}
  <div className="mix-workspace">
   <div ref={monitor} className="mix-monitor">
    <div className="mix-heading"><span>02 / Experiment</span><button type="button" aria-pressed={pinned} onClick={()=>setPinned(v=>!v)}>{pinned?'Unpin preview':'Pin preview'}</button></div>
    <div className="mix-preview">{preview}</div>
    {playback&&<div className="mix-transport" aria-label="Playback and inspection">{playback}</div>}
    {evidence&&<div className="mix-evidence mix-evidence-desktop" aria-label="Measured evidence"><span>Measured outcome</span><strong>{evidence}</strong></div>}
   </div>
   <div className="mix-console" aria-labelledby={id}>{evidence&&<div className="mix-evidence mix-evidence-mobile" aria-label="Measured evidence"><span>Measured outcome</span><strong>{evidence}</strong></div>}<div className="mix-console-heading"><h3 id={id}>Tune the experiment</h3><span>Change one input. Keep the comparison.</span></div>{controls}</div>
  </div>
  {guide&&<div className="experiment-reveal"><button type="button" aria-expanded={revealed} aria-controls={`${id}-reveal`} onClick={()=>setRevealed(v=>!v)}>03 / {revealed?'Hide the explanation':'Explain the result'} <span aria-hidden="true">{revealed?'−':'+'}</span></button>{revealed&&<div id={`${id}-reveal`}><p>{guide.why}</p><p><strong>Where it breaks:</strong> {guide.limit}</p>{prediction&&<p>Your prediction was “{prediction}”. Compare it with the measured outcome; try the opposite extreme before deciding why.</p>}</div>}</div>}
  {details&&<details className="mix-details"><summary>Response charts, context &amp; explanation</summary><div>{details}</div></details>}
 </section>;
}
