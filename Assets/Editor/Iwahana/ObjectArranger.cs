using UnityEngine;
using UnityEditor;
using System.Linq;

public class AlignObjectsEditorWindow : EditorWindow
{
    private enum AlignType
    {
        Maximum,
        Center,
        Minimum
    }

    private enum Axis
    {
        X,
        Y,
        Z
    }

    private AlignType alignType = AlignType.Center;
    private Axis axis = Axis.X;

    [MenuItem("Window/Align Objects")]
    public static void ShowWindow()
    {
        GetWindow<AlignObjectsEditorWindow>("Align Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Align Type", EditorStyles.boldLabel);
        alignType = (AlignType)GUILayout.SelectionGrid((int)alignType, 
            new string[] { "Maximum", "Center", "Minimum" }, 3);

        GUILayout.Label("Axis", EditorStyles.boldLabel);
        axis = (Axis)GUILayout.SelectionGrid((int)axis, 
            new string[] { "X", "Y", "Z" }, 3);

        if (GUILayout.Button("Align"))
        {
            AlignSelectedObjects();
        }
    }

    private void AlignSelectedObjects()
    {
        Transform[] selectedTransforms = Selection.transforms;

        if (selectedTransforms.Length < 2)
        {
            Debug.LogError("Please select at least two objects to align.");
            return;
        }

        float[] values = new float[selectedTransforms.Length];

        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            switch (axis)
            {
                case Axis.X:
                    values[i] = selectedTransforms[i].position.x;
                    break;
                case Axis.Y:
                    values[i] = selectedTransforms[i].position.y;
                    break;
                case Axis.Z:
                    values[i] = selectedTransforms[i].position.z;
                    break;
            }
        }

        float targetValue = 0f;

        switch (alignType)
        {
            case AlignType.Maximum:
                targetValue = Mathf.Max(values);
                break;
            case AlignType.Center:
                targetValue = values.Average();
                break;
            case AlignType.Minimum:
                targetValue = Mathf.Min(values);
                break;
        }

        Undo.RecordObjects(selectedTransforms, "Align Objects");

        foreach (Transform t in selectedTransforms)
        {
            Vector3 pos = t.position;

            switch (axis)
            {
                case Axis.X:
                    pos.x = targetValue;
                    break;
                case Axis.Y:
                    pos.y = targetValue;
                    break;
                case Axis.Z:
                    pos.z = targetValue;
                    break;
            }

            t.position = pos;
        }

        Undo.FlushUndoRecordObjects();
    }
}
