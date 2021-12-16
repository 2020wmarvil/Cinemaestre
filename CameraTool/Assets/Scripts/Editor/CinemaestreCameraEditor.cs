using UnityEditor;
using Cinemaestre;
using UnityEngine;

[CustomEditor(typeof(CinemaestreCamera))]
public class CinemaestreCameraEditor : Editor {
    CinemaestreCamera cam;

    SerializedProperty OnStart, OnLoop, OnComplete;

    void OnEnable() {
        cam = (CinemaestreCamera)target;
        OnStart = serializedObject.FindProperty("OnStart");
        OnLoop = serializedObject.FindProperty("OnLoop");
        OnComplete = serializedObject.FindProperty("OnComplete");
    }

    public override void OnInspectorGUI() {
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 20;
        myStyle.richText = true;

        EditorGUILayout.LabelField("<b><color=white>Cinemaestre Camera Tool</color></b>", myStyle); 

        EditorGUILayout.Space();
        GuiLine(2);
        EditorGUILayout.Space();

        myStyle.fontSize = 14;
        EditorGUILayout.LabelField("<color=white>General Properties</color>", myStyle); 
        cam.autoplay = EditorGUILayout.Toggle(new GUIContent("Autoplay", "Play the camera effect automatically on start"), cam.autoplay);

        EditorGUILayout.Space();
        GuiLine(2);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("<color=white>Effects</color>", myStyle); 
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"), GUILayout.ExpandHeight(true));
		serializedObject.ApplyModifiedProperties();

		serializedObject.Update();
        EditorGUILayout.Space();
        GuiLine(2);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<color=white>Events</color>", myStyle); 
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(OnStart);
        EditorGUILayout.PropertyField(OnLoop);
        EditorGUILayout.PropertyField(OnComplete);
		serializedObject.ApplyModifiedProperties();
    }

    void GuiLine(int i_height = 1) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color (0.5f, 0.5f, 0.5f, 1));
    }
}
