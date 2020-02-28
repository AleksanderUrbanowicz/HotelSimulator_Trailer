using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public enum shaderSelection
{
    Standard = 0,
    Unlit = 1
}
public class BillboardMakerWindow : EditorWindow
{
    public string textureSavePath = "";
    private GameObject[] billboardColliders;
    public GameObject bakedMeshesFolder;
    public Color cameraBackgroundColor = Color.black;
    public int Resolution = 1024;
    public float CameraDistance = 20f;
    public bool CustomCameraSettings = false;
    public float ClippingNear = 0.01f;
    public GameObject parentToBake;
    private Camera camera;
    public LayerMask bakingLayer;
    public LayerMask bakedLayer;
    public GameObject collidersHolder;
    private bool useCustomColliderParent;
    
    [MenuItem("Katana/Billboard maker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BillboardMakerWindow));
    }
    
    public shaderSelection shaderSelected;

    private void OnInspectorUpdate() => Repaint();


    void OnGUI()
    {
        GUIStyle errorStyle = new GUIStyle();
        errorStyle.fontSize = 16;
        errorStyle.fontStyle = FontStyle.Bold;
        errorStyle.alignment = TextAnchor.MiddleCenter;
        errorStyle.wordWrap = true;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Camera distance:");
        CameraDistance = EditorGUILayout.FloatField(CameraDistance);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Resolution:");
        Resolution = EditorGUILayout.IntField(Resolution);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Camera background color:");
        cameraBackgroundColor = EditorGUILayout.ColorField(cameraBackgroundColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use custom camera settings ?");
        CustomCameraSettings = EditorGUILayout.Toggle(CustomCameraSettings);
        EditorGUILayout.EndHorizontal();
        
        if(CustomCameraSettings)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clipping near:");
            ClippingNear = EditorGUILayout.FloatField(ClippingNear);
            EditorGUILayout.EndHorizontal();
        }
        
            

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Objects to bake layer:");
        bakingLayer = EditorGUILayout.LayerField(bakingLayer);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Baked meshes layer:");
        bakedLayer = EditorGUILayout.LayerField(bakedLayer);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Baked shader:");
        shaderSelected = (shaderSelection)EditorGUILayout.EnumPopup(shaderSelected);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Baked meshes hierarchy parent:");
        bakedMeshesFolder = EditorGUILayout.ObjectField(bakedMeshesFolder,typeof(Object)) as GameObject;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Use custom collider holder ?");
        useCustomColliderParent = EditorGUILayout.Toggle(useCustomColliderParent);
        EditorGUILayout.EndHorizontal();

        if (useCustomColliderParent)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Custom collider holder:");
            collidersHolder = EditorGUILayout.ObjectField(collidersHolder, typeof(object)) as GameObject;
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (checkIfTagExists("BakeCollider"))
        {
            if (GUILayout.Button("Create new collider"))
            {
                createNewCollider();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Check if tag \"BakeCollider\" and \"BakedBillboards\" is added", errorStyle);
        }

            if (textureSavePath != "" )
        {
            if (bakedMeshesFolder != null)
            {
                if (checkIfTagExists("BakeCollider"))
                {
                    if (GUILayout.Button("Bake"))
                    {
                        createCamera();
                        clearBakedBillboards();
                        gatherColliders();
                        DestroyImmediate(camera.transform.gameObject);
                    }
                }
                else
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Check if tag \"BakeCollider\" and \"BakedBillboards\" is added", errorStyle);
                }
            }
            else
            {
                EditorGUILayout.LabelField("Select mesh baked parent", errorStyle);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Select texture path", errorStyle);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (textureSavePath == "")
        {
            if (GUILayout.Button("Select texture save path"))
            {
                textureSavePath = EditorUtility.OpenFolderPanel("Path to save textures", "", "");

            }
        }
    }


    void createNewCollider()
    {
        if(collidersHolder == null)
        {
            collidersHolder = new GameObject();
            collidersHolder.name = "CollidersHolder";
        }
        GameObject newCollider = new GameObject();
        newCollider.AddComponent<BillboardCollider>();
        newCollider.transform.parent = collidersHolder.transform;
        newCollider.name = "collider_" + GameObject.FindGameObjectsWithTag("BakeCollider").Length;
        newCollider.tag = "BakeCollider";
        newCollider.AddComponent<BoxCollider>();
        Rigidbody rb = newCollider.AddComponent<Rigidbody>();
        rb.drag = 0f;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

    }


    bool checkIfTagExists(string tag)
    {
        try
        {
            if (GameObject.FindGameObjectsWithTag(tag).Length != 0)
            {
                return true;
            }
        }
        catch
        {

        }
          return false;
    }
    private void clearBakedBillboards()
    {

        GameObject[] bakedBillboard = GameObject.FindGameObjectsWithTag("BakedBillboards");

        foreach (GameObject billboard in bakedBillboard)
        {
            DestroyImmediate(billboard);
        }
    }
    private void gatherColliders()
    {
        if (collidersHolder == null)
        {
            collidersHolder = new GameObject();
            collidersHolder.name = "CollidersHolder";
        }
      
       

        billboardColliders = GameObject.FindGameObjectsWithTag("BakeCollider");

        foreach(GameObject collider in billboardColliders)
        {
            camera.cullingMask = 1 << bakingLayer;
            if (CustomCameraSettings)
            {
                camera.nearClipPlane = ClippingNear;
            }
            else
            {
                camera.nearClipPlane = 0.01f;
            }
            collider.transform.parent = collidersHolder.transform;
            BillboardMaker.ConvertToImage(collider, Resolution, CameraDistance, camera, bakedLayer, textureSavePath, bakedMeshesFolder, shaderSelected);
        }
    }

    private void createCamera()
    {
        if(camera == null)
        {
            GameObject RenderCamera = new GameObject();
            camera = RenderCamera.AddComponent<Camera>();
        }

        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.cullingMask = 1 << bakingLayer;
        camera.backgroundColor = cameraBackgroundColor;
        

    }
}
