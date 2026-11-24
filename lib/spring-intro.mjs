// Exact normalized step response: mass=1, stiffness=36, c=12*zeta, target 0 -> 1.
export function springPosition(t,zeta){
 if(!Number.isFinite(t)||t<0||t>3||!Number.isFinite(zeta)||zeta<.2||zeta>1.8)throw new RangeError('Intro bounds');
 const w=6;
 if(Math.abs(zeta-1)<1e-5)return 1-Math.exp(-w*t)*(1+w*t);
 if(zeta<1){const q=Math.sqrt(1-zeta*zeta);return 1-Math.exp(-zeta*w*t)*(Math.cos(w*q*t)+zeta/q*Math.sin(w*q*t));}
 const q=Math.sqrt(zeta*zeta-1),a=-w*(zeta-q),b=-w*(zeta+q);
 return 1+(b*Math.exp(a*t)-a*Math.exp(b*t))/(a-b);
}
export function peakOvershoot(zeta){return zeta>=1?0:100*Math.exp(-Math.PI*zeta/Math.sqrt(1-zeta*zeta));}
