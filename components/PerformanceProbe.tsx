'use client';
import {useEffect,useRef} from 'react';
// Explicit QA opt-in. No telemetry, network reporting or sampling on normal visits.
export default function PerformanceProbe(){
 const output=useRef<HTMLOutputElement>(null);
 useEffect(()=>{
  if(!new URLSearchParams(location.search).has('labPerf'))return;
  const frames:number[]=[],events:number[]=[],longTasks:number[]=[];const observers:PerformanceObserver[]=[];
  function observe(type:string,collect:(entry:any)=>void){if(!PerformanceObserver.supportedEntryTypes.includes(type))return;try{const o=new PerformanceObserver(list=>list.getEntries().forEach(collect));o.observe({type,buffered:true,...(type==='event'?{durationThreshold:16}:{})});observers.push(o);}catch{}}
  observe('longtask',e=>{longTasks.push(e.duration);if(longTasks.length>100)longTasks.shift();});
  observe('event',e=>{if(e.interactionId)events.push(e.duration);if(events.length>1000)events.shift();});
  let previous=0,animation=0;const tick=(now:number)=>{if(!document.hidden&&previous){frames.push(now-previous);if(frames.length>240)frames.shift();}previous=document.hidden?0:now;animation=requestAnimationFrame(tick);};animation=requestAnimationFrame(tick);
  const percentile=(xs:number[],p:number)=>{const sorted=[...xs].sort((a,b)=>a-b);return sorted.length?+sorted[Math.min(sorted.length-1,Math.floor(sorted.length*p))].toFixed(2):null;};
  const timer=setInterval(()=>{if(!output.current)return;const resources=performance.getEntriesByType('resource') as PerformanceResourceTiming[];const scripts=resources.filter(r=>/\.js(?:[?#]|$)/.test(r.name));output.current.textContent=JSON.stringify({visibility:document.visibilityState,frameSamples:frames.length,frameP95Ms:percentile(frames,.95),frameMaxMs:percentile(frames,1),eventSamples:events.length,eventP95Ms:percentile(events,.95),eventMaxMs:percentile(events,1),longTaskCount:longTasks.length,longTaskMaxMs:percentile(longTasks,1),scriptCount:scripts.length,scriptEncodedBytes:scripts.reduce((n,r)=>n+r.encodedBodySize,0),scripts:scripts.map(r=>r.name.replace(location.origin,''))});},500);
  return()=>{cancelAnimationFrame(animation);clearInterval(timer);observers.forEach(o=>o.disconnect());};
 },[]);
 return <output hidden ref={output} id="lab-performance-report" aria-hidden="true"/>;
}
