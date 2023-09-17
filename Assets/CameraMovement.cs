using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float sensitivity = 100.0f;
    public bool rotateAroundPlayer = true;
    public Transform player;

    private float rotationAroundYAxis = 0.0f;
    private float rotationAroundXAxis = 0.0f;

    // Update is called once per frame
    void Update()
    {
        // Basic camera movement controls
        float translationX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float translationY = 0;

        if (Input.GetKey(KeyCode.E))  // Move up
        {
            translationY = speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q))  // Move down
        {
            translationY = -speed * Time.deltaTime;
        }

        float translationZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(translationX, translationY, translationZ);

        // Mouse-based rotation controls
        if (Input.GetMouseButton(1)) // Right mouse button to rotate
        {
            rotationAroundYAxis += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            rotationAroundXAxis -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            rotationAroundXAxis = Mathf.Clamp(rotationAroundXAxis, -90, 90);

            if (rotateAroundPlayer)
            {
                Quaternion q = Quaternion.AngleAxis(rotationAroundYAxis, Vector3.up);
                q *= Quaternion.AngleAxis(rotationAroundXAxis, Vector3.right);
                Vector3 direction = q * new Vector3(0, 0, -1);
                transform.position = player.position + direction * 10.0f;
                transform.LookAt(player.position);
            }
            else
            {
                transform.rotation = Quaternion.Euler(rotationAroundXAxis, rotationAroundYAxis, 0);
            }
        }
    }
}
