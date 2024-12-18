Shader "Custom/FractalShader"
{
    Properties
    {
        _Color("Fractal Color", Color) = (1, 1, 1, 1)
        _Iterations("Iterations", Int) = 64
        _Zoom("Zoom", Float) = 1.0
        _Center("Fractal Center", Vector) = (0.0, 0.0, 0.0, 0.0)
        _c("Fractal Constant", Vector) = (-0.8, 0.156, 0.0, 0.0)
        _ColorStart("Color Start", Color) = (0.05, 0.05, 0.3, 1.0) // Dark blue
        _ColorMid("Color Mid", Color) = (0.8, 0.6, 0.2, 1.0)       // Gold
        _ColorEnd("Color End", Color) = (0.4, 0.2, 0.1, 1.0)       // Brown
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Shader properties
            float4 _Color;
            int _Iterations;
            float _Zoom;
            float4 _Center;
            float4 _c;
            float4 _ColorStart;
            float4 _ColorMid;
            float4 _ColorEnd;

            // Vertex data
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Vertex to fragment data
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // Vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy * 0.5 + 0.5; // UV mapping
                return o;
            }

            // Fractal rendering logic in fragment shader
            float4 frag(const v2f i) : SV_Target
            {
                // Adjust position based on zoom and center
                float3 pos = float3((i.uv - 0.5) * 2.0 / _Zoom + _Center.xy, 0.0);

                float4 z = float4(pos, 0.0);
                float colorValue = 0.0;

                // Fractal iteration loop
                for (int iter = 0; iter < _Iterations; iter++)
                {
                    float x = z.x * z.x - z.y * z.y - z.z * z.z - z.w * z.w;
                    float y = 2.0 * z.x * z.y;
                    float z1 = 2.0 * z.x * z.z;
                    float w = 2.0 * z.x * z.w;
                    z = float4(x, y, z1, w) + _c;

                    // Escape condition
                    if (dot(z, z) > 4.0)
                    {
                        colorValue = float(iter) / _Iterations; // Normalize iteration count
                        break;
                    }
                }

                // Generate a gradient using configurable colors
                float3 gradient;
                if (colorValue < 0.5)
                {
                    gradient = lerp(_ColorStart.rgb, _ColorMid.rgb, colorValue * 2.0);
                }
                else
                {
                    gradient = lerp(_ColorMid.rgb, _ColorEnd.rgb, (colorValue - 0.5) * 2.0);
                }

                return float4(gradient, 1.0); // Return the color as RGBA
            }
            ENDCG
        }
    }
}
