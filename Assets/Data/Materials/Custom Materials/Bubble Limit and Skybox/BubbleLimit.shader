// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TimeFight/BubbleLimit"
{
	Properties
	{
		_NoiseScale("NoiseScale", Float) = 15.9
		_NoiseScale1("NoiseScale1", Float) = 15.9
		_NoiseScale2("NoiseScale2", Float) = 15.9
		_TimeMultiplierX("TimeMultiplierX", Float) = 1.82
		_TimeMultiplierX1("TimeMultiplierX1", Float) = 1.82
		_TimeMultiplierX2("TimeMultiplierX2", Float) = 1.82
		_TimeMultiplierY("TimeMultiplierY", Float) = 0
		_TimeMultiplierY1("TimeMultiplierY1", Float) = 0
		_TimeMultiplierY2("TimeMultiplierY2", Float) = 0
		_TimeMultiplierZ("TimeMultiplierZ", Float) = 0
		_TimeMultiplierZ1("TimeMultiplierZ1", Float) = 0
		_TimeMultiplierZ2("TimeMultiplierZ2", Float) = 0
		_BaseColor("BaseColor", Color) = (0,0,0,0)
		_TextureWeight2("TextureWeight2", Range( 0 , 1)) = 0
		_TextureWeight1("TextureWeight1", Range( 0 , 1)) = 0
		_TextureWeight("TextureWeight", Range( 0 , 1)) = 0
		_AlphaMultiplier("AlphaMultiplier", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _BaseColor;
		uniform float _TimeMultiplierX;
		uniform float _TimeMultiplierY;
		uniform float _TimeMultiplierZ;
		uniform float _NoiseScale;
		uniform float _TextureWeight;
		uniform float _TimeMultiplierX1;
		uniform float _TimeMultiplierY1;
		uniform float _TimeMultiplierZ1;
		uniform float _NoiseScale1;
		uniform float _TextureWeight1;
		uniform float _TimeMultiplierX2;
		uniform float _TimeMultiplierY2;
		uniform float _TimeMultiplierZ2;
		uniform float _NoiseScale2;
		uniform float _TextureWeight2;
		uniform float _AlphaMultiplier;


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
			float4 appendResult29 = (float4(_BaseColor.r , _BaseColor.g , _BaseColor.b , 0.0));
			float3 ase_worldPos = i.worldPos;
			float4 appendResult20 = (float4(( ase_worldPos.x + ( _Time.y * _TimeMultiplierX ) ) , ( ase_worldPos.y + ( _Time.y * _TimeMultiplierY ) ) , ( ase_worldPos.z + ( _Time.y * _TimeMultiplierZ ) ) , 0.0));
			float simplePerlin3D1 = snoise( appendResult20.xyz*_NoiseScale );
			simplePerlin3D1 = simplePerlin3D1*0.5 + 0.5;
			float4 appendResult47 = (float4(( ase_worldPos.x + ( _Time.y * _TimeMultiplierX1 ) ) , ( ase_worldPos.y + ( _Time.y * _TimeMultiplierY1 ) ) , ( ase_worldPos.z + ( _Time.y * _TimeMultiplierZ1 ) ) , 0.0));
			float simplePerlin3D48 = snoise( appendResult47.xyz*_NoiseScale1 );
			simplePerlin3D48 = simplePerlin3D48*0.5 + 0.5;
			float4 appendResult59 = (float4(( ase_worldPos.x + ( _Time.y * _TimeMultiplierX2 ) ) , ( ase_worldPos.y + ( _Time.y * _TimeMultiplierY2 ) ) , ( ase_worldPos.z + ( _Time.y * _TimeMultiplierZ2 ) ) , 0.0));
			float simplePerlin3D60 = snoise( appendResult59.xyz*_NoiseScale2 );
			simplePerlin3D60 = simplePerlin3D60*0.5 + 0.5;
			float temp_output_72_0 = ( ( simplePerlin3D1 * _TextureWeight ) + ( simplePerlin3D48 * _TextureWeight1 ) + ( simplePerlin3D60 * _TextureWeight2 ) );
			o.Albedo = ( appendResult29 * temp_output_72_0 ).xyz;
			o.Alpha = ( temp_output_72_0 * _AlphaMultiplier );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
27.2;14.4;1932.8;1063;2232.705;1120.316;1.758572;True;True
Node;AmplifyShaderEditor.RangedFloatNode;35;-1795.963,711.7913;Inherit;False;Property;_TimeMultiplierY1;TimeMultiplierY1;7;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1610.374,-284.5472;Inherit;False;Property;_TimeMultiplierX;TimeMultiplierX;3;0;Create;True;0;0;0;False;0;False;1.82;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;9;-1611.901,-366.6268;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;13;-1636.265,-175.5506;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1636.362,-95.09461;Inherit;False;Property;_TimeMultiplierY;TimeMultiplierY;6;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;50;-1724.329,1642.863;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1642.619,99.87798;Inherit;False;Property;_TimeMultiplierZ;TimeMultiplierZ;9;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;51;-1718.072,1447.891;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1724.426,1723.319;Inherit;False;Property;_TimeMultiplierZ2;TimeMultiplierZ2;11;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;25;-1642.522,19.42193;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1802.22,906.7639;Inherit;False;Property;_TimeMultiplierZ1;TimeMultiplierZ1;10;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-1784.598,524.5883;Inherit;False;Property;_TimeMultiplierX1;TimeMultiplierX1;4;0;Create;True;0;0;0;False;0;False;1.82;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;36;-1795.866,631.3353;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1706.805,1341.144;Inherit;False;Property;_TimeMultiplierX2;TimeMultiplierX2;5;0;Create;True;0;0;0;False;0;False;1.82;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-1802.123,826.3079;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1718.169,1528.347;Inherit;False;Property;_TimeMultiplierY2;TimeMultiplierY2;8;0;Create;True;0;0;0;False;0;False;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;49;-1693.708,1256.814;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;33;-1771.502,440.2587;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1635.809,824.3361;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-1629.552,629.3635;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1476.208,17.45012;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1445.587,-368.5986;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1469.951,-177.5224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1527.395,1254.843;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-1605.188,438.2869;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;55;-1554.396,1088.197;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-1551.759,1445.919;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1558.015,1640.891;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;42;-1632.19,271.6414;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;21;-1472.589,-535.2439;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1232.522,1374.904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-1144.344,-374.1399;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-1150.714,-248.5377;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1152.813,-125.4006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-1312.414,681.4854;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1234.62,1498.041;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-1303.945,432.7456;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;-1310.316,558.3481;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-1226.151,1249.301;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1110.139,91.94318;Inherit;False;Property;_NoiseScale;NoiseScale;0;0;Create;True;0;0;0;False;0;False;15.9;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-1122.38,535.8416;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1269.74,898.829;Inherit;False;Property;_NoiseScale1;NoiseScale1;1;0;Create;True;0;0;0;False;0;False;15.9;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;59;-1018.839,1264.12;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1191.946,1715.384;Inherit;False;Property;_NoiseScale2;NoiseScale2;2;0;Create;True;0;0;0;False;0;False;15.9;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-962.7786,-271.0442;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-837.5916,188.1346;Inherit;False;Property;_TextureWeight;TextureWeight;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-776.863,1481.14;Inherit;False;Property;_TextureWeight2;TextureWeight2;13;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-860.3754,717.2745;Inherit;False;Property;_TextureWeight1;TextureWeight1;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;60;-787.9077,1177.747;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;48;-841.3788,417.395;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;1;-789.7125,-72.48637;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;28;-569.7781,-598.8099;Inherit;False;Property;_BaseColor;BaseColor;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-473.1089,1232.224;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-550.0438,27.3334;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-578.7544,517.7842;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-304.631,637.2737;Inherit;False;Property;_AlphaMultiplier;AlphaMultiplier;16;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-385.685,456.4041;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-364.2754,-569.359;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-165.91,-207.5647;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;-168.7887,453.8715;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3.369764,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TimeFight/BubbleLimit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;34;0
WireConnection;40;1;37;0
WireConnection;41;0;36;0
WireConnection;41;1;35;0
WireConnection;27;0;25;0
WireConnection;27;1;26;0
WireConnection;5;0;9;0
WireConnection;5;1;6;0
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;52;0;49;0
WireConnection;52;1;61;0
WireConnection;39;0;33;0
WireConnection;39;1;38;0
WireConnection;54;0;51;0
WireConnection;54;1;62;0
WireConnection;53;0;50;0
WireConnection;53;1;63;0
WireConnection;56;0;55;2
WireConnection;56;1;54;0
WireConnection;10;0;21;1
WireConnection;10;1;5;0
WireConnection;15;0;21;2
WireConnection;15;1;14;0
WireConnection;24;0;21;3
WireConnection;24;1;27;0
WireConnection;44;0;42;3
WireConnection;44;1;40;0
WireConnection;57;0;55;3
WireConnection;57;1;53;0
WireConnection;45;0;42;1
WireConnection;45;1;39;0
WireConnection;43;0;42;2
WireConnection;43;1;41;0
WireConnection;58;0;55;1
WireConnection;58;1;52;0
WireConnection;47;0;45;0
WireConnection;47;1;43;0
WireConnection;47;2;44;0
WireConnection;59;0;58;0
WireConnection;59;1;56;0
WireConnection;59;2;57;0
WireConnection;20;0;10;0
WireConnection;20;1;15;0
WireConnection;20;2;24;0
WireConnection;60;0;59;0
WireConnection;60;1;64;0
WireConnection;48;0;47;0
WireConnection;48;1;46;0
WireConnection;1;0;20;0
WireConnection;1;1;3;0
WireConnection;68;0;60;0
WireConnection;68;1;65;0
WireConnection;70;0;1;0
WireConnection;70;1;67;0
WireConnection;69;0;48;0
WireConnection;69;1;66;0
WireConnection;72;0;70;0
WireConnection;72;1;69;0
WireConnection;72;2;68;0
WireConnection;29;0;28;1
WireConnection;29;1;28;2
WireConnection;29;2;28;3
WireConnection;32;0;29;0
WireConnection;32;1;72;0
WireConnection;74;0;72;0
WireConnection;74;1;73;0
WireConnection;0;0;32;0
WireConnection;0;9;74;0
ASEEND*/
//CHKSM=65AC9CDFF1E7ACEB8AB3DEC5F97F8E930CCA0C3B