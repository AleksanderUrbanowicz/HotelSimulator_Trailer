using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BillboardMaker
{

    public static void ConvertToImage(GameObject objectToRender, int resolution, float cameraDistance, Camera cam, LayerMask layerToSet, string pathToSave, GameObject parent, shaderSelection selectedShader)
    {
        int imageWidth;
        int imageHeight;
        Vector3 bb = objectToRender.GetComponent<BoxCollider>().size;
        float height = bb.y;
        float width = bb.x;
        LayerMask oldLayer = cam.cullingMask;
        if(objectToRender.GetComponent<BillboardCollider>().useCustomValues)
        {
            resolution = objectToRender.GetComponent<BillboardCollider>().resolution;
            cam.cullingMask = 1 << objectToRender.GetComponent<BillboardCollider>().renderLayer;
            cam.nearClipPlane = objectToRender.GetComponent<BillboardCollider>().cameraNearClipping;

        }

            if (height > width)
        {
            imageWidth = Mathf.RoundToInt(resolution * (width / height));
            imageHeight = resolution;
            cam.orthographicSize = (1.0f * Mathf.Max(bb.y / 2.0f, bb.x / 2.0f));
            Debug.Log("image Height: " + imageHeight + " image Width: " + imageWidth);
        }
        else
        {
            imageHeight = Mathf.RoundToInt(resolution * (height / width));
            imageWidth = resolution;
            cam.orthographicSize = (1.0f * Mathf.Min(bb.y / 2.0f, bb.x / 2.0f));
        }
        cam.orthographic = false;
        cam.fieldOfView = 60;
        float rw = imageWidth;
        float rh = imageHeight;
        cam.transform.position = objectToRender.transform.position;
        Vector3 cameraTempPos = objectToRender.transform.position + objectToRender.transform.forward * cameraDistance;
        cam.transform.position = cameraTempPos;
        
        cam.farClipPlane = 10.0f + cameraDistance + bb.z;
        cameraTempPos = new Vector3(cam.transform.position.x, cam.orthographicSize * 0.05f, cam.transform.position.y);
        cam.transform.LookAt(objectToRender.transform);
        Vector3 tempRot = cam.transform.eulerAngles;
        tempRot.z = -objectToRender.transform.eulerAngles.z;
        cam.transform.eulerAngles = tempRot;
        Texture2D tex = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 32, RenderTextureFormat.ARGB32);
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
        tex.alphaIsTransparency = true;
        tex.Apply();
        
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        string pathName = pathToSave.Replace(Application.dataPath, "Assets") + "/"+objectToRender.name + "_DA_TEX.png";
        System.IO.File.WriteAllBytes(pathName, bytes);
        AssetDatabase.Refresh();
        cam.cullingMask = 1 << oldLayer;
        /*
        GameObject Quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Quad.tag = "BakedBillboards";
        Quad.name = objectToRender.name + "_baked";
        Quad.transform.parent = parent.transform;
        Quad.transform.position = objectToRender.transform.position;
        Quad.transform.rotation = objectToRender.transform.rotation;
        Vector3 tempScale = objectToRender.GetComponent<BoxCollider>().size;
        tempScale.x *= -1;
        tempScale.z *= -1;
        Quad.transform.localScale = tempScale;
        Quad.layer = layerToSet;
        
        
        Texture2D texLoaded = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object)) as Texture2D;
        if (selectedShader.ToString() == "Standard")
        {
            Material mat = new Material(Shader.Find("Standard"));
            Quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.EnableKeyword("_EMISSION");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 2450;
            Quad.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_EmissionColor", Color.white);
            Quad.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_EmissionMap", texLoaded);
            Quad.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", texLoaded);
            
        }
        else if (selectedShader.ToString() == "Unlit")
        {
            Material mat = new Material(Shader.Find("Unlit/Transparent Cutout"));
            Quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
            Quad.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", texLoaded);
            
        }
        else
        {
            Material mat = new Material(Shader.Find("Unlit/Transparent Cutout"));
            Quad.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        */


    }
}
