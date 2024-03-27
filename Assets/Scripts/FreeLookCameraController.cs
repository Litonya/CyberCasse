using UnityEngine;
using Cinemachine;

public class FreeLookCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public GameManager _instance; // Instance du GameManager

    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;
    public float zoomSpeed = 5f;
    public float yRotationSpeed = 2f; // Vitesse de rotation sur l'axe Y
    public float smoothTime = 0.5f; // Temps de lissage pour le mouvement de la caméra

    private Vector3 originalCameraPosition; // Position initiale de la caméra en Y
    private Vector3 velocity = Vector3.zero; // Vélocité pour le lissage du mouvement de la caméra
    private Transform target; // Cible de la caméra

    public static FreeLookCameraController instance;

    private bool isMovingTowardsTarget = false;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        instance = this;
    }

    private void Update()
    {
        HandleMovementInput();
        // HandleRotationInput();
        HandleZoomInput();
        HandleSelectionInput();
        if (isMovingTowardsTarget) // Appeler MoveCameraTowardsTarget() seulement si la caméra ne bouge pas manuellement
        {
            MoveCameraTowardsTarget();
        }
        //MoveCameraTowardsTarget();
    }

    private void HandleMovementInput()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            SetTarget(null);
            isMovingTowardsTarget = false;
        }

        Vector3 forwardDirection = virtualCamera.transform.forward.normalized;
        forwardDirection.y = 0;
        Vector3 rightDirection = virtualCamera.transform.right.normalized;
        rightDirection.y = 0;
        Vector3 moveDirection = (forwardDirection * verticalInput + rightDirection * horizontalInput).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(1))
        {
            SetTarget(null);
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            transform.Rotate(Vector3.up, mouseX, Space.World);
        }
    }

    private void HandleZoomInput()
    {
        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");
        virtualCamera.m_Lens.FieldOfView += zoomInput * zoomSpeed;
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, 10f, 80f);
    }

    private void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<PlayerCharacter>())
                    {
                        PlayerCharacter playerCharacter = hit.collider.gameObject.GetComponent<PlayerCharacter>();
                        SetTarget(playerCharacter.transform);
                    }
                }
            }
        }
    }

    private void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void MoveCameraTowardsTarget()
    {
        if (target != null)
        {
            float originalY = transform.position.y;
            float offsetX = target.position.x - transform.position.x;
            float offsetY = target.position.y - transform.position.y;
            float offsetZ = target.position.z - transform.position.z;

            Vector3 newPosition = new Vector3(transform.position.x + offsetX, originalY, transform.position.z + offsetZ - 4f);
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

            if (Vector3.Distance(transform.position, newPosition) < 0.01f)
            {
                target = null;
            }
        }
    }

    public void ButtonClicked(PlayerCharacter player)
    {
        SetTarget(player.transform);
        isMovingTowardsTarget = true;
    }
}
