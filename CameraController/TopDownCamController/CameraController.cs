using UnityEngine;

[System.Serializable]
public struct CameraFocusData
{
    public float zoomFactor;
    public Transform target;
}
[System.Serializable]
public struct ZoomData
{
    public float fov;
    public float distance;
    public float angle;
}

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector2 speedRange;
    [SerializeField]
    private float zoomSpeed = 0.1f; // Speed of the zoom


    [SerializeField]
    public ZoomData minZoomData;
    [SerializeField]
    public ZoomData maxZoomData;

    [SerializeField]
    public AnimationCurve angleCurve;
    [SerializeField]
    public AnimationCurve zoomCurve;

    private float currentZoomFactor = 0.16f;



    [field: SerializeField]
    public Camera mainCamera { get; private set; }

    [SerializeField]
    private Transform pivot;



    [SerializeField]
    private float focusSpeed = 1;
    public AnimationCurve focusCurve;

    public Vector2 areaLimit_X;
    public Vector2 areaLimit_Z;



    private float focusTime = 0;
    private CameraFocusData focusData;

    private float startZoomFactor;
    private Vector3 startPosition;

    private Vector2? touchStart;
    private bool wasZooming = false;

    private float speed;

    private bool lookAtTarget;

    private void Start()
    {
        UpdateWithZoomFactor();
    }

    void Update()
    {
        if (GlobalVariables.isMenuOpened)
            return;

        if (GlobalVariables.isUpgradeMenuOpened || GlobalVariables.isMapEditorMode || GlobalVariables.isTutorialActive)
        {
            if (focusData.target == null || focusData.zoomFactor < 0)
                return;

            if (focusTime < 1)
            {
                FocusTargetData();
            }
            else
            {
                FollowTarget();

            }
            return;
        }
        else if (lookAtTarget)
        {
            if (focusTime < 1)
            {
                FocusTargetData();
            }
            else
                lookAtTarget = false;

            return;
        }


        if (focusTime > 0)
        {

            focusTime -= Time.deltaTime * focusSpeed;
            if (focusTime < 0.01f)
                focusTime = 0;

            float _time = focusCurve.Evaluate(focusTime);

            currentZoomFactor = Mathf.Lerp(startZoomFactor, focusData.zoomFactor, _time);
            UpdateWithZoomFactor();

            return;
        }

        bool isZooming = CameraZoom();

        if (isZooming)
            wasZooming = true;

        if (!wasZooming)
            CameraMovement();

        if (Input.GetMouseButtonUp(0))
        {
            wasZooming = false;
        }
    }

    public void UpdateZoomFactor(float factor)
    {
        if (focusTime < 1)
        {
            focusData.zoomFactor = factor;
        }
        else
        {
            currentZoomFactor = factor;
            UpdateWithZoomFactor();
        }
    }

    public void SetCameraFocus(CameraFocusData focusData, bool lookAtTarget)
    {
        this.lookAtTarget = lookAtTarget;

        if (focusData.zoomFactor ==-1) {
            focusData.zoomFactor = currentZoomFactor;
        }

        SetCameraFocus(focusData);
    }

    public void SetCameraFocus(CameraFocusData focusData)
    {
        focusTime = 0;
        touchStart = null;
        this.focusData = focusData;
        startZoomFactor = currentZoomFactor;
        startPosition = transform.position;
    }

    public void ResetZoomData(float zoomFactor, Vector3 position)
    {
        startZoomFactor = zoomFactor;
        transform.position = position;
    }


    private void CameraMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            if (touchStart == null)
                touchStart = Input.mousePosition;

            Vector2 touchEnd = Input.mousePosition;
            Vector2 direction = touchStart.Value - touchEnd;


            Vector3 targetPosition = transform.position + (transform.right * direction.x + transform.forward * direction.y) * speed;

            targetPosition.x = Mathf.Clamp(targetPosition.x, areaLimit_X.x, areaLimit_X.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, areaLimit_Z.x, areaLimit_Z.y);

            transform.position = targetPosition;
            touchStart = touchEnd;
        }
    }


    private void FocusTargetData()
    {
        focusTime += Time.deltaTime * focusSpeed;

        if (focusTime > 1)
            focusTime = 1;


        float _time = focusCurve.Evaluate(focusTime);

        currentZoomFactor = Mathf.Lerp(startZoomFactor, focusData.zoomFactor, _time);
        UpdateWithZoomFactor();

        var newPosition = Vector3.Lerp(startPosition, focusData.target.position, _time);
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    private void FollowTarget()
    {
        var newPosition = focusData.target.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    private bool CameraZoom()
    {

#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStart = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 touchEnd = Input.mousePosition;
                Vector2 direction = touchEnd - touchStart.Value;

                currentZoomFactor += direction.y * zoomSpeed;
                currentZoomFactor = Mathf.Clamp(currentZoomFactor, 0, 0.7f);

                UpdateWithZoomFactor();

                touchStart = touchEnd;
            }

            return true;
        }
#else

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            currentZoomFactor += deltaMagnitudeDiff * zoomSpeed;
            currentZoomFactor = Mathf.Clamp(currentZoomFactor,0,0.7f);
            UpdateWithZoomFactor();

            return true;
        }
#endif

        return false;
    }

    private void UpdateWithZoomFactor()
    {
        currentZoomFactor = Mathf.Clamp01(currentZoomFactor);

        float zoomTime = zoomCurve.Evaluate(currentZoomFactor);
        float angleTime = angleCurve.Evaluate(currentZoomFactor);

        speed = Mathf.Lerp(speedRange.x, speedRange.y, zoomTime);
        mainCamera.fieldOfView = Mathf.Lerp(minZoomData.fov, maxZoomData.fov, zoomTime);
        pivot.localEulerAngles = Vector3.right * Mathf.Lerp(minZoomData.angle, maxZoomData.angle, angleTime);
        mainCamera.transform.localPosition = Vector3.forward * Mathf.Lerp(minZoomData.distance, maxZoomData.distance, zoomTime);
    }
}