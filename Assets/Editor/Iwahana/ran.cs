using UnityEngine;
using UnityEditor;

public class RandomizeScaleEditor : EditorWindow
{
bool maintainRatio = false;
Vector3 minScale = Vector3.one;
Vector3 maxScale = Vector3.one;
bool showDetail = false;

[MenuItem("Window/Randomize Scale")]
static void Init()
{
    RandomizeScaleEditor window = (RandomizeScaleEditor)EditorWindow.GetWindow(typeof(RandomizeScaleEditor));
    window.Show();
}

void OnGUI()
{
    showDetail = EditorGUILayout.Toggle("Show Detail", showDetail);
    if (showDetail)
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
}
}