// Fixed across presets so changing the tuning never silently rescales the comparison.
export const MOTION_DOMAIN = Object.freeze({min:-1.25,max:1.75});
export const PLOT_TICKS = Object.freeze([-1,0,1]);
export function plotY(value){return 180-(value-MOTION_DOMAIN.min)/(MOTION_DOMAIN.max-MOTION_DOMAIN.min)*160;}
export function highlightX(value){return 210+value*120;}
export function cameraX(value){return 190+value*150;}
export function inspectionTime(mode){return mode==='camera'?1.5:.75;}
