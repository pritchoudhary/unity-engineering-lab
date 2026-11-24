mergeInto(LibraryManager.library, {
  EngineeringLabValidationReport: function (jsonPointer) {
    var json = UTF8ToString(jsonPointer);
    var details = document.getElementById('engineering-lab-validation-details');
    if (!details) {
      details = document.createElement('details');
      details.id = 'engineering-lab-validation-details';
      var summary = document.createElement('summary');
      summary.textContent = 'Unity validation report';
      details.appendChild(summary);
      var pre = document.createElement('pre');
      pre.id = 'engineering-lab-validation-result';
      pre.style.maxHeight = '360px';
      pre.style.overflow = 'auto';
      pre.style.whiteSpace = 'pre-wrap';
      pre.style.overflowWrap = 'anywhere';
      details.appendChild(pre);
      details.style.position = 'absolute';
      details.style.left = '50%';
      details.style.transform = 'translateX(-50%)';
      details.style.width = 'calc(100% - 32px)';
      details.style.maxWidth = '960px';
      details.style.boxSizing = 'border-box';
      details.style.padding = '12px';
      details.style.font = '14px/1.5 system-ui, sans-serif';
      details.style.color = '#e4eee4';
      details.style.background = '#18332d';
      details.style.border = '1px solid #45634e';
      document.body.appendChild(details);
    }
    // Position below the actual canvas, including templates with an absolutely positioned game.
    var canvas = document.getElementById('unity-canvas') || document.querySelector('canvas');
    var bottom = canvas ? canvas.getBoundingClientRect().bottom + window.scrollY : document.documentElement.clientHeight;
    details.style.top = (bottom + 16) + 'px';
    // Text only: the result is local diagnostic data, never HTML, executable code or a network request.
    document.getElementById('engineering-lab-validation-result').textContent = json;
  }
});
