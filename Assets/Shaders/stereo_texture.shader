Shader "XR/Stereo Sphere" {
    Properties{
        _TexRight("Texture", 2D) = "white" {}
        _TexLeft("Texture", 2D) = "white" {}
    }
    SubShader{
        Pass {
            GLSLPROGRAM

            uniform sampler2D _TexRight;
            uniform sampler2D _TexLeft;

            uniform int unity_StereoEyeIndex;

            #ifdef VERTEX

            varying vec4 textureCoordinates;
            varying vec4 color;

            void main()
            {
                if (unity_StereoEyeIndex == 0)
                {
                    color = vec4(1, 0, 1, 1);
                }
                else
                {
                    color = vec4(0, 1, 1, 1);
                }

                textureCoordinates = gl_MultiTexCoord0;
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
            }

            #endif

            #ifdef FRAGMENT

            varying vec4 color;
            varying vec4 textureCoordinates;

            void main()
            {
                gl_FragColor = color;
                //gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates)); // set the output fragment color
            }

            #endif

            ENDGLSL
        }
    }
}