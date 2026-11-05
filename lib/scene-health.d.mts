export type Reference={name:string;target:string|null;kind:'object'|'asset';required:boolean;runtimeAssigned:boolean};
export type SceneObject={id:string;name:string;parent:string|null;active:boolean;components:{id:string;type:string|null;references:Reference[]}[]};
export type SceneManifest={objects:SceneObject[];assets:string[]};
export type ScanOptions={rootId?:string|null;includeInactive?:boolean;reviewInactive?:boolean;minimumSeverity?:'info'|'warning'|'error'};
export type Finding={id:string;objectId:string;path:string;componentId:string|null;field:string|null;rule:string;severity:'info'|'warning'|'error';evidence:string;nextStep:string};
export function inspectScene(scene:SceneManifest,options?:ScanOptions):{findings:Finding[];scannedObjects:number;scopedObjects:number;totalObjects:number;excludedInactive:number;hiddenBySeverity:number;detectedCounts:{error:number;warning:number;info:number}};
