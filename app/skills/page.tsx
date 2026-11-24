import type { Metadata } from 'next';
import Link from 'next/link';
export const metadata:Metadata={title:'Skills',description:'Unity and C# capabilities with practical examples in physics, motion, editor tooling and interactive learning.',alternates:{canonical:'/skills/'}};
const capabilities=[
 ['Gameplay & motion','C#, Unity systems, AI behavior, movement constraints and physics-based interactions.','steering-behaviors','Explore arrival, avoidance and pursuit'],
 ['Tools for developers','Editor workflows, scene inspection, reusable authoring controls and repeatable placement.','scene-health-inspector','Inspect scene health problems'],
 ['Math you can use','Springs, curves, trajectory solving, controlled comparisons and useful numerical evidence.','ballistics-lab','Solve aiming and interception problems'],
 ['Immersive experiences','VR, AR, WebGL and 360° video, applied to interactive learning through EndeaVR.',null,'See professional experience'],
 ['Engineering leadership','Hands-on Unity development, architecture decisions, technical guidance, code reviews and coordination across product, design, art and QA.',null,'See professional experience'],
 ['Performance & delivery','Profiling, memory and loading optimization, testing, code review and version control.',null,'See professional experience']
];
export default function Skills(){return <main id="main" className="wrap profile-page"><span className="eyebrow">SKILLS / SHOWN THROUGH THE WORK</span><h1>Capabilities,<br/><span>with something to inspect.</span></h1><p className="profile-lead">The useful question is what a skill lets you build. Explore a working study or see how it connects to professional work.</p><div className="capability-list">{capabilities.map(([title,body,slug,label])=><article key={title}><h2>{title}</h2><div><p>{body}</p><Link className="text-link" href={slug?`/projects/${slug}/`:'/experience/'}>{label}</Link></div></article>)}</div><section className="profile-ending"><span className="eyebrow">WORKING TOOLKIT</span><div className="profile-grid"><article><h3>Languages</h3><p>C#, C++, Java, HTML and Swift.</p></article><article><h3>Development</h3><p>Unity, Shader Graph, Visual Studio, Android Studio and Xcode.</p></article><article><h3>Collaboration & content</h3><p>Git, GitHub, GitLab, Firebase, Blender, QA and game testing.</p></article></div></section></main>}
