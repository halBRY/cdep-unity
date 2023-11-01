Shader "CDEP/ODS DEP Shader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _cameraIPD("Camera IPD", Float) = 0
        _cameraFD("Camera Focal Distance", Float) = 0
        _xrfovy("XR fovy", Float) = 0
        _xrAspect("XR Aspect Ratio", Float) = 0
        _xrViewDir("XR View Direction", Vector) = (0,0,0)
        _cameraEye("Camera Eye", Float) = 0
        _imgIndex("Image Index", Float) = 0
        _cameraPos("Camera Position", Vector) = (0,0,0)
        _depths("Depth Texture", 2D) = "white" {}
        _testFloat("Color test", Float) = 0
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

            uniform float _cameraIPD;
            uniform float _cameraFD;
            uniform float _xrfovy;
            uniform float _xrAspect;
            uniform vec3 _xrViewDir;
            uniform float _cameraEye;

            uniform float _imgIndex;
            uniform vec3 _cameraPos;

            uniform float _testFloat;

            uniform float _aperture;
            uniform float _focalLen;
            uniform float _planeFocus;
            uniform vec3 _texScale;
            uniform vec3 _texOffset;

            uniform mat4 UNITY_MATRIX_P;
            uniform mat4 UNITY_MATRIX_MV;

            #ifdef VERTEX 

                precision highp float;

                #define M_PI 3.1415926535897932384626433832795
                #define EPSILON 0.000001

                varying vec4 textureCoordinates;
                varying float pt_depth;
                varying vec4 position;

                void main()
                {

                    // Calculate projected point position (relative to projection sphere center)
                    float azimuth = gl_Vertex.x;
                    
                    float inclination = gl_Vertex.y;
                    
                    float vertex_depth = texture(_depths, vec2(gl_MultiTexCoord0)).r;
                    
                    vec3 pt = vec3(vertex_depth * cos(azimuth) * sin(inclination),
                                    vertex_depth * sin(azimuth) * sin(inclination),
                                    vertex_depth * cos(inclination));
                    
                    // Backproject to new ODS panorama
                    vec3 camera_spherical = vec3(_cameraPos.z, _cameraPos.x, _cameraPos.y);
                    vec3 vertex_direction = pt - camera_spherical;
                    float magnitude = length(vertex_direction);
                    float center_azimuth = (abs(vertex_direction.x) < EPSILON && abs(vertex_direction.y) < EPSILON) ?
                                            (1.0 - 0.5 * sign(vertex_direction.z)) * M_PI :
                                            atan(vertex_direction.y, vertex_direction.x);
                    float center_inclination = acos(vertex_direction.z / magnitude);

                    float camera_radius = 0.5 * _cameraIPD * cos(center_inclination - (M_PI / 2.0));
                    float camera_azimuth = center_azimuth + _cameraEye * acos(camera_radius / magnitude);
                    vec3 camera_pt = vec3(camera_radius * cos(camera_azimuth),
                                        camera_radius * sin(camera_azimuth),
                                        0.0);
                    vec3 camera_to_pt = vertex_direction - camera_pt;
                    float camera_distance = length(camera_to_pt);
                    vec3 camera_ray = camera_to_pt / camera_distance;
                    float img_sphere_dist = sqrt(_cameraFD * _cameraFD - camera_radius * camera_radius);
                    vec3 img_sphere_pt = camera_pt + img_sphere_dist * camera_ray;
                    float projected_azimuth = (abs(img_sphere_pt.x) < EPSILON && abs(img_sphere_pt.y) < EPSILON) ?
                                            (1.0 - 0.5 * sign(img_sphere_pt.z)) * M_PI :
                                            mod(atan(img_sphere_pt.y, img_sphere_pt.x), 2.0 * M_PI);
                    float projected_inclination = acos(img_sphere_pt.z / _cameraFD);

                    // Set point size (1.25 seems to be a good balance between filling small holes and blurring image)
                    //gl_PointSize = 1.0;
                    float size_ratio = vertex_depth / camera_distance;
                    float size_scale = 1.1 + (0.4 - (0.16 * min(camera_distance, 2.5))); // scale ranges from 1.1 to 1.5
                    gl_PointSize = size_scale * size_ratio;

                    // XR viewport only
                    float diag_aspect = sqrt(_xrAspect * _xrAspect + 1.0);
                    float vertical_fov = 0.5 * _xrfovy + 0.005;
                    //float horizontal_fov = atan(tan(vertical_fov) * _xrAspect);
                    float diagonal_fov = atan(tan(vertical_fov) * diag_aspect);
                    vec3 point_dir = normalize(img_sphere_pt.yzx);
                    // discard point (move outside view volume) if angle between point direction and view diretion > diagonal FOV
                    projected_azimuth -= float(dot(point_dir, _xrViewDir) < cos(diagonal_fov)) * 10.0;

                    // Set point position
                    float depth_hint = 0.015 * _imgIndex; // favor image with lower index when depth's match (index should be based on dist)
                    gl_Position = UNITY_MATRIX_P * vec4(projected_azimuth, projected_inclination, -camera_distance - depth_hint, 1.0);
                    position = UNITY_MATRIX_P * vec4(projected_azimuth, projected_inclination, -camera_distance - depth_hint, 1.0);

                    // Pass along texture coordinate and depth
                    pt_depth = camera_distance;
                    textureCoordinates = gl_MultiTexCoord0;
                }

            #endif


            #ifdef FRAGMENT 

                precision mediump float;

                varying vec4 textureCoordinates;
                varying float pt_depth;
                varying vec4 position;

                //uniform mat4 modelview; -> UNITY_MATRIX_MV
                //uniform mat4 projection; -> unity_CameraProjection

                vec4 gaussianBlur(vec2 uv, float radius);

                void main()
                {
                    
                    vec2 texScale = vec2(_texScale.x, _texScale.y);
                    vec2 texOffset = vec2(_texOffset.x, _texOffset.y);

                    vec2 uv = vec2(textureCoordinates) * texScale + texOffset;

                    // Color
                    float obj_distance = texture(_depths, uv).r;
                    float CoC = abs(_aperture * (_focalLen * (obj_distance - _planeFocus)) / (obj_distance * (_planeFocus - _focalLen)));
                    float blur_radius = 5000.0 * CoC;
                    gl_FragColor = gaussianBlur(uv, blur_radius);
                    //FragColor = texture2D(image, uv);

                    // Depth
                    vec3 frag_pos = obj_distance * vec3(position.xyz);
                    float far = gl_DepthRange.far;
                    float near = gl_DepthRange.near;
                    vec4 v_clip_coord = UNITY_MATRIX_P * UNITY_MATRIX_MV * vec4(frag_pos, 1.0);
                    float f_ndc_depth = v_clip_coord.z / v_clip_coord.w;
                    float frag_depth = (((far - near) * f_ndc_depth) + far + near) * 0.5;

                    gl_FragDepth = frag_depth;

                    //gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates));
                }

                vec4 gaussianBlur(vec2 uv, float radius)
                {
                    vec4 color = vec4(0.0);
                    vec2 resolution = vec2(4096, 2048);
                    vec2 dirs[2] = vec2[](vec2(radius, 0.0), vec2(0.0, radius));
                    for (int i = 0; i < 2; i++) {
                        vec2 off1 = vec2(1.3846153846) * dirs[i];
                        vec2 off2 = vec2(3.2307692308) * dirs[i];
                        color += texture2D(_MainTex, uv) * 0.2270270270;
                        color += texture2D(_MainTex, uv + (off1 / resolution)) * 0.3162162162;
                        color += texture2D(_MainTex, uv - (off1 / resolution)) * 0.3162162162;
                        color += texture2D(_MainTex, uv + (off2 / resolution)) * 0.0702702703;
                        color += texture2D(_MainTex, uv - (off2 / resolution)) * 0.0702702703;
                    }
                    return color / 2.0;
                }


            #endif 

    ENDGLSL
     }
        }
}