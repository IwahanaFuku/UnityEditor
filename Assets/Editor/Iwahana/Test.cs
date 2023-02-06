using UnityEditor;
using UnityEngine;

public class ScaleRandomizerEditor : EditorWindow
{
    private static float minimum = 0.5f;
    private static float maximum = 1.5f;
    private float scaleRatio = 1f;
    private bool isDetail = false;
    Vector3 minScale = new Vector3(minimum, minimum, minimum);
    Vector3 maxScale = new Vector3(maximum, maximum, maximum);
    

    [MenuItem("Tools/Scale Randomizer")]
    public static void ShowWindow()
    {
        GetWindow<ScaleRandomizerEditor>("Scale Randomizer");
    }

    private void OnGUI()
    {
           
        scaleRatio = EditorGUILayout.FloatField("Scale Ratio", scaleRatio);

        using (new EditorGUI.DisabledGroupScope(isDetail))
        {
            minimum = EditorGUILayout.FloatField("Minimum", minimum);
            maximum = EditorGUILayout.FloatField("Maximum", maximum);
        }

        isDetail = EditorGUILayout.Toggle("Detail", isDetail);

        if(isDetail == false)
        {
            if (GUILayout.Button("Randomize Scale"))
            {
                Undo.RecordObjects(Selection.transforms, "Random Scale Objects");
                RandomScaleSelectedObjects();
            }
        }
        else
        {            
            minScale = EditorGUILayout.Vector3Field("Minimum Scale", minScale);
            maxScale = EditorGUILayout.Vector3Field("Maximum Scale", maxScale);
            if (GUILayout.Button("Randomize Scale"))
            {
                Undo.RecordObjects(Selection.transforms, "Random Detail Scale Objects");                   
                RandomDetailScaleSelectedObjects();
            }
        }
          
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
}
