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

    private int axisSelection = 0;

    private AlignType alignType = AlignType.Center;
    private Axis arrangerAxis = Axis.X;
    private bool parentCenterAlignerAccordion = true;
    private bool arrangerAccordion = false;
    private bool distributeAccordion = false;

    [MenuItem("Iwahana Tools/インスタンス整理ツール", false, 100)]
    public static void ShowWindow()
    {
        GetWindow<AlignObjectsEditorWindow>("インスタンス整理ツール");
    }

    private void OnGUI()
    {    
        parentCenterAlignerAccordion = EditorGUILayout.Foldout(parentCenterAlignerAccordion, "親を生成");
        if (parentCenterAlignerAccordion)
        {
            if (GUILayout.Button("生成"))
            {
                ParentCenterAligner();
            }
        }

        arrangerAccordion = EditorGUILayout.Foldout(arrangerAccordion, "整列");
        if (arrangerAccordion)
        {
            GUILayout.Label("基準値", EditorStyles.boldLabel);
            alignType = (AlignType)GUILayout.SelectionGrid((int)alignType, 
                new string[] { "最大", "中央", "最小" }, 3);

            GUILayout.Label("軸", EditorStyles.boldLabel);
            arrangerAxis = (Axis)GUILayout.SelectionGrid((int)arrangerAxis, 
                new string[] { "X軸", "Y軸", "Z軸" }, 3);

            if (GUILayout.Button("整列"))
            {
                AlignSelectedObjects();
            }
        }
        
        distributeAccordion = EditorGUILayout.Foldout(distributeAccordion, "分布");
        if (distributeAccordion)
        {
            GUILayout.Label("軸", EditorStyles.boldLabel);
            axisSelection = GUILayout.SelectionGrid(axisSelection, new string[] { "X軸", "Y軸", "Z軸" }, 3);

            if (GUILayout.Button("分布"))
            {
                Distribute();
            }
        }
    }

    private void AlignSelectedObjects()
    {
        Transform[] selectedTransforms = Selection.transforms;

        if(checkSelectedTransform(selectedTransforms)){return;}

        float[] values = new float[selectedTransforms.Length];

        for (int i = 0; i < selectedTransforms.Length; i++)
        {
            switch (arrangerAxis)
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

            switch (arrangerAxis)
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
    
    private void Distribute()
    {
        if(checkSelectedTransform(Selection.transforms)){return;}
        
        Vector3[] positions = new Vector3[Selection.gameObjects.Length];

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            positions[i] = Selection.gameObjects[i].transform.position;
        }

        switch (axisSelection)
        {
            case 0:
                System.Array.Sort(positions, (a, b) => a.x.CompareTo(b.x));
                break;
            case 1:
                System.Array.Sort(positions, (a, b) => a.y.CompareTo(b.y));
                break;
            case 2:
                System.Array.Sort(positions, (a, b) => a.z.CompareTo(b.z));
                break;
        }

        Transform[] selectedTransforms = Selection.transforms;
        Undo.RecordObjects(selectedTransforms, "Align Objects");

        float interval = (positions[positions.Length - 1][axisSelection] - positions[0][axisSelection]) / (positions.Length - 1);
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 newPos = positions[i];
            newPos[axisSelection] = positions[0][axisSelection] + i * interval;
            Selection.gameObjects[i].transform.position = newPos;
        }
        Undo.FlushUndoRecordObjects();
    }

    private void ParentCenterAligner()
    {
        if(checkSelectedTransform(Selection.transforms)){return;}

        Undo.RecordObjects(Selection.transforms, "Align Objects");
        Transform[] selectedTransforms = Selection.transforms;

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

    private bool checkSelectedTransform(Transform[] selectedTransforms)
    {
    if (selectedTransforms.Length < 2)
        {
            Debug.LogError("Please select at least two objects to align.");
            return true;
        }
        return false;
    } 
}
