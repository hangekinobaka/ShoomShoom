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
    [SerializeField] bool _hasLimitX = false;
    [ConditionalDisplay("_hasLimitX", true)]
    [SerializeField] float _maxDeltaX = 1.0f;
    [SerializeField] bool _hasLimitY = false;
    [ConditionalDisplay("_hasLimitY", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _startPos;
    Vector3 _plusPos = Vector3.zero;

    RangeTester _rangeTester;
    bool _inRangeOnce = false;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.localPosition;

        if (_hasActivePoint)
        {
            InitRangeTester();
        }

        _curPlusMultiplier = _plusMultiplier;
    }

    private void LateUpdate()
    {
        // Calculate the plus pos(with the _plusMultiplier)
        Vector3 plusDelta = _cameraTransform.position - _startCameraPos;
        _plusPos = plusDelta * _curPlusMultiplier;

        // Limit the pluspos if there is a limitation set
        if (_hasLimitX)
        {
            if (Mathf.Abs(_plusPos.x) > _maxDeltaX)
            {
                _plusPos.x = Mathf.Sign(_plusPos.x) * _maxDeltaX;
            }
        }
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

    void UpdateRangeIn()
    {
        if (!_inRangeOnce)
        {
            _startPos = transform.localPosition;
            _startCameraPos = _cameraTransform.position;
            _inRangeOnce = true;
        }
        _curPlusMultiplier = _plusMultiplier;
    }

    void UpdateRangeOut()
    {
        _startPos = transform.localPosition;
        _startCameraPos = _cameraTransform.position;
        _curPlusMultiplier = 0;
    }

    void InitRangeTester()
    {
        _rangeTester = gameObject.AddComponent<RangeTester>();
        _rangeTester.OnInRangeHandler = _ => UpdateRangeIn();
        _rangeTester.OnOutRangeHandler = _ => UpdateRangeOut();
        _rangeTester.Init(_cameraTransform, _lActivePoint, _rActivePoint);
    }
}