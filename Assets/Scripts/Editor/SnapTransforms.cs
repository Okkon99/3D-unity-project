using UnityEngine;
using UnityEditor;

public class SnapTransforms : EditorWindow
{
    float positionSnap = 0.01f;
    float rotationSnap = 90f;
    float scaleSnap = 0.01f;

    [MenuItem("Tools/Snap Transforms")]
    static void Open()
    {
        GetWindow<SnapTransforms>("Snap Transforms");
    }

    void OnGUI()
    {
        positionSnap = EditorGUILayout.FloatField("Position Snap", positionSnap);
        rotationSnap = EditorGUILayout.FloatField("Rotation Snap", rotationSnap);
        scaleSnap = EditorGUILayout.FloatField("Scale Snap", scaleSnap);

        if (GUILayout.Button("Snap Selected"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj.transform, "Snap Transform");

                obj.transform.position = SnapVector(obj.transform.position, positionSnap);
                obj.transform.eulerAngles = SnapVector(obj.transform.eulerAngles, rotationSnap);
                obj.transform.localScale = SnapVector(obj.transform.localScale, scaleSnap);
            }
        }
    }

    Vector3 SnapVector(Vector3 v, float snap)
    {
        if (snap <= 0f) return v;

        return new Vector3(
            Mathf.Round(v.x / snap) * snap,
            Mathf.Round(v.y / snap) * snap,
            Mathf.Round(v.z / snap) * snap
        );
    }
}
