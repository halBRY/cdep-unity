using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class point_cloud_gen : MonoBehaviour
{

    public double M_PI = 3.14159265358979323846f;

    public MeshFilter MeshFilter;
    public Mesh Mesh;
    public Material pointCloudMaterial;

	private int[] indices;

    public int height;
    public int width;

	// Start is called before the first frame update
	void Start()
    {
        //height = 2;
        //width = 2;

		Mesh = new Mesh();
        Mesh.name = "GeneratedMesh";
        
        createOdsPointCloud();


		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = Mesh;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = pointCloudMaterial;

	}

	// Update is called once per frame
	void Update()
    {

	}

    void createOdsPointCloud()
    {
        //hardcode image width and height
        height = 2048;
        width = 4096;

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
}
