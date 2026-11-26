export type Sample={t:number;z:number;v:number;road:number;a:number;travel:number};
export const DT:number;
export function road(t:number,duration?:number):{y:number;v:number};
export function acceleration(t:number,z:number,v:number,c:number,duration?:number):number;
export function simulate(c:number,dt?:number,duration?:number):{points:Sample[];peak:number;travel:number;settle:number|null};
