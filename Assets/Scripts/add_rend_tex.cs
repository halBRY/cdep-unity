using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class add_rend_tex : MonoBehaviour
{
    public RenderTexture renderTex;
    public Texture2D baseTex;

    public bool testMode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Renderer rend = GetComponent<Renderer>();
        
        if(testMode == true)
            Graphics.Blit(baseTex, renderTex);
        
        rend.material.mainTexture = renderTex;

    }

}