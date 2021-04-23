using UnityEngine;

namespace Scripts.Vehicles
{
    public class Car : MonoBehaviour, IVehicle
    {
        public GameObject GameObject => gameObject;
        public Vector3 Velocity => _rigidbody.velocity;

        [SerializeField] private TextAsset textAsset;

        private CarCharacteristics _carCharacteristics;

        [SerializeField] private bool canAccelerateBackwards;

        [SerializeField] private float gripForward;
        [SerializeField] private float gripSideway;

        [SerializeField] private float wheelBaseLength;
        [SerializeField] private float maxWheelAngle;
        [SerializeField] private float angularVelocityToSlip;
        [SerializeField] private float gcDisplacement;

        [SerializeField] private float slippingSpeedModifier;
        [SerializeField] private float slippingRotationModifier;

        private bool _isSlipping;
        private bool _isRotating;
        private Quaternion _lastRotation;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _carCharacteristics = JsonUtility.FromJson<CarCharacteristics>(textAsset.text);
        }

        private void Update()
        {
            var localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);

            if (_isSlipping || localVelocity.x != 0f)
            {
                localVelocity = ApplySidewaysGrip(localVelocity);

                if (localVelocity.x == 0f)
                    _isSlipping = false;
            }
            else if (_isRotating)
            {
                localVelocity = _lastRotation * localVelocity;
            }

            localVelocity = ApplyForwardGrip(localVelocity);
            localVelocity.z = Mathf.Clamp(localVelocity.z, -_carCharacteristics.maxSpeed, _carCharacteristics.maxSpeed);

            _rigidbody.velocity = transform.TransformDirection(localVelocity);
            _isRotating = false;
        }

        private Vector3 ApplySidewaysGrip(Vector3 localSpeed)
        {
            var sideSpeed = localSpeed.x;
            var sideDeceleration = gripSideway * Time.deltaTime;
            localSpeed.x = Mathf.Abs(sideSpeed) <= sideDeceleration
                ? 0f
                : localSpeed.x - sideDeceleration * Mathf.Sign(sideSpeed);
            return localSpeed;
        }

        private Vector3 ApplyForwardGrip(Vector3 localSpeed)
        {
            var isForward = Mathf.Sign(localSpeed.z);
            localSpeed.z -= isForward * gripForward * Time.deltaTime;
            if (localSpeed.z * isForward <= 0f) localSpeed.z = 0f;
            return localSpeed;
        }

        public void Accelerate(float strength)
        {
            var localVelocity = transform.InverseTransformDirection(_rigidbody.velocity);

            var currentAcceleration =
                (strength >= 0f ? _carCharacteristics.acceleration : _carCharacteristics.deceleration) /
                _carCharacteristics.mass * strength * Time.deltaTime;

            if (_isSlipping)
                currentAcceleration *= slippingSpeedModifier;

            if (strength < 0f && !canAccelerateBackwards)
            {
                if (localVelocity.z * (localVelocity.z + currentAcceleration) <= 0f)
                    localVelocity.z = 0f;
                else if (localVelocity.z > 0f)
                    localVelocity.z += currentAcceleration;
            }
            else
                localVelocity.z += currentAcceleration;

            _rigidbody.velocity = transform.TransformDirection(localVelocity);
        }

        public void Rotate(float steerStrength)
        {
            if (steerStrength == 0f)
                return;

            _isRotating = true;

            var alpha = 90 - Mathf.Abs(steerStrength) * maxWheelAngle;
            var radius = wheelBaseLength / Mathf.Cos(alpha * Mathf.Deg2Rad);
            var speed = _rigidbody.velocity.magnitude;
            var additionalRadius = _carCharacteristics.mass * speed * gcDisplacement;
            var angularVelocity = speed / (radius + additionalRadius) * Mathf.Rad2Deg;

            if (_isSlipping)
            {
                angularVelocity *= slippingRotationModifier;
            }
            else if (angularVelocity >= angularVelocityToSlip)
            {
                _isSlipping = true;
            }

            _lastRotation =
                Quaternion.AngleAxis(angularVelocity * Mathf.Sign(steerStrength) * Time.deltaTime, transform.up);
            transform.rotation *= _lastRotation;
        }
    }
}