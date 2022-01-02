using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
 
[CustomEditor(typeof(CustomList))]
 
public class CustomListEditor : Editor {
 
    enum displayFieldType {DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields}
    displayFieldType DisplayFieldType;
 
    CustomList t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;
 
    void OnEnable(){
        t = (CustomList)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("MyList"); // Find the List in our script and create a refrence of it
    }
 
    public override void OnInspectorGUI(){
        //Update our list
   
        GetTarget.Update();
   
        ////Choose how to display the list<> Example purposes only
        //EditorGUILayout.Space ();
        //EditorGUILayout.Space ();
        //DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("",DisplayFieldType);
   
        if(GUILayout.Button("Add New")) t.MyList.Add(new CustomList.MyClass());
   
        EditorGUILayout.Space ();
        EditorGUILayout.Space ();
   
        //Display our list to the inspector window
 
        for(int i = 0; i < ThisList.arraySize; i++){
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty effectList = MyListRef.FindPropertyRelative("effects");

            if(GUILayout.Button("Add New Index",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
                effectList.InsertArrayElementAtIndex(effectList.arraySize);
            }
 
            for(int a = 0; a < effectList.arraySize; a++){
                EditorGUILayout.PropertyField(effectList.GetArrayElementAtIndex(a));
                if(GUILayout.Button("Remove  (" + a.ToString() + ")",GUILayout.MaxWidth(100),GUILayout.MaxHeight(15))){
                    effectList.DeleteArrayElementAtIndex(a);
                }
            }
 
            // Display the property fields in two ways.
       
            //if(DisplayFieldType == 0){// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
            //    //1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
 
            //}else{
 
            //    // Array fields with remove at index
            //    if(GUILayout.Button("Add New Index",GUILayout.MaxWidth(130),GUILayout.MaxHeight(20))){
            //        MyArray.InsertArrayElementAtIndex(MyArray.arraySize);
            //        MyArray.GetArrayElementAtIndex(MyArray.arraySize -1).intValue = 0;
            //    }
 
            //    for(int a = 0; a < MyArray.arraySize; a++){
            //        EditorGUILayout.BeginHorizontal();
            //        EditorGUILayout.LabelField("My Custom Int (" + a.ToString() + ")",GUILayout.MaxWidth(120));
            //        MyArray.GetArrayElementAtIndex(a).intValue = EditorGUILayout.IntField("",MyArray.GetArrayElementAtIndex(a).intValue, GUILayout.MaxWidth(100));
            //        if(GUILayout.Button("-",GUILayout.MaxWidth(15),GUILayout.MaxHeight(15))){
            //            MyArray.DeleteArrayElementAtIndex(a);
            //        }
            //        EditorGUILayout.EndHorizontal();
            //    }
            //}
       
            //EditorGUILayout.Space ();
       
            //Remove this index from the List
            if(GUILayout.Button("Remove This Index (" + i.ToString() + ")")){
                ThisList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
            EditorGUILayout.Space ();
        }
   
        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}
