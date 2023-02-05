using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class Renamer : EditorWindow
{
    private bool renameToMaterialNameAccordion = true;
    private bool addObjectNumberAccordion = true;
    private bool removeObjectNumberAccordion = true;

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
                RenameToMaterialName();
            }
        }

        addObjectNumberAccordion = EditorGUILayout.Foldout(addObjectNumberAccordion, "オブジェクトに番号を付与");
        if (addObjectNumberAccordion)
        {
            if (GUILayout.Button("実行"))
            {
                AddObjectNumber();
            }
        }

        removeObjectNumberAccordion = EditorGUILayout.Foldout(removeObjectNumberAccordion, "オブジェクトから番号を除去");
        if (removeObjectNumberAccordion)
        {
            if (GUILayout.Button("実行"))
            {
                RemoveObjectNumber();
            }
        }
    }

    private void RenameToMaterialName()
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

    private void AddObjectNumber()
    {
        Undo.RecordObjects(Selection.gameObjects, "Add Object Number");
        GameObject[] selectedObjects = Selection.gameObjects;
        System.Array.Sort(selectedObjects, (x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
        Selection.objects = selectedObjects;
        Dictionary<string, int> names = new Dictionary<string, int>();
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            string newName;
            if (names.ContainsKey(selectedObjects[i].name))
            {
                newName = selectedObjects[i].name + " " + "(" + (names[selectedObjects[i].name] + 1).ToString() + ")";
                names[selectedObjects[i].name]++;
            }
            else
            {
                newName = selectedObjects[i].name + " " + "(" + (1).ToString() + ")";
                names[selectedObjects[i].name] = 1;
            }
            selectedObjects[i].name = newName;
        }
    }

    private void RemoveObjectNumber()
    {
        Undo.RecordObjects(Selection.gameObjects, "Remove Object Number");
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Deep | SelectionMode.Editable);
        foreach (Transform transform in transforms)
        {
            string name = transform.name;
            string newName = Regex.Replace(name, @"\s*\(\d+\)$", "");
            transform.name = newName;
        }
    }
}