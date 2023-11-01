Shader "CDEP/ODS Shader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _depths("Depth Texture", 2D) = "white" {}
        _texScale("Texture Scale", Vector) = (0,0,0)
        _texOffset("Texture Offset", Vector) = (0,0,0)
        _aperture("Aperture", Float) = 0
        _focalLen("Focal Length", Float) = 0
        _planeFocus("Plane in Focus", Float) = 0
    }
        SubShader{
           Pass {
              GLSLPROGRAM

              uniform sampler2D _MainTex;
              uniform sampler2D _depths;

              uniform float aperture;
              uniform float focal_length;
              uniform float plane_in_focus;
              uniform vec2 texture_scale;
              uniform vec2 texture_offset;

              #define M_PI 3.1415926535897932384626433832795
              #define M_e 2.71828182845904523536


              #ifdef VERTEX 

                  precision highp float;

                  #define M_PI 3.1415926535897932384626433832795
                  #define EPSILON 0.000001

                  varying vec4 textureCoordinates;
                  varying vec3 position;


                  void main()
                  {
                      /*
                      vec4 world_pos = vec4(gl_Vertex, 1.0);
                      gl_Position = unity_CameraProjection * UNITY_MATRIX_MV * world_pos;

                      // Pass along position and texture coordinate
                      position = world_pos.xyz;
                      textureCoordinates = gl_MultiTexCoord0;*/

                      textureCoordinates = gl_MultiTexCoord0;
                      gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                     
                  }

               #endif


               #ifdef FRAGMENT 

                   precision mediump float;

                   varying vec4 textureCoordinates;
                   varying vec3 position;

                   //vec4 gaussianBlur(vec2 uv, float radius);

                   void main() {
                       /*
                       vec2 texScale = vec2(_texScale.x, _texScale.y);
                       vec2 texOffset = vec2(_texOffset.x, _texOffset.y);

                       vec2 uv = textureCoordinates * texScale + texOffset;

                       // Color
                       float obj_distance = texture(depths, uv).r;
                       float CoC = abs(_aperture * (_focalLen * (obj_distance - _planeFocus)) / (obj_distance * (_planeFocus - _focalLen)));
                       float blur_radius = 5000.0 * CoC;
                       gl_FragColor = gaussianBlur(uv, blur_radius);
                       //FragColor = texture2D(image, uv);

                       // Depth
                       vec3 frag_pos = obj_distance * position;
                       float far = gl_DepthRange.far;
                       float near = gl_DepthRange.near;
                       vec4 v_clip_coord = unity_CameraProjection * UNITY_MATRIX_MV * vec4(frag_pos, 1.0);
                       float f_ndc_depth = v_clip_coord.z / v_clip_coord.w;
                       float frag_depth = (((far - near) * f_ndc_depth) + far + near) * 0.5;

                       gl_FragDepth = frag_depth;*/
                       gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates));
                   }

                   /*
                   vec4 gaussianBlur(vec2 uv, float radius) {
                       vec4 color = vec4(0.0);
                       vec2 resolution = vec2(4096, 2048);
                       vec2 dirs[2] = vec2[](vec2(radius, 0.0), vec2(0.0, radius));
                       for (int i = 0; i < 2; i++) {
                           vec2 off1 = vec2(1.3846153846) * dirs[i];
                           vec2 off2 = vec2(3.2307692308) * dirs[i];
                           color += texture2D(image, uv) * 0.2270270270;
                           color += texture2D(image, uv + (off1 / resolution)) * 0.3162162162;
                           color += texture2D(image, uv - (off1 / resolution)) * 0.3162162162;
                           color += texture2D(image, uv + (off2 / resolution)) * 0.0702702703;
                           color += texture2D(image, uv - (off2 / resolution)) * 0.0702702703;
                       }
                       return color / 2.0;
                   }*/

                #endif 

        ENDGLSL
     }
        }
}