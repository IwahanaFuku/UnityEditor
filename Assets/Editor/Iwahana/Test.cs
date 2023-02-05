using UnityEditor;
using UnityEngine;

public class RemoveNumberFromSelectedObjects : ScriptableObject
{
    [MenuItem("Tools/Remove Number from Selected Objects")]
    static void RemoveNumber()
    {
        foreach (Transform transform in Selection.transforms)
        {
            string[] newNames = transform.name.Split(' ');
            transform.name = string.Join(" ", newNames, 0, newNames.Length - 1);
        }
    }
}