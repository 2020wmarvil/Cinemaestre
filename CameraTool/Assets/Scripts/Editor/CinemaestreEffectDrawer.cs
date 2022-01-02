using UnityEditor;
using UnityEngine;
using Cinemaestre;

[CustomPropertyDrawer(typeof(CinemaestreEffect))]
public class CinemaestreEffectDrawer : PropertyDrawer {
	float extraHeight = 0f;
	float lineHeight = 20f;

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		float yVal = 0f;

		label = EditorGUI.BeginProperty(position, label, property); 
		EditorGUI.indentLevel = 2;

		#region GENERAL
		EditorGUI.indentLevel = 0;
		EditorGUI.LabelField(new Rect(position.x, position.y + yVal, position.width, lineHeight), new GUIContent("General")); yVal += lineHeight;
		EditorGUI.indentLevel = 2;
		EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("duration"),
			new GUIContent("Duration", "Duration of the camera effect")); yVal += lineHeight;
		#endregion

		#region EASING
		SerializedProperty easeProp = property.FindPropertyRelative("customEase");
		if (easeProp.boolValue) {
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("easeAnimationCurve"),
				new GUIContent("Ease Curve", "Custom animation curve for the ease function")); yVal += lineHeight;
		} else {
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("easeType"),
				new GUIContent("Ease Function", "Ease function for smooth camera animation")); yVal += lineHeight;
		}
		EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("customEase"),
			new GUIContent("Custom Ease Function", "Toggle between built-in and custom ease function")); yVal += lineHeight;
		#endregion
		
		yVal += 5f;
        EditorGUI.DrawRect(new Rect(position.x, position.y + yVal, position.width, 1), new Color (0.5f, 0.5f, 0.5f, 1)); yVal += 1;
		yVal += 5f;

		#region EFFECT TYPE
		EditorGUI.indentLevel = 0;
		EditorGUI.LabelField(new Rect(position.x, position.y + yVal, position.width, lineHeight), new GUIContent("Effect")); yVal += lineHeight;
		EditorGUI.indentLevel = 2;

		EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("effectType"), 
			new GUIContent("Effect Type", "Type of CinemaestreEffect to play")); yVal += lineHeight;
		SerializedProperty effectProp = property.FindPropertyRelative("effectType");
		if (effectProp.enumValueIndex == (int)CinemaestreEffectType.SLIDE) {
			Debug.Log("slide");
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("slideType"), 
				new GUIContent("Slide Type", "Defines how the Slide should interpret the position data")); yVal += lineHeight;

			SerializedProperty slideProp = property.FindPropertyRelative("slideType");
			if (slideProp.enumValueIndex == (int)SlideType.WORLD_POS) {
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("slideWorldPos"), 
					new GUIContent("World Position", "Slides the camera to a position in world space")); yVal += lineHeight;
			} else if (slideProp.enumValueIndex == (int)SlideType.LOCAL_OFFSET) {
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("slideLocalOffset"), 
					new GUIContent("Local Offset", "Slides the camera to a position in world space")); yVal += lineHeight;
			} else if (slideProp.enumValueIndex == (int)SlideType.DIR_AND_MAG) {
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("slideMoveDir"), 
					new GUIContent("Direction", "Slides the camera in this direction")); yVal += lineHeight;
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("slideMoveDistance"), 
					new GUIContent("Distance", "Slides the camera this distance")); yVal += lineHeight;
			}
		} else if (effectProp.enumValueIndex == (int)CinemaestreEffectType.PAN) {
			SerializedProperty panProp = property.FindPropertyRelative("panCustomDirection");
			if (!panProp.boolValue) {
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("panDirection"), 
					new GUIContent("Pan Direction", "The axis on which the camera will pan")); yVal += lineHeight;
			} else {
				EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("panAxisOfRotation"), 
					new GUIContent("Axis of Rotation", "Defines the axis on which the camera will rotate")); yVal += lineHeight;
			}

			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("panCustomDirection"), 
				new GUIContent("Custom Axis", "Toggle between a base direction and a custom axis")); yVal += lineHeight;
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("panAngle"), 
				new GUIContent("Angle Offset", "The angle, in degrees, to rotate. Can be negative.")); yVal += lineHeight;
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("panGlobalSpace"), 
				new GUIContent("Use Global Space", "")); yVal += lineHeight;
		} else if (effectProp.enumValueIndex == (int)CinemaestreEffectType.ZOOM) {
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("zoomTargetFOV"), 
				new GUIContent("Target FOV", "The final FOV of the camera after completing the zoom")); yVal += lineHeight;
		} else if (effectProp.enumValueIndex == (int)CinemaestreEffectType.FADE) {
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("fadeColor"), 
				new GUIContent("Fade Color", "Color of the fade effect")); yVal += lineHeight;
			EditorGUI.PropertyField(new Rect(0f, position.y + yVal, position.width, lineHeight), property.FindPropertyRelative("fadeOut"), 
				new GUIContent("Fade Out", "Toggle between fading in and fading out")); yVal += lineHeight;
		} else if (effectProp.enumValueIndex == (int)CinemaestreEffectType.DELAY) {
		}
		#endregion

		EditorGUI.indentLevel = 0;
		EditorGUI.EndProperty();

		extraHeight = yVal;

		if (effectProp.enumValueIndex == (int)CinemaestreEffectType.SLIDE) {
			Debug.Log(extraHeight);
		}
	}

	public override float GetPropertyHeight (SerializedProperty prop, GUIContent label) {
	   return base.GetPropertyHeight(prop, label) + extraHeight - lineHeight;
	}
}
