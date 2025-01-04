Shader "Custom/FogProjection"
{
	Properties
	{
		_Color ("Color", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent+100" } // to cover other transparent non-z-write things
 
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Equal
 			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
 			
			#include "UnityCG.cginc"
 			
			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};
 			
			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
 			
 			
			float4x4 unity_Projector;
			sampler2D _CurrTexture;
			sampler2D _PrevTexture;
			fixed4 _Color;
			float _Blend;

			// Gaussian blur weights (3x3 kernel)
            static const float kernel[9] = {
                1.0 / 16.0, 2.0 / 16.0, 1.0 / 16.0,
                2.0 / 16.0, 4.0 / 16.0, 2.0 / 16.0,
                1.0 / 16.0, 2.0 / 16.0, 1.0 / 16.0
            };
            
            // Offsets for a 3x3 kernel (normalized to a [-1, 1] UV space)
            static const float2 offsets[9] = {
                float2(-1, -1), float2(0, -1), float2(1, -1),
                float2(-1,  0), float2(0,  0), float2(1,  0),
                float2(-1,  1), float2(0,  1), float2(1,  1)
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = mul(unity_Projector, v.vertex);
                return o;
            }
            
            float4 BlurTexture(sampler2D blurTexture, float4 uv)
            {
                float4 color = float4(0, 0, 0, 0);
                float totalWeight = 0;
            
                // Apply Gaussian blur using the kernel
                for (int i = 0; i < 9; i++)
                {
                    float2 offsetUV = uv.xy + offsets[i] * uv.w * 0.015; // Adjust scale for offsets
                    float4 sample = tex2Dproj(blurTexture, float4(offsetUV, uv.z, uv.w));
                    color += sample * kernel[i];
                    totalWeight += kernel[i];
                }
            
                return color / totalWeight; // Normalize the result
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Apply blur to the current and previous textures
                float4 blurredPrev = BlurTexture(_PrevTexture, i.uv);
                float4 blurredCurr = BlurTexture(_CurrTexture, i.uv);
            
                float aPrev = blurredPrev.a;
                float aCurr = blurredCurr.a;
            
                fixed a = lerp(aPrev, aCurr, _Blend);
            
                // Ensure alpha value doesn't go negative
                _Color.a = max(0, _Color.a - a);
                return _Color;
            }
            ENDCG
		}
	}
}
