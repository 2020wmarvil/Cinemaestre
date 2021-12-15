using UnityEditor;
using UnityEngine;
using Cinemaestre;

[CustomPropertyDrawer(typeof(CinemaestreEffect))]
public class ColorPointDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.PrefixLabel(position, label);

		EditorGUI.indentLevel = 2;

		EditorGUI.PropertyField(new Rect(0f, 20f, position.width, 20f), property.FindPropertyRelative("effect"));
		EditorGUI.PropertyField(new Rect(0f, 40f, position.width, 20f), property.FindPropertyRelative("duration"));

		EditorGUI.PropertyField(new Rect(0f, 60f, position.width, 20f), property.FindPropertyRelative("loop"));

		SerializedProperty loopProp = property.FindPropertyRelative("loop");
		if (loopProp.boolValue) {
			EditorGUI.PropertyField(new Rect(0f, 80f, position.width, 20f), property.FindPropertyRelative("iterations"));
			EditorGUI.PropertyField(new Rect(0f, 100f, position.width, 20f), property.FindPropertyRelative("loopForever"));
			EditorGUI.PropertyField(new Rect(0f, 120f, position.width, 20f), property.FindPropertyRelative("pingpong"));
		}

		EditorGUI.PropertyField(new Rect(0f, 140f, position.width, 20f), property.FindPropertyRelative("easeType"));
		EditorGUI.PropertyField(new Rect(0f, 160f, position.width, 20f), property.FindPropertyRelative("customEase"));
		EditorGUI.PropertyField(new Rect(0f, 180f, position.width, 20f), property.FindPropertyRelative("easeAnimationCurve"));


		EditorGUI.EndProperty();

		EditorGUI.indentLevel = 0;
	}

	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
	   return base.GetPropertyHeight(prop, label) + 200f;
	}
}
