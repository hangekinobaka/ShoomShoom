using UnityEngine;

/// <summary>
/// This part's multiplier is based on anoter layer
/// </summary>
public class ParallaxParts : MonoBehaviour
{
    [Header("Activate Point")]
    [SerializeField] bool _hasActivePoint = true;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _lActivePoint;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _rActivePoint;

    [Header("Local")]
    [SerializeField] float _plusMultiplier = 0.0f;
    float _curPlusMultiplier = 0.0f;

    [Header("Limits")]
    [SerializeField] bool _hasLimitY = false;
    [ConditionalDisplay("_hasLimitY", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _initStartCameraPos;
    Vector3 _startPos;
    Vector3 _plusPos = Vector3.zero;

    Vector3 _rangeStartPos;
    Vector3 _rangeEndPos;

    float _lCamStartX;
    float _rCamStartX;

    RangeTester _rangeTester;
    bool _inRangeOnce = false;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _initStartCameraPos = _startCameraPos;
        _startPos = transform.localPosition;

        _curPlusMultiplier = _plusMultiplier;

        if (_hasActivePoint)
        {
            InitRangePos();
            InitRangeTester();
        }

    }

    private void LateUpdate()
    {
        // Calculate the plus pos(with the _plusMultiplier)
        Vector3 plusDelta = _cameraTransform.position - _startCameraPos;
        _plusPos.x = plusDelta.x * _curPlusMultiplier;
        _plusPos.y = plusDelta.y * _plusMultiplier;

        if (_hasLimitY)
        {
            if (Mathf.Abs(_plusPos.y) > _maxDeltaY)
            {
                _plusPos.y = Mathf.Sign(_plusPos.y) * _maxDeltaY;
            }
        }

        // Calc the real position
        Vector3 localPosition = _startPos;
        localPosition += _plusPos;
        transform.localPosition = localPosition;
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
        _rangeTester = gameObject.AddComponent<RangeTester>();
        _rangeTester.OnInRangeHandler = UpdateRangeIn;
        _rangeTester.OnOutRangeHandler = UpdateRangeOut;
        _rangeTester.Init(_cameraTransform, _lActivePoint, _rActivePoint);
    }
}