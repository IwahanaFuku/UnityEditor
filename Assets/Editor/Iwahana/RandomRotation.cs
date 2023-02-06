using UnityEngine;
using UnityEditor;

public class RandomRotator : EditorWindow
{
    private float rotationAmount = 90f;
    private bool rotateX = false;
    private bool rotateY = true;
    private bool rotateZ = false;

    [MenuItem("Window/Random Rotator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RandomRotator));
    }

    private void OnGUI()
    {
        rotationAmount = EditorGUILayout.Slider("Rotation Amount:", rotationAmount, 1f, 360f);
        rotateX = EditorGUILayout.Toggle("Rotate X", rotateX);
        rotateY = EditorGUILayout.Toggle("Rotate Y", rotateY);
        rotateZ = EditorGUILayout.Toggle("Rotate Z", rotateZ);

        if (GUILayout.Button("Rotate Selected Object"))
        {
            RotateSelectedObject();
        }
    }

    private void RotateSelectedObject()
    {
        Undo.RecordObjects(Selection.transforms, "Rotate Objects");
        GameObject selectedObject = Selection.activeGameObject;
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
                Debug.Log("Result: " + yRotation);
            }

            if (rotateZ)
            {
                zRotation = Mathf.Floor(Random.Range(0, 360) / rotationAmount) * rotationAmount;
            }

            selectedObject.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }
    }
}
