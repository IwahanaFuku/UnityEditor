/* using UnityEngine;
using UnityEditor;

public class RandomizeScaleEditor : EditorWindow
{
    bool maintainRatio = false;
    Vector3 minScale = Vector3.one;
    Vector3 maxScale = Vector3.one;

    [MenuItem("Tools/Randomize Scale")]
    static void Init()
    {
        RandomizeScaleEditor window = (RandomizeScaleEditor)EditorWindow.GetWindow(typeof(RandomizeScaleEditor));
        window.Show();
    }

    void OnGUI()
    {
        maintainRatio = EditorGUILayout.Toggle("Maintain Ratio", maintainRatio);
        minScale = EditorGUILayout.Vector3Field("Minimum Scale", minScale);
        maxScale = EditorGUILayout.Vector3Field("Maximum Scale", maxScale);

        if (GUILayout.Button("Randomize Selected Scales"))
        {
            Undo.RecordObjects(Selection.transforms, "Randomize Scale");

            foreach (Transform transform in Selection.transforms)
            {
                Vector3 scale = transform.localScale;
                if (maintainRatio)
                {
                    float randomValue = Random.Range(Mathf.Min(minScale.x, maxScale.x), Mathf.Max(minScale.x, maxScale.x));
                    scale = new Vector3(randomValue, randomValue, randomValue);
                }
                else
                {
                    scale.x = Random.Range(Mathf.Min(minScale.x, maxScale.x), Mathf.Max(minScale.x, maxScale.x));
                    scale.y = Random.Range(Mathf.Min(minScale.y, maxScale.y), Mathf.Max(minScale.y, maxScale.y));
                    scale.z = Random.Range(Mathf.Min(minScale.z, maxScale.z), Mathf.Max(minScale.z, maxScale.z));
                }

                transform.localScale = scale;
            }
        }
    }
} */


using UnityEditor;
using UnityEngine;

public class RandomizeScaleEditor : EditorWindow {
    private Vector2 scaleRange = new Vector2(1.0f, 2.0f);
    private Vector3 minScale = Vector3.one;
    private Vector3 maxScale = Vector3.one;

    [MenuItem("Window/Randomize Scale Editor")]
    static void ShowWindow() {
        GetWindow<RandomizeScaleEditor>().Show();
    }

    private void OnGUI() {
        scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);
        

        if (GUILayout.Button("Randomize Scale")) {
            RandomizeScale();
        }

        minScale = EditorGUILayout.Vector3Field("Minimum Scale", minScale);
        maxScale = EditorGUILayout.Vector3Field("Maximum Scale", maxScale);
        if (GUILayout.Button("Ditail Randomize Scale")) {
            DetailRandmizeScale();
        }
    }

    private void RandomizeScale() {
        
        Transform[] selectedTransforms = Selection.transforms;
        foreach (Transform selectedTransform in selectedTransforms) {
            Vector3 scale = selectedTransform.localScale;
            float randomValue = Random.Range(scaleRange.x, scaleRange.y);
            selectedTransform.localScale = new Vector3(randomValue, randomValue, randomValue);
        }
    }

    private void DetailRandmizeScale()
    {
        Undo.RecordObjects(Selection.transforms, "Detail Randomize Scale");
        foreach (Transform transform in Selection.transforms)
        {
            Vector3 scale = transform.localScale;
            scale.x = Random.Range(Mathf.Min(minScale.x, maxScale.x), Mathf.Max(minScale.x, maxScale.x));
            scale.y = Random.Range(Mathf.Min(minScale.y, maxScale.y), Mathf.Max(minScale.y, maxScale.y));
            scale.z = Random.Range(Mathf.Min(minScale.z, maxScale.z), Mathf.Max(minScale.z, maxScale.z));

            transform.localScale = scale;
        }
    }
}


