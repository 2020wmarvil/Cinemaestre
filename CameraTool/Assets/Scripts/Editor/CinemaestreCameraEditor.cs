using UnityEditor;
using CinemaestreCamera;

[CustomEditor(typeof(CinemaestreCamera.CinemaestreCamera))]
public class CinemaestreCameraEditor : Editor {
    CinemaestreCamera.CinemaestreCamera cam;
    void OnEnable() {
        cam = (CinemaestreCamera.CinemaestreCamera)target;
    }

    public override void OnInspectorGUI() {
        cam.cameraEffect = (CinemaestreCamera.CameraEffect)EditorGUILayout.EnumPopup("Camera Effect", cam.cameraEffect);
        switch(cam.cameraEffect) {
            case CameraEffect.PAN: {
                EditorGUILayout.LabelField("Pan");
                break;
            } case CameraEffect.ZOOM: {
                EditorGUILayout.LabelField("Zoom");
                break;
            } case CameraEffect.FADE: {
                EditorGUILayout.LabelField("Fade");
                break;
            } case CameraEffect.FOV: {
                EditorGUILayout.LabelField("FOV");
                break;
            }
        }
    }
}
