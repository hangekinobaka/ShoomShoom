using UnityEngine;

/// <summary>
/// Define Parallax range by range
/// </summary>
public class ParallaxRange : ParallaxLayer
{
    [Header("Activate Point")]
    [SerializeField] Transform _lActivePoint;
    [SerializeField] Transform _rActivePoint;

    bool _posInited = false;

    RangeTester _rangeTester;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.position;
        InitRangeTester();
    }

    private void LateUpdate()
    {

        if (_posInited)
        {
            switch (_rangeTester.CurRangeState)
            {
                case RangeState.LeftIn:
                case RangeState.RightIn:
                    UpdatePos();
                    break;
                default:
                    break;
            }
        }

    }

    void InitPos()
    {
        if (_posInited) return;
        _startCameraPos.x = _cameraTransform.position.x;
        _posInited = true;
    }

    void InitRangeTester()
    {
        _rangeTester = gameObject.AddComponent<RangeTester>();

        _rangeTester.OnInRangeHandler = _ => InitPos();

        _rangeTester.Init(_cameraTransform, _lActivePoint, _rActivePoint);
    }
}