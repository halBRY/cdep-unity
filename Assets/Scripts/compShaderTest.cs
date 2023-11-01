using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class compShaderTest : MonoBehaviour
{

    public ComputeShader compShader;
    public RenderTexture rendTexture;

    public Texture2D texture1;
    public Texture2D texture2;

    public Camera activeCamera;

    public Color colorTest;
    public float testFloat;

    // Start is called before the first frame update
    void Start()
    {
		//rendTexture = new RenderTexture(256, 256, 24);
		//rendTexture.enableRandomWrite = true;
		//rendTexture.Create();

        compShader.SetVector("testColor", colorTest);
        compShader.SetFloat("testFloat", testFloat);

        compShader.SetTexture(0, "texture1", texture1);
        compShader.SetTexture(0, "texture2", texture2);

        compShader.SetTexture(0, "InputTexture", texture2);

        compShader.SetTexture(0, "Result", rendTexture);

		compShader.SetFloat("Resolution", rendTexture.width);
		compShader.Dispatch(0, rendTexture.width / 8, rendTexture.height / 8, 1);
	}

    // Update is called once per frame
    void Update()
    {
		compShader.SetVector("testPos", activeCamera.transform.position);
        compShader.SetVector("testColor", colorTest);
        compShader.SetFloat("testFloat", testFloat);

        //dispatch shader twice per frame...?
        for(int i = 0; i < 2; i++)
        {
            compShader.SetInt("eyeIndex", i);
            compShader.Dispatch(0, rendTexture.width / 8, rendTexture.height / 8, 1);
        }

		//compShader.Dispatch(0, rendTexture.width / 8, rendTexture.height / 8, 1);
	}

}
