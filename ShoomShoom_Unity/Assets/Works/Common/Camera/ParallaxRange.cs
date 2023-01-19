using UnityEngine;

/// <summary>
/// Only move in a certain range
/// </summary>
public class ParallaxRange : ParallaxLayer
{
    [SerializeField] Transform _lActivePoint;
    [SerializeField] Transform _rActivePoint;

    bool _cameraPosInited = false;

    RangeTester _rangeTester;
    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startPos = transform.position;

        _rangeTester = gameObject.AddComponent<RangeTester>();
        _rangeTester.OnInRangeHandler = _ => InitCameraPos();
        _rangeTester.Init(_cameraTransform, _lActivePoint, _rActivePoint);
    }

    private void LateUpdate()
    {
        if (_cameraPosInited)
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

    void InitCameraPos()
    {
        if (_cameraPosInited) return;
        _startCameraPos = _cameraTransform.position;
        _cameraPosInited = true;
    }
}