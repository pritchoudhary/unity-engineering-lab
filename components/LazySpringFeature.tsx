'use client';
import dynamic from 'next/dynamic';
import {useState} from 'react';
const SpringApplications=dynamic(()=>import('./SpringApplications'),{loading:()=> <p role="status">Opening the four application studies…</p>});
export default function LazySpringFeature(){const [open,setOpen]=useState(false);return open?<SpringApplications/>:<div className="spring-feature-gate"><div><span className="eyebrow">GO DEEPER WHEN YOU ARE READY</span><h3>One idea. Four practical jobs.</h3><p>Compare ride comfort, interrupted interfaces, following cameras and impact recovery. Each has its own inputs, trade-offs and challenge.</p></div><button className="button dark" onClick={()=>setOpen(true)}>Open the four spring studies</button></div>;}
