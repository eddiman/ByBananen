namespace Dreamteck.Splines.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class SplineCompEditor : SplineEditorBase
    {
        SplineComputer spline = null;
        SplineComputer[] splines = new SplineComputer[0];
        SerializedObject serializedObject;
        bool pathToolsFoldout = false, interpolationFoldout = false;
        public bool drawComputer = true;
        public bool drawConnectedComputers = true;
        DreamteckSplinesEditor pathEditor;
        int operation = -1, module = -1;
        ComputerEditorModule[] modules = new ComputerEditorModule[0];
        Dreamteck.Editor.Toolbar toolbar;

        public SplineCompEditor(SplineComputer[] splines, SerializedObject serializedObject, DreamteckSplinesEditor pathEditor) : base()
        {
            spline = splines[0];
            this.splines = splines;
            this.pathEditor = pathEditor;
            this.serializedObject = serializedObject;
            modules = new ComputerEditorModule[2];
            modules[0] = new ComputerMergeModule(spline);
            modules[1] = new ComputerSplitModule(spline);
            GUIContent[] toolbarContents = new GUIContent[modules.Length], toolbarContentsSelected = new GUIContent[modules.Length];
            for (int i = 0; i < modules.Length; i++)
            {
                toolbarContents[i] = modules[i].GetIconOff();
                toolbarContentsSelected[i] = modules[i].GetIconOn();
                modules[i].undoHandler += OnRecordUndo;
                modules[i].repaintHandler += OnRepaint;
            }
            toolbar = new Dreamteck.Editor.Toolbar(toolbarContents, toolbarContentsSelected, 35f);
            toolbar.newLine = false;
        }

        void OnRecordUndo(string title)
        {
            if (undoHandler != null) undoHandler(title);
        }

        void OnRepaint()
        {
            if (repaintHandler != null) repaintHandler();
        }

        protected override void Load()
        {
            base.Load();
            pathToolsFoldout = EditorPrefs.GetBool("DreamteckSplinesEditor.pathToolsFoldout", false);
            interpolationFoldout = EditorPrefs.GetBool("DreamteckSplinesEditor.interpolationFoldout", false);
        }

        protected override void Save()
        {
            base.Save();
            EditorPrefs.SetBool("DreamteckSplinesEditor.pathToolsFoldout", pathToolsFoldout);
            EditorPrefs.SetBool("DreamteckSplinesEditor.interpolationFoldout", interpolationFoldout);
        }

        public override void Destroy()
        {
            base.Destroy();
            for (int i = 0; i < modules.Length; i++) modules[i].Deselect();
        }

        public override void DrawInspector()
        {
            base.DrawInspector();
            if (spline == null) return;
            SplineEditorGUI.SetHighlightColors(SplinePrefs.highlightColor, SplinePrefs.highlightContentColor);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            string text2D = spline.is2D ? "3D Mode" : "2D Mode";
            operation = GUILayout.Toolbar(operation, new string[] { spline.isClosed ? "Break" : "Close", "Reverse", text2D }, GUILayout.Width(220f));
            if (EditorGUI.EndChangeCheck()) PerformOperation();
            GUILayout.Space(30f);
            EditorGUI.BeginChangeCheck();
            if (splines.Length == 1)
            {
                int mod = module;
                toolbar.Draw(ref mod);
                if (EditorGUI.EndChangeCheck()) ToggleModule(mod);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (module >= 0 && module < modules.Length) modules[module].DrawInspector();
            EditorGUILayout.Space();
            DreamteckEditorGUI.DrawSeparator();

            EditorGUILayout.Space();
            
            serializedObject.Update();
            SerializedProperty sampleRate = serializedObject.FindProperty("spline").FindPropertyRelative("sampleRate");
            SerializedProperty type = serializedObject.FindProperty("spline").FindPropertyRelative("type");
            SerializedProperty linearAverageDirection = serializedObject.FindProperty("spline").FindPropertyRelative("linearAverageDirection");
            SerializedProperty space = serializedObject.FindProperty("_space");
            SerializedProperty sampleMode = serializedObject.FindProperty("_sampleMode");
            SerializedProperty optimizeAngleThreshold = serializedObject.FindProperty("_optimizeAngleThreshold");
            SerializedProperty updateMode = serializedObject.FindProperty("updateMode");
            SerializedProperty rebuildOnAwake = serializedObject.FindProperty("rebuildOnAwake");
            SerializedProperty multithreaded = serializedObject.FindProperty("multithreaded");

            EditorGUI.BeginChangeCheck();
            Spline.Type lastType = (Spline.Type)type.intValue;
            EditorGUILayout.PropertyField(type);
            if(lastType == Spline.Type.Hermite && type.intValue == (int)Spline.Type.Bezier)
            {
                if(EditorUtility.DisplayDialog("Hermite to Bezier", "Would you like to retain the Hermite shape in Bezier mode?", "Yes", "No"))
                {
                    for (int i = 0; i < splines.Length; i++) splines[i].HermiteToBezierTangents();
                    
                    serializedObject.Update();
                    pathEditor.Refresh();
                }
            }
            if(spline.type == Spline.Type.Linear) EditorGUILayout.PropertyField(linearAverageDirection);
            int lastSpace = space.intValue;
            EditorGUILayout.PropertyField(space, new GUIContent("Space"));
            EditorGUILayout.PropertyField(sampleMode, new GUIContent("Sample Mode"));
            if (sampleMode.intValue == (int)SplineComputer.SampleMode.Optimized) EditorGUILayout.PropertyField(optimizeAngleThreshold);
            EditorGUILayout.PropertyField(updateMode);
            if (updateMode.intValue == (int)SplineComputer.UpdateMode.None)
            {
                if (GUILayout.Button("Manual Update"))
                {
                    for (int i = 0; i < splines.Length; i++) splines[i].RebuildImmediate(true, true);
                }
            }
            if (spline.type != Spline.Type.Linear) EditorGUILayout.PropertyField(sampleRate, new GUIContent("Sample Rate"));
            EditorGUILayout.PropertyField(rebuildOnAwake);
            EditorGUILayout.PropertyField(multithreaded);

            if (EditorGUI.EndChangeCheck())
            {
                if (sampleRate.intValue < 2) sampleRate.intValue = 2;
                if (lastSpace != space.intValue)
                {
                    for (int i = 0; i < splines.Length; i++) splines[i].space = (SplineComputer.Space)space.intValue;
                    serializedObject.Update();
                    if (splines.Length == 1) pathEditor.Refresh();
                }
                serializedObject.ApplyModifiedProperties();
                for (int i = 0; i < splines.Length; i++) splines[i].Rebuild(true);
            }

            if (splines.Length == 1) { 
            EditorGUI.indentLevel++;
            interpolationFoldout = EditorGUILayout.Foldout(interpolationFoldout, "Custom interpolation");
            if (interpolationFoldout)
            {
                if (spline.customValueInterpolation == null || spline.customValueInterpolation.keys.Length == 0)
                {
                    if (GUILayout.Button("Add Value Interpolation"))
                    {
                        AnimationCurve curve = new AnimationCurve();
                        curve.AddKey(new Keyframe(0, 0, 0, 0));
                        curve.AddKey(new Keyframe(1, 1, 0, 0));
                        spline.customValueInterpolation = curve;
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    spline.customValueInterpolation = EditorGUILayout.CurveField("Value interpolation", spline.customValueInterpolation);
                    if (GUILayout.Button("x", GUILayout.MaxWidth(25))) spline.customValueInterpolation = null;
                    EditorGUILayout.EndHorizontal();
                }
                if (spline.customNormalInterpolation == null || spline.customNormalInterpolation.keys.Length == 0)
                {
                    if (GUILayout.Button("Add Normal Interpolation"))
                    {
                        AnimationCurve curve = new AnimationCurve();
                        curve.AddKey(new Keyframe(0, 0));
                        curve.AddKey(new Keyframe(1, 1));
                        spline.customNormalInterpolation = curve;
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    spline.customNormalInterpolation = EditorGUILayout.CurveField("Normal interpolation", spline.customNormalInterpolation);
                    if (GUILayout.Button("x", GUILayout.MaxWidth(25))) spline.customNormalInterpolation = null;
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
            EditorGUI.indentLevel--;
        }

        void PerformOperation()
        {
            switch (operation)
            {
                case 0:
                    if (spline.isClosed) BreakSpline();
                    else CloseSpline();
                    operation = -1;
                    break;
                case 1:
                    ReversePointOrder();
                    operation = -1;
                    break;
                case 2:
                    spline.is2D = !spline.is2D;
                    operation = -1;
                    break;
            }
        }

        void ToggleModule(int index)
        {
            if (module >= 0 && module < modules.Length) modules[module].Deselect();
            if (module == index) index = -1;
            module = index;
            if (module >= 0 && module < modules.Length) modules[module].Select();
        }

        public void BreakSpline()
        {
            RecordUndo("Break path");
            if (splines.Length == 1 && pathEditor.selectedPoints.Count == 1) spline.Break(pathEditor.selectedPoints[0]);
            else
            {
                for (int i = 0; i < splines.Length; i++) splines[i].Break();
            }
        }

        public void CloseSpline()
        {
            RecordUndo("Close path");
            for (int i = 0; i < splines.Length; i++)
            {
                splines[i].Close();
            }
        }

        void ReversePointOrder()
        {
            for (int i = 0; i < splines.Length; i++)
            {
                ReversePointOrder(splines[i]);
            }
        }

        void ReversePointOrder(SplineComputer spline)
        {
            SplinePoint[] points = spline.GetPoints();
            for (int i = 0; i < Mathf.FloorToInt(points.Length / 2); i++)
            {
                SplinePoint temp = points[i];
                points[i] = points[(points.Length - 1) - i];
                Vector3 tempTan = points[i].tangent;
                points[i].tangent = points[i].tangent2;
                points[i].tangent2 = tempTan;
                int opposideIndex = (points.Length - 1) - i;
                points[opposideIndex] = temp;
                tempTan = points[opposideIndex].tangent;
                points[opposideIndex].tangent = points[opposideIndex].tangent2;
                points[opposideIndex].tangent2 = tempTan;
            }
            if (points.Length % 2 != 0)
            {
                Vector3 tempTan = points[Mathf.CeilToInt(points.Length / 2)].tangent;
                points[Mathf.CeilToInt(points.Length / 2)].tangent = points[Mathf.CeilToInt(points.Length / 2)].tangent2;
                points[Mathf.CeilToInt(points.Length / 2)].tangent2 = tempTan;
            }
            spline.SetPoints(points);
        }

        public override void DrawScene()
        {
            base.DrawScene();
            if (drawComputer)
            {
                for (int i = 0; i < splines.Length; i++)
                {
                    SplineDrawer.DrawSplineComputer(splines[i]);
                }

            }
            if (drawConnectedComputers)
            {
                for (int i = 0; i < splines.Length; i++)
                {
                    List<SplineComputer> computers = splines[i].GetConnectedComputers();
                    for (int j = 1; j < computers.Count; j++)
                    {
                        SplineDrawer.DrawSplineComputer(computers[j], 0.0, 1.0, 0.5f);
                    }
                }
            }
            if (module >= 0 && module < modules.Length) modules[module].DrawScene();
        }
    }
}
