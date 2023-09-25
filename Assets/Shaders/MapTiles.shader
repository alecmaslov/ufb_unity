Shader "Custom/MapTiles"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "white" {}
        _MaxHeight ("Max Raise Height", float) = 1.0      // maximum height based on brightness value of 1 in the heightmap.
        _BoardDimensionX ("Board Dimension X", float) = 8      // Number of tiles in x dimension.
        _BoardDimensionY ("Board Dimension Y", float) = 8      // Number of tiles in y dimension.\
        _MipLevel ("Mip Level", float) = 0
        _BiasLevel ("Bias Level", float) = 0
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
            sampler2D _HeightMap;
            float _MaxHeight;
            float _BoardDimensionX;
            float _BoardDimensionY;
            float _MipLevel;
            float _BiasLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;

                //float heightValue = tex2D(_HeightMap, v.uv).r; // Get the red channel value
                //float offset = tex2D(_HeightMap, v.uv).r * _MaxHeight;
                //v.vertex.y += offset;
                float heightValue = tex2Dlod(_HeightMap, float4(v.uv, _MipLevel, _BiasLevel)).r;
                v.vertex.y = heightValue * _MaxHeight;


                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}