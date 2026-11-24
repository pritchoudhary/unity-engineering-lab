import type { Metadata } from 'next';
import Link from 'next/link';
import PerformanceProbe from '../components/PerformanceProbe';
import EmbedMode from '../components/EmbedMode';
import './globals.css';
export const metadata: Metadata = {
 metadataBase: new URL(process.env.SITE_ORIGIN || 'https://pritishchoudhary.com'),
 title: { default: 'Pritish Choudhary / Engineering Lab', template: '%s | Pritish Choudhary' },
 description: 'Interactive studies in Unity engineering, physics and tools. Explore practical applications, tune the inputs and compare measured outcomes.',
 robots: { index: process.env.LAB_EMBED_MIRROR !== '1', follow: true },
 openGraph: { type: 'website', siteName: 'Pritish Choudhary / Engineering Lab', title: 'Pritish Choudhary / Engineering Lab', url: '/', images: [{ url: '/social-preview.jpg', alt: 'Pritish Choudhary Engineering Lab — practical interactive studies' }] },
 twitter: { card: 'summary_large_image', images: ['/social-preview.jpg'] }
};
export default function Layout({ children }: { children: React.ReactNode }) { return <html lang="en"><body><PerformanceProbe/><EmbedMode/><a className="skip" href="#main">Skip to content</a><header className="wrap nav"><Link href="/" className="brand"><span className="brandmark">pc.</span><span>Pritish Choudhary<small>UNITY ENGINEERING LAB</small></span></Link><nav aria-label="Main navigation"><Link href="/">Lab</Link><Link href="/about/">About</Link><Link href="/experience/">Experience</Link><Link href="/skills/">Skills</Link><Link href="/contact/">Contact</Link></nav></header>{children}<footer className="wrap footer"><span>PRITISH CHOUDHARY <span className="muted">/ Unity Engineering Lab</span></span><a href="https://github.com/pritchoudhary/unity-engineering-lab">GitHub</a></footer></body></html> }
