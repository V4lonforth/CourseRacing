using UnityEngine;

namespace Scripts.Input
{
    public interface IInputHandler
    {
        int Priority { get; }
        bool Active { get; }
        bool HandleTouch(int inputId, Vector2 inputPosition, TouchPhase inputPhase);
    }
}