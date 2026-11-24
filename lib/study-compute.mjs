import {systemStudies} from './system-study-config.mjs';
// Lives in the worker. Cache a bounded number of deterministic runs, including shared references.
export function createStudyComputer(limit=6){
 const cache=new Map();
 function run(cfg,kind,mode,p){
  const key=JSON.stringify([kind,mode,p]);
  if(cache.has(key)){const result=cache.get(key);cache.delete(key);cache.set(key,result);return result;}
  const result=cfg.run(mode,p);cache.set(key,result);
  while(cache.size>limit)cache.delete(cache.keys().next().value);
  return result;
 }
 return ({kind,mode,p})=>{
  if(!Object.hasOwn(systemStudies,kind)||!Number.isInteger(mode)||mode<0||mode>2)throw new RangeError('Unknown study');
  const cfg=systemStudies[kind],c=cfg.cases[mode];
  return {run:run(cfg,kind,mode,p),reference:run(cfg,kind,mode,{...p,[c.key]:c.reference})};
 };
}
