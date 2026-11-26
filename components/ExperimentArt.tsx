import type { ReactNode } from 'react';
import styles from './ExperimentArt.module.css';

type Props = { slug: string; large?: boolean; className?: string; decorative?: boolean };
const descriptions: Record<string, string> = {
 'pathfinding-lab':'A robot navigates around warehouse storage through a doorway, with visible graph connections and a sound source.',
  'spring-physics-lab': 'A menu highlight overshoots its destination, then settles along a spring response curve.',
  'scene-health-inspector': 'Three scene objects expose a disconnected dependency beside a deliberate repair connection.',
  'vehicle-suspension': 'A suspended vehicle body rides above two wheels on an uneven proving road.',
  'spline-editor': 'Two movable control handles bend a courier rail through an arrival gate.',
  'scene-placement': 'A sculpture, a bench and a display occupy separate snapped gallery positions.',
  'ballistics-lab': 'Two projectile arcs compare clearing a wall with striking its face.',
  'steering-behaviors': 'A delivery robot bends its route around an obstacle toward an open dock.',
  'flocking-lab': 'Individual agents converge through a narrow gate toward a shared destination.',
  'camera-composition': 'A moving subject leaves a camera dead zone while a wider safe frame preserves context.',
  'visibility-occlusion': 'A wall interrupts one sight ray while a second route reaches the visible relay.',
};

function Drawing({ children }: { children: ReactNode }) {
  return <g fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">{children}</g>;
}
function Node({ x, y, active = false }: { x: number; y: number; active?: boolean }) {
  return <g transform={`translate(${x} ${y})`}><rect x="-23" y="-23" width="46" height="46" rx="12" className={active ? styles.rustFill : styles.panelFill}/><rect x="-9" y="-9" width="18" height="18" rx="4" className={styles.creamFill}/></g>;
}
function Robot({ x, y, rotate = 0 }: { x: number; y: number; rotate?: number }) {
  return <g transform={`translate(${x} ${y}) rotate(${rotate})`}><rect x="-27" y="-20" width="54" height="40" rx="12" className={styles.panelFill}/><rect x="-19" y="-26" width="12" height="7" rx="3" className={styles.creamFill}/><rect x="7" y="-26" width="12" height="7" rx="3" className={styles.creamFill}/><rect x="-19" y="19" width="12" height="7" rx="3" className={styles.creamFill}/><rect x="7" y="19" width="12" height="7" rx="3" className={styles.creamFill}/><path d="M-9-7h18v14H-9z" className={styles.rustFill}/><path d="m16-5 6 5-6 5"/></g>;
}

function Scene({ slug }: { slug: string }) {
  switch (slug) {
    case 'pathfinding-lab': return <Drawing>
      <rect x="65" y="40" width="350" height="220" rx="12" className={styles.faint}/>
      <path d="M205 40v90m0 55v75" strokeWidth="15"/>
      <rect x="280" y="75" width="70" height="45" rx="5" className={styles.panelFill}/>
      <rect x="280" y="190" width="70" height="45" rx="5" className={styles.panelFill}/>
      <path d="M112 87 190 143 220 143 265 135 365 155M112 87 190 172 220 172 365 155M265 135 265 175 365 155" className={styles.faint}/>
      <path d="M112 87 190 143 220 143 365 155" className={styles.rustLine} strokeWidth="5"/>
      {[190,220].map(x=><circle key={x} cx={x} cy="143" r="5" className={styles.rustFill}/>)}
      <Robot x={112} y={87} rotate={35}/><circle cx="365" cy="155" r="11" className={styles.rustFill}/>
      <path d="M382 138q17 17 0 34m10-43q26 26 0 52" className={styles.rustLine}/>
    </Drawing>;
    case 'spring-physics-lab': return <Drawing>
      <rect x="54" y="48" width="372" height="84" rx="20" className={styles.panelFill}/>
      <path d="M178 68v44m124-44v44" className={styles.faint}/>
      <rect x="318" y="66" width="88" height="48" rx="12" className={styles.dashed}/>
      <rect x="222" y="66" width="88" height="48" rx="12" className={`${styles.rustFill} ${styles.lift}`}/>
      <path d="M78 90h53m-53 8h32m73-8h23" className={styles.faint}/>
      <path d="M62 226h356M70 160v74" className={styles.faint}/>
      <path d="M78 222c27 0 34-88 70-62s33 68 64 31 39-24 60-9 32 5 49 0 37 0 77 0" className={styles.rustLine} strokeWidth="3"/>
      <path d="M78 182h320" className={styles.dashed}/><circle cx="398" cy="182" r="5" className={styles.creamFill}/>
    </Drawing>;
    case 'scene-health-inspector': return <Drawing>
      <path d="M115 104v83h87m76 0h87v-83" className={styles.faint}/>
      <path d="M140 78h40m20 0h48" className={styles.rustLine}/>
      <path d="m180 70 8 16m5-16 8 16" className={styles.rustLine}/>
      <path d="M294 78h46"/>
      <Node x={115} y={78}/><Node x={271} y={78} active/><Node x={365} y={78}/><Node x={240} y={187}/>
      <path d="M215 187h-13m76 0h-13" className={styles.rustLine}/>
      <path d="M193 139c30-21 64-21 94 0" className={styles.dashed}/>
      <circle cx="240" cy="138" r="11" className={styles.panelFill}/><path d="m235 138 4 4 7-8"/>
      <path d="M78 235h324" className={styles.faint}/>
    </Drawing>;
    case 'vehicle-suspension': return <Drawing>
      <path d="M40 232h73l15-12h55l19 12h63l20-22h54l15 22h86" className={styles.faint}/>
      <path d="M112 100h62l25-37h98l28 37h45v52H112z" className={styles.panelFill}/>
      <path d="m208 77-15 23h103l-15-23z" className={styles.creamFill}/>
      <path d="M158 143v12l-9 6 18 10-18 10 18 10-9 6v13m164-67v12l-9 6 18 10-18 10 18 10-9 6v13" className={styles.rustLine} strokeWidth="3"/>
      <path d="M112 154h258M240 110v32" className={styles.faint}/>
      {[158,322].map((x) => <g key={x}><circle cx={x} cy="210" r="23" className={styles.panelFill}/><circle cx={x} cy="210" r="10"/><path d={`M${x-5} 206l10 8m0-8-10 8`}/></g>)}
      <path d="M81 115v65m-6-58 6-7 6 7m-12 51 6 7 6-7" className={styles.dashed}/>
    </Drawing>;
    case 'spline-editor': return <Drawing>
      <path d="M65 214 155 55m170 0 90 159" className={styles.faint}/>
      <path d="M65 214C155 55 325 55 415 214" strokeWidth="13" className={styles.track}/>
      <path d="M65 214C155 55 325 55 415 214" strokeWidth="2"/>
      {[155,325].map(x => <g key={x}><circle cx={x} cy="55" r="10" className={styles.rustFill}/><circle cx={x} cy="55" r="3" className={styles.creamFill}/></g>)}
      <g className={styles.lift}><rect x="214" y="82" width="52" height="31" rx="8" className={styles.rustFill}/><path d="M224 92h31m-24 8h17"/></g>
      <path d="M387 233v-43h45v43" strokeWidth="5"/>
      <circle cx="65" cy="214" r="7" className={styles.creamFill}/><path d="M48 242h68m269 0h61" className={styles.faint}/>
    </Drawing>;
    case 'scene-placement': return <Drawing>
      <path d="m48 193 192-98 192 98-192 74z" className={styles.panelFill}/>
      <g className={styles.faint}><path d="m96 169 192 79m-144-103 192 84m-144-109 192 90M96 211l192-93m-144 113 192-86m-144 105 192-80"/></g>
      <g className={styles.lift}><path d="m116 160 27-14 29 14-28 13zM116 160v21l28 13 28-13v-21" className={styles.panelFill}/><path d="m128 146 16-39 16 39-16 10z" className={styles.rustFill}/></g>
      <path d="m217 188 42-22 51 22-43 21z" className={styles.creamFill}/><path d="M225 193v23m73-23v23m-31-7v20"/>
      <path d="m285 137 24-12 25 12v-53l-25-11-24 12z" className={styles.panelFill}/><path d="m293 91 16-8 17 8v30l-17-8-16 8z" className={styles.rustFill}/>
      <path d="m90 87 20 10 20-10m-20 10v29m257 72 18-8 18 8" className={styles.dashed}/>
    </Drawing>;
    case 'ballistics-lab': return <Drawing>
      <path d="M42 229h396" className={styles.faint}/>
      <path d="M218 229v-94h39v94" className={styles.panelFill}/><path d="M225 145h25m-25 15h25m-25 15h25m-25 15h25" className={styles.faint}/>
      <path d="M80 207Q240-98 397 210" className={styles.rustLine} strokeWidth="3"/>
      <path d="M80 207q80-104 138-45" className={styles.dashed}/>
      <path d="m210 154 16 16m0-16-16 16" className={styles.rustLine}/>
      <g className={styles.lift}><circle cx="305" cy="98" r="8" className={styles.rustFill}/><path d="m283 86 8 4m-1-14 8 6"/></g>
      <path d="M66 229v-25l27-18 12 16-23 15v12" className={styles.panelFill}/>
      <ellipse cx="397" cy="213" rx="24" ry="10"/><ellipse cx="397" cy="213" rx="11" ry="4" className={styles.rustLine}/>
    </Drawing>;
    case 'steering-behaviors': return <Drawing>
      <path d="M47 229h386M48 49h384" className={styles.faint}/>
      <path d="M95 193c88 0 61-120 152-120s69 122 138 122" className={styles.dashed}/>
      <ellipse cx="239" cy="173" rx="29" ry="13" className={styles.panelFill}/><path d="M210 173v-40c0-17 58-17 58 0v40" className={styles.panelFill}/><ellipse cx="239" cy="133" rx="29" ry="13"/>
      <Robot x={121} y={176} rotate={-30}/>
      <path d="M367 154h48v73h-48m-7-59h-19m19 45h-19" strokeWidth="4"/>
      <path d="m378 184 8 8-8 8" className={styles.rustLine}/><circle cx="408" cy="143" r="5" className={styles.rustFill}/>
    </Drawing>;
    case 'flocking-lab': return <Drawing>
      <path d="M290 35v79m0 51v80" strokeWidth="8" className={styles.track}/><path d="M290 35v79m0 51v80"/>
      <path d="M63 68c112-13 123 63 227 71 47 0 60-14 91-14M61 210c117 9 134-64 229-71" className={styles.dashed}/>
      {[[76,96,17],[112,73,18],[131,121,8],[83,163,-12],[127,194,-20],[169,93,23],[172,155,-8],[203,124,10],[220,154,-7],[257,137,0],[318,138,-5],[353,120,-10]].map(([x,y,r], i) => <path key={i} transform={`translate(${x} ${y}) rotate(${r})`} d="m-9-6 18 6-18 6 4-6z" className={i>8 ? styles.rustFill : styles.creamFill}/>)}
      <circle cx="392" cy="135" r="22" className={styles.panelFill}/><circle cx="392" cy="135" r="8" className={styles.rustFill}/>
      <path d="M282 118h16m-16 43h16" className={styles.rustLine}/>
    </Drawing>;
    case 'camera-composition': return <Drawing>
      <rect x="48" y="36" width="384" height="208" rx="17" className={styles.panelFill}/>
      <path d="M72 95V60h42m252 0h42v35M72 185v35h42m252 0h42v-35" strokeWidth="3"/>
      <rect x="167" y="90" width="146" height="100" rx="9" className={styles.dashed}/>
      <path d="M68 202h344m-291-9v18m214-18v18" className={styles.faint}/>
      <path d="M160 160h47m-8-6 8 6-8 6" className={styles.rustLine}/>
      <g className={styles.lift}><circle cx="310" cy="111" r="13" className={styles.rustFill}/><path d="M310 126v43m0-30-19 17m19-17 21 12m-21 18-15 28m15-28 17 28" strokeWidth="7" className={styles.rustLine}/></g>
      <path d="M232 133h16m-8-8v16" className={styles.faint}/>
    </Drawing>;
    case 'visibility-occlusion': return <Drawing>
      <path d="M48 228h384M48 49h384" className={styles.faint}/>
      <path d="M233 80v140h32V80z" className={styles.panelFill}/><path d="M240 91h18m-18 15h18m-18 15h18m-18 15h18m-18 15h18m-18 15h18m-18 15h18m-18 15h18" className={styles.faint}/>
      <circle cx="101" cy="166" r="17" className={styles.rustFill}/><path d="M101 193v-10m-17 15h34"/>
      <path d="m118 166 115-15" className={styles.rustLine} strokeWidth="3"/>
      <path d="m265 147 103-13" className={styles.dashed}/><path d="m225 143 16 16m0-16-16 16" className={styles.rustLine}/>
      <path d="M114 155 174 64h194" className={styles.dashed}/>
      <path d="M368 59v19m-10 0h20m-10 46v40m-12 0h24"/>
      <circle cx="368" cy="64" r="13" className={styles.creamFill}/><circle cx="368" cy="134" r="13" className={styles.panelFill}/>
    </Drawing>;
    default: return <Drawing><path d="M60 220h360M60 140h360M60 60h360" className={styles.faint}/><circle cx="240" cy="140" r="60"/><path d="m205 140 25 25 45-50" className={styles.rustLine}/></Drawing>;
  }
}

/** Static, server-renderable artwork; the actual interactive lesson is loaded separately. */
export default function ExperimentArt({ slug, large = false, className = '', decorative = false }: Props) {
  return <div className={`${styles.art} ${large ? styles.large : ''} ${className}`}>
    <svg viewBox="0 0 480 280" width="480" height="280" role={decorative ? undefined : 'img'} aria-hidden={decorative || undefined} aria-label={decorative ? undefined : descriptions[slug] || 'An engineering study with a controlled comparison.'} focusable="false">
      <Scene slug={slug}/>
    </svg>
  </div>;
}
