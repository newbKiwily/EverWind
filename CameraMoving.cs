using System;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public static Action OnCameraTClear;

    public float Yaxis;
    public float Xaxis;
    public Transform target;

    public float rotSensitive = 2.0f;
    private float dis = 20.0f;

    private float RotationMin = -10f;
    private float RotationMax = 80f;
    private float smoothTime = 0.05f;

    private Vector3 targetRotation;
    private Vector3 currentVel;
    private float cameraMoveTime;

    public GameObject previewCamera;

    public float zoomSensitive = 8.0f;
    private float minDis = 8.0f;
    private float maxDis = 20.0f;

    private Vector3 previewOffset = new Vector3(0, 2f, -4f);
    private float previewFollowSpeed = 50.0f;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraMoveTime = 5.0f;
    }

    void LateUpdate()
    {
        if (InputManager.Instance == null || target == null)
            return;

        // ===== 메인 카메라 =====
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            dis -= scroll * zoomSensitive;
            dis = Mathf.Clamp(dis, minDis, maxDis);
        }

        if (InputManager.Instance.GetControlCamera())
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;
            Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

            targetRotation = Vector3.SmoothDamp(
                targetRotation,
                new Vector3(Xaxis, Yaxis),
                ref currentVel,
                smoothTime
            );

            cameraMoveTime -= Time.deltaTime;
            if (cameraMoveTime <= 0)
                OnCameraTClear?.Invoke();
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        transform.eulerAngles = targetRotation;
        transform.position = target.position - transform.forward * dis;


        if (previewCamera != null && previewCamera.activeSelf)
        {
            Transform pCam = previewCamera.transform;

            
            float frontDistance = 4f;   
            float height = 2f;          

            Vector3 desiredPos = target.position + (target.forward * frontDistance) + (Vector3.up * height);

            pCam.position = Vector3.Lerp(pCam.position, desiredPos, Time.deltaTime * previewFollowSpeed);

            Vector3 lookPos = target.position + Vector3.up * 1.2f;
            pCam.LookAt(lookPos);
        }
    }

    public void onPreviewCam()
    {
        if (previewCamera != null)
            previewCamera.SetActive(true);
    }

    public void offPreviCam()
    {
        if (previewCamera != null)
            previewCamera.SetActive(false);
    }
}
