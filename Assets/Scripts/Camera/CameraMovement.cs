using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float perspectiveZoomSpeed = 0.5f;
    [SerializeField] private float minFieldOfView;
    [SerializeField] private float maxFieldOfView;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Touch initTouch = new Touch();
    private Vector3 originRot;

    private ChessBoardManager chessManager;

    void Awake() {
        chessManager = GameObject.FindObjectOfType<ChessBoardManager>();
        originRot = this.transform.eulerAngles;
        xRotation = originRot.x;
        yRotation = originRot.y;
    }

    void Update() {
        if (IsZooming())
            CameraZoom();
        else
            if (IsCameraRotating())
                CameraRotation();

       ResumeTheGame();
    }

    private void ResumeTheGame() {
        if (chessManager.IsGameplayOnHold())
            return;
        chessManager.ResumeTheGame();
    }

    private bool IsZooming() {
        return (Input.touchCount == 2);
    }
    private bool IsCameraRotating() {
        return (Input.touchCount > 0);
    }

    private void CameraRotation() {
        chessManager.GameIsOnHold();
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
                initTouch = touch;
            if (touch.phase == TouchPhase.Moved)
                RotateCamera(touch);
            if (touch.phase == TouchPhase.Ended)
                initTouch = new Touch();
        }
    }

    private void RotateCamera(Touch currTouch) {
        float deltaX = initTouch.position.x - currTouch.position.x;
        float deltaY = initTouch.position.y - currTouch.position.y;
        xRotation -= deltaY * Time.deltaTime * rotationSpeed;
        yRotation -= deltaX * Time.deltaTime * rotationSpeed;

        this.transform.eulerAngles = new Vector3(xRotation, yRotation, 0f);
    }

    private void CameraZoom() {
        chessManager.GameIsOnHold();
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        Vector3 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector3 touch2PrevPos = touch2.position - touch2.deltaPosition;

        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float touchDeltaMag = (touch1.position - touch2.position).magnitude;
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, minFieldOfView, maxFieldOfView);
    }
}
