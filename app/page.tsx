import type { Metadata } from 'next';
import Link from 'next/link';
import SpringLab from '../components/SpringApplications';
import { projects } from '../lib/projects';
export const metadata:Metadata={alternates:{canonical:'/'}};
const sketches=[
 <path key="spring" d="M20 62h24l8-22 12 44 12-44 12 44 12-22h40"/>,
 <path key="health" d="M30 25h60v70H30z M90 42h35v53H90 M44 45h25 M44 60h25 M44 75h12 M101 60l7 7 14-17"/>,
 <path key="vehicle" d="M23 57h113l-15-21H52z M40 58v22 M118 58v22 M21 93h120 M24 82h33 M102 82h33"/>,
 <path key="spline" d="M20 85C45-25 105 140 140 30 M20 85L52 20 M140 30l-28 65"/>,
 <path key="place" d="M25 30h22v22H25z M65 30h22v22H65z M105 30h22v22h-22z M25 70h22v22H25z M65 70h22v22H65z M105 70h22v22h-22z"/>,
 <path key="aim" d="M20 90Q75-45 139 70 M128 64l11 6-8 13 M110 90h35"/>,
 <path key="steer" d="M20 88C55 88 40 30 78 30s20 57 60 42 M125 64l13 8-14 8"/>,
 <path key="flock" d="M25 30l12 5-12 5 M55 60l12 5-12 5 M80 25l12 5-12 5 M110 65l12 5-12 5 M45 90l12 5-12 5"/>,
 <path key="camera" d="M20 25h120v70H20z M65 48h30v25H65z M30 60h20 M110 60h20 M80 32v10 M80 80v10"/>,
 <path key="visibility" d="M20 60l120-35 M20 60l120 35 M20 60h60 M80 40v40 M140 25v70"/>
];
export default function Home(){return <main id="main" className="portfolio-home">
 <section className="wrap new-hero"><div><span className="eyebrow">PRITISH CHOUDHARY / UNITY DEVELOPER</span><h1>Unity engineering.<br/><span>Made explorable.</span></h1><p>Physics, motion and tools you can understand by using them. Pick a practical problem, change one input and see what happens.</p><div className="hero-actions"><a className="button dark" href="#experiments">Explore the lab</a><Link className="text-link" href="/about/">Meet the developer</Link></div><div className="hero-facts"><span>{projects.length} working studies</span><span>Multiple real use cases</span><span>You control the experiment</span></div></div><aside className="field-guide"><span className="eyebrow">A BETTER WAY TO LEARN A SYSTEM</span><h2>Small changes.<br/>Visible consequences.</h2><ol><li><strong>Choose the job</strong><p>A camera, a vehicle or a moving target.</p></li><li><strong>Tune one thing</strong><p>Keep a fixed reference beside your setup.</p></li><li><strong>Explain the trade-off</strong><p>Try a challenge. Find where the model breaks.</p></li></ol></aside></section>
 <section id="experiments" className="wrap lab-collection"><div className="section-heading"><div><span className="eyebrow">THE PORTFOLIO</span><h2>Unity Engineering Lab</h2></div><p>{projects.length} working studies. Each connects one underlying idea to different applications, useful controls and an honest set of limits.</p></div><div className="experiment-cards">{projects.map((p,i)=><Link key={p.slug} href={`/projects/${p.slug}/`} className="experiment-card"><div className="card-visual" aria-hidden="true"><svg viewBox="0 0 160 120" fill="none" stroke="currentColor" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">{sketches[i]}</svg><span>{p.number}</span></div><div className="card-copy"><span className="eyebrow">{p.category}</span><h3>{p.title}</h3><p>{p.description}</p><span className="card-action">Open experiment <span aria-hidden="true">+</span></span></div></Link>)}</div></section>
 <section className="feature"><div className="wrap"><div className="section-heading"><div><span className="eyebrow">START WITH A FAMILIAR PROBLEM</span><h2>One spring.<br/>Four different jobs.</h2></div><p>Suspension, interfaces, cameras and impacts.<br/>Use the controls to discover why the tuning changes.</p></div><SpringLab/><div className="feature-foot"><span>Browser teaching models with corresponding native Unity work.</span><Link className="text-link" href="/projects/spring-physics-lab/">Explore Spring Physics Lab</Link></div></div></section>
 <section className="wrap lab-principles"><span className="eyebrow">HOW THE LAB IS BUILT</span><h2>Show the mechanism.<br/>Keep the limits visible.</h2><div className="profile-grid"><article><h3>Practical first</h3><p>Multiple applications of the same system. A useful tuning change should explain a different design decision.</p></article><article><h3>Compare and measure</h3><p>Named references, repeatable stress cases and learning challenges make the trade-offs inspectable.</p></article><article><h3>Clear about validation</h3><p>Browser models and native Unity work are identified separately. A working browser study is not a validated WebGL player.</p></article></div></section>
 </main>}
