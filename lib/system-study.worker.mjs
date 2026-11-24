import {createStudyComputer} from './study-compute.mjs';
const compute=createStudyComputer();
self.onmessage=({data})=>{
 const {id,key}=data;
 try{const started=performance.now(),result=compute(data);self.postMessage({id,key,...result,computeMs:performance.now()-started});}
 catch{self.postMessage({id,key,error:'The comparison could not be calculated.'});}
};
