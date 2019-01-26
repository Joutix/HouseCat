// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

Shader "HouseCat/sketch subsurface"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (1,1,1,1)
		_HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1.0)

		//DIFFUSE
		_MainTex ("Main Texture", 2D) = "white" {}
		_DiffTint ("Diffuse Tint", Color) = (0.7,0.8,1,1)
	[TCP2Separator]

		//TOONY COLORS RAMP
		[TCP2Header(RAMP SETTINGS)]

		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]

	[TCP2HeaderHelp(SUBSURFACE SCATTERING, Subsurface Scattering)]
		_SSDistortion ("Distortion", Range(0,2)) = 0.2
		_SSPower ("Power", Range(0.1,16)) = 3.0
		_SSScale ("Scale", Float) = 1.0
		_SSColor ("Color (RGB)", Color) = (0.5,0.5,0.5,1)
	[TCP2Separator]

	[TCP2HeaderHelp(RIM, Rim)]
		//RIM LIGHT
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim Min", Range(0,2)) = 0.5
		_RimMax ("Rim Max", Range(0,2)) = 1.0
	[TCP2Separator]

	[TCP2HeaderHelp(SKETCH, Sketch)]
		//SKETCH
		_SketchTex ("Sketch (Alpha)", 2D) = "white" {}
		_SketchColor ("Sketch Color", Color) = (0,0,0,1)
	[TCP2Separator]

	[TCP2HeaderHelp(MISC)]
		[NoScaleOffset] _NoTileNoiseTex ("Non-repeat tiling Noise", 2D) = "white" {}
	[TCP2Separator]


		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{

		Tags { "RenderType"="Opaque" }

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom fullforwardshadows addshadow vertex:vert exclude_path:deferred exclude_path:prepass
		#pragma target 3.0

		//================================================================
		// VARIABLES

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _NoTileNoiseTex;
		float4 _NoTileNoiseTex_TexelSize;
		half _SSDistortion;
		half _SSPower;
		half _SSScale;
		fixed4 _SSColor;
		fixed4 _RimColor;
		fixed _RimMin;
		fixed _RimMax;
		float4 _RimDir;

		#define UV_MAINTEX uv_MainTex

		struct Input
		{
			half2 uv_MainTex;
			float3 viewDir;
			half4 sketchUv;
		};
	// No Tiling texture fetch function
	// (c) 2017 Inigo Quilez - MIT License
	// Source: http://www.iquilezles.org/www/articles/texturerepetition/texturerepetition.htm
	float4 tex2DnoTile( sampler2D samp, in float2 uv )
	{
		// sample variation pattern    
		float k = tex2D(_NoTileNoiseTex, (1/_NoTileNoiseTex_TexelSize.zw)*uv).a; // cheap (cache friendly) lookup    

		// compute index    
		float index = k*8.0;
		float i = floor(index);
		float f = frac(index);

		// offsets for the different virtual patterns    
		float2 offa = sin(float2(3.0,7.0)*(i+0.0)); // can replace with any other hash    
		float2 offb = sin(float2(3.0,7.0)*(i+1.0)); // can replace with any other hash    

		// compute derivatives for mip-mapping    
		float2 dx = ddx(uv);
		float2 dy = ddy(uv);

		// sample the two closest virtual patterns    
		float4 cola = tex2Dgrad(samp, uv + offa, dx, dy);
		float4 colb = tex2Dgrad(samp, uv + offb, dx, dy);

		// interpolate between the two virtual patterns    
		return lerp(cola, colb, smoothstep(0.2,0.8,f-0.1*dot(cola-colb, 1)));
	}


		//================================================================
		// CUSTOM LIGHTING

		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		half _RampThreshold;
		half _RampSmooth;
		sampler2D _SketchTex;
		float4 _SketchTex_ST;
		half4 _SketchColor;
		fixed4 _DiffTint;

		// Instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			half2 ScreenUVs;
		};

		inline half4 LightingToonyColorsCustom (inout SurfaceOutputCustom s, half3 viewDir, UnityGI gi)
		{
		#define IN_NORMAL s.Normal
	
			half3 lightDir = gi.light.dir;
		#if defined(UNITY_PASS_FORWARDBASE)
			half3 lightColor = _LightColor0.rgb;
			half atten = s.atten;
		#else
			half3 lightColor = gi.light.color.rgb;
			half atten = 1;
		#endif

			IN_NORMAL = normalize(IN_NORMAL);
			fixed ndl = max(0, dot(IN_NORMAL, lightDir));
			#define NDL ndl

			#define		RAMP_THRESHOLD	_RampThreshold
			#define		RAMP_SMOOTH		_RampSmooth

			fixed3 ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, NDL);
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif
			//Sketch
			#define SKETCH_RGB	sketchRgb
			fixed sketch = tex2DnoTile(_SketchTex, s.ScreenUVs).a;
			sketch = lerp(sketch, 1, ramp);	//Regular sketch overlay
			half3 sketchRgb = lerp(_SketchColor, half3(1,1,1), sketch);
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
			fixed3 wrappedLight = saturate(_DiffTint.rgb + saturate(dot(IN_NORMAL, lightDir)));
			ramp *= wrappedLight;
			fixed4 c;
			c.rgb = s.Albedo * lightColor.rgb * ramp;
			c.a = s.Alpha;
		#if (POINT || SPOT)
			//Subsurface Scattering
			half3 ssLight = lightDir + IN_NORMAL * _SSDistortion;
			half ssDot = pow(saturate(dot(viewDir, -ssLight)), _SSPower) * _SSScale;
			half3 ssColor = (ssDot * _SSColor.rgb);
			ssColor.rgb *= lightColor.rgb;
		#if !defined(UNITY_PASS_FORWARDBASE)
			ssColor.rgb *= atten;
		#endif
			c.rgb += s.Albedo * ssColor;
		#endif
			c.rgb *= SKETCH_RGB;

		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

			return c;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom s, UnityGIInput data, inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, IN_NORMAL);

			s.atten = data.atten;	//transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb;	//remove attenuation
		}

		//Vertex input
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		#if defined(LIGHTMAP_ON) && defined(DIRLIGHTMAP_COMBINED)
			float4 tangent : TANGENT;
		#endif
	#if UNITY_VERSION >= 550
			UNITY_VERTEX_INPUT_INSTANCE_ID
	#endif
		};

		//================================================================
		// VERTEX FUNCTION

		void vert(inout appdata_tcp2 v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			//Sketch
			float4 pos = UnityObjectToClipPos(v.vertex);
			o.sketchUv = ComputeScreenPos(pos);
			o.sketchUv.xy = TRANSFORM_TEX(o.sketchUv, _SketchTex);
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.UV_MAINTEX);
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;

			//Sketch
			float2 screenUV = IN.sketchUv.xy / IN.sketchUv.w;
			float screenRatio = _ScreenParams.y / _ScreenParams.x;
			screenUV.y *= screenRatio;
			o.ScreenUVs = screenUV;

			//Rim
			float3 viewDir = normalize(IN.viewDir);
			half rim = 1.0f - saturate( dot(viewDir, o.Normal) );
			rim = smoothstep(_RimMin, _RimMax, rim);
			o.Emission += (_RimColor.rgb * rim) * _RimColor.a;
		}

		ENDCG
	}

	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
