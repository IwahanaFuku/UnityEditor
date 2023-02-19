/* using UnityEngine;
using UnityEditor;

public class SpecialDuplicateEditorWindow : EditorWindow
{
    [MenuItem("Iwahana Tools/特殊な複製", false, 100)]
    public static void ShowWindow()
    {
        GetWindow<SpecialDuplicateEditorWindow>("配置支援ツール");
    }

    private bool specialDuplicateAccordion = false;
    int copyCount = 5;
    int maxCopyCount = 100;
    private TransformData duplicateTransform = new TransformData(Vector3.zero, Quaternion.identity, Vector3.one);

    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;

        public TransformData(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
    }

    private void OnGUI()
    {    
        specialDuplicateAccordion = EditorGUILayout.Foldout(specialDuplicateAccordion, "特殊な複製");
        if (specialDuplicateAccordion)
        {
            duplicateTransform.position = EditorGUILayout.Vector3Field("Position", duplicateTransform.position);
            duplicateTransform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", 
                                          new Vector3(
                                              Mathf.Round(duplicateTransform.rotation.eulerAngles.x * 1000f) / 1000f,
                                              Mathf.Round(duplicateTransform.rotation.eulerAngles.y * 1000f) / 1000f,
                                              Mathf.Round(duplicateTransform.rotation.eulerAngles.z * 1000f) / 1000f
                                          )
                                          ));
            duplicateTransform.localScale = EditorGUILayout.Vector3Field("Scale", duplicateTransform.localScale);
            copyCount = EditorGUILayout.IntSlider("コピー数", copyCount, 0, maxCopyCount);
                if (GUILayout.Button("複製"))
                {
                    SpecialDuplicate(duplicateTransform, copyCount);
                }
        }
    }

    private void SpecialDuplicate(TransformData duplicateTransform, int copyCount)
    {
        Transform[] selectedTransforms = Selection.transforms;
        foreach (Transform selectedTransform in selectedTransforms)
        {
            TransformData newTransform = new TransformData(selectedTransform.position, selectedTransform.rotation, selectedTransform.localScale);
            if(copyCount < 2){break;}

            GameObject parentObject = new GameObject(selectedTransform.name + "_Duplicates");
            selectedTransform.SetParent(parentObject.transform, true);
            Undo.RegisterCreatedObjectUndo(parentObject, "Special Duplicate");

            for(int i = 1; i < copyCount; i++)
            {
                newTransform.position = newTransform.position + duplicateTransform.position;
                newTransform.rotation = newTransform.rotation * duplicateTransform.rotation;
                newTransform.localScale = Vector3.Scale(newTransform.localScale, duplicateTransform.localScale);

                GameObject newObject = Instantiate(selectedTransform.gameObject, newTransform.position, newTransform.rotation);
                newObject.transform.localScale = newTransform.localScale;
                newObject.name = selectedTransform.name;
                newObject.transform.parent = parentObject.transform;

                Undo.RegisterCreatedObjectUndo(newObject, "Special Duplicate");
            }
        }
    }

} */