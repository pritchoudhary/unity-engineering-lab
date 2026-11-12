export type DriveMode='rough'|'brake'|'dip'|'flat';
export function drive(mode:DriveMode,k?:number,c?:number,hz?:number):{frames:{t:number;z:number;p:number;front:number;rear:number;roadF:number;roadR:number;loadF:number;loadR:number}[];rms:number;pitch:number;air:number};
