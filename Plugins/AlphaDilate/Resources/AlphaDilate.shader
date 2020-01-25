Shader "Custom/AlphaDilate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend Off //SrcAlpha OneMinusSrcAlpha
        LOD 100
		ZWrite Off
		ZTest Always
		Cull Off
        
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // If this pixel is not opaque, get all the pixels around it and average their colors, as long as they are more opaque than this one.
                float4 col = tex2D(_MainTex, i.uv);
                
                float4 dilate = float4(0.0, 0.0, 0.0, col.a);
                
                float weight = 0.0;
                float threshold = col.a + 0.0001;
                float _dU = _MainTex_TexelSize.x;
                float _dV = _MainTex_TexelSize.y;
                float2 max_uv = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                float2 min_uv = float2(0.0, 0.0);
                
                float4 temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(_dU, 0.0))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb;
                weight = weight + step(threshold, temp.a);

                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(-_dU, 0.0))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb;
                weight = weight + step(threshold, temp.a);

                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(0.0, _dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb;
                weight = weight + step(threshold, temp.a);
                
                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(0.0, -_dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb;
                weight = weight + step(threshold, temp.a);
                
                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(_dU, _dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb * 0.666666;
                weight = weight + step(threshold, temp.a) * 0.666666;

                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(_dU, -_dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb * 0.666666;
                weight = weight + step(threshold, temp.a) * 0.666666;
                
                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(-_dU, _dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb * 0.666666;
                weight = weight + step(threshold, temp.a) * 0.666666;

                temp = tex2D(_MainTex, min(max_uv, max(min_uv, i.uv + float2(-_dU, -_dV))));
                dilate.rgb = dilate.rgb + step(threshold, temp.a) * temp.rgb * 0.666666;
                weight = weight + step(threshold, temp.a) * 0.666666;
                
                dilate.rgb = weight > 0.0 ? dilate.rgb / weight : col.rgb;
                
                return col.a >= 0.9999 ? col : dilate;
            }
            ENDCG
        }
    }
}
