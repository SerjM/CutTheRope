using CurvedLine_2016_03_11.CurvedLine;
using Cut_the_diet_game.Scripts.Rope;
using UnityEditor;

namespace Rope_2D_Editor.Editor
{
    [CustomEditor(typeof(CurvedLineRenderer))]
    class CurvedLineRendererInspector : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CurvedLineRenderer сurvedLineRenderer = target as CurvedLineRenderer;


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            сurvedLineRenderer.SetPointsAutomatically = EditorGUILayout.ToggleLeft("Auto generate points in children",
                сurvedLineRenderer.SetPointsAutomatically);
            
                

                    EditorGUI.BeginChangeCheck();
                    сurvedLineRenderer.AutoPointsFrequency =
                        EditorGUILayout.Slider("Frequency", сurvedLineRenderer.AutoPointsFrequency, 0f, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                if (сurvedLineRenderer.SetPointsAutomatically)
                {
                    сurvedLineRenderer.SetAutoPoints();
                }
                else
                {
                    сurvedLineRenderer.DeleteAutoPoints();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
