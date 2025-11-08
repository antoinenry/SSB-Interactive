Shader "Unlit/round_cut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", float) = 100
        _CenterX ("CenterX", float) = 960
        _CenterY ("CenterY", float) = 540
        _EdgeColor ("EdgeColor", Color) = (1,1,1,1)
        _EdgeThickness("EdgeThickness", float) = 10
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull [_Cull]
            Offset [_ZBias], [_ZBias]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f {
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _CenterX;
            float _CenterY;
            float _Radius;
            float4 _EdgeColor;
            float _EdgeThickness;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 center;
                center.x = _CenterX;
                center.y = _CenterY;
                if (_Radius <= 0) return i.color;
                if (distance(center, i.vertex) < _Radius) return _Color;
                if (distance(center, i.vertex) < _Radius + _EdgeThickness) return _EdgeColor;
                return i.color;
            }
            ENDCG
        }
    }
}
