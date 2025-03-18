using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample.Gameplay
{
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0, 1, -10);
        [SerializeField] private float _smoothTime = 0.3f;

        [Header("Bounds Settings")]
        [SerializeField] private bool _useBounds = true;
        [SerializeField] private float _minX, _maxX, _minY, _maxY;

        private Vector3 _velocity = Vector3.zero;

        private void Awake()
        {
            SetInitialPosition();
        }

        void LateUpdate()
        {
            FollowTarget();
        }

        private void SetInitialPosition()
        {
            if (_target == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{gameObject.name} is missing a target in CameraFollow2D script.");
#endif
                return;
            }

            transform.position = GetClampedTargetPosition();
        }

        public void FollowTarget()
        {
            if (_target == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"{gameObject.name} is missing a target in CameraFollow2D script.");
#endif
                return;
            }

            Vector3 targetPosition = GetClampedTargetPosition();
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
        }

        private Vector3 GetClampedTargetPosition()
        {
            Vector3 targetPosition = _target.position + _offset;

            if (_useBounds)
            {
                float cameraHalfHeight = Camera.main.orthographicSize;
                float cameraHalfWidth = cameraHalfHeight * Camera.main.aspect;

                float clampX = Mathf.Clamp(targetPosition.x, _minX + cameraHalfWidth, _maxX - cameraHalfWidth);
                float clampY = Mathf.Clamp(targetPosition.y, _minY + cameraHalfHeight, _maxY - cameraHalfHeight);

                targetPosition = new Vector3(clampX, clampY, targetPosition.z);
            }

            return targetPosition;
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_useBounds) return;

            Gizmos.color = Color.red;
            Vector3 center = new Vector3((_minX + _maxX) / 2f, (_minY + _maxY) / 2f, 0f);
            Vector3 size = new Vector3(_maxX - _minX, _maxY - _minY, 0f);
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
