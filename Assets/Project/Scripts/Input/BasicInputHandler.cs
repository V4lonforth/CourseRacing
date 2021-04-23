using System;
using UnityEngine;

namespace Scripts.Input
{
    public abstract class BasicInputHandler: MonoBehaviour, IInputHandler
    {
        [SerializeField] private int priority;
        
        private int _inputId;
        private bool _isPressed;
        
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
            if (_isPressed) return false;
            
            _isPressed = true;
            _inputId = inputId;
            
            return HandlePress(inputPosition);
        }

        private bool Hold(int inputId, Vector2 inputPosition)
        {
            if (!_isPressed || _inputId != inputId) return false;
            
            return HandleHold(inputPosition);
        }

        private bool Release(int inputId, Vector2 inputPosition)
        {
            if (!_isPressed || _inputId != inputId) return false;
            
            _isPressed = false;
            
            return HandleRelease(inputPosition);
        }

        protected abstract bool HandlePress(Vector2 inputPosition);
        protected abstract bool HandleHold(Vector2 inputPosition);
        protected abstract bool HandleRelease(Vector2 inputPosition);
    }
}