using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.EventSystems;

public class PointCloudImport : MonoBehaviour

{
 // Note: create an empty gameobject called control or whatever and assign this script to it
    // public GameObject spherePrefab; // for halos

    public GameObject parentPrefab; // create an empty gameobject in the hierarchy then drag and drop to the project menu to make it a prefab then drag the created prefab to its spot in the inspector 

    private List<GameObject> timeStepParents = new List<GameObject>();
   // private List<GameObject> haloObjects = new List<GameObject>();
    private List<GameObject> pointCloudObjects = new List<GameObject>();
    private int currentTimestepIndex = 0;
    private float timer;
    private bool isTimeLapseActive = false;
    private bool isFirstTimestep = true;
    public float delayBetweenTimeSteps = 1.0f;

    private int startFrameNum = 0;// adjust based on your frame num
    private int endFrameNum = 630; // adjust based on your frame num
    private int delta = 10; // adjust based on your frame num
    private int totalTimesteps;
   // private bool areHalosVisible = true;
    private bool arePointCloudsVisible = true;
  //  public TextMeshProUGUI detailsText;
   // private List<bool> haloVisibility= new List<bool>();
    private List<bool> particleVisibility = new List<bool>();
   private Camera mainCamera;

   // private Transform highlight;
  //  private Transform selection;
  //  private RaycastHit raycastHit;

    void Start()
    {
        totalTimesteps = Mathf.CeilToInt((endFrameNum - startFrameNum + 1) / (float)delta);
        CreateTimeStepParents(); // create parents on start
        ImportData();
        SetActiveTimeStep(currentTimestepIndex, true);
        mainCamera = Camera.main;
      
    }

    void Update()
    {
        if (isTimeLapseActive)
        {
            timer += Time.deltaTime;

            if (timer >= delayBetweenTimeSteps)
            {
                timer = 0f;
                SetActiveTimeStep(currentTimestepIndex, false);
                currentTimestepIndex = (currentTimestepIndex + 1) % timeStepParents.Count;
                SetActiveTimeStep(currentTimestepIndex);
               // for (int i = 0; i < haloObjects.Count; i++)
                //{
                   // haloObjects[i].SetActive(haloVisibility[i]);
               // }
                for (int i = 0; i < pointCloudObjects.Count; i++)
                {
                    pointCloudObjects[i].SetActive(particleVisibility[i]);
                }
            }
            else
            {
                SetActiveTimeStep(currentTimestepIndex, true); //handles pause at index
            }
}
        /* Vector2 mousePosition = Mouse.current.position.ReadValue();
     Ray ray = mainCamera.ScreenPointToRay(mousePosition);
     RaycastHit hit;

     if (Physics.Raycast(ray, out hit))
     {
         GameObject hitObject = hit.collider.gameObject;

         int objectIndex = -1;

         if (haloObjects.Contains(hitObject))
         {
             objectIndex = haloObjects.IndexOf(hitObject) * 5; // Assuming each halo corresponds to 5 spheres
         }

         if (objectIndex >= 0)
         {
             ShowObjectDetails(objectIndex);
         }
         else
         {
             ClearObjectDetails();
         }
     }
     else
     {
         ClearObjectDetails();
     }
     // Highlight
     if (highlight != null)
     {
         highlight.gameObject.GetComponent<Outline>().enabled = false;
         highlight = null;
     }
     Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
     if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray2, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
     {
         highlight = raycastHit.transform;
         if (highlight.CompareTag("Selectable") && highlight != selection)
         {
             if (highlight.gameObject.GetComponent<Outline>() != null)
             {
                 highlight.gameObject.GetComponent<Outline>().enabled = true;
             }
             else
             {
                 Outline outline = highlight.gameObject.AddComponent<Outline>();
                 outline.enabled = true;
                 highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
                 highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
             }
         }
         else
         {
             highlight = null;
         }
     }

     // Selection
     if (Input.GetMouseButtonDown(0))
     {
         if (highlight)
         {
             if (selection != null)
             {
                 selection.gameObject.GetComponent<Outline>().enabled = false;
             }
             selection = raycastHit.transform;
             selection.gameObject.GetComponent<Outline>().enabled = true;
             highlight = null;
         }
         else
         {
             if (selection)
             {
                 selection.gameObject.GetComponent<Outline>().enabled = false;
                 selection = null;
             }
         }
     }*/
    }


   /* private void ShowObjectDetails(int objectIndex)
    {
        if (objectIndex >= 0 && objectIndex < (haloObjects.Count))
        {
            string filePath = $"Assets/hydro_particles/hydro_particles_step_{currentTimestepIndex * delta + startFrameNum:D4}.csv";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                int lineIndex = objectIndex + 1;

                if (lineIndex >= 0 && lineIndex < lines.Length)
                {
                    string[] values = lines[lineIndex].Split(',');

                    // CSV file structure
                    string valueDetails = $"Value: {values[0]}\n";
                    string positionDetails = $"Position: ({values[2]}, {values[3]}, {values[4]})\n";

                    string details = $"Details for Halo {objectIndex}:\n{valueDetails}{positionDetails}";
                    detailsText.text = details;
                }
            }
        }
    }
   
    private void ClearObjectDetails()
    {
        detailsText.text = "";
    }
   */

    private void CreateTimeStepParents()
    {
        for (int i = 0; i < totalTimesteps; i++)
        {
            GameObject timeStepParent = Instantiate(parentPrefab);
            timeStepParents.Add(timeStepParent);
        }
    }
    private GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }
        return children;
    }
    private void ImportData()
    {
        for (int frameNum = startFrameNum; frameNum <= endFrameNum; frameNum += delta)
        {
            int parentIndex = (frameNum - startFrameNum) / delta;

            string filePath = $"Assets/hydro_particles/hydro_particles_step_{frameNum:D4}.csv";

            if (File.Exists(filePath))
            {
                GameObject timeStepObjectsParent = Instantiate(parentPrefab);

                // empty parent for halos
                //GameObject haloParent = new GameObject("Halos");
               // haloParent.transform.parent = timeStepObjectsParent.transform;

                // empty parent for particles
                GameObject pointCloudParent = new GameObject("Particle Data");
                pointCloudParent.transform.parent = timeStepObjectsParent.transform;

                // Import halos and particles
              //  ImportHalos(frameNum, haloParent);
                ImportPointCloud(frameNum, pointCloudParent);

                // Set parent for halos and particles
               // haloParent.transform.parent = timeStepParents[parentIndex].transform;
                pointCloudParent.transform.parent = timeStepParents[parentIndex].transform;
                timeStepParents[parentIndex].SetActive(false);

                // Add halo objects and particle objects to the respective lists
               // haloObjects.AddRange(GetChildren(haloParent));
                pointCloudObjects.AddRange(GetChildren(pointCloudParent));
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
            }
        }
    }

    /*
   private void ImportHalos(int frameNum, GameObject parent)
{
    string filePath = $"Assets/hydro_particles/hydro_particles_step_{frameNum:D4}.csv";

    if (File.Exists(filePath))
    {
        string[] lines = File.ReadAllLines(filePath);
        int n = Mathf.Min(10, lines.Length - 1);

        for (int i = 0; i < n; i++)
        {
            string[] values = lines[i + 1].Split(',');
            float x = float.Parse(values[2]);
            float y = float.Parse(values[3]);
            float z = float.Parse(values[4]);
            float scalarNum = float.Parse(values[0]);

            GameObject sphere = Instantiate(spherePrefab, new Vector3(x, y, z), Quaternion.identity);
            sphere.name = $"Halo_{frameNum}_{i}";
            sphere.GetComponent<Renderer>().material.color = RemapColor(scalarNum);
            sphere.transform.parent = parent.transform;
            haloVisibility.Add(areHalosVisible);

            // Apply the "Selectable" tag to the halo
            sphere.tag = "Selectable";
        }
    }
    else
    {
        Debug.LogWarning("File not found: " + filePath);
    }*/

    //pointcloud import script
    void ImportPointCloud(int frameNum, GameObject parent)
    {

        string filePath = string.Format("Assets/hydro_particles/hydro_particles_step_{0:D4}.csv", frameNum); // replace with folder path 
        Debug.Log(filePath);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            int n = Mathf.Min(10000, lines.Length - 1);

            Vector3[] points = new Vector3[n];
            Color[] colors = new Color[n];
            int[] indices = new int[n];
            float[] scalarNum = new float[n];
            float minScalarNum = float.MaxValue;
            float maxScalarNum = float.MinValue;

            for (int i = 0; i < n; i++)
            {
                string[] values = lines[i + 1].Split(',');
                float x = float.Parse(values[2]);
                float y = float.Parse(values[3]);
                float z = float.Parse(values[4]);
                scalarNum[i] = float.Parse(values[0]);
                points[i] = new Vector3(x, y, z);
            }

            // map min and max 
            Color minScalarColor = new Color(1, 0, 0);
            Color maxScalarColor = new Color(1, 1, 0);

            for (int i = 0; i < n; i++)
            {
                minScalarNum = Mathf.Min(minScalarNum, scalarNum[i]);
                maxScalarNum = Mathf.Max(maxScalarNum, scalarNum[i]);
                colors[i] = Color.Lerp(minScalarColor, maxScalarColor, remap(minScalarNum, maxScalarNum, 0, 1, scalarNum[i]));
                colors[i].a = 0.5f;
                indices[i] = i;
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = points;
            mesh.colors = colors;
            mesh.SetIndices(indices, MeshTopology.Points, 0);

            GameObject pointCloudObject = new GameObject("HydroParticles_" + frameNum); // rename if needed

            MeshFilter meshFilter = pointCloudObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            MeshRenderer meshRenderer = pointCloudObject.AddComponent<MeshRenderer>();

            Material pointCloudMaterial = Resources.Load<Material>("PointCloud"); // create a folder in the project menu called "Resources" that contains the shader. Change the name of the shader accordingly or rename as PointCloud
            if (pointCloudMaterial != null)
            {
                meshRenderer.material = new Material(pointCloudMaterial);
            }
            else
            {
                Debug.LogWarning("Material 'PointCloud' not found.");
            }
            pointCloudObject.transform.parent = parent.transform;
            particleVisibility.Add(arePointCloudsVisible);
        }
        else
        {
            Debug.LogWarning("File not found: " + filePath);
        }

    }

    Color RemapColor(float radius)
    {
        Color minColor = new Color(1, 0, 0);
        Color maxColor = new Color(1, 1, 0);

        float minRadius = 0;
        float maxRadius = 1;

        float remappedValue = remap(minRadius, maxRadius, 0, 1, radius);

        return Color.Lerp(minColor, maxColor, remappedValue);
    }

    private float remap(float a, float b, float c, float d, float x)
    {
        return c + (x - a) * (d - c) / (b - a);
    }

    private void SetActiveTimeStep(int index, bool active = true)
    {
        if (index >= 0 && index < timeStepParents.Count)
        {
            timeStepParents[index].SetActive(active);
        }
    }

    // create a ui toggle to toggle visibility and assign each methods. comment out if not needed
   /* public void ToggleHalosVisibility()
    {
        areHalosVisible = !areHalosVisible;
        foreach (GameObject halo in haloObjects)
        {
            halo.SetActive(areHalosVisible);
        }
    }
*/
    public void TogglePointCloudVisibility()
    {
        arePointCloudsVisible = !arePointCloudsVisible;
        foreach (GameObject pointCloud in pointCloudObjects)
        {
            pointCloud.SetActive(arePointCloudsVisible);
        }
    }

    //create ui buttons for this portion and assign each methods to their respective buttons. comment out if not needed
    public void StartButton()
    {
        if (isFirstTimestep)
        {
            isFirstTimestep = false;
        }
        SetActiveTimeStep(currentTimestepIndex);
        isTimeLapseActive = true;
    }

    public void StopButton()
    {
        isTimeLapseActive = false;
        SetActiveTimeStep(currentTimestepIndex, true);
    }

    public void RefreshButton()
    {
        currentTimestepIndex = 0;
        SetActiveTimeStep(currentTimestepIndex);
    }

  

}
