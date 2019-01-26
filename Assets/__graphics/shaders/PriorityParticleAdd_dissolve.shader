Shader "Particles/Priority Additive (dissolve)"
{
Properties
{
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	[Space]
	_DissolveTex ("Dissolve Tex", 2D) = "gray" {}
	_DissolveSmooth ("Dissolve Smoothing", Range(0,0.5)) = 0.2
}

Category
{
	Tags { "Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			sampler2D _DissolveTex;
			float _DissolveSmooth;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 customData : TEXCOORD1;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float2 texcoord_dissolve : TEXCOORD1;
				float4 customData : TEXCOORD2;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD3;
				#endif
			};
			
			float4 _MainTex_ST;
			float4 _DissolveTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex);
				o.texcoord_dissolve.xy = TRANSFORM_TEX(v.texcoord.xy,_DissolveTex);
				o.texcoord.zw = v.texcoord.zw;
				o.customData = v.customData;
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				// particle vertex streams
				float vs_dissolve = i.customData.x;
				float vs_age = i.texcoord.z;
				float vs_stable_random = i.texcoord.w;

				float sign = vs_stable_random > 0.5 ? -1 : 1;

				float dissolve = tex2D(_DissolveTex, sign * i.texcoord_dissolve + vs_stable_random).a;
				dissolve = smoothstep(vs_dissolve - _DissolveSmooth, vs_dissolve + _DissolveSmooth, dissolve);

				float4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				col.a *= dissolve;

				return col;
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
