using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private float targetZoom;

    private float zoomMultiplier = 5f;
    private float minZoom = 4f;
    private float maxZoom = 60f;
    private float smoothTime = 0.15f;

    private float zoomVelocity = 0f;

    private float moveSpeed = 30f;

    GameObject gridManagerObject;
    GridManager gridManager;

    void Start()
    {
        gridManagerObject = GameObject.Find("GameManager");

        if (gridManagerObject != null) {
            gridManager = gridManagerObject.GetComponent<GridManager>();
        }
        cam = GetComponent<Camera>();
        targetZoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        // get scrolling
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f) {
            targetZoom -= scroll * zoomMultiplier;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            smoothTime
        );

        // get camera movement
        if (Input.GetKey("w")) {
            transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeed * Time.deltaTime, transform.position.z);
        }
        if (Input.GetKey("a")) {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
        if (Input.GetKey("s")) {
            transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed * Time.deltaTime, transform.position.z);
        }
        if (Input.GetKey("d")) {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 0, gridManager.width), Mathf.Clamp(transform.position.y, 0, gridManager.height), transform.position.z);
    }
}
