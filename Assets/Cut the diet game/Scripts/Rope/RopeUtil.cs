using System.Collections.Generic;
using UnityEngine;

namespace Cut_the_diet_game.Scripts.Rope
{
    public static class RopeUtil // : Editor
    {

        public static float GetRopeLength(Rope rope)
        {
            float len = 0f;

            for (int i = 0; i + 1 < rope.nodes.Count; i++)
            {
                len += Vector2.Distance(rope.nodes[i], rope.nodes[i + 1]);
            }

            if (rope.AttachedObject != null)
            {
                len += Vector2.Distance(rope.AttachedObject.position, rope.nodes[rope.nodes.Count - 1]);
            }
            Debug.Log("Rope len = " + len, rope);
            
            return len;
        }

        public static void DetachLast(Rope rope)
        {
            if (rope.transform.childCount == 0) return;
            Transform lastSegment = rope.transform.GetChild(rope.transform.childCount - 1);
            Joint2D[] joints = lastSegment.gameObject.GetComponents<Joint2D>();
            if (joints.Length > 1)
                for (int i = 1; i < joints.Length; i++)
                    Object.DestroyImmediate(joints[i]);
        }

        public static Rigidbody2D GetConnectedObject(Vector2 position, Rigidbody2D originalObj)
        {
            Rigidbody2D[] sceneRigidbodies = GameObject.FindObjectsOfType<Rigidbody2D>();
            for (int i = 0; i < sceneRigidbodies.Length; i++)
            {
                SpriteRenderer sprite = sceneRigidbodies[i].GetComponent<SpriteRenderer>();
                if (sprite == null)
                {
                    sprite = sceneRigidbodies[i].GetComponentInChildren<SpriteRenderer>();
                }
                if (originalObj != sceneRigidbodies[i] && sprite && sprite.bounds.Contains(position))
                {
                    return sceneRigidbodies[i];
                }
            }
            return null;
        }
        
        public static void UpdateRope(Rope rope)
        {
            DestroyChildren(rope);
            if (rope.SegmentsPrefabs == null || rope.SegmentsPrefabs.Length == 0)
            {
                Debug.LogWarning("Rope Segments Prefabs is Empty");
                return;
            }
            float segmentHeight = rope.SegmentsPrefabs[0].bounds.size.y * (1 + rope.overlapFactor);
            List<Vector3> nodes = rope.nodes;
            int currentSegPrefIndex = 0;
            Rigidbody2D previousSegment = null;
            float previousTheta = 0;
            int currentSegment = 0;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                //construct line between nodes[i] and nodes[i+1]
                float theta = Mathf.Atan2(nodes[i + 1].y - nodes[i].y, nodes[i + 1].x - nodes[i].x);
                float dx = segmentHeight * Mathf.Cos(theta);
                float dy = segmentHeight * Mathf.Sin(theta);
                float startX = nodes[i].x + dx / 2;
                float startY = nodes[i].y + dy / 2;
                float lineLength = Vector2.Distance(nodes[i + 1], nodes[i]);
                int segmentCount = 0;
                switch (rope.OverflowMode)
                {
                    case LineOverflowMode.Round:
                        segmentCount = Mathf.RoundToInt(lineLength / segmentHeight);
                        break;
                    case LineOverflowMode.Shrink:
                        segmentCount = (int)(lineLength / segmentHeight);
                        break;
                    case LineOverflowMode.Extend:
                        segmentCount = Mathf.CeilToInt(lineLength / segmentHeight);
                        break;
                }
                for (int j = 0; j < segmentCount; j++)
                {
                    if (rope.SegmentsMode == SegmentSelectionMode.RoundRobin)
                    {
                        currentSegPrefIndex++;
                        currentSegPrefIndex %= rope.SegmentsPrefabs.Length;
                    }
                    else if (rope.SegmentsMode == SegmentSelectionMode.Random)
                    {
                        currentSegPrefIndex = Random.Range(0, rope.SegmentsPrefabs.Length);
                    }
                    GameObject segment = (Object.Instantiate(rope.SegmentsPrefabs[currentSegPrefIndex]) as SpriteRenderer).gameObject;
                    segment.name = "Segment_" + currentSegment;
                    segment.transform.parent = rope.transform;
                    segment.transform.localPosition = new Vector3(startX + dx * j, startY + dy * j);
                    segment.transform.localRotation = Quaternion.Euler(0, 0, theta * Mathf.Rad2Deg - 90);
                    if (rope.EnablePhysics)
                    {
                        Rigidbody2D segRigidbody = segment.GetComponent<Rigidbody2D>();
                        if (segRigidbody == null)
                            segRigidbody = segment.AddComponent<Rigidbody2D>();
                        //if not the first segment, make a joint
                        if (currentSegment != 0)
                        {
                            float dtheta = 0;
                            if (j == 0)
                            {
                                //first segment in the line
                                dtheta = (theta - previousTheta) * Mathf.Rad2Deg;
                                if (dtheta > 180) dtheta -= 360;
                                else if (dtheta < -180) dtheta += 360;
                            }
                            //add Hinge
                            float yScale = rope.SegmentsPrefabs[0].transform.localScale.y;
                            AddJoint(rope, dtheta, segmentHeight / yScale, previousSegment, segment);
                        }
                        previousSegment = segRigidbody;
                    }
                    currentSegment++;
                }
                previousTheta = theta;
            }
            UpdateEndsJoints(rope);

            rope.InitVisuals();

            rope.UpdateLength();
        }

        public static void UpdateEndsJoints(Rope rope)
        {
            if (rope.transform.childCount == 0) return;

            Transform firstSegment = rope.transform.GetChild(0);
            if (rope.EnablePhysics &&
                /*rope.HangFirstSegment &&*/
                rope.transform.childCount > 0)
            {

                HingeJoint2D joint = firstSegment.gameObject.GetComponent<HingeJoint2D>();
                if (!joint)
                    joint = firstSegment.gameObject.AddComponent<HingeJoint2D>();
                Vector2 hingePositionInWorldSpace = rope.transform.TransformPoint(rope.FirstSegmentConnectionAnchor);
                joint.connectedAnchor = hingePositionInWorldSpace;
                joint.anchor = firstSegment.transform.InverseTransformPoint(hingePositionInWorldSpace);
                joint.connectedBody = null;//GetConnectedObject(hingePositionInWorldSpace, firstSegment.GetComponent<Rigidbody2D>());
                if (joint.connectedBody)
                {
                    joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(hingePositionInWorldSpace);
                }
            }
            else
            {
                HingeJoint2D joint = firstSegment.gameObject.GetComponent<HingeJoint2D>();
                if (joint) Object.DestroyImmediate(joint);
            }
            Transform lastSegment = rope.transform.GetChild(rope.transform.childCount - 1);
            if (rope.EnablePhysics && (/*rope.HangLastSegment ||*/ rope.AttachedObject != null))
            {
                if (true || rope.UseSpringJoints)
                {
                    SpringJoint2D[] joints = lastSegment.gameObject.GetComponents<SpringJoint2D>();
                    SpringJoint2D spring = null;
                    if (joints.Length > 1)
                        spring = joints[1];
                    else
                        spring = lastSegment.gameObject.AddComponent<SpringJoint2D>();

                    spring.autoConfigureDistance = rope.autoConfigureDistance;
                    spring.distance = rope.distance;
                    spring.frequency = rope.frequency / 2f;
                    spring.dampingRatio = rope.dampingRatio;
                    spring.anchor = new Vector2(0, spring.transform.lossyScale.y);//Vector2.zero;
                    spring.connectedAnchor = Vector2.zero;
                    Vector2 hingePositionInWorldSpace = rope.transform.TransformPoint(rope.LastSegmentConnectionAnchor);
                    if (rope.AttachedObject != null)
                    {
                        spring.connectedBody =
                            rope.AttachedObject;
                    }
                    else
                    {
                        spring.connectedBody = GetConnectedObject(hingePositionInWorldSpace, lastSegment.GetComponent<Rigidbody2D>());
                    }
                }
                else
                {
                    /*HingeJoint2D[] joints = lastSegment.gameObject.GetComponents<HingeJoint2D>();
                    HingeJoint2D joint = null;
                    if (joints.Length > 1)
                        joint = joints[1];
                    else
                        joint = lastSegment.gameObject.AddComponent<HingeJoint2D>();
                    Vector2 hingePositionInWorldSpace = rope.transform.TransformPoint(rope.LastSegmentConnectionAnchor);
                    joint.connectedAnchor = hingePositionInWorldSpace;
                    joint.anchor = lastSegment.transform.InverseTransformPoint(hingePositionInWorldSpace);
                    joint.connectedBody = GetConnectedObject(hingePositionInWorldSpace, lastSegment.GetComponent<Rigidbody2D>());
                    if (joint.connectedBody)
                    {
                        joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(hingePositionInWorldSpace);
                    }*/
                }



            }
            else
            {
                Joint2D[] joints = lastSegment.gameObject.GetComponents<Joint2D>();
                if (joints.Length > 1)
                    for (int i = 1; i < joints.Length; i++)
                        Object.DestroyImmediate(joints[i]);

            }
        }

        public static void AddJoint(Rope rope, float dtheta, float segmentHeight, Rigidbody2D previousSegment, GameObject segment)
        {
            if (rope.UseSpringJoints)
            {
                SpringJoint2D joint = segment.AddComponent<SpringJoint2D>();
                joint.connectedBody = previousSegment;
                joint.anchor = new Vector2(0, -segmentHeight / 2);
                joint.connectedAnchor = new Vector2(0, segmentHeight / 2);

                var spring = joint;

                spring.autoConfigureDistance = rope.autoConfigureDistance;
                spring.distance = rope.distance;
                spring.frequency = rope.frequency;
                spring.dampingRatio = 1f;

#if UNITY_5_3_OR_NEWER
                if (rope.BreakableJoints)
                    joint.breakForce = rope.BreakForce;
#endif
            }
            else
            {
                HingeJoint2D joint = segment.AddComponent<HingeJoint2D>();

                joint.autoConfigureConnectedAnchor = false;




                joint.connectedBody = previousSegment;
                joint.anchor = new Vector2(0, -segmentHeight / 2);
                joint.connectedAnchor = new Vector2(0, segmentHeight / 2);
                if (rope.useBendLimit)
                {
                    joint.useLimits = true;
                    joint.limits = new JointAngleLimits2D()
                    {
                        min = dtheta - rope.bendLimit,
                        max = dtheta + rope.bendLimit
                    };
                }
#if UNITY_5_3_OR_NEWER
                if (rope.BreakableJoints)
                    joint.breakForce = rope.BreakForce;
#endif
            }




        }

        public static void DestroyChildren(Rope rope)
        {
            while (rope.transform.childCount > 0)
                Object.DestroyImmediate(rope.transform.GetChild(0).gameObject);
        }



    }
}