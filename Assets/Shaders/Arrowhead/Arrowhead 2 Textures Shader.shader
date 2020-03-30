Shader "Unlit/Arrowhead 2 Textures Shader"
{
    Properties
    {
        _MainTex ("Mask", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)

		_Texture1("Texture 1", 2D) = "black" {}
		_Color1("Color 1", Color) = (1,1,1,1)
		_Back1("Background 1", 2D) = "white" {}

		_Texture2("Texture 2", 2D) = "black" {}
		_Color2("Color 2", Color) = (1,1,1,1)
		_Back2("Background 2", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;

			sampler2D _Texture1;
			float4 _Texture1_ST;
			float4 _Color1;
			sampler2D _Back1;
			float4 _Back1_ST;

			sampler2D _Texture2;
			float4 _Texture2_ST;
			float4 _Color2;
			sampler2D _Back2;
			float4 _Back2_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _Texture1);
				o.uv2 = TRANSFORM_TEX(v.uv2, _Texture2);
				o.uv3 = TRANSFORM_TEX(v.uv3, _Back1);
				o.uv4 = TRANSFORM_TEX(v.uv4, _Back2);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 mask = tex2D(_MainTex, i.uv);
				fixed4 tex1 = tex2D(_Texture1, i.uv1);
				fixed4 tex2 = tex2D(_Texture2, i.uv2);
                fixed4 col = mask.r*(tex1 * tex1.a *_Color + tex2D(_Back1, i.uv3)*(1-tex1.a)*_Color1) + mask.g*(tex2 * tex2.a *_Color + tex2D(_Back2, i.uv4)*(1-tex2.a)*_Color2);
				col.a = mask.a;
                return col;
            }
            ENDCG
        }
    }
}
