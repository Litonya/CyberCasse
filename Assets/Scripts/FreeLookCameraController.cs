using UnityEngine;
using Cinemachine;

public class FreeLookCameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;
    public float zoomSpeed = 5f;
    public float yRotationSpeed = 2f; // Vitesse de rotation sur l'axe Y

    private void Update()
    {
        // D�placement horizontal de la cam�ra
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calcul du d�placement horizontal selon la direction de la cam�ra (axes X et Z uniquement)
        Vector3 forwardDirection = virtualCamera.transform.forward.normalized;
        forwardDirection.y = 0; // Ignorer la composante Y pour le d�placement vertical
        Vector3 rightDirection = virtualCamera.transform.right.normalized;
        rightDirection.y = 0; // Ignorer la composante Y pour le d�placement horizontal
        Vector3 moveDirection = (forwardDirection * verticalInput + rightDirection * horizontalInput).normalized;

        // Appliquer le d�placement avec la vitesse de d�placement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotation de la cam�ra sur l'axe Y
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -yRotationSpeed, Space.World);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, yRotationSpeed, Space.World);
        }
        else if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            transform.Rotate(Vector3.up, mouseX, Space.World);
        }

        // Emp�cher la rotation de la cam�ra lorsque le bouton droit de la souris est enfonc�
        if (Input.GetMouseButton(1))
        {
            return;
        }

        // Zoom de la cam�ra avec la molette de la souris (inverse le sens)
        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");
        virtualCamera.m_Lens.FieldOfView += zoomInput * zoomSpeed;

        // Limiter le zoom de la cam�ra
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, 10f, 60f);
    }
}
