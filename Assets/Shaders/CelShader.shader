Shader "Custom/Cel-Shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}

			//How far along we want to clip pixels
			_AlphaThreshold("Alpha Cutoff", Range(0,1)) = 0.5
	}
		SubShader
		{
			Pass
			{
				Tags { "LightMode" = "ForwardBase" "PassFlags" = "OnlyDirectional" }

				//This makes the shader double sided, but we will need to make some 
				//vertex shader adjustments so that both sides look right
				Cull Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"

				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float4 uv : TEXCOORD0;
					float3 normal : NORMAL;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float3 worldNormal : NORMAL;
					float2 uv : TEXCOORD0;
					float3 viewDir : TEXCOORD1;

					SHADOW_COORDS(2)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.viewDir = WorldSpaceViewDir(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					//This makes sure that the normals are facing the right way on both sides of the object
					o.worldNormal *= lerp(1.0, -1.0, dot(o.viewDir, o.worldNormal) < 0.0);

					TRANSFER_SHADOW(o)
					return o;
				}

				float4 _Color;

				//Also declare down here
				half _AlphaThreshold;

				float4 frag(v2f i) : SV_Target
				{
					//Move the texture sample up here
					float4 sample = tex2D(_MainTex, i.uv);

				//Check if our alpha is lower than the threshold;
				//If it is, then discard this pixel and return early.
				if (sample.a < _AlphaThreshold)
				{
					discard;
					return 0;
				}

				float3 normal = normalize(i.worldNormal);
				float3 viewDir = normalize(i.viewDir);

				float NdotL = dot(_WorldSpaceLightPos0, normal);

				float shadow = SHADOW_ATTENUATION(i);

				float lightIntensity = smoothstep(0, 0.01, NdotL * shadow);

				float4 light = lightIntensity * _LightColor0;

				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);

				float4 ambient = UNITY_LIGHTMODEL_AMBIENT;

				return (light + ambient) * _Color * sample;
			}
			ENDCG
		}

			//We will have to use our own shadowcaster pass now that we are discarding pixels
			//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

			//Don't worry too much about this pass; shadowcaster passes are completely undocumented in Unity, 
			//but it basically just renders the bare minimum so that the shadow functions can see the object as it is in the scene
			Pass
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }

				Cull Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_shadowcaster
				#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				half _AlphaThreshold;

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = (v.uv * _MainTex_ST.xy) + _MainTex_ST.zw;
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float a = tex2D(_MainTex, i.uv).a;
					if (a < _AlphaThreshold)
						discard;

					return 0;
				}
				ENDCG
			}
		}
}