using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.Runtime.InteropServices;

[ScriptedImporter(1, "ply")]

public class PlyImporter : ScriptedImporter
{
    #region DLL Functions

        [DllImport ("OctreePly", EntryPoint = "LoadPly")]
        public static extern int LoadPly(string str);

        [DllImport ("OctreePly", EntryPoint = "TestDLL")]
        public static extern int TestDLL();
        
        [DllImport ("OctreePly", EntryPoint = "GetPointCount")]
        public static extern int GetPointCount();
        
        [DllImport ("OctreePly",EntryPoint = "GetPointsByIndex_X")]
        public static extern float GetPointsByIndex_X(int n);
        
        [DllImport ("OctreePly",EntryPoint = "GetPointsByIndex_Y")]
        public static extern float GetPointsByIndex_Y(int n);
        
        [DllImport ("OctreePly",EntryPoint = "GetPointsByIndex_Z")]
        public static extern float GetPointsByIndex_Z(int n);
        
        [DllImport ("OctreePly",EntryPoint = "GetEncryptedGEOCenter")]
        public static extern float GetEncryptedGEOCenter();
    #endregion
   
    
    
    public override void OnImportAsset(AssetImportContext ctx) { 
        List<Vector3> vertices;
        Mesh plyMesh ;
        var plyObj = new GameObject();
        plyObj.AddComponent<MeshFilter>();
        plyObj.AddComponent<MeshRenderer>();
        
        vertices = new List<Vector3>();
        plyMesh = new Mesh();
        if (TestDLL() != 19) {
            Debug.LogError("OctreePlyDll Error.");
            return;
        }

        if (LoadPly(ctx.assetPath) == -1) {
            Debug.LogError("Ply couldnt loaded.");
            return;
        }
        
        int maxN = GetPointCount();
        Debug.Log("Point count: "+maxN);
        float x, y, z;
        
        float temp = GetEncryptedGEOCenter();
        z = temp - (int) (temp);
        y = temp / 1e3f - (int) (temp / 1e3f) - z / 1e3f;
        x = temp / 1e6f - y / 1e3f - z / 1e6f;
        Vector3 geometricCenter = new Vector3(x, y, z);
        for (int i = 0; i < maxN; i++) {
            x = GetPointsByIndex_X(i);
            y = GetPointsByIndex_Y(i);
            z = GetPointsByIndex_Z(i);
            Vector3 tempV = new Vector3(x, y, z);
            vertices.Add(tempV - geometricCenter);
        }

       
        plyMesh.SetVertices(vertices);
        plyMesh.SetIndices(
            Enumerable.Range(0, vertices.Count).ToArray(),
            MeshTopology.Points, 0
        );
        plyMesh.UploadMeshData(false);
        plyMesh.name = "Ply";
        plyObj.GetComponent<MeshFilter>().sharedMesh = plyMesh;
        ctx.AddObjectToAsset("main obj",plyObj);
        ctx.AddObjectToAsset("mesh", plyMesh);
        ctx.SetMainObject(plyObj);
    }
}
