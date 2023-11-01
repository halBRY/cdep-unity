Shader"XR/Test Stereo GLSL" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Vector) = (0,0,0,0)
    }
    SubShader{
       Pass {
          GLSLPROGRAM

            uniform sampler2D _MainTex;
            uniform vec4 _Color;

            uniform int unity_StereoEyeIndex;

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

                    vec4 newcolor;

                    //gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates)); // set the output fragment color
                    if(unity_StereoEyeIndex == 0)
                        newcolor = vec4(1,0,0,1);
                    else
                        newcolor = vec4(0,1,0,1);

                    gl_FragColor = newcolor;
                  }

            #endif

          ENDGLSL
       }
    }
}