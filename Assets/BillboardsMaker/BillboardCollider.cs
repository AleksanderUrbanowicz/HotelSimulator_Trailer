using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BillboardCollider : MonoBehaviour
{
    public bool useCustomValues = false;
    public int resolution = 1024;
    public LayerMask renderLayer = 0;
    public float cameraNearClipping = 0.01f;
}
[CustomEditor(typeof(BillboardCollider))]
public class BillboardCollisionEditor : Editor
{
     public override void OnInspectorGUI()
    {
        var BillboardCol = target as BillboardCollider;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        BillboardCol.useCustomValues = GUILayout.Toggle(BillboardCol.useCustomValues, "Use custom values?");

        if (BillboardCol.useCustomValues)
        {
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Resolution:");
            BillboardCol.resolution = EditorGUILayout.IntField(BillboardCol.resolution);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Render layer:");
            BillboardCol.renderLayer = EditorGUILayout.LayerField(BillboardCol.renderLayer);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Near clipping:");
            BillboardCol.cameraNearClipping = EditorGUILayout.FloatField(BillboardCol.cameraNearClipping);
            EditorGUILayout.EndHorizontal();

        }
    }
}