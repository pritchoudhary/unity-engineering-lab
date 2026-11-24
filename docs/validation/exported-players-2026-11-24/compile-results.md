# Exact-source C# compilation — 2026-11-24

Five configurations passed using Unity 6000.5.2f1's installed Roslyn compiler and Unity reference assemblies. All final invocations exited 0 with no compiler diagnostics. No production source changes were needed.

| Configuration | C# files | Conditional symbols | Exit | Diagnostics |
| --- | ---: | --- | ---: | --- |
| macos-runtime | 28 | `UNITY_STANDALONE, UNITY_STANDALONE_OSX` | 0 | None |
| webgl-runtime | 28 | `UNITY_WEBGL` | 0 | None |
| macos-validation | 28 | `UNITY_STANDALONE, UNITY_STANDALONE_OSX, ENGINEERING_LAB_VALIDATION, DEVELOPMENT_BUILD` | 0 | None |
| webgl-validation | 28 | `UNITY_WEBGL, ENGINEERING_LAB_VALIDATION, DEVELOPMENT_BUILD` | 0 | None |
| editor | 51 | `UNITY_EDITOR, UNITY_EDITOR_OSX, UNITY_STANDALONE, UNITY_STANDALONE_OSX` | 0 | None |

All configurations additionally define `UNITY_6000_0_OR_NEWER` and `UNITY_6000_5_OR_NEWER`. Runtime configurations exclude every source under an `Editor` directory and exclude all `UnityEditor*` module assemblies. The Editor configuration includes all 51 sources, Editor module references and the installed macOS build-support extension.

## Source identity

All-source SHA-256 manifest digest: `8d806560ca7ca91cfa42d04d068b47d777465b398dacba97afcade51413b4b9f`.

The digest covers the 51 current `unity/Assets/**/*.cs` files, including previously untracked additions. It is SHA-256 of a newline-joined, path-sorted manifest of `repository-relative-path`, one space and SHA-256 of the file bytes, without a trailing newline. The digest matched before/after compilation and again after the final Editor invocation. This identifies the checked source independently of the future commit containing this report.

This compile does not hash or validate shaders, link.xml, JavaScript plugins, HTML templates, serialized scenes or package configuration. Those require their own checks and actual exports.

## Compiler command and reproduction

Run the following Python from the repository root. It writes compiler response files, assemblies, source manifest and JSON results to a temporary directory, and prints that result location. Compiler arguments are `-target:library -nostdlib+ -langversion:latest -warn:4 -unsafe+`, the symbols shown above, explicit references and explicit source paths. This uses the installed Unity .NET Standard 2.1 reference set and compatibility shims; it is a direct Roslyn compile, not Unity's full import/build pipeline.

```python
from pathlib import Path
import hashlib, json, subprocess, tempfile
root=Path.cwd()
u=Path('/Applications/Unity/Hub/Editor/6000.5.2f1/Unity.app/Contents/Resources/Scripting')
out=Path(tempfile.mkdtemp(prefix='lab-final-compile-'))
files=sorted((root/'unity/Assets').rglob('*.cs'))
def digest():
 manifest='\n'.join(str(f.relative_to(root))+' '+hashlib.sha256(f.read_bytes()).hexdigest() for f in files)
 return hashlib.sha256(manifest.encode()).hexdigest(),manifest
initial,manifest=digest()
framework=sorted((u/'NetStandard/ref/2.1.0').glob('*.dll'))+sorted((u/'NetStandard/compat/2.1.0/shims/netfx').glob('*.dll'))
engine=sorted((u/'Managed/UnityEngine').glob('*.dll'))
configs=[('macos-runtime',['UNITY_STANDALONE','UNITY_STANDALONE_OSX'],False),('webgl-runtime',['UNITY_WEBGL'],False),('macos-validation',['UNITY_STANDALONE','UNITY_STANDALONE_OSX','ENGINEERING_LAB_VALIDATION','DEVELOPMENT_BUILD'],False),('webgl-validation',['UNITY_WEBGL','ENGINEERING_LAB_VALIDATION','DEVELOPMENT_BUILD'],False),('editor',['UNITY_EDITOR','UNITY_EDITOR_OSX','UNITY_STANDALONE','UNITY_STANDALONE_OSX'],True)]
results=[]
for name,defs,editor in configs:
 sources=files if editor else [f for f in files if 'Editor' not in f.relative_to(root/'unity/Assets').parts]
 refs=framework+[p for p in engine if editor or not p.name.startswith('UnityEditor')]
 if editor: refs+=[Path('/Applications/Unity/Hub/Editor/6000.5.2f1/PlaybackEngines/MacStandaloneSupport/UnityEditor.OSXStandalone.Extensions.dll')]
 args=['-nologo','-target:library','-nostdlib+','-langversion:latest','-warn:4','-unsafe+','-define:'+','.join(defs+['UNITY_6000_0_OR_NEWER','UNITY_6000_5_OR_NEWER']),'-out:'+str(out/(name+'.dll'))]
 args+=['-r:'+str(p) for p in refs]+[str(p) for p in sources]
 rsp=out/(name+'.rsp');rsp.write_text('\n'.join('"'+x+'"' for x in args))
 p=subprocess.run([str(u/'NetCoreRuntime/dotnet'),str(u/'DotNetSdk/sdk/8.0.318/Roslyn/bincore/csc.dll'),'@'+str(rsp)],capture_output=True,text=True,cwd=root)
 result={'configuration':name,'defines':defs,'editor_references':editor,'sources':len(sources),'exit_code':p.returncode,'diagnostics':p.stdout+p.stderr}
 results.append(result)
 print(json.dumps({**result, 'diagnostics': result['diagnostics'][:1800]}))
final,_=digest()
summary={'unity':'6000.5.2f1','initial_digest':initial,'final_digest':final,'unchanged':initial==final,'source_count':len(files),'results':results,'artifact_directory':str(out)}
(out/'result.json').write_text(json.dumps(summary,indent=2));(out/'source-manifest.txt').write_text(manifest)
print('RESULT_JSON '+str(out/'result.json'))

```

## Setup corrections and limits

An initial ad hoc command incorrectly combined the legacy monolithic `Managed/UnityEngine.dll` with modular assemblies, causing duplicate-type reference errors. Removing that duplicate reference corrected the command. An initial Editor command omitted `UnityEditor.OSXStandalone.Extensions.dll`; supplying the installed extension resolved its namespace reference errors. Neither required source edits; final commands above have both corrections.

These checks establish conditional C# syntax/API-reference compatibility with the installed Unity assemblies. They do not establish player execution, IL2CPP linking, WebGL stripping, shader compilation, physical input behavior or scene appearance. No new native Editor, macOS player or WebGL runtime success is claimed here. Parent task reports the independent Editor retry stalled in macOS HIServices before import and was stopped; that limitation is not converted into a passing Unity run.
