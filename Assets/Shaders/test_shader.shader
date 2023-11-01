Shader "Debug/Test Shader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Vector) = (0,0,0,0)
    }
    SubShader{
       Pass {
          GLSLPROGRAM

          uniform sampler2D _MainTex;
          uniform vec4 _Color;

          #ifdef VERTEX

          varying vec4 textureCoordinates;
          varying vec4 color;

          void main()
          {
             color = _Color;

             //textureCoordinates = gl_MultiTexCoord0;
             gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

          }

          #endif

          #ifdef FRAGMENT

          varying vec4 color;
          varying vec4 textureCoordinates;

          void main()
          {
            //gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates)); // set the output fragment color
            gl_FragColor = color;
          }

          #endif

          ENDGLSL
       }
    }
}