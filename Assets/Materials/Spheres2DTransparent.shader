/***************************************************************
** Created by Federico Bachis, alias Federyuk91               **
** More content on https://itch.io/profile/federyuk91         **
** and on https://diariodiunprogrammatoreblog.wordpress.com/  **
****************************************************************/


// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Spheres2DTransparent"
{
    Properties{
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _HorizontalSpeed("Horizontal speed",  Range(-0.25,0.25)) = 0.1
        _VerticalSpeed("Vertical speed",  Range(-0.25,0.25)) = 0.0
    }

        SubShader{
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma multi_compile_fog

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        half2 texcoord : TEXCOORD0;
                        UNITY_FOG_COORDS(1)
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float _HorizontalSpeed;
                    float _VerticalSpeed;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                        UNITY_TRANSFER_FOG(o,o.vertex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = tex2D(_MainTex, i.texcoord);
                        //UNITY_APPLY_FOG(i.fogCoord, col);
                        float2 p = float2(i.texcoord.x * 2 - 1, i.texcoord.y * 2 - 1);

                        // get the squared magnitude of this vector
                        float magSq = p.x * p.x + p.y * p.y;

                        // if we're outside the circle, draw transparent background and skip ahead
                        if (magSq > 1)
                        {
                            return fixed4(0, 0, 0, 0);
                        }

                        //warp our local offset vector px py to imitate 3D bulge

                        //LONGITUDE & LATITUDE
                        float widthAtHeight = sqrt(1 - p.y * p.y);
                        p.x = (asin(p.x / widthAtHeight) * 2 / 3.141592653589);
                        p.y = (asin(p.y) * 2 / 3.141592653589);

                        float2 uv;
                        // vertical speed must be fixed, it doesn't work properly with equirectangular image,
                          // maybe it can be solved with a second vertical equirectangular image
                          // another solution can be to read only a part of the pole data 
                        uv.x = _Time.y * _HorizontalSpeed + (p.x + 1) / 4; 
                        uv.y = _Time.y * _VerticalSpeed +(p.y + 1) / 2;

                        col = tex2D(_MainTex, uv);
                        return col;
                    }
                ENDCG
            }
    }

}