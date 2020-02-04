﻿using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using c = HighPrecisionStepperJuggler.Constants;

namespace HighPrecisionStepperJuggler
{
    public class ImageProcessingInstructionSender : MonoBehaviour
    {
        [SerializeField] private UVCCameraPlugin _cameraPlugin;
        [SerializeField] private MachineController _machineController;

        private float _lastFramesXDistance;
        private float _lastFramesYDistance;

        private List<float> _xVelocities = new List<float>();
        private List<float> _yVelocities = new List<float>();

        private void Update()
        {
            var ballData = _cameraPlugin.UpdateImageProcessing();
            var distance = FOVCalculations.RadiusToDistance(ballData.Radius);
            var xDistance = FOVCalculations.PixelPositionToDistanceFromCenter(ballData.PositionX, distance);
            var yDistance = FOVCalculations.PixelPositionToDistanceFromCenter(ballData.PositionY, distance);

            if (distance >= float.MaxValue)
            {
                return;
            }

            if (_machineController.IsReadyForNextInstruction)
            {
                // if we're here the plate is flat in bottom position, there is a ball and that ball is moving up.

                var p_x = -xDistance * c.k_p;
                var p_y = yDistance * c.k_p;

                var v_x = (xDistance - _lastFramesXDistance) / Time.deltaTime;
                var v_y = (yDistance - _lastFramesYDistance) / Time.deltaTime;

                _xVelocities.Add(v_x);
                _yVelocities.Add(v_y);

                if (distance < 130f)
                {
                    var accumulatedV_x = 0f;
                    _xVelocities.ForEach(v => accumulatedV_x += v);
                    var xVelocity = accumulatedV_x / _xVelocities.Count;
                    var d_x = -xVelocity * c.k_d;

                    var accumulatedV_y = 0f;
                    _yVelocities.ForEach(v => accumulatedV_y += v);
                    var yVelocity = accumulatedV_y / _yVelocities.Count;
                    var d_y = yVelocity * c.k_d;

                    var xCorrection = Mathf.Clamp(p_x + d_x, c.MinTiltAngle, c.MaxTiltAngle);
                    var yCorrection = Mathf.Clamp(p_y + d_y, c.MinTiltAngle, c.MaxTiltAngle);

                    var moveTime = 0.1f;
                    _machineController.SendInstructions(new List<HLInstruction>()
                    {
                        new HLInstruction(0.06f, xCorrection, yCorrection, moveTime),
                        new HLInstruction(0.05f, 0f, 0f, moveTime),
                    });
                    
                    _xVelocities.Clear();
                    _yVelocities.Clear();
                }
            }

            _lastFramesXDistance = xDistance;
            _lastFramesYDistance = yDistance;
        }
    }
}