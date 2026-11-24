// A frame's timestamp can precede an effect registered later in that same frame.
export function playbackTime(initial,start,now,duration){
 if(![initial,start,now,duration].every(Number.isFinite)||duration<=0||initial<0||initial>duration)throw new RangeError('Playback bounds');
 return Math.min(duration,initial+Math.max(0,now-start)/1000);
}
