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
		_dotMagicNumber0("dotMagicNumber0", Range( 0 , 10)) = 1
		_triplanarBlending("triplanarBlending", Range( 0 , 10)) = 3.873528
		_dotMagicNumber2("dotMagicNumber2", Range( 0 , 10)) = 1
		_dotMagicNumber1("dotMagicNumber1", Range( 0 , 10)) = 1
		_magicNumber0("magicNumber0", Range( -1 , 1)) = 0.7554802
		_magicNumber1("magicNumber1", Range( -1 , 1)) = 0.7554802
		_magicNumber3("magicNumber3", Range( -1 , 1)) = -0.09411765
		_magicNumber2("magicNumber2", Range( -1 , 1)) = 0.6378332
		_magicNumber4("magicNumber4", Range( -1 , 1)) = -0.3764706
		_magicNumber5("magicNumber5", Range( -1 , 1)) = 0.2588235
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
		uniform float _triplanarBlending;
		uniform float _dotMagicNumber0;
		uniform float _dotMagicNumber1;
		uniform float _dotMagicNumber2;
		uniform float _magicNumber0;
		uniform float _magicNumber1;
		uniform float _magicNumber2;
		uniform float _magicNumber3;
		uniform float _magicNumber4;
		uniform float _magicNumber5;
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
			float2 temp_output_90_0 = (worldToObj53).xz;
			float2 temp_output_92_0 = (worldToObj53).yz;
			float3 worldToObj146 = mul( unity_WorldToObject, float4( ase_worldPos, 1 ) ).xyz;
			float3 temp_cast_0 = (_triplanarBlending).xxx;
			float3 temp_output_87_0 = pow( abs( worldToObj146 ) , temp_cast_0 );
			float3 appendResult150 = (float3(_dotMagicNumber0 , _dotMagicNumber1 , _dotMagicNumber2));
			float dotResult88 = dot( temp_output_87_0 , appendResult150 );
			float3 break93 = ( temp_output_87_0 / dotResult88 );
			float2 lerpResult94 = lerp( temp_output_90_0 , temp_output_92_0 , break93.z);
			float2 temp_output_91_0 = (worldToObj53).xy;
			float2 lerpResult104 = lerp( temp_output_91_0 , temp_output_90_0 , break93.x);
			float2 appendResult130 = (float2(_magicNumber0 , _magicNumber1));
			float2 lerpResult115 = lerp( lerpResult94 , lerpResult104 , appendResult130);
			float2 lerpResult108 = lerp( temp_output_92_0 , temp_output_91_0 , break93.y);
			float2 appendResult129 = (float2(_magicNumber2 , _magicNumber3));
			float2 lerpResult116 = lerp( lerpResult104 , lerpResult108 , appendResult129);
			float2 appendResult128 = (float2(_magicNumber4 , _magicNumber5));
			float2 lerpResult117 = lerp( lerpResult115 , lerpResult116 , appendResult128);
			float2 coords5 = lerpResult117 * _voronoiScale;
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
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
27.2;14.4;1932.8;1063;6449.426;1734.334;5.367582;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;145;-4584.778,215.6177;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TransformPositionNode;146;-4421.77,211.7166;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;147;-4196.691,486.1712;Inherit;False;Property;_dotMagicNumber0;dotMagicNumber0;10;0;Create;True;0;0;0;False;0;False;1;9.411764;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-4412.33,410.2819;Inherit;False;Property;_triplanarBlending;triplanarBlending;11;0;Create;True;0;0;0;False;0;False;3.873528;9.411764;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-4197.744,563.498;Inherit;False;Property;_dotMagicNumber1;dotMagicNumber1;13;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-4193.324,634.1401;Inherit;False;Property;_dotMagicNumber2;dotMagicNumber2;12;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;86;-4208.956,221.7584;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;150;-3937.764,534.1971;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;87;-4054.269,235.821;Inherit;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;50;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;88;-3787.267,334.821;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;52;-3827.638,664.3649;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;89;-3574.921,233.0603;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TransformPositionNode;53;-3664.63,660.4639;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;121;-3231.735,236.6604;Inherit;False;Property;_magicNumber0;magicNumber0;14;0;Create;True;0;0;0;False;0;False;0.7554802;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;92;-3444.891,621.353;Inherit;False;FLOAT2;1;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-3515.162,1091.701;Inherit;False;Property;_magicNumber3;magicNumber3;16;0;Create;True;0;0;0;False;0;False;-0.09411765;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;93;-3375.203,235.1654;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SwizzleNode;91;-3433.19,542.2651;Inherit;False;FLOAT2;0;1;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-3225.735,304.6604;Inherit;False;Property;_magicNumber1;magicNumber1;15;0;Create;True;0;0;0;False;0;False;0.7554802;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;90;-3446.891,696.353;Inherit;False;FLOAT2;0;2;2;3;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-3515.162,1018.245;Inherit;False;Property;_magicNumber2;magicNumber2;17;0;Create;True;0;0;0;False;0;False;0.6378332;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;104;-3194.54,648.7581;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;94;-3193.476,529.729;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;130;-2932.358,289.7091;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;129;-3238.826,1041.246;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;125;-2956.008,996.9155;Inherit;False;Property;_magicNumber4;magicNumber4;18;0;Create;True;0;0;0;False;0;False;-0.3764706;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-2948.918,1067.917;Inherit;False;Property;_magicNumber5;magicNumber5;19;0;Create;True;0;0;0;False;0;False;0.2588235;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;108;-3196.248,767.5469;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;116;-2812.019,688.252;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0.27,0.53;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;115;-2809.019,555.252;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0.22,0.94;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;128;-2686.64,1006.194;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;117;-2543.014,431.1071;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0.63,0.62;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2267.716,684.7038;Inherit;False;Property;_voronoiScale;voronoiScale;3;0;Create;True;0;0;0;False;0;False;4.07;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2269.716,600.7031;Inherit;False;Property;_voronoiAngle;voronoiAngle;2;0;Create;True;0;0;0;False;0;False;1.15;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2040.459,749.3237;Inherit;False;Property;_voronoiPow;voronoiPow;4;0;Create;True;0;0;0;False;0;False;0.99;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;5;-2065.469,459.4856;Inherit;True;0;0;1;4;1;True;6;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;29;-1771.458,755.3237;Inherit;False;Property;_voronoiMultiplier;voronoiMultiplier;5;0;Create;True;0;0;0;False;0;False;11.22;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;62;-1875.209,970.6287;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;26;-1837.459,513.3232;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1594.162,1350.63;Inherit;False;Property;_breakValue;breakValue;8;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-1684.222,1109.932;Inherit;False;Property;_noiseScale;noiseScale;7;0;Create;True;0;0;0;False;0;False;3.26;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-1590.747,1266.949;Inherit;False;Property;_maxBrokenValue;maxBrokenValue;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;63;-1691.998,966.9127;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1582.458,520.3232;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;65;-1451.403,953.8331;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;-1287.747,1229.949;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;56;-1379.667,518.4751;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;70;-1094.897,967.866;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;60;-1211.491,512.1084;Inherit;True;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;30;-1368.846,153.2406;Inherit;False;Property;_cracksColor;cracksColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-814.6152,-331.1469;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-894.1926,-160.2577;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;68;-953.1328,485.3096;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-1004.361,159.0949;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-567.5867,-193.8021;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ASinOpNode;138;-4143.639,-179.6591;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;134;-4142.059,-266.2247;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;107;-4725.262,-447.018;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;133;-3952.059,-451.2248;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ATan2OpNode;132;-4167.059,-493.2247;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;135;-3848.06,-455.2248;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;-3196.688,-22.36275;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;152;-3455.193,-13.76763;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;59;-138.0306,-49.60556;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;144;-3578.493,-341.46;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NormalizeNode;142;-4541.28,-437.4372;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-3739.06,-456.2248;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;106;-4888.27,-443.1169;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;139;-3959.64,-140.6591;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;143;-4404.247,-450.0655;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleSubtractOpNode;141;-3809.64,-148.6591;Inherit;False;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;293,-38;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;TimeFight/Asteroid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;146;0;145;0
WireConnection;86;0;146;0
WireConnection;150;0;147;0
WireConnection;150;1;148;0
WireConnection;150;2;149;0
WireConnection;87;0;86;0
WireConnection;87;1;98;0
WireConnection;88;0;87;0
WireConnection;88;1;150;0
WireConnection;89;0;87;0
WireConnection;89;1;88;0
WireConnection;53;0;52;0
WireConnection;92;0;53;0
WireConnection;93;0;89;0
WireConnection;91;0;53;0
WireConnection;90;0;53;0
WireConnection;104;0;91;0
WireConnection;104;1;90;0
WireConnection;104;2;93;0
WireConnection;94;0;90;0
WireConnection;94;1;92;0
WireConnection;94;2;93;2
WireConnection;130;0;121;0
WireConnection;130;1;122;0
WireConnection;129;0;123;0
WireConnection;129;1;124;0
WireConnection;108;0;92;0
WireConnection;108;1;91;0
WireConnection;108;2;93;1
WireConnection;116;0;104;0
WireConnection;116;1;108;0
WireConnection;116;2;129;0
WireConnection;115;0;94;0
WireConnection;115;1;104;0
WireConnection;115;2;130;0
WireConnection;128;0;125;0
WireConnection;128;1;126;0
WireConnection;117;0;115;0
WireConnection;117;1;116;0
WireConnection;117;2;128;0
WireConnection;5;0;117;0
WireConnection;5;1;8;0
WireConnection;5;2;9;0
WireConnection;26;0;5;0
WireConnection;26;1;27;0
WireConnection;63;0;62;0
WireConnection;28;0;26;0
WireConnection;28;1;29;0
WireConnection;65;0;63;0
WireConnection;65;1;64;0
WireConnection;74;1;75;0
WireConnection;74;2;67;0
WireConnection;56;0;28;0
WireConnection;70;0;65;0
WireConnection;70;1;74;0
WireConnection;60;1;56;0
WireConnection;68;1;60;0
WireConnection;68;2;70;0
WireConnection;31;0;30;0
WireConnection;31;1;60;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;138;0;143;1
WireConnection;107;0;106;0
WireConnection;133;0;132;0
WireConnection;133;1;134;0
WireConnection;132;0;143;2
WireConnection;132;1;143;0
WireConnection;135;0;133;0
WireConnection;151;0;144;0
WireConnection;151;2;152;0
WireConnection;152;0;143;1
WireConnection;59;0;4;0
WireConnection;59;1;31;0
WireConnection;59;2;68;0
WireConnection;144;0;136;0
WireConnection;144;1;141;0
WireConnection;142;0;107;0
WireConnection;136;0;135;0
WireConnection;139;0;138;0
WireConnection;139;1;134;0
WireConnection;143;0;142;0
WireConnection;141;1;139;0
WireConnection;0;0;59;0
ASEEND*/
//CHKSM=D45332B702F2F8FDB8F730C9AF516B54E8BB87EC