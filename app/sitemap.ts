export const dynamic = 'force-static';
import type { MetadataRoute } from 'next';
import {projects} from '../lib/projects';
export default function sitemap():MetadataRoute.Sitemap{const origin=process.env.SITE_ORIGIN || 'https://pritishchoudhary.com';return [{url:`${origin}/`,priority:1},...['about','experience','contact'].map(slug=>({url:`${origin}/${slug}/`,priority:.6})),...projects.map(p=>({url:`${origin}/projects/${p.slug}/`,priority:.8}))];}
