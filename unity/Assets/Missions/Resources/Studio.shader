Shader "EngineeringLab/Studio" {
 Properties {_Color("Color",Color)=(.2,.4,.4,1)}
 SubShader { Tags {"RenderType"="Opaque"} LOD 100
 Pass { Tags {"LightMode"="ForwardBase"}
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_fwdbase
 #include "UnityCG.cginc"
 #include "Lighting.cginc"
 #include "AutoLight.cginc"
 struct v2f {float4 pos:SV_POSITION;float3 normal:TEXCOORD0;float3 world:TEXCOORD1;SHADOW_COORDS(2)};
 float4 _Color;
 v2f vert(appdata_base v){v2f o;o.pos=UnityObjectToClipPos(v.vertex);o.normal=UnityObjectToWorldNormal(v.normal);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;TRANSFER_SHADOW(o);return o;}
 fixed4 frag(v2f i):SV_Target{float3 n=normalize(i.normal);float3 l=normalize(UnityWorldSpaceLightDir(i.world));float3 v=normalize(UnityWorldSpaceViewDir(i.world));float shadow=SHADOW_ATTENUATION(i);float diffuse=.36+.56*max(0,dot(n,l))*shadow;float spec=pow(max(0,dot(n,normalize(l+v))),40)*.11*shadow;float rim=pow(1-max(0,dot(n,v)),3)*.045;return fixed4(_Color.rgb*diffuse+spec+rim,1);}
 ENDCG
 }
 UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
 }
 Fallback "Diffuse"
}
