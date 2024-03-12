using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskTest : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) GetClickObject();
    }
    private void GetClickObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<PlayerCharacter>())
                {
                    Debug.Log("A PlayerCharacter is clicked");
                }
                else if (hit.collider.gameObject.GetComponent<Cell>())
                {
                    Debug.Log("Cell is selected");
                }
            }
        }
    }
}
