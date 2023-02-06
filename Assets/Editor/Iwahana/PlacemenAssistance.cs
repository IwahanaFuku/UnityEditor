using UnityEngine;
using UnityEditor;
using System.Linq;

public class PlacemenAssistanceEditorWindow : EditorWindow
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

    private float rotationAmount = 1f;
    private bool rotateX = false;
    private bool rotateY = true;
    private bool rotateZ = false;

    private static float minimum = 0.5f;
    private static float maximum = 1.5f;
    private float scaleRatio = 1f;
    private bool isDetail = false;
    Vector3 minScale = new Vector3(minimum, minimum, minimum);
    Vector3 maxScale = new Vector3(maximum, maximum, maximum);

    private AlignType alignType = AlignType.Center;
    private Axis arrangerAxis = Axis.X;
    private bool parentCenterAlignerAccordion = false;
    private bool arrangerAccordion = false;
    private bool distributeAccordion = false;
    private bool randomRotationAccordion = false;
    private bool randomScalenAccordion = false;

    [MenuItem("Iwahana Tools/配置支援ツール", false, 100)]
    public static void ShowWindow()
    {
        GetWindow<PlacemenAssistanceEditorWindow>("配置支援ツール");
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
                DistributeObjects();
            }
        }
        
        randomScalenAccordion = EditorGUILayout.Foldout(randomScalenAccordion, "ランダムスケール");
        if (randomScalenAccordion)
        {
            scaleRatio = EditorGUILayout.FloatField("オフセット", scaleRatio);

            using (new EditorGUI.DisabledGroupScope(isDetail))
            {
                minimum = EditorGUILayout.FloatField("最小", minimum);
                maximum = EditorGUILayout.FloatField("最大", maximum);
            }

            isDetail = EditorGUILayout.Toggle("詳細", isDetail);

            if(isDetail == false)
            {
                if (GUILayout.Button("スケール"))
                {
                    Undo.RecordObjects(Selection.transforms, "Random Scale Objects");
                    RandomScaleSelectedObjects();
                }
            }
            else
            {            
                minScale = EditorGUILayout.Vector3Field("最小", minScale);
                maxScale = EditorGUILayout.Vector3Field("最大", maxScale);
                if (GUILayout.Button("スケール"))
                {
                    Undo.RecordObjects(Selection.transforms, "Random Detail Scale Objects");                   
                    RandomDetailScaleSelectedObjects();
                }
            }
        }

        randomRotationAccordion = EditorGUILayout.Foldout(randomRotationAccordion, "ランダム回転");
        if (randomRotationAccordion)
        {
            rotationAmount = EditorGUILayout.Slider("閾値:", rotationAmount, 1f, 360f);
            rotateX = EditorGUILayout.Toggle("X軸", rotateX);
            rotateY = EditorGUILayout.Toggle("Y軸", rotateY);
            rotateZ = EditorGUILayout.Toggle("Z軸", rotateZ);

            if (GUILayout.Button("回転"))
            {
                RandomRotateSelectedObjects();
            }
        }
    }

    //================================================================================
    /// <summary>
    /// 選択オブジェクトのTransformを四捨五入する
    /// </summary>
    [MenuItem ("Iwahana Tools/カスタム ショートカット/グループを生成 %g")]
    private static void Log2 () {
        ParentCenterAligner();
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
    
    private void DistributeObjects()
    {
        if(checkSelectedTransform(Selection.transforms)){return;}

        int[] originalIndices = new int[Selection.gameObjects.Length];
        Vector3[] positions = new Vector3[Selection.gameObjects.Length];

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            originalIndices[i] = i;
            positions[i] = Selection.gameObjects[i].transform.position;
        }

        switch (axisSelection)
        {
            case 0:
                System.Array.Sort(originalIndices, (a, b) => positions[a].x.CompareTo(positions[b].x));
                break;
            case 1:
                System.Array.Sort(originalIndices, (a, b) => positions[a].y.CompareTo(positions[b].y));
                break;
            case 2:
                System.Array.Sort(originalIndices, (a, b) => positions[a].z.CompareTo(positions[b].z));
                break;
        }

        Undo.RecordObjects(Selection.transforms, "Align Objects");

        float interval = (positions[positions.Length - 1][axisSelection] - positions[0][axisSelection]) / (positions.Length - 1);
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 newPos = positions[originalIndices[i]];
            newPos[axisSelection] = positions[0][axisSelection] + i * interval;
            Selection.gameObjects[originalIndices[i]].transform.position = newPos;
        }
        Undo.FlushUndoRecordObjects();
    }


    private static void ParentCenterAligner()
    {
        if(checkSelectedTransform(Selection.transforms)){return;}

        Undo.RecordObjects(Selection.transforms, "Align Group Objects");
        Transform[] selectedTransforms = Selection.transforms;

        Vector3 center = Vector3.zero;
        foreach (Transform transform in selectedTransforms)
        {
            center += transform.position;
        }
        center /= selectedTransforms.Length;

        GameObject group = new GameObject("Group");
        Undo.RegisterCreatedObjectUndo(group, "Create Group");
        group.transform.position = center;

        foreach (Transform transform in selectedTransforms)
        {
            Undo.SetTransformParent(transform, group.transform, "Set Group");
        }

        Selection.activeGameObject = group;
    }

    private void RandomScaleSelectedObjects()
    {
        Undo.RecordObjects(Selection.objects, "Randomize Scale");
        foreach (var selectedObject in Selection.objects)
        {
            var transform = (selectedObject as GameObject).transform;
            var newscale = 1f;

            if(isDetail == false)
            {
                newscale = newscale * Random.Range(minimum, maximum);
            }

            var scale = scaleRatio * newscale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    private void RandomDetailScaleSelectedObjects()
    {
        foreach (Transform transform in Selection.transforms)
        {
            Vector3 scale = transform.localScale;
            scale.x = scaleRatio * Random.Range(Mathf.Min(minScale.x, maxScale.x), Mathf.Max(minScale.x, maxScale.x));
            scale.y = scaleRatio * Random.Range(Mathf.Min(minScale.y, maxScale.y), Mathf.Max(minScale.y, maxScale.y));
            scale.z = scaleRatio * Random.Range(Mathf.Min(minScale.z, maxScale.z), Mathf.Max(minScale.z, maxScale.z));

            transform.localScale = scale;
        }
    }

    private void RandomRotateSelectedObjects()
    {
        Undo.RecordObjects(Selection.transforms, "Random Rotate Objects");

        foreach (GameObject selectedObject in Selection.gameObjects)
        {
            if (selectedObject != null)
            {
                float xRotation = 0f;
                float yRotation = 0f;
                float zRotation = 0f;

                if (rotateX)
                {
                    xRotation = Mathf.Floor(Random.Range(0, 360) / rotationAmount) * rotationAmount;
                }

                if (rotateY)
                {
                    yRotation = Mathf.Floor(Random.Range(0, 360) / rotationAmount) * rotationAmount;
                }

                if (rotateZ)
                {
                    zRotation = Mathf.Floor(Random.Range(0, 360) / rotationAmount) * rotationAmount;
                }

                selectedObject.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            }
        }
    }


    private static bool checkSelectedTransform(Transform[] selectedTransforms)
    {
    if (selectedTransforms.Length < 2)
        {
            Debug.LogError("Please select at least two objects to align.");
            return true;
        }
        return false;
    } 
}
