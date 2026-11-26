export type SearchAlgorithm='astar'|'dijkstra'|'bfs';
export type SearchResult={path:number[];expanded:number[];cost:number|null;steps:number|null;frontierPeak:number};
export function searchGrid(grid:number[][],start:number,goal:number,algorithm?:SearchAlgorithm):SearchResult;
export function pathfindingScenario(mode?:number):{grid:number[][];start:number;goal:number};
export function hearSound(grid:number[][],source:number,listener:number,budget:number):{heard:boolean;attenuation:number|null;location:number|null};
