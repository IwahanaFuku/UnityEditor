using UnityEngine;
using UnityEditor;

public class Renamer : EditorWindow
{
    private bool renameToMaterialNameAccordion = true;

    [MenuItem("Iwahana Tools/リネーマー")]
    public static void ShowWindow()
    {
        GetWindow<Renamer>("Renamer");
    }

    void OnGUI()
    {
        renameToMaterialNameAccordion = EditorGUILayout.Foldout(renameToMaterialNameAccordion, "マテリアルの名前でリネーム");
        if (renameToMaterialNameAccordion)
        {
            if (GUILayout.Button("実行"))
            {
                renameToMaterialName();
            }
        }
    }

    private void renameToMaterialName()
    {
        Undo.RecordObjects(Selection.gameObjects, "Change Material Name");
        GameObject[] selectedObjects = Selection.gameObjects;
        foreach (GameObject obj in selectedObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                continue;
            }

            Material mat = renderer.sharedMaterial;
            if (mat == null)
            {
                continue;
            }

            obj.name = mat.name;
        }
    }
}