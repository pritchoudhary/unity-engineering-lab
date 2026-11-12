'use client';
import {useState,useEffect,useRef} from 'react';
export default function ExperimentTimeline({t,setT,duration=4}:{t:number;setT:(v:number)=>void;duration?:number}){
 const [play,setPlay]=useState(false),[manual,setManual]=useState(false);const current=useRef(t);current.current=t;
 useEffect(()=>{const media=matchMedia('(prefers-reduced-motion: reduce)');const sync=()=>{setManual(media.matches);if(media.matches)setPlay(false)};sync();media.addEventListener('change',sync);return()=>media.removeEventListener('change',sync)},[]);
 useEffect(()=>{if(!play)return;const start=performance.now(),initial=current.current>=duration?0:current.current;let id=0;function tick(now:number){const next=Math.min(duration,initial+(now-start)/1000);setT(next);if(next===duration)setPlay(false);else id=requestAnimationFrame(tick)}id=requestAnimationFrame(tick);return()=>cancelAnimationFrame(id)},[play,setT,duration]);
 return <div><label className="scrubber">Inspect time <output>{t.toFixed(2)} s</output><input type="range" min="0" max={duration} step=".01" value={t} onChange={e=>{setPlay(false);setT(+e.target.value)}}/></label><div className="scenario-actions"><button className="button dark" disabled={manual} onClick={()=>setPlay(v=>!v)}>{play?'Pause':t>=duration?'Replay':'Play'}</button><button className="button outline" onClick={()=>{setPlay(false);setT(0)}}>Rewind</button></div></div>
}
