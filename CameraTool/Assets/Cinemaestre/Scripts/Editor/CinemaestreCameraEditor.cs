using UnityEditor;
using UnityEngine;
using Cinemaestre;
using System.Collections.Generic;

[CustomEditor(typeof(CinemaestreCamera))]
public class CinemaestreCameraEditor : Editor {
    CinemaestreCamera cam;
    SerializedObject GetTarget;
    SerializedProperty stackList;

    List<bool> showStackList = new List<bool>();
    List<bool> showStackEventList = new List<bool>();
 
    void OnEnable(){
        cam = (CinemaestreCamera)target;
        GetTarget = new SerializedObject(cam);
        stackList = GetTarget.FindProperty("stacks");

        showStackList = new List<bool>();
        showStackEventList = new List<bool>();
        for (int i=0; i<stackList.arraySize; i++) {
            showStackList.Add(false);
            showStackEventList.Add(false);
		}
    }

    public override void OnInspectorGUI(){
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 20;
        myStyle.richText = true;

        EditorGUILayout.LabelField("<b><color=white>Cinemaestre Camera Tool</color></b>", myStyle); 

        EditorGUILayout.Space();
        Line(Color.gray, 2);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("<color=white>Effect Stacks</color>", myStyle); 

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        GetTarget.Update();

        if (GUILayout.Button("Add Stack")) {
            cam.stacks.Add(new CinemaestreStack());
            showStackList.Add(false);
            showStackEventList.Add(false);
        }
        
        for(int i = 0; i < stackList.arraySize; i++) {
            showStackList[i] = EditorGUILayout.Foldout(showStackList[i] , "Stack " + i);
            if (showStackList[i]) {
                SerializedProperty stackRef = stackList.GetArrayElementAtIndex(i);
                SerializedProperty effectList = stackRef.FindPropertyRelative("effects");

				#region STACK GENERAL
				cam.stacks[i].autoplay = EditorGUILayout.ToggleLeft(new GUIContent("Autoplay", "Play the camera effect automatically on start"), cam.stacks[i].autoplay);
		        cam.stacks[i].loop = EditorGUILayout.ToggleLeft(new GUIContent("Loop", "Enable the camera effect to loop"), cam.stacks[i].loop);
                if (cam.stacks[i].loop) {
                    cam.stacks[i].loopForever = EditorGUILayout.ToggleLeft(new GUIContent("Loop Forever", "Loop indefinitely"), cam.stacks[i].loopForever);
                    if (!cam.stacks[i].loopForever) {
                        cam.stacks[i].iterations = EditorGUILayout.IntField(new GUIContent("Iterations", "The number of times to loop"), cam.stacks[i].iterations);
                    }
                    cam.stacks[i].pingpong = EditorGUILayout.ToggleLeft(new GUIContent("Ping-Pong", "Loop will alterante between playing forwards and backwards"), cam.stacks[i].pingpong);
                }
				#endregion

				if (GUILayout.Button("Add Effect",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                    effectList.InsertArrayElementAtIndex(effectList.arraySize);
                }
        
                for(int a = 0; a < effectList.arraySize; a++){
                    //Repaint();
                    EditorGUILayout.PropertyField(effectList.GetArrayElementAtIndex(a));

                    float height = EditorGUI.GetPropertyHeight(effectList.GetArrayElementAtIndex(a), null, true);

                    //Repaint();
                    if(GUILayout.Button("Remove Effect", GUILayout.MaxWidth(100),GUILayout.MaxHeight(15))){
                        effectList.DeleteArrayElementAtIndex(a);
                    } Line(Color.gray, 2);
                }

                if (GUILayout.Button("Remove Stack")) {
                    stackList.DeleteArrayElementAtIndex(i);
                    showStackList.RemoveAt(i);
                }

				#region STACK EVENTS
				int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent + 1;
                showStackEventList[i] = EditorGUILayout.Foldout(showStackEventList[i] , "Events");
                if (showStackEventList[i]) {
                    serializedObject.Update();
                    EditorGUILayout.PropertyField(stackRef.FindPropertyRelative("OnStart"));
                    EditorGUILayout.PropertyField(stackRef.FindPropertyRelative("OnLoop"));
                    EditorGUILayout.PropertyField(stackRef.FindPropertyRelative("OnComplete"));
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.indentLevel = indent;
				#endregion
			}
		}
        
        GetTarget.ApplyModifiedProperties();
    }

    void Line(Color color, int i_height = 1) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, color);
    }

    public static void RepaintInspector(System.Type t) {
        Editor[] ed = (Editor[])Resources.FindObjectsOfTypeAll<Editor>();
        for (int i = 0; i < ed.Length; i++) {
            if (ed[i].GetType() == t) {
                ed[i].Repaint();
                return;
            }
        }
    }
}
