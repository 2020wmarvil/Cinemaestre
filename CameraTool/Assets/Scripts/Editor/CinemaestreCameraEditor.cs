using UnityEditor;
using Cinemaestre;
using UnityEngine;

[CustomEditor(typeof(CinemaestreCamera))]
public class CinemaestreCameraEditor : Editor {
    CinemaestreCamera cam;
    void OnEnable() {
        cam = (CinemaestreCamera)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        cam.cameraEffect = (CameraEffect)EditorGUILayout.EnumPopup("Camera Effect", cam.cameraEffect);
        switch(cam.cameraEffect) {
            case CameraEffect.SLIDE: {
                    EditorGUILayout.LabelField("Slide");
                    break;
            } case CameraEffect.PAN: {
                EditorGUILayout.LabelField("Pan");
                break;
            } case CameraEffect.ZOOM: {
                EditorGUILayout.LabelField("Zoom");
                break;
            } case CameraEffect.FADE: {
                EditorGUILayout.LabelField("Fade");
                break;
            } 
        }
    }
}
