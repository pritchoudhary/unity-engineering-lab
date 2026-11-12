export const initialRail:number[][];
export function point(p:number[][],t:number):number[];
export function tangent(p:number[][],t:number):number[];
export function rail(p:number[][],steps?:number):{samples:{t:number;pos:number[];s:number}[];length:number;at:(f:number)=>number};
export function offsets(p:number[][],t:number,width:number):number[][];
export function patrol(time:number,pause:number):{fraction:number;phase:'Outbound'|'Waiting'|'Returning';leg:number};
