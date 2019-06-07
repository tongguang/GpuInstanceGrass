// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LearnShader/Grass-0" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_UV0Offset ("UV0Offset", Vector) = (0,0,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Opaque"
			"IgnoreProject"="True"
			"DisableBatching" = "True"
		}
		LOD 100
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _UV0Offset)
            UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert(appdata_t IN)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, o);

				//o.vertex = UnityObjectToClipPos(IN.vertex);

				// float3 vpos = mul((float3x3)unity_ObjectToWorld, IN.vertex.xyz);
				// float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
				// float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
				// float4 outPos = mul(UNITY_MATRIX_P, viewPos);
				// o.vertex = outPos;

				// billboard
				float3 viewerLocal = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
				float3 localDir = viewerLocal - float3(0,0,0);
 				localDir.y = lerp(0, localDir.y, 0.1);
				localDir = normalize(localDir);
				// float3 upLocal = abs(localDir.y) > 0.999f ? float3(0, 0, 1) : float3(0, 1, 0);
				float3 upLocal = float3(0, 1, 0);
				float3 rightLocal = normalize(cross(localDir, upLocal));
				upLocal = cross(rightLocal, localDir);
				float3 BBLocalPos = rightLocal * IN.vertex.x + upLocal * IN.vertex.y;
				o.vertex = UnityObjectToClipPos(float4(BBLocalPos, 1));

				fixed4 uV0Offset = UNITY_ACCESS_INSTANCED_PROP(Props, _UV0Offset);
				o.texcoord.x = uV0Offset.x + IN.texcoord.x * (uV0Offset.z - uV0Offset.x);
				o.texcoord.y = uV0Offset.y + IN.texcoord.y * (uV0Offset.w - uV0Offset.y);
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				half4 color = tex2D(_MainTex, IN.texcoord) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
				return color;
			}
		ENDCG
		}
	}
}
