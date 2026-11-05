const reference = (name,target,required=true,kind='object',runtimeAssigned=false) => ({name,target,required,kind,runtimeAssigned});
const component = (id,type,references=[]) => ({id,type,references});
const object = (id,name,parent=null,active=true,components=[]) => ({id,name,parent,active,components});
export const sceneHealthLessons = [
  {id:'prefab-migration',title:'A prefab lost its behaviour',question:'Can you find a missing script without confusing optional fields with errors?',
   tunable:'Scan the whole scene, then only the migrated prefab subtree.',
   use:'Audit a known migration before testing the affected prefab.',avoid:'A null optional field does not prove corruption. This manifest cannot discover which script should replace a missing one.',
   scene:{assets:[],objects:[object('scene','Migration scene'),object('door','Migrated door','scene',true,[component('slot-1',null),component('audio','DoorAudio',[reference('optionalChime',null,false,'asset')])]),object('player','Player','scene',true,[component('move','PlayerMovement')])] }},
  {id:'dependency-handoff',title:'A scene looks correct but cannot connect its systems',question:'Which links are broken, and which unassigned fields are intentional?',
   tunable:'Compare error-only with warning-and-error review; scan just the player while resolving links against the complete scene.',
   use:'Check explicit reference contracts before handing a scene to another developer.',avoid:'Runtime-assigned fields and incomplete exports need context. A missing manifest ID is not proof that the original Unity asset was deleted.',
   scene:{assets:['jump-audio'],objects:[object('scene','Handoff scene'),object('camera','Camera','scene'),object('player','Player','scene',true,[component('controller','PlayerController',[reference('camera','camera'),reference('spawn','deleted-spawn'),reference('optionalSound','absent-sound',false,'asset'),reference('runtimeService',null,true,'object',true),reference('jumpSound','jump-audio',true,'asset')])])] }},
  {id:'inactive-hierarchy',title:'A working object disappears under a disabled parent',question:'Does hiding inactive objects make the scene healthier, or merely hide evidence?',
   tunable:'Toggle inclusion of inactive objects, then the inactive-review rule. Compare coverage and detected counts.',
   use:'Review inactive hierarchy and intentionally pooled objects alongside actual defects.',avoid:'Inactive is informational, not automatically a bug. Enabling everything can break pooling and staging.',
   scene:{assets:[],objects:[object('scene','Level'),object('pool','Pool','scene',false),object('enemy','Pooled enemy','pool',true,[component('ai','EnemyAI',[reference('patrolRoute',null)])]),object('exit','Exit','scene',true,[component('trigger','ExitTrigger')])] }},
];
