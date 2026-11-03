import type {Mode} from './motion-experiments.mjs';
export const MOTION_DOMAIN:Readonly<{min:number;max:number}>;
export const PLOT_TICKS:readonly number[];
export function plotY(value:number):number;
export function highlightX(value:number):number;
export function cameraX(value:number):number;
export function inspectionTime(mode:Mode):number;
