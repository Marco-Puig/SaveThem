Shader "Hidden/Backdrop"
{
    Properties
    {
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // make this constant and static so it is marked as non-dynamic and can be optimized in compilation
            const static int _LayerCount = 8;
            const static float _WaveWidth = 5;
            const static float _WaveAmplitude = 0.03;
            const static float _WaveRipple = 0.009;

            const static float3 _WaveColorHSV = float3(0.8, 0.5, 0.5);
            const static float3 _EdgeColorRGB = float3(0, 0, 0);
            const static float _DepthHueFactor = 0.25;
            const static float _DepthSatFactor = 0.5;
            const static float _DepthValFactor = -0.1;
            const static float _TimeHueShiftFactor = 0.1;

            // simple wave height function using sin waves and time
            float getWaveHeight(float x)
            {
                float timeFactor = _Time * 25;
                float wave = (sin((x * (_WaveWidth / 1000))+timeFactor)) * sin((x + timeFactor) * _WaveRipple);
                return wave * _WaveAmplitude;
            }

            // some hue shift function i found, because fuck writing this from scratch
            float3 hsv2rgb(float3 c) {
              c = float3(c.x, clamp(c.yz, 0.0, 1.0));
              float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
              float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
              return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            // this function runs for every pixel
            fixed4 frag (v2f p) : SV_Target
            {
                // sore uv with an offset to prevent effect cutoff
                float2 uv = float2(p.uv.x * _ScreenParams.x, p.uv.y - (1.0 / _LayerCount) - _WaveAmplitude);

                // calculate wave heights
                [unroll] // unroll makes the compiler unroll the for loop into a sequence of statements - less overhead
                for(int i = 0; i < _LayerCount; i++)
                {
                    // get the current layer step by dividing current by the total
                    float layerStep = float(i) / _LayerCount;

                    // calculate wave height
                    float waveHeight = layerStep + getWaveHeight(uv.x + (layerStep * (_WaveWidth * 1000)));

                    // check if we're in the current wave layer or not
                    if(uv.y < waveHeight)
                    {
                        // make an edge factor based on the absolute of the distance of the y coord from the wave height
                        float edgeFactor = min(abs(uv.y - waveHeight) * 5, 1);

                        // apply hue shift & lerp colors, then darken with depth
                        return float4(lerp(hsv2rgb(float3(
                            _WaveColorHSV.x + (layerStep * _DepthHueFactor),
                            _WaveColorHSV.y + (layerStep * _DepthSatFactor),
                            _WaveColorHSV.z + (layerStep * _DepthValFactor)
                            )),_EdgeColorRGB, edgeFactor) * (layerStep * 0.5 + 0.5), 1);
                    }
                }

                // put this here so we always return a value no matter what
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
