Shader "Debug/PointCloud" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader{
       Pass {
          GLSLPROGRAM

          uniform sampler2D _MainTex;

          #ifdef VERTEX

          varying vec4 textureCoordinates;

          void main()
          {
             //textureCoordinates = gl_MultiTexCoord0;
             gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
          }

          #endif

          #ifdef FRAGMENT

          varying vec4 textureCoordinates;

          void main()
          {
            gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates)); // set the output fragment color
            //gl_FragColor = color;
          }

          #endif

          ENDGLSL
       }
    }
}