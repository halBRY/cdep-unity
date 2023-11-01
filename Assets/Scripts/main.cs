using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class main : MonoBehaviour
{
    public Camera activeCamera;

    public bool sceneID;
    public bool debugFlag;
    
    // App Data (maybe convert to a struct later...)
    public Texture2D[] color_textures;
    public Texture2D[] depth_textures;

    public float testFloat;

    float camera_pitch;
    float camera_yaw;

    float PI = 3.14159265358979323846f;
    double M_PI = 3.14159265358979323846d;

    public Vector3[] camera_positions;
    public Vector3 synthesized_position;

    int ods_max_views = 8;
    int ods_num_views = 8;

    float aperture;
    float focalLen;
    float planeFocus;
    
    float near = 0.1f;
    float far = 50.0f;

    public Mesh Mesh;
    public Material pointCloudMaterial;

    // Stuff for output texture
    public RenderTexture renderTex;
    public Texture2D inputTexture;

	private int[] indices;

    public ComputeShader compShader;

	// Start is called before the first frame update
	void Start()
    {
        //Point Cloud 
        Mesh = new Mesh();
        Mesh.name = "OdsPointCloud";
        
        createOdsPointCloud();

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = Mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = pointCloudMaterial;

		// Texture Initialization
		// App Data Variables
		ods_max_views = 8;
        ods_num_views = 8;

        color_textures = new Texture2D[8];
        depth_textures = new Texture2D[8];
        camera_positions = new Vector3[8];

        float[] cam_position1 = { -0.35f, 1.85f, 0.55f };
        float[] cam_position2 = { 0.35f, 1.55f, 0.90f };
        float[] cam_position3 = { -0.10f, 1.75f, 0.85f };
        float[] cam_position4 = { 0.25f, 1.70f, 0.60f };
        float[] cam_position5 = { -0.30f, 1.67f, 0.75f };
        float[] cam_position6 = { -0.20f, 1.60f, 0.70f };
        float[] cam_position7 = { 0.15f, 1.78f, 0.57f };
        float[] cam_position8 = { 0.05f, 1.82f, 0.87f };


        if (debugFlag)
        {
            ods_max_views = 5;
            ods_num_views = 5;

            initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/debug_textures/red", cam_position1, 4);
            initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/debug_textures/green", cam_position2, 1);
            initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/debug_textures/blue", cam_position3, 2);
            initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/debug_textures/black", cam_position4, 3);
            initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/debug_textures/white", cam_position5, 0);
        }
        else
        {

			/* Paths per machine: 
			HAL9000: E:/Argonne/23/cdep/cdep-unity
			HAL9001: ???
			EVL NUC: C:/Users/hal9000/cdep-unity
			*/

			if (sceneID)
			{
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_1", cam_position1, 0);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_2", cam_position2, 1);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_3", cam_position3, 2);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_4", cam_position4, 3);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_5", cam_position5, 4);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_6", cam_position6, 5);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_7", cam_position7, 6);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Office (Virtual)/ods_cdep_4k_camera_8", cam_position8, 7);
			}
			else
			{
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_1", cam_position1, 0);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_2", cam_position2, 1);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_3", cam_position3, 2);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_4", cam_position4, 3);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_5", cam_position5, 4);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_6", cam_position6, 5);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_7", cam_position7, 6);
				initializeOdsTextures("E:/Argonne/23/cdep/cdep-unity/Assets/Textures/Scenes/Spheres (Virtual)/spheres_ods_cdep_4k_camera_8", cam_position8, 7);
			}

		}

        //Apply texture to GameObject (to send to shader)
        Renderer rend = GetComponent<Renderer>();
        rend.material.mainTexture = color_textures[0];

        aperture = 0.027f;
        focalLen = 0.05f;
        planeFocus = 2.15f;

        
        //Compute Shader set up
        compShader.SetTexture(0, "Result", renderTex);

        // initialization time: ask for “Raw” compute access
        Mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        // get vertex buffer, setup for the compute shader, dispatch, release
        var vb = Mesh.GetVertexBuffer(0);
        compShader.SetBuffer(0, "vertexBuffer", vb);
        //computeShader.Dispatch(...);
        vb.Dispose();


        compShader.SetTexture(0, "Result", renderTex);
    }

    // Update is called once per frame
    void Update()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.SetFloat("_testFloat", testFloat);

        //Draw synthesized view 
        //synthesized_position = new Vector3(0.0f, 1.70f, 0.725f);

        synthesized_position = activeCamera.transform.position;
        synthesizeOdsImage(synthesized_position);

        //Uniforms
        //rend.material.SetVector("_texScale", rend.material.mainTextureScale);
        //rend.material.SetVector("_texOffset", rend.material.mainTextureOffset);
        Vector2 scale = new Vector2(1.0f, 0.5f);
        Vector2 offset = new Vector2(0.0f, 0.0f); //left
        //Vector2 offset = new Vector2(0.0f, 0.0f); //right

        rend.material.SetVector("_texScale", scale);
        rend.material.SetVector("_texOffset", offset);

        rend.material.SetFloat("_aperture", aperture);
        rend.material.SetFloat("_focalLen", focalLen);
        rend.material.SetFloat("_planeFocus", planeFocus);

        //Apply new texture as a render texture
        //Graphics.Blit(rend.material.mainTexture, renderTex);
        //Graphics.BlitMultiTap(rend.material.mainTexture, renderTex, rend.material);


        compShader.SetTexture(0, "InputTexture", color_textures[0]);

        for(int i = 0; i < 2; i++)
        {
            compShader.SetInt("eyeIndex", i);
            compShader.Dispatch(0, renderTex.width / 8, renderTex.height / 8, 1);
        }

    }

    void initializeOdsTextures(string file_name, float[] camera_position, int index)
    {

        // Load from file path and save as texture - color
        string textureImagePath = file_name + ".png";

        byte[] bytes = File.ReadAllBytes(textureImagePath);

        Texture2D loadTexture = new Texture2D(1, 1); //mock size 1x1
        loadTexture.LoadImage(bytes);

        color_textures[index] = loadTexture;

        // Load from file path to texture asset - depth
        string depthImagePath = file_name + ".depth";

        byte[] depthBytes = File.ReadAllBytes(depthImagePath);

        Texture2D depthLoadTexture = new Texture2D(1, 1); //mock size 1x1
        depthLoadTexture.LoadImage(depthBytes);

        depth_textures[index] = depthLoadTexture;

        camera_positions[index] = new Vector3(camera_position[0], camera_position[1], camera_position[2]);
    }

    void synthesizeOdsImage(Vector3 camera_position) 
    {

        //Debug.Log(camera_positions[0]);

        Renderer rend = GetComponent<Renderer>();
        int i, j = 0;

        //Uniforms 
        //Camera Uniforms
        rend.material.SetFloat("_cameraIPD", 0.065f);
        rend.material.SetFloat("_cameraFD", 1.95f);
        rend.material.SetFloat("_xrfovy", 75.0f * PI / 180.0f);
        rend.material.SetFloat("_xrAspect", activeCamera.aspect);
        rend.material.SetVector("_xrViewDir", activeCamera.transform.forward);

        //Compute Style
        compShader.SetFloat("_cameraIPD", 0.065f);
        compShader.SetFloat("_cameraFD", 1.95f);
        compShader.SetFloat("_xrfovy", 75.0f * PI / 180.0f);
        compShader.SetFloat("_xrAspect", activeCamera.aspect);
        compShader.SetVector("_xrViewDir", activeCamera.transform.forward);

        //Create ortho_projection matirx
        var ortho_projection = Matrix4x4.Ortho(2.0f * PI, 0.0f, PI, 0.0f, near, far);
        rend.material.SetMatrix("_orthoProjection", ortho_projection);

        //Determine Views - WIP
        //List<int> view_indices = new List<int>();
        int num_views = Mathf.Min(ods_num_views, ods_max_views);
        //determineViews(camera_position, num_views, ref view_indices);

        for(i = 0; i < 2; i++)
        {
            rend.material.SetFloat("_cameraEye", 2.0f * (i - 0.5f));

            for(j = 0; j < num_views; j++)
            {
                rend.material.SetFloat("_imgIndex", (float)j);

                //Camera Position
                Vector3 relative_cam_pos = camera_position - camera_positions[j];
                rend.material.SetVector("_cameraPos", relative_cam_pos);

                //Texture
                rend.material.mainTexture = color_textures[j];
                rend.material.SetTexture("_depths", depth_textures[j]);
            }
        }
    }

    void createOdsPointCloud()
    {
		//hardcode image width and height
		int height = 2048;
		int width = 4096;

		//create point cloud for first shader pass. 
		int i, j, k = 0;
		int size = width * height;

		indices = new int[size];

		// Create arrays for vertex positions and texture coordinates
		//float arrays
		//float vertices[] = new float[2 * size];
		//float texcoords[] = new float[2 * size];

		Vector3[] vertices = new Vector3[size];
		Vector2[] texcoords = new Vector2[size];


		for (j = 0; j < height; j++)
		{
			for (i = 0; i < width; i++)
			{
				double norm_x = (i + 0.5d) / (double)width;
				double norm_y = (j + 0.5d) / (double)height;
				double azimuth = 2.0d * M_PI * (1.0d - norm_x);
				double inclination = M_PI * norm_y;

				//vertices[i + 0] = azimuth;
				//vertices[i + 1] = inclination;
				//texcoords[i + 0] = norm_x;
				//texcoords[i + 1] = norm_y;

				vertices[k] = new Vector3((float)azimuth, (float)inclination, 0.0f);
				texcoords[k] = new Vector2((float)norm_x, (float)norm_y);

				//Debug.Log(k);
				//Debug.Log(vertices[k]);

				indices[k] = k;

				k++;
			}
		}

		Mesh.vertices = vertices;
		Mesh.uv = texcoords;

		Mesh.indexFormat = IndexFormat.UInt32;
		Mesh.SetIndices(indices, MeshTopology.Points, 0);
	}

    /*
    void determineViews(Vector3 camera_position, int num_views, ref List<int> view_indices)
    {
        // Start by adding bounding corners (in num_views >= 2)
        if (num_views >= 2)
        {
            view_indices.Add(0);
            view_indices.Add(1);
        }

        // Continue adding closest views
        int i, j;
        for (j = view_indices.Count; j < num_views; j++)
        {
            float closest_dist2 = 9900000000000f;
            int closest_index = -1;

            for (i = 0; i < ods_num_views; i++)
            {
                //this is DUBIOUS. 
                int test_index = view_indices.FindIndex(a => a == i);
                if (test_index != view_indices.Count)
                {
                    continue;
                }
                float dist2 = glm::distance2(camera_position, camera_positions[i]);
                if (dist2 < closest_dist2)
                {
                    closest_dist2 = dist2;
                    closest_index = i;
                }
            }
            view_indices.Add(closest_index);
        }

    }*/
}
