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
		serializedObject.Update();

		EditorGUILayout.PropertyField(serializedObject.FindProperty("effect"));

		serializedObject.ApplyModifiedProperties();

		serializedObject.Update();

        EditorGUILayout.Space();
        GuiLine(1);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events");
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(OnStart);
        EditorGUILayout.PropertyField(OnLoop);
        EditorGUILayout.PropertyField(OnComplete);

		serializedObject.ApplyModifiedProperties();


        return;





        //base.OnInspectorGUI();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("effects"));

        return;

        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 20;
        myStyle.richText = true;

        EditorGUILayout.LabelField("<b><color=white>Cinemaestre Camera Tool</color></b>", myStyle); 

        //EditorGUILayout.PropertyField();
        EditorGUILayout.Space();
        GuiLine(1);
        EditorGUILayout.Space();

        cam.duration = EditorGUILayout.FloatField(new GUIContent("Duration", "Duration of the camera effect"), cam.duration);
        cam.autoplay = EditorGUILayout.Toggle(new GUIContent("Autoplay", "Play the camera effect automatically on start"), cam.autoplay);
        cam.loop = EditorGUILayout.ToggleLeft(new GUIContent("Loop", "Enable the camera effect to loop"), cam.loop);
        if (cam.loop) {
            cam.loopForever = EditorGUILayout.Toggle(new GUIContent("Loop Forever", "Loop indefinitely"), cam.loopForever);
            if (!cam.loopForever) {
                cam.iterations = EditorGUILayout.IntField(new GUIContent("Loops", "The number of times to loop"), cam.iterations);
			}
            cam.pingpong = EditorGUILayout.Toggle(new GUIContent("PingPong", "Loop will alternate between playing forwards and backwards"), cam.pingpong);
		}

        if (cam.customEase) {
            cam.easeAnimationCurve = EditorGUILayout.CurveField(new GUIContent("Ease Curve", "Custom animation curve for the ease function"), cam.easeAnimationCurve);
		} else {
            cam.easeType = (LeanTweenType)EditorGUILayout.EnumPopup(new GUIContent("Ease Function", "Ease function for smooth camera animaton"), cam.easeType);
		}
        cam.customEase = EditorGUILayout.ToggleLeft(new GUIContent("Custom Ease Function", "Toggle between built-in and custom ease function"), cam.customEase);

        EditorGUILayout.Space();
        GuiLine(1);
        EditorGUILayout.Space();

        cam.cameraEffect = (CameraEffect)EditorGUILayout.EnumPopup(new GUIContent("Camera Effect", "The actual camera effect type"), cam.cameraEffect);
        switch(cam.cameraEffect) {
            case CameraEffect.SLIDE: {
                cam.slideType = (SlideType)EditorGUILayout.EnumPopup(new GUIContent("Slide Type", "Defines how the Slide should interpret the position data"), cam.slideType);
                if (cam.slideType == SlideType.WORLD_POS) {
                    cam.slideWorldPos = EditorGUILayout.Vector3Field(new GUIContent("World Position", "Slides the camera to a position in world space"), cam.slideWorldPos);
                } else if (cam.slideType == SlideType.LOCAL_OFFSET) {
                    cam.slideLocalOffset = EditorGUILayout.Vector3Field(new GUIContent("Local Offset", "Slides the camera by the given offset values"), cam.slideLocalOffset);
                } else if (cam.slideType == SlideType.DIR_AND_MAG) {
                    cam.slideMoveDir = EditorGUILayout.Vector3Field(new GUIContent("Direction", "Slides the camera in this direction"), cam.slideMoveDir);
                    cam.slideMoveDistance = EditorGUILayout.FloatField(new GUIContent("Distance", "Slides the camera this distance"), cam.slideMoveDistance);
                }

                break;
            } case CameraEffect.PAN: {
                if (!cam.panCustomDirection) {
                    cam.panDirection = (PanDirection)EditorGUILayout.EnumPopup(new GUIContent("Pan Direction", "The axis on which the camera will pan"), cam.panDirection);
				} else {
                    cam.panAxisOfRotation = EditorGUILayout.Vector3Field(new GUIContent("Axis of Rotation", "Defines the axis on which the camera will rotate"), cam.panAxisOfRotation);
			    }
                cam.panCustomDirection = EditorGUILayout.ToggleLeft(new GUIContent("Custom Axis", "Toggle between a base direction and a custom axis"), cam.panCustomDirection);
                cam.panAngle = EditorGUILayout.FloatField(new GUIContent("Angle Offset", "The angle, in degrees, to rotate. Can be negative."), cam.panAngle);
                cam.panGlobalSpace = EditorGUILayout.Toggle(new GUIContent("Use Global Space", "Rotate in global space or local space"), cam.panGlobalSpace);
                break;
            } case CameraEffect.ZOOM: {
                cam.zoomTargetFOV = EditorGUILayout.FloatField(new GUIContent("Target FOV", "The final FOV of the camera after completing the zoom"), cam.zoomTargetFOV);
                break;
            } case CameraEffect.FADE: {
                cam.fadeColor = EditorGUILayout.ColorField(new GUIContent("Fade Color", "Color of the fade effect"), cam.fadeColor);
                cam.fadeOut = EditorGUILayout.Toggle(new GUIContent("Fade Out", "Toggle between fading in and fading out"), cam.fadeOut);
                break;
            } 
        }

        EditorGUILayout.Space();
        GuiLine(1);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events");
        EditorGUILayout.Space();

        serializedObject.Update();
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
