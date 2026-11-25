import type { Metadata } from 'next';
import Link from 'next/link';
import {notFound} from 'next/navigation';
import ProjectExperiment from '../../../components/ProjectExperiment';
import ExperimentArt from '../../../components/ExperimentArt';
import SpringLab from '../../../components/SpringLab';
import styles from '../../../components/LabExperience.module.css';
import {projects} from '../../../lib/projects';
import {experimentGuides} from '../../../lib/experiment-guides';
export async function generateMetadata({params}:{params:Promise<{slug:string}>}):Promise<Metadata>{
 const {slug}=await params,p=projects.find(x=>x.slug===slug);if(!p)return {};
 return {title:p.title,description:p.description,alternates:{canonical:`/projects/${slug}/`},openGraph:{type:'website',title:p.title,description:p.description,url:`/projects/${slug}/`,images:[{url:'/social-preview.jpg'}]},twitter:{card:'summary_large_image',title:p.title,description:p.description,images:['/social-preview.jpg']}};
}
export function generateStaticParams(){return projects.map(p=>({slug:p.slug}));}
export default async function Project({params}:{params:Promise<{slug:string}>}){
 const {slug}=await params,p=projects.find(x=>x.slug===slug);if(!p)notFound();
 const guide=experimentGuides[slug],index=projects.indexOf(p),next=projects[(index+1)%projects.length];
 return <main id="main" className={`wrap ${styles.detail}`}><Link className={styles.back} href="/#experiments">All experiments</Link>
  <section className={styles.detailHero}><div><span className={styles.introLabel}>{p.title} / {guide.group}</span><h1>{guide.hook}</h1><p>{guide.payoff}</p><div className={styles.detailActions}><a href="#workbench" className="button dark">Try the experiment</a><a href="#failure" className="text-link">Find its limits</a></div></div><div className={styles.detailArt}><ExperimentArt slug={slug} large/></div></section>
  <div className={styles.applicationStrip} aria-label="Practical applications">{guide.applications.map(a=><span key={a}>{a}</span>)}</div>
  <section className={styles.brief} aria-label="Your experiment briefing"><article><h2>The question</h2><p>{p.question}</p></article><article><h2>Your first change</h2><p>{guide.tune}</p></article><article><h2>The evidence</h2><p>{guide.compare}</p></article></section>
  <section id="workbench" className={styles.workbench}><h2>Take the controls</h2><p>Choose an application below. Use its named reference and repeatable inputs to judge your result.</p><ProjectExperiment slug={slug}/></section>
  <section id="failure" className={styles.failure}><h2>Push it until the trade-off appears.</h2><p>{guide.failure}</p></section>
  {slug==='spring-physics-lab'&&<details className="model-details"><summary>Under the hood: inspect the spring model</summary><SpringLab/><div className="notes"><section><h2>What pulls it back?</h2><p>A spring pulls toward equilibrium with force −kx. A damper opposes velocity with force −cv. Together they produce acceleration (−kx − cv) / m.</p></section><section><h2>Why does it feel different?</h2><p>The damping ratio ζ = c / 2√(mk) separates oscillation, critical damping and a slower non-oscillating response. The browser integrator uses a fixed 1/240 second step; numerical approximation still introduces error.</p></section><section><h2>What is validated?</h2><p>This is a JavaScript teaching model with corresponding C# source and saved Unity scenes. It is not an embedded Unity WebGL build. Clean final exported-player validation remains pending.</p></section></div></details>}
  <nav className={styles.next} aria-label="Keep exploring"><Link prefetch={false} href={`/projects/${next.slug}/`}>Next question: {experimentGuides[next.slug].hook}</Link><a href="https://github.com/pritchoudhary/unity-engineering-lab">Inspect the Unity source</a></nav>
 </main>;
}
