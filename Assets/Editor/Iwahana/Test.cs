using UnityEngine;
using UnityEditor;

public static class GetSelectedObjectIndex
{
    [MenuItem("Tools/Get Selected Object Index")]
    static void GetSelectedObjectHierarchyIndex()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.Log("No object selected.");
            return;
        }

        Transform parent = selectedObject.transform.parent;
        int index = 0;

        if (parent != null)
        {
            index = parent.GetSiblingIndex();
            Debug.Log("Selected object index: " + index);
        }
        else
        {
            Debug.Log("Selected object is a root object.");
        }
    }
}
