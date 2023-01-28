using UnityEngine;

public class CameraRangeTester : RangeTester
{
    Transform _cameraTransform;

    Vector3 _initStartCameraPos;
    Vector3 _startCameraPos;
    public Vector3 StartCameraPos => _startCameraPos;

    private float _lCamStartX;
    private float _rCamStartX;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _initStartCameraPos = _startCameraPos;
    }

    public void Init(Transform lActivePoint, Transform rActivePoint)
    {
        // init cam pos
        _lCamStartX = lActivePoint.position.x > _initStartCameraPos.x ? lActivePoint.position.x : _initStartCameraPos.x;
        _rCamStartX = rActivePoint.position.x > _initStartCameraPos.x ? rActivePoint.position.x : _initStartCameraPos.x;

        base.Init(_cameraTransform, lActivePoint, rActivePoint);
    }

    override protected void BeforeSetState(RangeState state)
    {
        // reset the start camera position whenever the range state is changed
        _startCameraPos = _cameraTransform.position;
        switch (state)
        {
            case RangeState.LeftIn:
                _startCameraPos.x = _lCamStartX;
                break;
            case RangeState.RightIn:
                _startCameraPos.x = _rCamStartX;
                break;
            case RangeState.LeftOut:
                _startCameraPos.x = _lCamStartX;
                break;
            case RangeState.RightOut:
                _startCameraPos.x = _rCamStartX;
                break;
            default:
                break;
        }

    }
}