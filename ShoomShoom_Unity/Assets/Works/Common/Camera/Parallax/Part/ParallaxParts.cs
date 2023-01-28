using UniRx;
using UnityEngine;

/// <summary>
/// This part's multiplier is based on anoter layer
/// </summary>
public class ParallaxParts : ParallaxPartsBase
{
    [Header("Activate Point")]
    [SerializeField] bool _hasActivePoint = true;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _lActivePoint;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _rActivePoint;

    Vector3 _rangeStartPos;
    Vector3 _rangeEndPos;

    float _lCamStartX;
    float _rCamStartX;

    CameraRangeTester _rangeTester;
    bool _inRangeOnce = false;

    override protected void Start()
    {
        base.Start();
        if (_hasActivePoint)
        {
            InitRangePos();
            InitRangeTester();
        }

    }

    void InitRangePos()
    {
        _lCamStartX = _lActivePoint.position.x > _initStartCameraPos.x ? _lActivePoint.position.x : _initStartCameraPos.x;
        _rCamStartX = _rActivePoint.position.x > _initStartCameraPos.x ? _rActivePoint.position.x : _initStartCameraPos.x;

        _rangeStartPos = _startPos;
        _rangeEndPos = _startPos;

        _rangeEndPos.x += Mathf.Min(
                Mathf.Abs(_rActivePoint.position.x - _lActivePoint.position.x),
                Mathf.Abs(_rActivePoint.position.x - _initStartCameraPos.x))
            * _plusMultiplier;

    }

    void UpdateRangeIn(RangeState state)
    {
        if (!_inRangeOnce)
        {
            _inRangeOnce = true;
            _startCameraPos = _cameraTransform.position;
            if (state == RangeState.LeftIn)
            {
                _startPos = _rangeStartPos;
                _startCameraPos.x = _lCamStartX;
            }
            else
            {
                _startPos = _rangeEndPos;
                _startCameraPos.x = _rCamStartX;
            }
            _startPos.y = transform.localPosition.y;
        }

        _curPlusMultiplier = _plusMultiplier;
    }

    void UpdateRangeOut(RangeState state)
    {
        _startCameraPos = _cameraTransform.position;
        if (state == RangeState.LeftOut)
        {
            _startPos = _rangeStartPos;
            _startCameraPos.x = _lCamStartX;
        }
        else
        {
            _startPos = _rangeEndPos;
            _startCameraPos.x = _rCamStartX;
        }
        _startPos.y = transform.localPosition.y;
        _curPlusMultiplier = 0;
    }

    void InitRangeTester()
    {
        _rangeTester = CameraRangeManager.Instance.AddRangeTester(_lActivePoint, _rActivePoint);

        _rangeTester.CurRangeState.State.Subscribe(state =>
        {
            switch (state)

            {
                case RangeState.LeftIn:
                case RangeState.RightIn:
                    UpdateRangeIn(state);
                    break;
                case RangeState.LeftOut:
                case RangeState.RightOut:
                    UpdateRangeOut(state);
                    break;
                default:
                    break;
            }
        }).AddTo(this);
    }
}