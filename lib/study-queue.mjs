// At most one active calculation and one latest request. Intermediate slider positions never queue up.
export class StudyQueue{
 constructor(send,accept){this.send=send;this.accept=accept;this.active=null;this.latest=null;this.disposed=false;}
 offer(job){if(this.disposed)return;this.latest=job;if(!this.active)this.dispatch();}
 dispatch(){this.active=this.latest;this.send(this.active);}
 resolve(message){
  if(this.disposed||!this.active||message.id!==this.active.id)return;
  this.active=null;
  if(message.id===this.latest.id)this.accept(message);else this.dispatch();
 }
 dispose(){this.disposed=true;this.active=this.latest=null;}
}
