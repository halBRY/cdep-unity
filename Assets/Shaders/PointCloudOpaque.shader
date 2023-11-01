Shader "PointCloud/SpherePointOpaque" {
	Properties{
		_Radius("Sphere Radius", float) = 0.01
		_ScaleFactor("Scale Factor", Range(0,1)) = 1
		_NumPolys("Number of Polygons", Range(3,16)) = 16
		_Rotation("Rotation", Range(0,90)) = 0
	}
		SubShader{
		LOD 200
		Tags{ "RenderType" = "Opaque" }
		//if you want transparency
		//Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		//Blend SrcAlpha OneMinusSrcAlpha
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma geometry geom
#pragma target 4.0                  // Use shader model 3.0 target, to get nicer looking lighting
#include "UnityCG.cginc"
		struct vertexIn {
		float4 pos : POSITION;
		float4 color : COLOR;
	};
	struct vertexOut {
		float4 pos : SV_POSITION;
		float4 color : COLOR0;
		float3 normal : NORMAL;
		float d : TEXCOORD0; // store distance to vertex
	};
	struct geomOut {
		float4 pos : POSITION;
		float4 color : COLOR0;
		float3 normal : NORMAL;
	};


	//Vertex shader: computes normal wrt camera
	vertexOut vert(vertexIn i) {
		vertexOut o;
		o.pos = UnityObjectToClipPos(i.pos);
		o.color = i.color;
		o.normal = ObjSpaceViewDir(o.pos);
		o.d = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, i.pos));

		return o;
	}

	float _Radius;
	float _ScaleFactor;
	float _NumPolys;
	float _Rotation;
	[maxvertexcount(48)]
	void geom(point vertexOut IN[1], inout TriangleStream<geomOut> OutputStream)
	{

		int nTriangles = floor(_NumPolys);

		float dist = IN[0].d;

		dist = (_ScaleFactor + (1 - _ScaleFactor) * dist) / 2.0;

		geomOut OUT;
		OUT.color = IN[0].color;
		OUT.normal = IN[0].normal;

		float2 p[3];
		p[0] = float2(0, 0);
		float xyscale = _ScreenParams.y / _ScreenParams.x;
		float angleOffset = _Rotation / 180 * 3.14159;

		p[2] = float2(_Radius* xyscale*cos(angleOffset), _Radius*sin(angleOffset));
		float twopion = 2.0 * 3.14159 / nTriangles;
		for (int it = 1; it <= nTriangles; it++) {
			float theta = (float(it)) * twopion + angleOffset;

			if (it == nTriangles) {
				p[1] = float2(_Radius * cos(theta) * xyscale, _Radius * sin(theta));
			}
			else {
				p[1] = p[2];
				p[2] = float2(_Radius * cos(theta) * xyscale, _Radius * sin(theta));
			}

			for (int i = 0; i < 3; i++) {
				OUT.pos = IN[0].pos + float4(p[i], 0, 0) * dist;
				OutputStream.Append(OUT);
			}

		}

	}
	float4 frag(geomOut i) : COLOR
	{
		return i.color;
	// could do some additional lighting calculation here based on normal
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}