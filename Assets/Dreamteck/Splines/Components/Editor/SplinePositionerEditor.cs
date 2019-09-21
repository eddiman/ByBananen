namespace Dreamteck.Splines.Editor
{
    using UnityEngine;
    using System.Collections;
    using UnityEditor;

    [CustomEditor(typeof(SplinePositioner), true)]
    [CanEditMultipleObjects]
    public class SplinePositionerEditor : SplineTracerEditor
    {
        protected override void BodyGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Positioning", EditorStyles.boldLabel);

            serializedObject.Update();
            SerializedProperty mode = serializedObject.FindProperty("_mode");
            EditorGUI.BeginChangeCheck();
            SplinePositioner positioner = (SplinePositioner)target;
            EditorGUILayout.PropertyField(mode, new GUIContent("Mode"));
            if (positioner.mode == SplinePositioner.Mode.Distance) positioner.position = EditorGUILayout.FloatField("Distance", (float)positioner.position);
            else
            {
                SerializedProperty percent = serializedObject.FindProperty("_result").FindPropertyRelative("percent");
                SerializedProperty position = serializedObject.FindProperty("_position");
                double pos = positioner.ClipPercent(percent.floatValue);
                EditorGUI.BeginChangeCheck();
                pos = EditorGUILayout.Slider("Percent", (float)pos, 0f, 1f);
                if (EditorGUI.EndChangeCheck()) position.floatValue = (float)pos;

            }
            SerializedProperty targetObject = serializedObject.FindProperty("_targetObject");
            EditorGUILayout.PropertyField(targetObject, new GUIContent("Target Object"));
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            base.BodyGUI();
        }
    }
}
