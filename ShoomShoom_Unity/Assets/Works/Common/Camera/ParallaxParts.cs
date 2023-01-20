using UnityEngine;

/// <summary>
/// This part's multiplier is based on anoter layer
/// </summary>
public class ParallaxPartsCustomParent : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] float _baseHorMultiplier;
    [SerializeField] float _baseVerMultiplier;

    [Header("Activate Point")]
    [SerializeField] bool _hasActivePoint = true;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _lActivePoint;
    [ConditionalDisplay("_hasActivePoint", true)]
    [SerializeField] Transform _rActivePoint;

    [Header("Local")]
    [SerializeField] float _plusHorMultiplier = 0.0f;
    [SerializeField] float _plusVerMultiplier = 0.0f;
    [SerializeField] bool _hasLimitX = false;
    [ConditionalDisplay("_hasLimitX", true)]
    [SerializeField] float _maxDeltaX = 1.0f;
    [SerializeField] bool _hasLimitY = false;
    [ConditionalDisplay("_hasLimitY", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _startCameraPosInRange;
    bool _cameraPosInited = false;
    Vector3 _startPos;
    Vector3 _plusPos = Vector3.zero;

    RangeTester _rangeTester;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startPos = transform.position;
        _startCameraPos = _cameraTransform.position;
        if (_hasActivePoint)
        {
            _rangeTester = gameObject.AddComponent<RangeTester>();
            _rangeTester.OnInRangeHandler = _ => InitRangeCameraPos();
            _rangeTester.Init(_cameraTransform, _lActivePoint, _rActivePoint);
        }
        else
        {
            InitRangeCameraPos();
        }
    }

    private void LateUpdate()
    {
        Vector3 position = _startPos;

        // Calculate the plus pos(with the _plusMultiplier)
        if (_hasActivePoint)
        {
            if (_cameraPosInited)
            {
                switch (_rangeTester.CurRangeState)
                {
                    case RangeState.LeftIn:
                    case RangeState.RightIn:
                        Vector3 plusDelta = _cameraTransform.position - _startCameraPos;
                        _plusPos.x = plusDelta.x * _plusHorMultiplier;
                        _plusPos.y = plusDelta.y * _plusVerMultiplier;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            Vector3 plusDelta = _cameraTransform.position - _startCameraPos;
            _plusPos.x = plusDelta.x * _plusHorMultiplier;
            _plusPos.y = plusDelta.y * _plusVerMultiplier;
        }

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
        Vector3 delta = _cameraTransform.position - _startCameraPos;
        position.x += _baseHorMultiplier * delta.x + _plusPos.x;
        position.y += _baseVerMultiplier * delta.y + _plusPos.y;

        transform.position = position;
    }

    void InitRangeCameraPos()
    {
        if (_cameraPosInited) return;
        _startCameraPosInRange = _cameraTransform.position;
        _cameraPosInited = true;
    }
}