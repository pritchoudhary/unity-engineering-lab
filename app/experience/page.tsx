import type { Metadata } from 'next';
import Link from 'next/link';
import styles from '../../components/Career.module.css';

export const metadata: Metadata = {
  title: 'Experience & Skills',
  description: 'Selected Unity engineering experience, practical capabilities and interactive examples across gameplay, immersive learning and developer tools.',
  alternates: { canonical: '/experience/' }
};
const roles=[
 {company:'Lumos Labs',role:'Lead Unity Developer (SDE 4)',dates:'November 2024–Present',summary:'Hands-on Unity development and day-to-day engineering leadership for the games team.',points:['Work with product, design, art and QA to plan features, clarify requirements and keep delivery on track.','Support technical decisions, code reviews, debugging, performance work and release preparation.','Maintain shared development practices and improve project structure, code quality and team workflows.']},
 {company:'Total Infrastructure Management Solutions',role:'Senior Unity Developer',dates:'June 2023–November 2024',summary:'Immersive learning across Unity, VR, AR, WebGL and 360° video.',points:['Developed EndeaVR, a platform for immersive learning.','Implemented multiplayer features and data collection systems for content optimization.','Worked on memory usage and frame-rate optimization.']},
 {company:'Iconic Engine',role:'Senior Unity Developer',dates:'May 2021–June 2023',summary:'Core gameplay, cross-platform performance and responsive user experiences.',points:['Developed gameplay mechanics and integrated physics and UI/UX systems.','Used asynchronous loading and asset streaming to reduce loading time.','Collaborated with QA on code reviews and feature testing.']},
 {company:'Time Searcher',role:'Unity Developer',dates:'July 2019–May 2021',summary:'Turning a physical board game into a digital interactive experience.',points:['Built gameplay in Unity and C#.','Worked with programming, design and audio contributors to implement and refine the game.','Streamlined CSV-to-JSON data handling and optimized mobile performance.']},
 {company:'Game4Hearoes',role:'Senior Unity Developer',dates:'April 2019–November 2019',summary:'A mobile application developed with Queensland Health for people with hearing loss.',points:['Built Firebase backend systems and two-factor sign-in.','Connected clinicians and patients through shared progress workflows.']},
 {company:'Lamplight Forest Ltd.',role:'Unity Developer',dates:'July 2018–February 2019',summary:'AI, gameplay prototyping and physics-based interactions.',points:['Implemented behavior trees, pathfinding and save/load systems.','Worked with designers, artists, animators and network engineers on integrated gameplay.']},
 {company:'Tiny Sail Games',role:'Unity Developer',dates:'February 2017–November 2017',summary:'Mobile input, monetization integrations and gameplay quality.',points:['Implemented touch controls, in-app purchases and virtual currency mechanics.','Integrated analytics and collaborated on assets, bug fixing and optimization.']}
];

const capabilities = [
  { title: 'Gameplay & motion', label: 'BEHAVIOR', body: 'C#, AI behavior, physics-based interactions and movement constraints.', href: '/projects/steering-behaviors/', example: 'Try arrival, avoidance & pursuit', question: 'How should an agent slow down without overshooting?' },
  { title: 'Developer tools', label: 'WORKFLOW', body: 'Scene inspection, editor workflows and reusable authoring controls.', href: '/projects/scene-health-inspector/', example: 'Investigate a scene health issue', question: 'Which dependency is broken, and what does the scan actually cover?' },
  { title: 'Applied mathematics', label: 'REASONING', body: 'Springs, curves, trajectory solving and controlled numerical comparisons.', href: '/projects/ballistics-lab/', example: 'Explore aiming & interception', question: 'Where should you aim when the target keeps moving?' },
  { title: 'Immersive experiences', label: 'INTERACTION', body: 'Unity, VR, AR, WebGL and 360° video for interactive learning through EndeaVR.', href: '#role-tims', example: 'Read the immersive learning role', question: 'Bring interaction, multiplayer features and learning data together.' },
  { title: 'Engineering leadership', label: 'DELIVERY', body: 'Hands-on development, architecture decisions, code reviews and coordination with product, design, art and QA.', href: '#role-lumos', example: 'Read the current role', question: 'Make technical decisions useful to the people building the experience.' },
  { title: 'Performance & quality', label: 'CONSTRAINTS', body: 'Profiling, memory and loading optimization, debugging, testing and release support.', href: '#role-iconic', example: 'Read the cross-platform role', question: 'Understand the bottleneck before choosing a fix.' }
];
const roleAnchors = ['role-lumos', 'role-tims', 'role-iconic'];

function Role({ role, index }: { role: typeof roles[number]; index: number }) {
  return <article className={styles.role} id={roleAnchors[index]}>
    <div className={styles.roleDate}><span className="eyebrow">{role.dates}</span>{index === 0 && <span className={styles.current}>Current role</span>}</div>
    <div><p className={styles.roleTitle}>{role.role}</p><h3>{role.company}</h3><p className={styles.summary}>{role.summary}</p><ul>{role.points.map(point => <li key={point}>{point}</li>)}</ul></div>
  </article>;
}

export default function Experience() {
  return <main id="main" className={`wrap ${styles.page}`}>
    <header className={styles.hero}>
      <div><span className="eyebrow">EXPERIENCE & SKILLS / PRITISH CHOUDHARY</span><h1>From the idea<br/>to the <span>working system.</span></h1><p className={styles.lead}>Hands-on Unity engineering across gameplay, immersive learning and developer tools. The experience behind the work, and examples you can explore yourself.</p></div>
      <aside className={styles.now}><span className="eyebrow">CURRENTLY</span><p>Lead Unity Developer<br/><strong>Lumos Labs</strong></p><span>Bangalore, India</span><div className={styles.nowFoot}>Development · Technical guidance · Team workflows</div></aside>
    </header>
    <nav className={styles.index} aria-label="Experience page sections"><a href="#capabilities">Capabilities</a><a href="#experience">Experience</a><a href="#toolkit">Toolkit</a></nav>

    <section className={styles.section} id="capabilities" aria-labelledby="capabilities-title">
      <div className={styles.sectionHead}><div><span className="eyebrow">CAPABILITIES</span><h2 id="capabilities-title">Skills with a way in.</h2></div><p>Start with a practical question. The lab examples are personal teaching projects; professional contributions are described below.</p></div>
      <div className={styles.capabilities}>{capabilities.map(capability => <article className={styles.capability} key={capability.title}>
        <span className="eyebrow">{capability.label}</span><h3>{capability.title}</h3><p className={styles.question}>{capability.question}</p><p>{capability.body}</p><Link className={styles.link} href={capability.href}>{capability.example}</Link>
      </article>)}</div>
    </section>

    <section className={styles.section} id="experience" aria-labelledby="experience-title">
      <div className={styles.sectionHead}><div><span className="eyebrow">SELECTED EXPERIENCE</span><h2 id="experience-title">Built around real constraints.</h2></div><p>Recent engineering roles, followed by the earlier work that shaped my approach to interactive systems.</p></div>
      <div className={styles.roles}>{roles.slice(0, 3).map((role, index) => <Role key={role.company} role={role} index={index}/>)}</div>
      <details className={styles.earlier}><summary><span>Earlier experience <small>2017–2021 · Four roles</small></span><span className={styles.expand} aria-hidden="true">+</span></summary><div className={styles.roles}>{roles.slice(3).map((role, index) => <Role key={role.company} role={role} index={index + 3}/>)}</div></details>
      <p className={styles.note}>The lab contains personal teaching projects. Professional experience does not imply that employer source code or internal materials are published here.</p>
    </section>

    <section className={styles.section} id="toolkit" aria-labelledby="toolkit-title">
      <div className={styles.sectionHead}><div><span className="eyebrow">WORKING TOOLKIT</span><h2 id="toolkit-title">Tools behind the decisions.</h2></div></div>
      <div className={styles.toolkit}><article><h3>Languages</h3><p>C# · C++ · Java · Swift · HTML</p></article><article><h3>Development</h3><p>Unity · Shader Graph · Visual Studio · Android Studio · Xcode</p></article><article><h3>Collaboration & content</h3><p>Git · GitHub · GitLab · Firebase · Blender · QA & game testing</p></article></div>
      <div className={styles.ending}><p>See how the engineering feels in practice.</p><Link className={styles.link} href="/#experiments">Explore Unity Engineering Lab</Link></div>
    </section>
  </main>;
}
