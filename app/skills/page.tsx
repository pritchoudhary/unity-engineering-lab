import type { Metadata } from 'next';
import Link from 'next/link';
import styles from '../../components/Career.module.css';

export const metadata: Metadata = {
  title: 'Skills & Experience',
  description: 'Explore Unity engineering capabilities alongside professional experience and practical lab examples.',
  alternates: { canonical: '/experience/' },
  robots: { index: false, follow: true }
};

export default function Skills() {
  return <main id="main" className={`wrap ${styles.legacy}`}>
    <span className="eyebrow">SKILLS & EXPERIENCE / NOW TOGETHER</span>
    <h1>The skills.<br/><span>The work behind them.</span></h1>
    <p>Unity engineering capabilities now sit alongside professional experience, practical examples and the tools behind the work.</p>
    <div className={styles.legacyLinks}>
      <Link className={styles.link} href="/experience/#capabilities">Explore capabilities</Link>
      <Link className={styles.link} href="/experience/#experience">Read professional experience</Link>
      <Link className={styles.link} href="/experience/#toolkit">See the toolkit</Link>
    </div>
  </main>;
}
