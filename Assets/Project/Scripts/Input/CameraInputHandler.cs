using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Utils;
using UnityEngine;

namespace Scripts.Input
{
    public class CameraInputHandler : MonoBehaviour, IInputHandler
    {
        private class InputData
        {
            public Vector2 Position;
            public float Time;

            public InputData(Vector2 position, float time)
            {
                Position = position;
                Time = time;
            }
        }
        
        [SerializeField] private CameraController cameraController; 
        [SerializeField] private int priority;

        [SerializeField] private float maxTimeToSwipe;
        [SerializeField] private float minSpeedToSwipe;

        private readonly Dictionary<int, InputData> _inputData = new Dictionary<int, InputData>();
        private float _lastInputDistance;

        private bool _isLockedControl;

        public int Priority => priority;
        public bool Active { get; private set; }


        protected void OnEnable()
        {
            InputManager.Instance.AddInputHandler(this);
            Active = true;
        }
        protected void OnDisable()
        {
            Active = false;
        }

        private void Update()
        {
            if (_inputData.Count != 2) return;
            
            var inputDistance = (_inputData.First().Value.Position - _inputData.Last().Value.Position).magnitude;
            cameraController.Scale(inputDistance - _lastInputDistance);
            _lastInputDistance = inputDistance;
        }

        public bool HandleTouch(int inputId, Vector2 inputPosition, TouchPhase inputPhase)
        {
            switch (inputPhase)
            {
                case TouchPhase.Began:
                    return Press(inputId, inputPosition);
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    return Hold(inputId, inputPosition);
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    return Release(inputId, inputPosition);
                default:
                    return false;
            }
        }
        
        private bool Press(int inputId, Vector2 inputPosition)
        {
            if (_inputData.Count < 2)
            {
                _inputData.Add(inputId, new InputData(inputPosition, 0f));
                _isLockedControl = false;
            }

            if (_inputData.Count == 2)
            {
                _lastInputDistance = (_inputData.First().Value.Position - _inputData.Last().Value.Position).magnitude;
                _isLockedControl = true;
                return true;
            }
            _isLockedControl = false;
            return false;
        }

        private bool Hold(int inputId, Vector2 inputPosition)
        {
            if (!_inputData.TryGetValue(inputId, out var inputData)) return false;
            
            var inputOffset = inputData.Position - inputPosition;
            inputData.Position = inputPosition;
            
            if (!_isLockedControl && inputOffset.sqrMagnitude >= minSpeedToSwipe * minSpeedToSwipe && inputData.Time < maxTimeToSwipe)
            {
                _isLockedControl = true;
            }
            
            if (_isLockedControl)
            {
                cameraController.Rotate(inputOffset);
                return true;
            }

            inputData.Time += Time.deltaTime;
            return false;
        }

        private bool Release(int inputId, Vector2 inputPosition)
        {
            var result = Hold(inputId, inputPosition);
            RemoveInput(inputId);
            return result;
        }

        private void RemoveInput(int inputId)
        {
            _inputData.Remove(inputId);
            if (_inputData.Count == 0)
            {
                _isLockedControl = false;
            }
        }
    }
}