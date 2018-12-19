using System.Collections.Generic;
using Cut_the_diet_game.Scripts.Rope;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace Rope_2D_Editor.Editor
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : UnityEditor.Editor
    {
        private Texture nodeTexture;
        private Texture dotHandleTexture;
        private static GUIStyle handleStyle = new GUIStyle();
        private static GUIStyle dothandleStyle = new GUIStyle();
        private List<int> alignedPoints = new List<int>();
        private bool showOptions;
        private Vector3 ropePosition;

        private void OnEnable()
        {
            nodeTexture = Resources.Load<Texture>("Handle");
            if (nodeTexture == null) nodeTexture = EditorGUIUtility.whiteTexture;
            dotHandleTexture = Resources.Load<Texture>("DotHandle");
            if (dotHandleTexture == null) nodeTexture = EditorGUIUtility.whiteTexture;
            handleStyle.alignment = TextAnchor.MiddleCenter;
            handleStyle.fixedWidth = 15;
            handleStyle.fixedHeight = 15;
            dothandleStyle.alignment = TextAnchor.MiddleCenter;
            dothandleStyle.fixedWidth = 8;
            dothandleStyle.fixedHeight = 8;
            ropePosition = (target as Rope).transform.position;
        }

        private void OnSceneGUI()
        {
            Rope rope = (target as Rope);
            Vector3[] localPoints = rope.nodes.ToArray();
            Vector3[] worldPoints = new Vector3[rope.nodes.Count];
            for (int i = 0; i < worldPoints.Length; i++)
                worldPoints[i] = rope.transform.TransformPoint(localPoints[i]);
            DrawPolyLine(worldPoints);
            DrawNodes(rope, worldPoints);

            DrawHinges(rope);

            if (Event.current.shift)
            {
                //Adding Points
                Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                Vector3 polyLocalMousePos = rope.transform.InverseTransformPoint(mousePos);
                Vector3 nodeOnPoly = HandleUtility.ClosestPointToPolyLine(worldPoints);
                float handleSize = HandleUtility.GetHandleSize(nodeOnPoly);
                int nodeIndex = FindNodeIndex(worldPoints, nodeOnPoly);
                Handles.DrawLine(worldPoints[nodeIndex - 1], mousePos);
                Handles.DrawLine(worldPoints[nodeIndex], mousePos);
                if (Handles.Button(mousePos, Quaternion.identity, handleSize * 0.1f, handleSize, HandleFunc))
                {
                    polyLocalMousePos.z = 0;
                    Undo.RecordObject(rope, "Insert Node");
                    rope.nodes.Insert(nodeIndex, polyLocalMousePos);
                    UpdateRope(rope);
                    Event.current.Use();
                }
            }
            if (Event.current.control && rope.nodes.Count > 2)
            {
                //Deleting Points
                int indexToDelete = FindNearestNodeToMouse(worldPoints);
                Handles.color = Color.red;
                float handleSize = HandleUtility.GetHandleSize(worldPoints[0]);
                if (Handles.Button(worldPoints[indexToDelete], Quaternion.identity, handleSize * 0.09f, handleSize, DeleteHandleFunc))
                {
                    Undo.RecordObject(rope, "Remove Node");
                    rope.nodes.RemoveAt(indexToDelete);
                    indexToDelete = -1;
                    UpdateRope(rope);
                    Event.current.Use();
                }
                Handles.color = Color.white;
            }
            if (ropePosition != rope.transform.position)
            {
                ropePosition = rope.transform.position;
                UpdateEndsJoints(rope);
            }
        }

        private void DrawHinges(Rope rope)
        {
            if (rope.transform.childCount == 0) return;

            if (rope.EnablePhysics &&
                /*rope.HangFirstSegment &&*/
                rope.transform.childCount > 0)
            {
                //Draw Hinge 
                Transform firstSegment = rope.transform.GetChild(0);
                float handleSize = HandleUtility.GetHandleSize(firstSegment.position);
                Vector2 hingePositionInWorldSpace = rope.transform.TransformPoint(rope.FirstSegmentConnectionAnchor);
                if (RopeUtil.GetConnectedObject(hingePositionInWorldSpace, firstSegment.GetComponent<Rigidbody2D>()))
                    GUI.color = Color.magenta;
                else
                    GUI.color = Color.blue;
                Vector2 newFirstSegmentAnchor = Handles.FreeMoveHandle(hingePositionInWorldSpace,
                    Quaternion.identity, handleSize * 0.05f, Vector3.one, SolidHandleFunc);
                if (newFirstSegmentAnchor != hingePositionInWorldSpace)
                {
                    rope.FirstSegmentConnectionAnchor = rope.transform.InverseTransformPoint(newFirstSegmentAnchor);
                    UpdateEndsJoints(rope);
                }
                GUI.color = Color.white;
            }

            if (rope.EnablePhysics/* && rope.HangLastSegment*/)
            {
                //Draw Hinge 
                Transform lastSegment = rope.transform.GetChild(rope.transform.childCount - 1);
                float handleSize = HandleUtility.GetHandleSize(lastSegment.position);
                Vector2 hingePositionInWorldSpace = rope.transform.TransformPoint(rope.LastSegmentConnectionAnchor);
                if (RopeUtil.GetConnectedObject(hingePositionInWorldSpace, lastSegment.GetComponent<Rigidbody2D>()))
                    GUI.color = Color.magenta;
                else
                    GUI.color = Color.blue;
                Vector2 newLastSegmentAnchor = Handles.FreeMoveHandle(hingePositionInWorldSpace,
                    Quaternion.identity, handleSize * 0.05f, Vector3.one, SolidHandleFunc);
                GUI.color = Color.white;
                if (newLastSegmentAnchor != hingePositionInWorldSpace)
                {
                    rope.LastSegmentConnectionAnchor = rope.transform.InverseTransformPoint(newLastSegmentAnchor);
                    UpdateEndsJoints(rope);
                }
            }
        }
        /*public static Rigidbody2D GetConnectedObject(Vector2 position, Rigidbody2D originalObj)
        {
            return RopeUtil.GetConnectedObject(position, originalObj);
        }*/

        private int FindNearestNodeToMouse(Vector3[] worldNodesPositions)
        {
            Vector3 mousePos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            mousePos.z = 0;
            int index = -1;
            float minDistnce = float.MaxValue;
            for (int i = 0; i < worldNodesPositions.Length; i++)
            {
                float distance = Vector3.Distance(worldNodesPositions[i], mousePos);
                if (distance < minDistnce)
                {
                    index = i;
                    minDistnce = distance;
                }
            }
            return index;
        }
        private int FindNodeIndex(Vector3[] worldNodesPositions, Vector3 newNode)
        {
            float smallestdis = float.MaxValue;
            int prevIndex = 0;
            for (int i = 1; i < worldNodesPositions.Length; i++)
            {
                float distance = HandleUtility.DistanceToPolyLine(worldNodesPositions[i - 1], worldNodesPositions[i]);
                if (distance < smallestdis)
                {
                    prevIndex = i - 1;
                    smallestdis = distance;
                }
            }
            return prevIndex + 1;
        }
        private void DrawPolyLine(Vector3[] nodes)
        {
            if (Event.current.shift) Handles.color = Color.green;
            else if (Event.current.control) Handles.color = Color.red;
            else Handles.color = Color.white;
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                if (alignedPoints.Contains(i) && alignedPoints.Contains(i + 1))
                {
                    Color currentColor = Handles.color;
                    Handles.color = Color.green;
                    Handles.DrawLine(nodes[i], nodes[i + 1]);
                    Handles.color = currentColor;
                }
                else
                    Handles.DrawLine(nodes[i], nodes[i + 1]);

            }
            Handles.color = Color.white;
        }
        private void DrawNodes(Rope rope, Vector3[] worldPoints)
        {
            for (int i = 0; i < rope.nodes.Count; i++)
            {
                Vector3 pos = rope.transform.TransformPoint(rope.nodes[i]);
                float handleSize = HandleUtility.GetHandleSize(pos);
                Vector3 newPos = Handles.FreeMoveHandle(pos, Quaternion.identity, handleSize * 0.09f, Vector3.one, HandleFunc);
                if (newPos != pos)
                {
                    CheckAlignment(worldPoints, handleSize * 0.1f, i, ref newPos);
                    Undo.RecordObject(rope, "Move Node");
                    rope.nodes[i] = rope.transform.InverseTransformPoint(newPos);
                    UpdateRope(rope);
                }
            }
        }

        private bool CheckAlignment(Vector3[] worldNodes, float offset, int index, ref Vector3 position)
        {
            alignedPoints.Clear();
            bool aligned = false;
            //check straight lines
            //check previous line
            if (index >= 2)
            {
                //represent the line with the equation y=mx+b
                float dy = worldNodes[index - 1].y - worldNodes[index - 2].y;
                float dx = worldNodes[index - 1].x - worldNodes[index - 2].x;
                float m = dy / dx;
                float b = worldNodes[index - 1].y - m * worldNodes[index - 1].x;

                float newX = (position.x + m * (position.y - b)) / (m * m + 1);
                float newY = (m * (position.x + m * position.y) + b) / (m * m + 1);
                Vector3 newPos = new Vector3(newX, newY);
                float distance = Vector3.Distance(newPos, position);
                if (distance * distance < offset * offset)
                {
                    position.x = newX;
                    position.y = newY;
                    aligned = true;
                    alignedPoints.Add(index - 1);
                    alignedPoints.Add(index - 2);
                }
            }
            //check next line
            if (index < worldNodes.Length - 2)
            {
                //represent the line with the equation y=mx+b
                float dy = worldNodes[index + 1].y - worldNodes[index + 2].y;
                float dx = worldNodes[index + 1].x - worldNodes[index + 2].x;
                float m = dy / dx;
                float b = worldNodes[index + 1].y - m * worldNodes[index + 1].x;

                float newX = (position.x + m * (position.y - b)) / (m * m + 1);
                float newY = (m * (position.x + m * position.y) + b) / (m * m + 1);
                Vector3 newPos = new Vector3(newX, newY);
                float distance = Vector3.Distance(newPos, position);
                if (distance * distance < offset * offset)
                {
                    position.x = newX;
                    position.y = newY;
                    aligned = true;
                    alignedPoints.Add(index + 1);
                    alignedPoints.Add(index + 2);
                }
            }
            //check vertical
            //check with the prev node
            //the node can be aligned to the prev and next node at once, we need to return more than one alginedTo Node
            if (index > 0)
            {
                float dx = Mathf.Abs(worldNodes[index - 1].x - position.x);
                if (dx < offset)
                {
                    position.x = worldNodes[index - 1].x;
                    alignedPoints.Add(index - 1);
                    aligned = true;
                }
            }
            //check with the next node
            if (index < worldNodes.Length - 1)
            {
                float dx = Mathf.Abs(worldNodes[index + 1].x - position.x);
                if (dx < offset)
                {
                    position.x = worldNodes[index + 1].x;
                    alignedPoints.Add(index + 1);
                    aligned = true;
                }
            }
            //check horizontal
            if (index > 0)
            {
                float dy = Mathf.Abs(worldNodes[index - 1].y - position.y);
                if (dy < offset)
                {
                    position.y = worldNodes[index - 1].y;
                    alignedPoints.Add(index - 1);
                    aligned = true;
                }
            }
            //check with the next node
            if (index < worldNodes.Length - 1)
            {
                float dy = Mathf.Abs(worldNodes[index + 1].y - position.y);
                if (dy < offset)
                {
                    position.y = worldNodes[index + 1].y;
                    alignedPoints.Add(index + 1);
                    aligned = true;
                }
            }


            if (aligned)
                alignedPoints.Add(index);

            return aligned;
        }

        private void HandleFunc(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (controlID == GUIUtility.hotControl)
                GUI.color = Color.red;
            else
                GUI.color = Color.green;
            Handles.Label(position, new GUIContent(nodeTexture), handleStyle);
            GUI.color = Color.white;
        }

        private void SolidHandleFunc(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            Handles.Label(position, new GUIContent(dotHandleTexture), dothandleStyle);
        }

        private void DeleteHandleFunc(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            GUI.color = Color.red;
            Handles.Label(position, new GUIContent(nodeTexture), handleStyle);
            GUI.color = Color.white;
        }

        public static void UpdateRope(Rope rope)
        {
            RopeUtil.UpdateRope(rope);
        }

        private static void UpdateEndsJoints(Rope rope)
        {
            RopeUtil.UpdateEndsJoints(rope);
        }

        [SerializeField] private Rigidbody2D _currentAttachedObject;

        public override void OnInspectorGUI()
        {
            Rope rope = target as Rope;

            EditorGUILayout.LabelField("ВЕРЁВКА");
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Объект, висящий на верёвке");


            /*_currentAttachedObject = EditorGUILayout.ObjectField("Attached Rigidbody2D",
                _currentAttachedObject, typeof(Rigidbody2D), true) as Rigidbody2D;*/
                rope.AttachedObject = EditorGUILayout.ObjectField("Attached Rigidbody2D",
                    rope.AttachedObject, typeof(Rigidbody2D), true) as Rigidbody2D;
            //rope.AttachObject(rope.AttachedObject);
            /*
            if (!Application.isPlaying)
            {
                if (currentAttachedObject == null)
                {
                    rope.Detach();
                }
                else if (currentAttachedObject != rope.AttachedObject)
                {
                    rope.AttachObject(currentAttachedObject);
                }
            }*/

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            showOptions = EditorGUILayout.ToggleLeft("Options", showOptions);
            if (showOptions)
            {
                base.OnInspectorGUI();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                bool EnablePhysics = EditorGUILayout.BeginToggleGroup(" Enable Physics", rope.EnablePhysics);
                if (EnablePhysics)
                {
                    /*bool hangFirstSegment = EditorGUILayout.Toggle("Hang First Segment", rope.HangFirstSegment);
                    if (hangFirstSegment != rope.HangFirstSegment)
                    {
                        rope.HangFirstSegment = hangFirstSegment;
                        if (hangFirstSegment)
                        {
                            rope.FirstSegmentConnectionAnchor = rope.nodes[0];
                        }

                        UpdateEndsJoints(rope);
                        SceneView.RepaintAll();
                    }*/

                    /*bool hangLastSegment = EditorGUILayout.Toggle("Hang Last Segment", rope.HangLastSegment);
                    if (hangLastSegment != rope.HangLastSegment)
                    {
                        rope.HangLastSegment = hangLastSegment;
                        if (hangLastSegment)
                        {
                            rope.LastSegmentConnectionAnchor = rope.nodes[rope.nodes.Count - 1];
                        }

                        UpdateEndsJoints(rope);
                        SceneView.RepaintAll();
                    }*/
#if UNITY_5_3_OR_NEWER
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
#else
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
#endif
                    bool useBendLimit = EditorGUILayout.BeginToggleGroup("Use Bend Limits", rope.useBendLimit);
                    if (useBendLimit != rope.useBendLimit)
                    {
                        rope.useBendLimit = useBendLimit;
                        UpdateRope(rope);
                    }

                    if (rope.useBendLimit)
                    {
                        int bendLimit = EditorGUILayout.IntSlider("Bend Limits", rope.bendLimit, 0, 180);
                        if (bendLimit != rope.bendLimit)
                        {
                            rope.bendLimit = bendLimit;
                            UpdateRope(rope);
                        }
                    }

                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.EndVertical();

#if UNITY_5_3_OR_NEWER
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    bool BreakableJoints = EditorGUILayout.BeginToggleGroup("Breakable Joints", rope.BreakableJoints);
                    if (BreakableJoints != rope.BreakableJoints)
                    {
                        rope.BreakableJoints = BreakableJoints;
                        UpdateRope(rope);

                    }

                    if (rope.BreakableJoints)
                    {
                        float BreakForce = EditorGUILayout.FloatField("Break Force", rope.BreakForce);
                        if (BreakForce != rope.BreakForce)
                        {
                            rope.BreakForce = BreakForce;
                            UpdateRope(rope);
                        }
                    }

                    EditorGUILayout.EndToggleGroup();
                    EditorGUILayout.EndVertical();
#endif
                }

                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndVertical();
                if (EnablePhysics != rope.EnablePhysics)
                {
                    rope.EnablePhysics = EnablePhysics;
                    UpdateEndsJoints(rope);
                    UpdateRope(rope);
                    SceneView.RepaintAll();
                }
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Update"))
            {
                UpdateRope(rope);

                if (rope.AttachedObject == null)
                {
                    rope.Detach();
                }
                else
                {
                    rope.AttachObject(rope.AttachedObject);
                }
            }
        }

        [MenuItem("GameObject/Rope", false, 1)]
        public static void CreateRope()
        {
            GameObject g = new GameObject();
            g.name = "Rope";
            g.AddComponent<Rope>();
            Vector3 position = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f)).origin;
            g.transform.position = new Vector3(position.x, position.y, 0);
            Selection.activeGameObject = g;
        }
    }
}
