using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class ColliderSelectionManager : MonoBehaviour
{
    

    public GameObject objectToRender;
    public int imageWidth = 128;
    public int imageHeight = 128;
    public int size =1024;
    public Material quadMat;
    public float cameraDistance;
    public void Start()
    {
        takeSS();
    }
    public void takeSS()
    {
        if (objectToRender)
            StartCoroutine(ConvertToImage());
    }
    
    IEnumerator ConvertToImage()
    {
        
        Vector3 bb = objectToRender.GetComponent<BoxCollider>().size;
        
        Camera cam = Camera.main;
        float height = bb.y;//bb.max.y - bb.min.y;
        float width = bb.x;//bb.max.x - bb.min.x;

        if (height > width)
        {
            imageWidth = Mathf.RoundToInt(size * (width / height)) ;
            imageHeight = size;
            cam.orthographicSize = (1.0f * Mathf.Max(bb.y / 2.0f, bb.x / 2.0f));
            //imageWidth = size;
            //cam.rect = new Rect(0, 0, width / height, 1);
            Debug.Log("image Height: " + imageHeight + " image Width: " + imageWidth);
        }
        else
        {
            imageHeight = Mathf.RoundToInt(size * (height / width));
            //imageHeight = size;
            imageWidth = size;
            cam.orthographicSize = (1.0f * Mathf.Min(bb.y / 2.0f, bb.x / 2.0f));
            Debug.Log("image Height: " + imageHeight + " image Width: " + imageWidth);
            //cam.rect = new Rect(0, 0, 1 , height / width);
        }
        
        Debug.Log("bb Height: " + height + " bb Width: " + width);
        //grab the main camera and mess with it for rendering the object - make sure orthographic
        
        cam.orthographic = true;
        //render to screen rect area equal to out image size
        float rw = imageWidth;
        //rw /= Screen.width;
        float rh = imageHeight;
        //rh /= Screen.height;
        //cam.rect = new Rect(0, 0, rw, rh);
        Debug.Log("rw: " + rw + " rh: " + rh);
        //grab size of object to render - place/size camera to fit
        //Bounds bb = objectToRender.GetComponent<Renderer>().bounds;
       

        //place camera looking at centre of object - and backwards down the z-axis from it
        cam.transform.position = objectToRender.transform.position;
        Vector3 cameraTempPos = objectToRender.transform.position + objectToRender.transform.forward * cameraDistance;
        
        
        //Vector3 cameraTempPos = new Vector3(cam.transform.position.x, cam.transform.position.y, -10.0f + (bb.min.z * 2.0f));
        cam.transform.position = cameraTempPos;
        //make clip planes fairly optimal and enclose whole mesh
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane =  10.0f + cameraDistance+ bb.z;
        //set camera size to just cover entire mesh
      
        cameraTempPos = new Vector3(cam.transform.position.x, cam.orthographicSize * 0.05f, cam.transform.position.y);
        //cam.transform.position = cameraTempPos;
        cam.transform.LookAt(objectToRender.transform);
        //render
        yield return new WaitForEndOfFrame();
        Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 32,RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        
        cam.Render();
        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        tex.Apply();

        //turn all pixels == background-color to transparent
        Color bCol = cam.backgroundColor;
        Color alpha = bCol;
        alpha.a = 0.0f;
        for (int y = 0; y < imageHeight; y++)
        {
            for (int x = 0; x < imageWidth; x++)
            {
                Color c = tex.GetPixel(x, y);
                if (c.r == bCol.r)
                    tex.SetPixel(x, y, alpha);
            }
        }
        tex.Apply();
        tex.alphaIsTransparency = true;
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        System.IO.File.WriteAllBytes(Application.dataPath + "/billboard.png", bytes);
        AssetDatabase.Refresh();
        GameObject Quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Quad.transform.position = objectToRender.transform.position;
        Quad.transform.rotation = objectToRender.transform.rotation;
        Quad.transform.localScale = objectToRender.GetComponent<BoxCollider>().size;
        Vector3 tempRot = Quad.transform.eulerAngles;
        tempRot.y += 180;
        Quad.transform.eulerAngles = tempRot;
        Quad.GetComponent<MeshRenderer>().material = quadMat;
        Quad.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath("Assets/billboard.png",typeof(Object)) as Texture2D);

    }


}
