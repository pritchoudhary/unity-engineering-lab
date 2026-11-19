export type Point = number[];
export function seededRandom(seed:number):()=>number;
export function grid(count?:number,columns?:number,spacing?:number):Point[];
export function scatter(count?:number,width?:number,spacing?:number,seed?:number):{points:Point[];attempts:number;budget:number};
export function footprint(points:Point[]):number[];
export function minimumDistance(points:Point[]):number|null;
export function surfaceLayout(angle?:number,slopeLimit?:number,aligned?:boolean):{
  points:{position:number[];up:number[]}[];normal:number[];alignmentError:number;
};
