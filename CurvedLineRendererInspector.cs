using CurvedLine_2016_03_11.CurvedLine;
using Rope_2D_Editor.Scripts;
using UnityEditor;

namespace Assets
{
    [CustomEditor(typeof(CurvedLineRenderer))]
    class CurvedLineRendererInspector : Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CurvedLineRenderer сurvedLineRenderer = target as CurvedLineRenderer;


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            сurvedLineRenderer.SetPointsAutomatically = EditorGUILayout.ToggleLeft("Auto generate points in children",
                сurvedLineRenderer.SetPointsAutomatically);

            if (сurvedLineRenderer.SetPointsAutomatically)
            {
                сurvedLineRenderer.AutoPointsFrequency =
                    EditorGUILayout.Slider("Frequency", сurvedLineRenderer.AutoPointsFrequency, 0f, 1f);

                сurvedLineRenderer.SetAutoPoints();
            }
            else
            {
                сurvedLineRenderer.DeleteAutoPoints();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
