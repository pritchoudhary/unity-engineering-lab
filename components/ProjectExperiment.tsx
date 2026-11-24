'use client';
import dynamic from 'next/dynamic';
function Loading(){return <p className="experiment-loading" role="status">Opening this experiment…</p>;}
const Spring=dynamic(()=>import('./SpringApplications'),{loading:Loading});
const Health=dynamic(()=>import('./SceneHealthLab'),{loading:Loading});
const Vehicle=dynamic(()=>import('./VehicleStudy'),{loading:Loading});
const Spline=dynamic(()=>import('./SplineStudy'),{loading:Loading});
const Placement=dynamic(()=>import('./PlacementStudy'),{loading:Loading});
const Ballistics=dynamic(()=>import('./BallisticsStudy'),{loading:Loading});
const Steering=dynamic(()=>import('./SteeringStudy'),{loading:Loading});
const Systems=dynamic(()=>import('./SystemStudies'),{loading:Loading});
export default function ProjectExperiment({slug}:{slug:string}){
 if(slug==='spring-physics-lab')return <Spring/>;
 if(slug==='scene-health-inspector')return <Health/>;
 if(slug==='vehicle-suspension')return <Vehicle/>;
 if(slug==='spline-editor')return <Spline/>;
 if(slug==='scene-placement')return <Placement/>;
 if(slug==='ballistics-lab')return <Ballistics/>;
 if(slug==='steering-behaviors')return <Steering/>;
 return <><Systems key={slug} kind={slug==='flocking-lab'?'flock':slug==='camera-composition'?'camera':'visibility'}/><p className="systems-native-note">Three browser studies and corresponding native scenes are implemented. The three new system models passed 98 native Unity checks. Each saved scene also has a separate player-controlled mission; exported player and WebGL validation remain pending.</p></>;
}
