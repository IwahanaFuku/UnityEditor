using UnityEngine;
using UnityEditor;

public class PlaceObjectsAtIntervals : ScriptableWizard
{
    public float interval = 1.0f;
    public bool alignToX = true;
    public bool alignToY = false;
    public bool alignToZ = false;

    [MenuItem("Tools/Place Objects At Intervals")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<PlaceObjectsAtIntervals>("Place Objects At Intervals", "Place");
    }

    void OnWizardCreate()
    {
        Undo.RecordObjects(Selection.transforms, "Place Objects At Intervals");
        float position = 0.0f;
        foreach (Transform transform in Selection.transforms)
        {
            Vector3 newPosition = transform.position;
            if (alignToX)
                newPosition.x = position;
            if (alignToY)
                newPosition.y = position;
            if (alignToZ)
                newPosition.z = position;
            transform.position = newPosition;
            position += interval;
        }
    }
}
