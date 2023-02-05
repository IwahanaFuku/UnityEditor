using UnityEngine;
using UnityEditor;

public class Renamer : EditorWindow
{
    private bool renameToMaterialNameAccordion = true;
    private bool addObjectNumberAccordion = true;

    [MenuItem("Iwahana Tools/リネーマー", false, 100)]
    public static void ShowWindow()
    {
        GetWindow<Renamer>("リネーマー");
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

        addObjectNumberAccordion = EditorGUILayout.Foldout(addObjectNumberAccordion, "オブジェクトに番号を付与");
        if (addObjectNumberAccordion)
        {
            if (GUILayout.Button("実行"))
            {
                addObjectNumber();
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

    private void addObjectNumber()
    {
        Undo.RecordObjects(Selection.gameObjects, "Add Object Number");
        GameObject[] selectedObjects = Selection.gameObjects;
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            selectedObjects[i].name = selectedObjects[i].name + " " + "(" + (i + 1).ToString("") + ")";
        }
    }
}