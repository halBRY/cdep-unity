Shader "XR/Stereo360Panorama_VerticalStack"
{
	Properties{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				struct vertexInput {
					float4 vertex : POSITION;
				};
				struct vertexOutput {
					float4 pos : SV_POSITION;
					float3 viewDir : TEXCOORD0;
				};
				inline float2 ToRadialCoords(float3 coords)
				{
					float3 normalizedCoords = normalize(coords);
					float latitude = acos(normalizedCoords.y);
					float longitude = atan2(normalizedCoords.z, normalizedCoords.x);
					float2 sphereCoords = float2(longitude, latitude) * float2(0.5 / UNITY_PI, 1.0 / UNITY_PI);
					//Use this if you want 180 instead of 360
					//sphereCoords.x = fmod(sphereCoords.x*2.0+1.0, 1.0)-0.5;
					return float2(0.5, 1.0) - sphereCoords;
				}
				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					float4x4 modelMatrix = unity_ObjectToWorld;
					float4x4 modelMatrixInverse = unity_WorldToObject;

					output.viewDir = mul(modelMatrix, input.vertex).xyz
						- _WorldSpaceCameraPos;
					output.pos = UnityObjectToClipPos(input.vertex);
					return output;
				}
				float4 frag(vertexOutput i) : COLOR
				{
					float2 uv = ToRadialCoords(i.viewDir);
					uv = (uv * fixed2(1, 0.5) + fixed2(0, unity_StereoEyeIndex * 0.5));
					//This is the version for if the left eye is on top
					//uv = (uv * fixed2(1, 0.5) + fixed2(0, 0.5 - unity_StereoEyeIndex * 0.5));
					return tex2D(_MainTex, uv, ddx(0), ddy(0));
				}
				ENDCG
			}
	}
}