'use client';
import { useEffect } from 'react';
export default function EmbedMode(){useEffect(()=>{if(window.self!==window.top)document.documentElement.classList.add('in-portfolio');return()=>document.documentElement.classList.remove('in-portfolio');},[]);return null;}
