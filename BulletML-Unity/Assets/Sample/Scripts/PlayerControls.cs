using UnityEngine;

namespace UnityBulletML
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerControls : MonoBehaviour
    {
        public float MoveSensitivity = 1f;
        public Rigidbody2D Rigidbody;

        private Vector3 _initialTouchPosition;
        private Vector3 _initialPosition;
        private Bounds _cameraBounds;

        private void Start()
        {
            _cameraBounds = Camera.main.OrthographicBounds();
        }

        void FixedUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            // Mouse inputs seem to be taken into account on Android
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _initialPosition = transform.position;
                    _initialTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }

                var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var deltaPosition = currentMousePosition - _initialTouchPosition;

                var newPosition = _initialPosition + (deltaPosition * MoveSensitivity);
                newPosition.z = _initialPosition.z; // Make sure we don't alter the Z position

                // Make sure the player stay in the camera area
                newPosition.x = Mathf.Clamp(newPosition.x, -_cameraBounds.extents.x, _cameraBounds.extents.x);
                newPosition.y = Mathf.Clamp(newPosition.y, -_cameraBounds.extents.y, _cameraBounds.extents.y);

                Rigidbody.MovePosition(newPosition);
            }
        }
    }
}