'use client';
import {useState,type ReactNode} from 'react';
import styles from './LabExperience.module.css';
type Entry={slug:string;title:string;group:string;content:ReactNode};
export default function ExperimentCollection({entries}:{entries:Entry[]}){
 const [group,setGroup]=useState('All');
 const shown=entries.filter(e=>group==='All'||e.group===group);
 return <><div className={styles.collectionTools}><div className={styles.filters} aria-label="Explore experiments by topic">{['All','Motion','Tools','Cameras'].map(name=><button key={name} aria-pressed={group===name} onClick={()=>setGroup(name)}>{name==='All'?'All experiments':name}</button>)}</div><p role="status">{shown.length} {shown.length===1?'experiment':'experiments'} to explore</p></div><div className={styles.collection}>{shown.map(entry=><article key={entry.slug} className={styles.experiment}>{entry.content}</article>)}</div></>;
}
