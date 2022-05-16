// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TimeFight/Asteroid"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,0,0,1)
		_voronoiAngle("voronoiAngle", Float) = 1.15
		_voronoiScale("voronoiScale", Float) = 4.07
		_voronoiPow("voronoiPow", Float) = 0.99
		_voronoiMultiplier("voronoiMultiplier", Float) = 11.22
		_cracksColor("cracksColor", Color) = (0,0,0,1)
		_noiseScale("noiseScale", Float) = 3.26
		_breakValue("breakValue", Range( 0 , 1)) = 1
		_maxBrokenValue("maxBrokenValue", Range( 0 , 1)) = 1
		_triplanarBlending("triplanarBlending", Range( 0 , 100)) = 16.47059
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _cracksColor;
		uniform float _voronoiScale;
		uniform float _voronoiAngle;
		uniform float _triplanarBlending;
		uniform float _voronoiPow;
		uniform float _voronoiMultiplier;
		uniform float _noiseScale;
		uniform float _maxBrokenValue;
		uniform float _breakValue;


		float2 voronoihash5( float2 p )
		{
			p = p - 6 * floor( p / 6 );
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash5( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			
F1 = 8.0;
for ( int j = -2; j <= 2; j++ )
{
for ( int i = -2; i <= 2; i++ )
{
float2 g = mg + float2( i, j );
float2 o = voronoihash5( n + g );
		o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
float d = dot( 0.5 * ( r + mr ), normalize( r - mr ) );
F1 = min( F1, d );
}
}
return F1;
		}


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float time5 = _voronoiAngle;
			float2 voronoiSmoothId5 = 0;
			float3 ase_worldPos = i.worldPos;
			float3 worldToObj53 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			ase_vertexNormal = normalize( ase_vertexNormal );
			float3 temp_cast_0 = (_triplanarBlending).xxx;
			float3 temp_output_87_0 = pow( abs( ase_vertexNormal ) , temp_cast_0 );
			float dotResult88 = dot( temp_output_87_0 , float3( 1,1,1 ) );
			float2 lerpResult94 = lerp( (worldToObj53).xz , (worldToObj53).yz , ( temp_output_87_0 / dotResult88 ).x);
			float2 lerpResult95 = lerp( lerpResult94 , (worldToObj53).xy , float2( 0,0 ));
			float2 coords5 = lerpResult95 * _voronoiScale;
			float2 id5 = 0;
			float2 uv5 = 0;
			float voroi5 = voronoi5( coords5, time5, id5, uv5, 0, voronoiSmoothId5 );
			float temp_output_60_0 = step( 0.5 , ( 1.0 - ( pow( voroi5 , _voronoiPow ) * _voronoiMultiplier ) ) );
			float3 worldToObj63 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float simplePerlin3D65 = snoise( worldToObj63*_noiseScale );
			simplePerlin3D65 = simplePerlin3D65*0.5 + 0.5;
			float lerpResult74 = lerp( 0.0 , _maxBrokenValue , _breakValue);
			float lerpResult68 = lerp( 0.0 , temp_output_60_0 , step( simplePerlin3D65 , lerpResult74 ));
			float4 lerpResult59 = lerp( ( _Color * tex2D( _Albedo, uv_Albedo ) ) , ( _cracksColor * temp_output_60_0 ) , lerpResult68);
			o.Albedo = lerpResult59.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
27.2;14.4;1932.8;1063;4586.543;431.9399;1.068459;True;True
Node;AmplifyShaderEditor.NormalVertexDataNode;97;-4073.335,-54.87749;Inherit;True;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;86;-3691.749,-17.80436;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3824.905,240.7445;Inherit;False;Property;_triplanarBlending;triplanarBlending;10;0;Create;True;0;0;0;False;0;False;16.47059;9.411764;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;87;-3535.063,-1.936417;Inherit;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;50;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;88;-3292.063,93.06356;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;52;-3363.146,431.264;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;53;-3200.139,427.3629;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;89;-3075.063,24.06358;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SwizzleNode;90;-2927.686,458.5958;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;93;-2880.766,27.65546;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SwizzleNode;92;-2925.686,383.5958;Inherit;False;FLOAT2;1;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;91;-2929.686,532.5958;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;94;-2740.373,381.0724;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2284.616,673.0037;Inherit;False;Property;_voronoiScale;voronoiScale;3;0;Create;True;0;0;0;False;0;False;4.07;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;95;-2550.962,472.1479;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2286.616,589.003;Inherit;False;Property;_voronoiAngle;voronoiAngle;2;0;Create;True;0;0;0;False;0;False;1.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;5;-2064.469,459.4856;Inherit;True;0;0;1;4;1;True;6;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;27;-2040.459,749.3237;Inherit;False;Property;_voronoiPow;voronoiPow;4;0;Create;True;0;0;0;False;0;False;0.99;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1762.458,758.3237;Inherit;False;Property;_voronoiMultiplier;voronoiMultiplier;5;0;Create;True;0;0;0;False;0;False;11.22;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;26;-1837.459,513.3232;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;62;-1875.209,970.6287;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;63;-1691.998,966.9127;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;67;-1592.275,1346.855;Inherit;False;Property;_breakValue;breakValue;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1590.747,1266.949;Inherit;False;Property;_maxBrokenValue;maxBrokenValue;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1582.458,520.3232;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1684.222,1109.932;Inherit;False;Property;_noiseScale;noiseScale;7;0;Create;True;0;0;0;False;0;False;3.26;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;65;-1453.403,953.8331;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-1379.667,518.4751;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;-1287.747,1229.949;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-814.6152,-331.1469;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-894.1926,-160.2577;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;60;-1211.491,512.1084;Inherit;True;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;70;-1024.797,1051.644;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-1368.846,153.2406;Inherit;False;Property;_cracksColor;cracksColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;68;-847.8127,611.3398;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-567.5867,-193.8021;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1004.361,159.0949;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;-138.0306,-49.60556;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-3275.491,1237.573;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;293,-38;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TimeFight/Asteroid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;86;0;97;0
WireConnection;87;0;86;0
WireConnection;87;1;98;0
WireConnection;88;0;87;0
WireConnection;53;0;52;0
WireConnection;89;0;87;0
WireConnection;89;1;88;0
WireConnection;90;0;53;0
WireConnection;93;0;89;0
WireConnection;92;0;53;0
WireConnection;91;0;53;0
WireConnection;94;0;90;0
WireConnection;94;1;92;0
WireConnection;94;2;93;0
WireConnection;95;0;94;0
WireConnection;95;1;91;0
WireConnection;5;0;95;0
WireConnection;5;1;8;0
WireConnection;5;2;9;0
WireConnection;26;0;5;0
WireConnection;26;1;27;0
WireConnection;63;0;62;0
WireConnection;28;0;26;0
WireConnection;28;1;29;0
WireConnection;65;0;63;0
WireConnection;65;1;64;0
WireConnection;56;0;28;0
WireConnection;74;1;75;0
WireConnection;74;2;67;0
WireConnection;60;1;56;0
WireConnection;70;0;65;0
WireConnection;70;1;74;0
WireConnection;68;1;60;0
WireConnection;68;2;70;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;31;0;30;0
WireConnection;31;1;60;0
WireConnection;59;0;4;0
WireConnection;59;1;31;0
WireConnection;59;2;68;0
WireConnection;0;0;59;0
ASEEND*/
//CHKSM=5F411BED92B2AA717250E3F3689D6442E4A4E0C5