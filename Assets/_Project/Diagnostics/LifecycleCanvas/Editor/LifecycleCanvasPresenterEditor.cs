using UnityEditor;
using UnityEngine;

namespace FirstGame.Diagnostics.Editor
{
    [CustomEditor(typeof(LifecycleCanvasPresenter))]
    public sealed class LifecycleCanvasPresenterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var presenter = (LifecycleCanvasPresenter)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime On-Demand Check", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Sources are shown only after a lifecycle callback is actually received. " +
                "Route and Activity names come from their authored assets.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Toggle("Ready", presenter.IsReady);
                EditorGUILayout.IntField("Events Received", presenter.Sequence);
                EditorGUILayout.IntField("Observed Sources", presenter.ObservedSourceCount);
                EditorGUILayout.LabelField("Rendered Check");
                EditorGUILayout.TextArea(presenter.RenderedReport, GUILayout.MinHeight(140f));
            }

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                if (GUILayout.Button("Reset On-Demand Check"))
                {
                    presenter.ResetReport();
                }
            }
        }
    }
}
