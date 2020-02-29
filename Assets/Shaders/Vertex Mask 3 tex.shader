Shader "Masked/Vertex Mask 3 tex"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("Texture", 2D) = "red" {}
		_ThirdTex("Texture", 2D) = "blue" {}
		_ThresholdLow("1&2 threshold (Low)", Range(0,0.5)) = 0.33
		_ThresholdHigh("2&3 threshold (High)", Range(0.5,1)) = 0.67
		_Mix("Mix value (Low+This, High+This)", Range(0,0.5)) = 0.05
	}
		SubShader
		{
			Tags { "LightMode" = "ForwardBase" }

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#pragma multi_compile_fwdbase
				#include "AutoLight.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float2 uv2 : TEXCOORD2;
					float3 normal: NORMAL;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float2 uv2 : TEXCOORD2;
					fixed4 color : COLOR;
					float4 vertex : SV_POSITION;
					float3 lightDir : TEXCOORD3;
					float3 normal : TEXCOORD4;
					LIGHTING_COORDS(5, 6)
					UNITY_FOG_COORDS(7)
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SecondTex;
				float4 _SecondTex_ST;
				sampler2D _ThirdTex;
				float4 _ThirdTex_ST;

				float _ThresholdLow;
				float _ThresholdHigh;
				float _Mix;

				float4 _LightColor0;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.uv1 = TRANSFORM_TEX(v.uv1, _SecondTex);
					o.uv2 = TRANSFORM_TEX(v.uv2, _ThirdTex);
					o.color = v.color;

					o.lightDir = normalize(ObjSpaceLightDir(v.vertex));
					o.normal = normalize(v.normal).xyz;

					UNITY_TRANSFER_FOG(o,o.vertex);
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col;
					if (i.color.r < _ThresholdLow) col = tex2D(_MainTex, i.uv);
					else if (i.color.r < _ThresholdLow + _Mix)
					{
						float lowf = (_ThresholdLow + _Mix - i.color.r) / _Mix;
						col = tex2D(_MainTex, i.uv)*lowf + tex2D(_SecondTex, i.uv1)*(1 - lowf);
					}
					else if (i.color.r < _ThresholdHigh) col = tex2D(_SecondTex, i.uv1);
					else if (i.color.r < _ThresholdHigh + _Mix)
					{
						float highf = (_ThresholdHigh + _Mix - i.color.r) / _Mix;
						col = tex2D(_SecondTex, i.uv1)*highf + tex2D(_ThirdTex, i.uv2)*(1 - highf);
					}
					else col = tex2D(_ThirdTex, i.uv2);

					float3 L = normalize(i.lightDir);
					float3 N = normalize(i.normal);

					float attenuation = LIGHT_ATTENUATION(i) ;
					float4 ambient = UNITY_LIGHTMODEL_AMBIENT ;

					float NdotL = saturate(dot(N, L));
					float4 diffuseTerm = NdotL * _LightColor0 * attenuation;

					col = col * (ambient + diffuseTerm);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}

		}
		Fallback "Diffuse"
}
