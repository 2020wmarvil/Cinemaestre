using UnityEditor;
using Cinemaestre;
using UnityEngine;

[CustomEditor(typeof(CinemaestreCamera))]
public class CinemaestreCameraEditor : Editor {
    CinemaestreCamera cam;
    SerializedObject GetTarget;
    SerializedProperty stackList;

    SerializedProperty OnStart, OnLoop, OnComplete;

    GUIStyle horizontalLine;
    Color gray = new Color(0.5f, 0.5f, 0.5f, 1f);
 
    void OnEnable(){
        cam = (CinemaestreCamera)target;
        GetTarget = new SerializedObject(cam);
        stackList = GetTarget.FindProperty("stacks"); 

        OnStart = serializedObject.FindProperty("OnStart");
        OnLoop = serializedObject.FindProperty("OnLoop");
        OnComplete = serializedObject.FindProperty("OnComplete");

        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset( 0, 0, 4, 4 );
        horizontalLine.fixedHeight = 2;
    }
 
    public override void OnInspectorGUI(){
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

        EditorGUILayout.LabelField("<color=white>Effect Stacks</color>", myStyle); 






        GetTarget.Update();
   
        if(GUILayout.Button("Add Stack")) cam.stacks.Add(new CinemaestreStack());
   
        EditorGUILayout.Space();
        EditorGUILayout.Space();
   
        for(int i = 0; i < stackList.arraySize; i++) {
            SerializedProperty stackRef = stackList.GetArrayElementAtIndex(i);
            SerializedProperty effectList = stackRef.FindPropertyRelative("effects");

            if(GUILayout.Button("Add Effect",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                effectList.InsertArrayElementAtIndex(effectList.arraySize);
            }
 
            for(int a = 0; a < effectList.arraySize; a++){
                EditorGUILayout.PropertyField(effectList.GetArrayElementAtIndex(a));
                if(GUILayout.Button("Remove Effect", GUILayout.MaxWidth(100),GUILayout.MaxHeight(15))){
                    effectList.DeleteArrayElementAtIndex(a);
                } HorizontalLine(gray, 2);
            }
 
            if(GUILayout.Button("Remove Stack")) stackList.DeleteArrayElementAtIndex(i);

            if (i != stackList.arraySize - 1) HorizontalLine(gray, 4);
        }
   
        GetTarget.ApplyModifiedProperties();

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

   void HorizontalLine(Color color, float height) {
       var c = GUI.color;
       GUI.color = color;
       float h = horizontalLine.fixedHeight;
       horizontalLine.fixedHeight = height;

       GUILayout.Box(GUIContent.none, horizontalLine);
       GUI.color = c;
       horizontalLine.fixedHeight = h;
   }

    void GuiLine(int i_height = 1) {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color (0.5f, 0.5f, 0.5f, 1));
    }
}
