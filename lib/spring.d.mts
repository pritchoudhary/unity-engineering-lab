export type Parameters = {mass:number; stiffness:number; damping:number};
export type State = {x:number;v:number};
export const STEP:number;
export function validate(p:Parameters):void;
export function step(s:State,p:Parameters,dt?:number):State;
export function energy(s:State,p:Parameters):number;
export function ratio(p:Parameters):number;
export function trajectory(p:Parameters,x?:number,seconds?:number):{t:number;x:number}[];
export function follow(s:State,target:number,p:Parameters,dt?:number):State;
