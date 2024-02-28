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
        // Déplacement horizontal de la caméra
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calcul du déplacement horizontal selon la direction de la caméra (axes X et Z uniquement)
        Vector3 forwardDirection = virtualCamera.transform.forward.normalized;
        forwardDirection.y = 0; // Ignorer la composante Y pour le déplacement vertical
        Vector3 rightDirection = virtualCamera.transform.right.normalized;
        rightDirection.y = 0; // Ignorer la composante Y pour le déplacement horizontal
        Vector3 moveDirection = (forwardDirection * verticalInput + rightDirection * horizontalInput).normalized;

        // Appliquer le déplacement avec la vitesse de déplacement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotation de la caméra sur l'axe Y
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

        // Empêcher la rotation de la caméra lorsque le bouton droit de la souris est enfoncé
        if (Input.GetMouseButton(1))
        {
            return;
        }

        // Zoom de la caméra avec la molette de la souris (inverse le sens)
        float zoomInput = -Input.GetAxis("Mouse ScrollWheel");
        virtualCamera.m_Lens.FieldOfView += zoomInput * zoomSpeed;

        // Limiter le zoom de la caméra
        virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(virtualCamera.m_Lens.FieldOfView, 10f, 60f);
    }
}
