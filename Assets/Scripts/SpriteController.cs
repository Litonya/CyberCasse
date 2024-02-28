using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Aligner la rotation du sprite avec celle de la caméra
        transform.LookAt(transform.position + mainCameraTransform.forward, mainCameraTransform.up);

        // Fixer la rotation sur l'axe X à 50 degrés
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = 50f;
        transform.eulerAngles = eulerAngles;
    }
}
