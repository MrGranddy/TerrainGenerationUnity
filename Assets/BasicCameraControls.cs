using UnityEngine;

public class BasicCameraControls : MonoBehaviour
{
    public float moveSpeed = 120.0f;
    public float zoomSpeed = 75.0f;
    public float rotateSpeed = 10.0f;

    public bool usePerspectiveZoom = true; // Set this to false if you're using an orthographic camera

    private float rotationY = 0f; // To keep track of vertical rotation

    void Update()
    {
        // Basic movement using WASD keys relative to the camera's direction
        float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float upward = 0;

        // Check for Shift or Control key presses
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            upward = moveSpeed * Time.deltaTime; // Move upwards
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            upward = -moveSpeed * Time.deltaTime; // Move downwards
        }

        transform.Translate(moveSide, upward, moveForward, Space.Self);

        // Zooming with mouse wheel
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (Camera.main.orthographic)
        {
            // For orthographic camera
            Camera.main.orthographicSize -= zoom;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 1, 20);
        }
        else
        {
            // For perspective camera
            Camera.main.fieldOfView -= zoom;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10, 90);
        }

        // Click and drag for 3D rotation
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

            rotationY -= mouseY;
            rotationY = Mathf.Clamp(rotationY, -80, 80); // Vertical angle range

            transform.localEulerAngles = new Vector3(
                rotationY,
                transform.localEulerAngles.y + mouseX,
                transform.localEulerAngles.z
            );
        }

        // Q and E for rotation around absolute Z axis
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, 10 * rotateSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, -10 * rotateSpeed * Time.deltaTime, 0);
        }

    }
}
