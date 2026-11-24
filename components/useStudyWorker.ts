'use client';
import {useEffect,useRef,useState} from 'react';
import {StudyQueue} from '../lib/study-queue.mjs';
export default function useStudyWorker(kind:string,mode:number,p:any){
 const key=JSON.stringify([kind,mode,p]);
 const [result,setResult]=useState<any>(null),[attempt,setAttempt]=useState(0),[fatal,setFatal]=useState<string|null>(null);
 const queue=useRef<any>(null),latest=useRef<any>(null),sequence=useRef(0);
 useEffect(()=>{
  let worker:Worker,alive=true;
  try{
   worker=new Worker(new URL('../lib/system-study.worker.mjs',import.meta.url),{type:'module'});
   const ownedQueue=new StudyQueue(job=>worker.postMessage(job),message=>{if(alive)setResult(message);});queue.current=ownedQueue;
   worker.onmessage=e=>ownedQueue.resolve(e.data);
   worker.onerror=()=>{if(!alive)return;ownedQueue.dispose();worker.terminate();setFatal('The comparison could not load. Please retry.');};
  }catch{setFatal('The comparison could not load. Please retry.');}
  return()=>{alive=false;queue.current?.dispose();queue.current=null;worker?.terminate();};
 },[attempt]);
 useEffect(()=>{
  const job={id:++sequence.current,key,kind,mode,p};latest.current=job;queue.current?.offer(job);
 },[key,attempt]);
 return {data:result?.key===key&&!result.error&&!fatal?result:null,error:fatal??(result?.key===key?result.error:null),retry:()=>{setResult(null);setFatal(null);setAttempt(n=>n+1);}};
}
