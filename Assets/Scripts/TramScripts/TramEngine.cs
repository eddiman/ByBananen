using Dreamteck.Splines.Examples;

namespace TramScripts
{
    using System.Collections.Generic;
    using UnityEngine;
    using Dreamteck.Splines;

    public class TramEngine : MonoBehaviour
    {
        private SplineTracer _tracer;

        private Wagon _wagon;

        public bool canSwitchTracks;

        private bool _shiftTrackLeft;
        private bool _shiftTrackRight;
        private void Awake()
        {
            _wagon = GetComponent<Wagon>();
        }

        private void Start()
        {
            _tracer = GetComponent<SplineTracer>();
            //Subscribe to the onNode event to receive junction information automatically when a Node is passed
            _tracer.onNode += OnJunction;
            //Subscribe to the onMotionApplied event so that we can immediately update the wagons' positions once the engine's position is set
            _tracer.onMotionApplied += OnMotionApplied;
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
            _wagon.UpdateOffset();
        }

        //Called when the tracer has passed a junction (a Node)
        private void OnJunction(List<SplineTracer.NodeConnection> passed)
        {
            Node node = passed[0].node; //Get the node of the junction

            var junctionSwitch = node.GetComponent<JunctionSwitch>(); //Look for a JunctionSwitch component
            var bridge = junctionSwitch.bridge;
            if (junctionSwitch == null) return; //No JunctionSwitch - ignore it - this isn't a real junction
            if (junctionSwitch.bridge == null) return; //The JunctionSwitch does not have bridge elements

            //Look for a suitable bridge element based on the spline we are currently traversing
            if (!bridge.active) return;
            if (bridge.aIndex == bridge.bIndex) return; //Skip bridge if it points to the same spline
            var currentConnectionIndex = 0;

            var connections = node.GetConnections();
            //get the connected splines and find the index of the tracer's current spline
            for (var i = 0; i < connections.Length; i++)
            {
                if (connections[i].spline != _tracer.spline) continue;
                currentConnectionIndex = i;
                break;
            }

            //Skip the bridge if we are not on one of the splines that the switch connects
            if (currentConnectionIndex != bridge.aIndex && currentConnectionIndex != bridge.bIndex) return;

            if (bridge.isTrackSwitcher)
            {
                //Determine if the next spline is left or right relative to the tram
                var nextSplineBPos = connections[bridge.bIndex].spline.GetPoint(connections[bridge.bIndex].pointIndex + 1);
                var relativePosition = transform.InverseTransformPoint(nextSplineBPos.position);
                var sign = relativePosition.x > 0f ? 1f : -1f;

                switch (sign)
                {
                    case -1f when _shiftTrackLeft && canSwitchTracks:
                        //This bridge is suitable and should use it
                        SwitchSpline(connections[bridge.aIndex], connections[bridge.bIndex]);
                        canSwitchTracks = false;
                        return;
                    case 1f when _shiftTrackRight && canSwitchTracks:
                        //This bridge is suitable and should use it
                        SwitchSpline(connections[bridge.aIndex], connections[bridge.bIndex]);
                        canSwitchTracks = false;
                        return;
                    default:
                        return;
                }
            }

            if (!bridge.isTrackSwitcher && currentConnectionIndex != bridge.bIndex)
            {
                SwitchSpline(connections[bridge.aIndex], connections[bridge.bIndex]);
            }

        }

        void SwitchSpline(Node.Connection from, Node.Connection to)
        {
            //See how much units we have travelled past that Node in the last frame
            float excessDistance = _tracer.spline.CalculateLength(_tracer.spline.GetPointPercent(from.pointIndex), _tracer.UnclipPercent(_tracer.result.percent));
            //Set the spline to the tracer
            _tracer.spline = to.spline;
            _tracer.RebuildImmediate();
            //Get the location of the junction point in percent along the new spline
            double startpercent = _tracer.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            if (Vector3.Dot(from.spline.Evaluate(from.pointIndex).direction, to.spline.Evaluate(to.pointIndex).direction) < 0f)
            {
                // if (_tracer.direction == Spline.Direction.Forward) _tracer.direction = Spline.Direction.Backward;
                //else _tracer.direction = Spline.Direction.Forward;
            }
            //Position the tracer at the new location and travel excessDistance along the new spline
            _tracer.SetPercent(_tracer.Travel(startpercent, excessDistance, _tracer.direction));
            //Notify the wagon that we have entered a new spline segment
            _wagon.EnterSplineSegment(from.pointIndex, _tracer.spline, to.pointIndex, _tracer.direction);
            _wagon.UpdateOffset();
        }
    }
}
