using ControllerScripts;
using Dreamteck.Splines.Examples;

namespace TramScripts
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Dreamteck.Splines;
    using System;

    public class TramEngine : MonoBehaviour
    {
        SplineTracer tracer = null;

        Wagon wagon;

        private bool _shiftTrackLeft;
        private bool _shiftTrackRight;
        private void Awake()
        {
            wagon = GetComponent<Wagon>();
        }

        void Start()
        {
            tracer = GetComponent<SplineTracer>();
            //Subscribe to the onNode event to receive junction information automatically when a Node is passed
            tracer.onNode += OnJunction;
            //Subscribe to the onMotionApplied event so that we can immediately update the wagons' positions once the engine's position is set
            tracer.onMotionApplied += OnMotionApplied;
        }

        private void Update()
        {
//TODO: Refactor this into keyboard controller
            _shiftTrackLeft = Input.GetKey("z");
            _shiftTrackRight = Input.GetKey("x");

        }



        void OnMotionApplied()
        {
            //Apply the wagon's offset (this will recursively apply the offsets to the rest of the wagons in the chain)
            wagon.UpdateOffset();
        }

        //Called when the tracer has passed a junction (a Node)
        private void OnJunction(List<SplineTracer.NodeConnection> passed)
        {
            Node node = passed[0].node; //Get the node of the junction

            JunctionSwitch junctionSwitch = node.GetComponent<JunctionSwitch>(); //Look for a JunctionSwitch component
            if (junctionSwitch == null) return; //No JunctionSwitch - ignore it - this isn't a real junction
            if (junctionSwitch.bridges.Length == 0) return; //The JunctionSwitch does not have bridge elements
            foreach (JunctionSwitch.Bridge bridge in junctionSwitch.bridges)
            {
                //Look for a suitable bridge element based on the spline we are currently traversing
                if (!bridge.active) continue;
                if (bridge.a == bridge.b) continue; //Skip bridge if it points to the same spline
                int currentConnection = 0;
                Vector3 currentDirection = tracer.result.direction;
                if (tracer.direction == Spline.Direction.Backward) currentDirection *= -1f;
                Node.Connection[] connections = node.GetConnections();
                //get the connected splines and find the index of the tracer's current spline
                for (int i = 0; i < connections.Length; i++)
                {
                    if (connections[i].spline == tracer.spline)
                    {
                        currentConnection = i;
                        break;
                    }
                }
                //Skip the bridge if we are not on one of the splines that the switch connects
                if (currentConnection != bridge.a && currentConnection != bridge.b) continue;
                if (currentConnection == bridge.a)
                {
                    if ((int)tracer.direction != (int)bridge.bDirection) continue;
                    var nextSplineBPos = connections[bridge.b].spline.GetPoint(connections[bridge.b].pointIndex - 1);
                    //////////////

                    Vector3 relativePosition = transform.InverseTransformPoint(nextSplineBPos.position);
                    float sign = relativePosition.x > 0f ? 1f : -1f;
                    Debug.Log(sign);

                    //canSwitchTracks is activated when the tram is close to a track-switch-junction,
                    //it when it is
                    if (SceneGlobals.canSwitchTrack)
                    {

                        if (sign == 1f && _shiftTrackRight)
                        {
                            //This bridge is suitable and should use it
                            SwitchSpline(connections[bridge.a], connections[bridge.b]);
                            return;
                        } if (sign == -1f && _shiftTrackLeft)
                        {
                            //This bridge is suitable and should use it
                            SwitchSpline(connections[bridge.a], connections[bridge.b]);
                            return;
                        }
                    }
                    else
                    {
                        //This is for a "normal" bridge, it bridges from a main track to one other
                        //no side tracks here
                        SwitchSpline(connections[bridge.a], connections[bridge.b]);
                        return;
                    }



//////////////
                    //This bridge is suitable and should use it
                    //SwitchSpline(connections[bridge.a], connections[bridge.b]);
                    return;
                }
                else
                {
                    if ((int)tracer.direction != (int)bridge.aDirection) continue;
                    //This bridge is suitable and should use it
                    SwitchSpline(connections[bridge.b], connections[bridge.a]);
                    return;
                }
            }
        }
        //returns -1 when to the left, 1 to the right, and 0 for forward/backward
        public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
        {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            if (dir > 0.0f) {
                return 1.0f;
            } else if (dir < 0.0f) {
                return -1.0f;
            } else {
                return 0.0f;
            }
        }

        /// <summary>
        /// Determine the signed angle between two vectors, with normal 'n'
        /// as the rotation axis.
        /// </summary>
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                       Vector3.Dot(n, Vector3.Cross(v1, v2)),
                       Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }
        void SwitchSpline(Node.Connection from, Node.Connection to)
        {
            //See how much units we have travelled past that Node in the last frame
            float excessDistance = tracer.spline.CalculateLength(tracer.spline.GetPointPercent(from.pointIndex), tracer.UnclipPercent(tracer.result.percent));
            //Set the spline to the tracer
            tracer.spline = to.spline;
            tracer.RebuildImmediate();
            //Get the location of the junction point in percent along the new spline
            double startpercent = tracer.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).direction, to.spline.Evaluate(to.pointIndex).direction) < 0f)
            {
                if (tracer.direction == Spline.Direction.Forward) tracer.direction = Spline.Direction.Backward;
                else tracer.direction = Spline.Direction.Forward;
            }
            //Position the tracer at the new location and travel excessDistance along the new spline
            tracer.SetPercent(tracer.Travel(startpercent, excessDistance, tracer.direction));
            //Notify the wagon that we have entered a new spline segment
            wagon.EnterSplineSegment(from.pointIndex, tracer.spline, to.pointIndex, tracer.direction);
            wagon.UpdateOffset();
        }
    }
}
