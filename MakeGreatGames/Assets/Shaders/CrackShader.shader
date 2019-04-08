Shader "Custom/CrackShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	//_HitTex("Hit", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members vertex)
//#pragma exclude_renderers d3d11
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
			sampler2D _HitTex;
			float2 _HitPoint;
			float4 _MainTex_ST;
			float4 _HitTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = TRANSFORM_TEX(v.uv + _HitPoint, _HitTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}