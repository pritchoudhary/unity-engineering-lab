'use client';
import {type ReactNode,useId,useState,useRef,useEffect} from 'react';

/** A stable stage and transport with native, keyboard-operable tuning channels. */
export default function MixingDesk({preview,playback,controls,details}:{preview:ReactNode;playback?:ReactNode;controls:ReactNode;details?:ReactNode}){
 const id=useId();
 const [pinned,setPinned]=useState(true);
 const monitor=useRef<HTMLDivElement>(null),desk=useRef<HTMLElement>(null);
 useEffect(()=>{const element=monitor.current;if(!element)return;const observer=new ResizeObserver(()=>{desk.current?.style.setProperty('--monitor-height',`${element.getBoundingClientRect().height}px`)});observer.observe(element);return()=>observer.disconnect()},[]);
 return <section ref={desk} className={`mixing-desk${pinned?' mix-pinned':''}`} aria-label="Experiment mixing desk">
  <div ref={monitor} className="mix-monitor">
   <div className="mix-heading"><span>Mixing desk</span><button type="button" aria-pressed={pinned} onClick={()=>setPinned(v=>!v)}>{pinned?'Unpin preview':'Pin preview'}</button></div>
   <div className="mix-preview">{preview}</div>
   {playback&&<div className="mix-transport" aria-label="Playback and inspection">{playback}</div>}
  </div>
  <div className="mix-console" aria-labelledby={id}><div className="mix-console-heading"><h3 id={id}>Tune the experiment</h3><span>Change a setting. Watch the response.</span></div>{controls}</div>
  {details&&<details className="mix-details"><summary>Response charts, context &amp; explanation</summary><div>{details}</div></details>}
 </section>;
}
