export type Mode='ui'|'camera'|'impact';
export type MotionSample={t:number;x:number;v:number;target:number};
export const scenarios:Record<Mode,{title:string;job:string;task:string;input:string;use:string;avoid:string;mapping:string;learn:string;presets:[string,number,number][]}>;
export function targetAt(mode:Mode,t:number):number;
export function experiment(mode:Mode,k:number,zeta:number):{points:MotionSample[];error:number;peak:number;tracking:number};
