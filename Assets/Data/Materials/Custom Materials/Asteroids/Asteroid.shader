// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TimeFight/Asteroid"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_voronoiAngle("voronoiAngle", Float) = 1.15
		_voronoiScale("voronoiScale", Float) = 4.07
		_voronoiPow("voronoiPow", Float) = 0.99
		_voronoiMultiplier("voronoiMultiplier", Float) = 11.22
		_cracksColor("cracksColor", Color) = (1,0.1170213,0,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _cracksColor;
		uniform float _voronoiScale;
		uniform float _voronoiAngle;
		uniform float _voronoiPow;
		uniform float _voronoiMultiplier;


		float2 voronoihash5( float2 p )
		{
			
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


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float time5 = _voronoiAngle;
			float2 voronoiSmoothId5 = 0;
			float3 ase_worldPos = i.worldPos;
			float3 worldToObj53 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float2 coords5 = worldToObj53.xy * _voronoiScale;
			float2 id5 = 0;
			float2 uv5 = 0;
			float voroi5 = voronoi5( coords5, time5, id5, uv5, 0, voronoiSmoothId5 );
			float temp_output_60_0 = step( 0.5 , ( 1.0 - ( pow( voroi5 , _voronoiPow ) * _voronoiMultiplier ) ) );
			float4 lerpResult59 = lerp( ( _Color * tex2D( _Albedo, uv_Albedo ) ) , ( _cracksColor * temp_output_60_0 ) , temp_output_60_0);
			o.Albedo = lerpResult59.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
28;12;1936.8;1065.4;1420.103;362.9349;1;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;52;-2196.565,144.1567;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;9;-1939.867,605.0248;Inherit;False;Property;_voronoiScale;voronoiScale;3;0;Create;True;0;0;0;False;0;False;4.07;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1941.867,521.0242;Inherit;False;Property;_voronoiAngle;voronoiAngle;2;0;Create;True;0;0;0;False;0;False;1.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;53;-2013.353,140.4407;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;27;-1695.708,681.3448;Inherit;False;Property;_voronoiPow;voronoiPow;4;0;Create;True;0;0;0;False;0;False;0.99;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;5;-1691.269,407.9365;Inherit;True;0;0;1;4;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.PowerNode;26;-1492.708,445.3441;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1417.707,690.3448;Inherit;False;Property;_voronoiMultiplier;voronoiMultiplier;5;0;Create;True;0;0;0;False;0;False;11.22;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1237.707,452.3441;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-1034.916,450.496;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-1024.095,85.26154;Inherit;False;Property;_cracksColor;cracksColor;6;0;Create;True;0;0;0;False;0;False;1,0.1170213,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-894.1926,-160.2577;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;60;-866.7394,444.1293;Inherit;True;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;3;-814.6152,-331.1469;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-567.5867,-193.8021;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-659.6107,91.1158;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-2138.693,330.1361;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;59;-156.2306,20.59442;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;293,-38;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TimeFight/Asteroid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;52;0
WireConnection;5;0;53;0
WireConnection;5;1;8;0
WireConnection;5;2;9;0
WireConnection;26;0;5;0
WireConnection;26;1;27;0
WireConnection;28;0;26;0
WireConnection;28;1;29;0
WireConnection;56;0;28;0
WireConnection;60;1;56;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;31;0;30;0
WireConnection;31;1;60;0
WireConnection;59;0;4;0
WireConnection;59;1;31;0
WireConnection;59;2;60;0
WireConnection;0;0;59;0
ASEEND*/
//CHKSM=689E342FC410B3F39BDC72076719A559210B6E12