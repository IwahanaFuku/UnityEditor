using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

//子オブジェクトを持たない場合はそのオブジェクトをいい感じに180 or 360度回す
//子オブジェクトを持つ場合はそのオブジェクトをいい感じに180 or 360度回す

public class BoundingBoxCube : EditorWindow
{
    [MenuItem("Tools/BoundingBoxCube")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<BoundingBoxCube>("BoundingBoxCube");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create Cube"))
        {
            RotationRound();
        }
    }

    private void RotationRound()
    {
        GameObject selectedObject = Selection.activeObject as GameObject;
        var boundsObjectsList = new List<GameObject>();

        if(selectedObject.transform.childCount == 0)
        {
            boundsObjectsList.Add(selectedObject);
        }
        else
        {
            Transform transform = selectedObject.transform;
            foreach (Transform childTransform in transform)
            {
                boundsObjectsList.Add(childTransform.gameObject);
            }
        }

        GameObject[] boundsObjects = boundsObjectsList.ToArray();
        Bounds bounds = new Bounds();
        bool hasBounds = false;

        foreach (GameObject boundsObject in boundsObjects)
        {
            Renderer renderer = boundsObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (! hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }

        Vector3 absCenter = new Vector3(Mathf.Abs(bounds.center.x), Mathf.Abs(bounds.center.y), Mathf.Abs(bounds.center.z));
        Vector3 edgeCenterX = new Vector3(absCenter.x + bounds.size.x / 2, absCenter.y, absCenter.z);
        Vector3 edgeCenterZ = new Vector3(absCenter.x, absCenter.y, absCenter.z + bounds.size.z / 2);
        Debug.Log(absCenter);

        float theta;
        Vector3 rotationAxis = Vector3.up;
        int numofCopy;
        float arc = 360f;

        if(edgeCenterZ.sqrMagnitude < edgeCenterX.sqrMagnitude)
        {
            theta = Mathf.Atan2(bounds.size.z / 2, edgeCenterX.x) * Mathf.Rad2Deg * 2;
            numofCopy= Convert.ToInt32(arc / theta);
            theta = Convert.ToInt32(360 / numofCopy);
        }else
        {
            theta = Mathf.Atan2(bounds.size.x / 2, edgeCenterZ.z) * Mathf.Rad2Deg * 2;
            numofCopy= Convert.ToInt32(arc / theta);
            theta = Convert.ToInt32(360 / numofCopy);
        }
        
        for(int i = 0; i < numofCopy + 1; i++)
        {
            
            Quaternion rotatinQuaternion =  Quaternion.AngleAxis(theta * i, rotationAxis);
            GameObject newObject = Instantiate(selectedObject, selectedObject.transform.position, selectedObject.transform.rotation * rotatinQuaternion);
            newObject.name = selectedObject.name;
        }
        
    }
}
