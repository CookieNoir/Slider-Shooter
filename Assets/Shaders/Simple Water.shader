Shader "Water/Simple Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Amplitude("Waves Amplitude", Float) = 0.2
		_Frequency("Waves Frequency", Float) = 0.2
		_Speed("Waves Speed", Float) = 0.2
		_FlowSpeed("Flow Speed", Float) = 0.2
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;

			float _Amplitude;
			float _Frequency;
			float _Speed;
			float _FlowSpeed;

            v2f vert (appdata v)
            {
                v2f o;
				const float PI = 3.14159265359;
				v.vertex.zx += _Amplitude * float2(cos(_Frequency*PI*v.vertex.x + _Time.y*_Speed), sin(_Frequency*PI*v.vertex.z + _Time.y*_Speed));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.x += _FlowSpeed * _Time.y;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv)*_Color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
