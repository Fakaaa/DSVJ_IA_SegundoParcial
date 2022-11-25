using System.Collections;

using UnityEngine;

namespace InteligenciaArtificial.SegundoParcial.Utils.CameraHandler
{
    public class CameraHandler : MonoBehaviour
    {
        #region EXPOSED_FIELDS
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float dampingCoefficient = 0f;
        [SerializeField] private float zoomSpeed = 0f;
        #endregion

        #region PRIVATE_FIELDS
        private Vector2 velocity = default;
        private Camera mainCamera = null;

        private Vector3 initialCameraPosition = default;
        private float initialCameraZoom = 0;
        #endregion

        #region PROPERTIES
        public bool Focused 
        {
            get
            {
                return Cursor.lockState==CursorLockMode.Locked;
            }
            set
            {
                Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = value == false;
            }
        }
        #endregion

        #region UNITY_CALLS
        private void Start()
        {
            if (mainCamera == null)
                mainCamera = GetComponent<Camera>();

            if (mainCamera == null)
                gameObject.AddComponent<Camera>();
        }

        private void Update()
        {
            if(Focused)
            {
                UpdateInput();
            }
            else if(Input.GetMouseButton(1))
            {
                Focused = true;
            }

            ApplyMovement();
        }
        #endregion

        #region PUBLIC_METHODS
        public void Initialize()
        {
            initialCameraPosition = mainCamera.transform.position;
            initialCameraZoom = mainCamera.orthographicSize;
        }

        public void ResetCamera()
        {
            StartCoroutine(RestoreCamera(1f));
        }
        #endregion

        #region PRIVATE_METHODS
        private void UpdateInput()
        {
            velocity += GetAccelerationVector() * Time.deltaTime;

            if(mainCamera != null)
            {
                mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                CalculareSprintSpeedUponZoom();
            }

            if(Input.GetKeyDown(KeyCode.Escape))
                Focused = false;
        }

        private Vector2 GetAccelerationVector()
        {
            Vector2 moveInput = default;

            void AddMovement(KeyCode key, Vector2 dir)
            {
                if(Input.GetKey(key))
                {
                    moveInput += dir;
                }
            }

            AddMovement(KeyCode.W, Vector2.up);
            AddMovement(KeyCode.S, Vector2.down);
            AddMovement(KeyCode.D, Vector2.right);
            AddMovement(KeyCode.A, Vector2.left);

            Vector2 direction = transform.TransformVector(moveInput.normalized);

            if (Input.GetKey(KeyCode.LeftShift))
                return direction * ( acceleration * sprintMultiplier );
            
            return direction * acceleration;
        }

        private void ApplyMovement()
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, dampingCoefficient * Time.deltaTime);
            transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        }

        private void CalculareSprintSpeedUponZoom()
        {
            if(mainCamera.orthographicSize >= 100f)
            {
                sprintMultiplier = (mainCamera.orthographicSize * 10f) / 100f;
            }
            else if(mainCamera.orthographicSize > 50f && mainCamera.orthographicSize < 100f)
            {
                sprintMultiplier = (mainCamera.orthographicSize * 5f) / 100f;
            }
            else if(mainCamera.orthographicSize > 5f && mainCamera.orthographicSize < 50f)
            {
                sprintMultiplier = (mainCamera.orthographicSize * 15f) / 100f;
            }
        }
        #endregion

        #region COROUTINES
        private IEnumerator RestoreCamera(float restoreDuration)
        {
            float time = 0;

            while (time < restoreDuration)
            {
                time += Time.deltaTime;

                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, initialCameraPosition, time);
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, initialCameraZoom, time);

                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}