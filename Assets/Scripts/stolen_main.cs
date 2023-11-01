using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stolen_main : MonoBehaviour
{
    public RenderTexture renderTex;
    public Texture2D baseTex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Renderer rend = GetComponent<Renderer>();
        Graphics.Blit(baseTex, renderTex);
        rend.material.mainTexture = renderTex;

    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}
