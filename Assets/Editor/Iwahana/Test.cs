using UnityEngine;
using UnityEditor;

public class DistributeObjects : EditorWindow
{
    private int axisSelection = 0;

    [MenuItem("Window/Distribute Objects")]
    public static void ShowWindow()
    {
        GetWindow<DistributeObjects>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Distribute", EditorStyles.boldLabel);
        axisSelection = GUILayout.SelectionGrid(axisSelection, new string[] { "X Axis", "Y Axis", "Z Axis" }, 3);

        if (GUILayout.Button("Distribute"))
        {
            Distribute();
        }
    }

    private void Distribute()
    {
        if (Selection.gameObjects.Length < 2)
        {
            Debug.LogWarning("Please select at least 2 objects.");
            return;
        }

        Vector3[] positions = new Vector3[Selection.gameObjects.Length];
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            positions[i] = Selection.gameObjects[i].transform.position;
        }

        switch (axisSelection)
        {
            case 0:
                // sort by x axis
                System.Array.Sort(positions, (a, b) => a.x.CompareTo(b.x));
                break;
            case 1:
                // sort by y axis
                System.Array.Sort(positions, (a, b) => a.y.CompareTo(b.y));
                break;
            case 2:
                // sort by z axis
                System.Array.Sort(positions, (a, b) => a.z.CompareTo(b.z));
                break;
        }

        float interval = (positions[positions.Length - 1][axisSelection] - positions[0][axisSelection]) / (positions.Length - 1);
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 newPos = positions[i];
            newPos[axisSelection] = positions[0][axisSelection] + i * interval;
            Selection.gameObjects[i].transform.position = newPos;
        }
    }
}
