using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobRotation : MonoBehaviour
{
    [SerializeField] private Camera Cam = null;
    public float rotationSpeed = 10f;  // Velocidad de rotaci�n del knob

    private bool isRotating = false;  // Bandera para indicar si el knob est� siendo rotado
    private Vector3 lastMousePosition;  // �ltima posici�n registrada del mouse


    private void OnMouseDown()
    {
        
        isRotating = true;
        lastMousePosition = Input.mousePosition;
    }

    // M�todo llamado cuando se suelta el clic del mouse
    private void OnMouseUp()
    {
        
        isRotating = false;
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 50))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ButtonPress();
                }
            }
        }
        else
        {
            isRotating = false;
        }*/



        if (isRotating)
        {
            print("apretado");
            // Calcular la diferencia de posici�n del mouse desde la �ltima actualizaci�n
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Calcular la rotaci�n en funci�n de la diferencia de posici�n
            float rotationAmount = -mouseDelta.y * rotationSpeed * Time.deltaTime;

            // Obtener la rotaci�n actual del knob
            Vector3 currentRotation = transform.localEulerAngles;

            // Aplicar la rotaci�n al knob
            transform.localEulerAngles = new Vector3(currentRotation.x , currentRotation.y + rotationAmount, currentRotation.z);

            // Actualizar la �ltima posici�n del mouse
            lastMousePosition = Input.mousePosition;
        }
    }

    /*void ButtonPress()
    {
        isRotating = true;
        lastMousePosition = Input.mousePosition;
    }*/

}
