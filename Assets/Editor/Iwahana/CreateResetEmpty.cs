using UnityEditor;
using UnityEngine;

public class CreateTransformResetObject : Editor
{
    [MenuItem("GameObject/Create Reset Empty", false, -1)]
    private static void CreateTransformReset(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("GameObject");
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        Selection.activeGameObject = go;

        Undo.RegisterCreatedObjectUndo(go, "Create Transform Reset Object");
    }
}