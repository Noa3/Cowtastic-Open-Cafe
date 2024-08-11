#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MilkTypeController))]
public class MilkTypeControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MilkTypeController controller = (MilkTypeController)target;
        if (GUILayout.Button("Find Buttons"))
        {
            controller.FindButtons();
        }
    }
}
#endif
