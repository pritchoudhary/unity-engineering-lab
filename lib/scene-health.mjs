/** Read-only rules for a synthetic scene manifest; this is not a Unity scene parser. */
const ranks = {info:0, warning:1, error:2};
const text = value => typeof value === 'string' && value.trim().length > 0;
function requireThat(condition, message) { if (!condition) throw new TypeError(message); }

function validate(scene) {
  requireThat(scene && Array.isArray(scene.objects) && Array.isArray(scene.assets), 'Expected objects and assets arrays');
  const objects = new Map();
  const assets = new Set();
  for (const id of scene.assets) {
    requireThat(text(id) && !assets.has(id), 'Asset IDs must be nonempty and unique');
    assets.add(id);
  }
  for (const object of scene.objects) {
    requireThat(object && text(object.id) && !objects.has(object.id), 'Object IDs must be nonempty and unique');
    requireThat(text(object.name) && typeof object.active === 'boolean' && (object.parent === null || text(object.parent)), 'Invalid object metadata');
    requireThat(Array.isArray(object.components), 'Expected components array');
    const ids = new Set();
    for (const component of object.components) {
      requireThat(component && text(component.id) && !ids.has(component.id), 'Component IDs must be unique within an object');
      ids.add(component.id);
      requireThat(component.type === null || text(component.type), 'A missing script must have type null');
      requireThat(Array.isArray(component.references), 'Expected references array');
      const names = new Set();
      for (const reference of component.references) {
        requireThat(reference && text(reference.name) && !names.has(reference.name), 'Reference names must be unique within a component');
        names.add(reference.name);
        requireThat(reference.target === null || text(reference.target), 'Reference target must be null or an ID');
        requireThat(reference.kind === 'object' || reference.kind === 'asset', 'Invalid reference kind');
        requireThat(typeof reference.required === 'boolean' && typeof reference.runtimeAssigned === 'boolean', 'Reference policy must be explicit');
      }
    }
    objects.set(object.id, object);
  }
  for (const object of objects.values()) {
    const visited = new Set(); let cursor = object;
    while (cursor) {
      requireThat(!visited.has(cursor.id), 'Hierarchy contains a cycle'); visited.add(cursor.id);
      requireThat(cursor.parent === null || objects.has(cursor.parent), 'Hierarchy contains an unknown parent');
      cursor = cursor.parent === null ? null : objects.get(cursor.parent);
    }
  }
  return {objects, assets};
}

export function inspectScene(scene, options = {}) {
  const {objects, assets} = validate(scene);
  const {rootId = null, includeInactive = true, reviewInactive = true, minimumSeverity = 'info'} = options;
  requireThat(rootId === null || objects.has(rootId), 'Unknown scan root');
  requireThat(typeof includeInactive === 'boolean' && typeof reviewInactive === 'boolean', 'Scan flags must be booleans');
  requireThat(Object.hasOwn(ranks, minimumSeverity), 'Unknown severity');
  const all = []; let scannedObjects = 0, scopedObjects = 0;
  for (const object of objects.values()) {
    const ancestors = []; let cursor = object;
    while (cursor) { ancestors.unshift(cursor); cursor = cursor.parent === null ? null : objects.get(cursor.parent); }
    if (rootId !== null && !ancestors.some(item => item.id === rootId)) continue;
    scopedObjects++;
    const inactiveAncestor = ancestors.find(item => !item.active);
    if (!includeInactive && inactiveAncestor) continue;
    scannedObjects++;
    const path = ancestors.map(item => item.name).join('/');
    const add = (rule, severity, componentId, field, evidence, nextStep) => all.push({
      id:JSON.stringify([object.id, componentId, field, rule]), objectId:object.id, path,
      componentId, field, rule, severity, evidence, nextStep,
    });
    if (reviewInactive && inactiveAncestor) add('inactive-object', 'info', null, null,
      `Inactive in hierarchy because ${inactiveAncestor.name} (${inactiveAncestor.id}) is inactive.`,
      'Check whether this is intentional pooling, staging, or a disabled parent. Do not enable automatically.');
    for (const component of object.components) {
      if (component.type === null) {
        add('missing-script', 'error', component.id, null, 'This modeled component slot has no resolved script type.',
          'Inspect the original component and restore its script or deliberately remove the obsolete slot after review.');
        continue; // Unknown script fields cannot be inspected reliably.
      }
      for (const reference of component.references) {
        if (reference.target === null) {
          if (reference.required && !reference.runtimeAssigned) add('required-reference', 'error', component.id, reference.name,
            `${component.type}.${reference.name} is required by the fixture contract but unassigned.`,
            'Assign the intended target or correct the documented contract. Null alone does not establish a Unity error.');
          continue;
        }
        const exists = reference.kind === 'object' ? objects.has(reference.target) : assets.has(reference.target);
        if (!exists) add('unresolved-reference', reference.required ? 'error' : 'warning', component.id, reference.name,
          `${component.type}.${reference.name} points to ${reference.kind} ID ${reference.target}, absent from this complete manifest.`,
          'Locate the intended target and verify the export includes it. Do not replace the reference with an arbitrary object.');
      }
    }
  }
  all.sort((a,b) => ranks[b.severity]-ranks[a.severity] || (a.id < b.id ? -1 : a.id > b.id ? 1 : 0));
  const findings = all.filter(item => ranks[item.severity] >= ranks[minimumSeverity]);
  return {findings, scannedObjects, scopedObjects, totalObjects:objects.size,
    excludedInactive:scopedObjects-scannedObjects, hiddenBySeverity:all.length-findings.length,
    detectedCounts:{error:all.filter(f=>f.severity==='error').length, warning:all.filter(f=>f.severity==='warning').length, info:all.filter(f=>f.severity==='info').length}};
}
