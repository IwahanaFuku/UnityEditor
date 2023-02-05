using UnityEngine;
using UnityEditor;

public class CenterAligner : EditorWindow
{
    [MenuItem("Tools/Center Aligner")]
    static void Init()
    {
        CenterAligner window = (CenterAligner)EditorWindow.GetWindow(typeof(CenterAligner));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Align"))
        {
            Undo.RecordObjects(Selection.transforms, "Align Objects");

            Transform[] selectedTransforms = Selection.transforms;
            if (checkSelectedTransform(selectedTransforms))
            {
                Vector3 center = Vector3.zero;
                foreach (Transform transform in selectedTransforms)
                {
                    center += transform.position;
                }
                center /= selectedTransforms.Length;

                GameObject parent = new GameObject("Parent");
                Undo.RegisterCreatedObjectUndo(parent, "Create Parent");
                parent.transform.position = center;

                foreach (Transform transform in selectedTransforms)
                {
                    Undo.SetTransformParent(transform, parent.transform, "Set Parent");
                }
            }
        }
    }

    private bool checkSelectedTransform(Transform[] selectedTransforms)
    {
        if (selectedTransforms.Length < 2)
        {
            Debug.LogError("Please select at least two objects to align.");
            return false;
        }

        return true;
    }
}