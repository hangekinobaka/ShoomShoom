using UnityEngine;


public class ParallaxPartsBase : MonoBehaviour
{
    [Header("Local")]
    [SerializeField] protected float _plusMultiplier = 0.0f;
    protected float _curPlusMultiplier = 0.0f;

    [Header("Limits")]
    [SerializeField] bool _hasLimitY = false;
    [ConditionalDisplay("_hasLimitY", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    protected Transform _cameraTransform;

    protected Vector3 _startCameraPos;
    protected Vector3 _initStartCameraPos;
    protected Vector3 _startPos;
    protected Vector3 _initStartPos;
    protected Vector3 _plusPos = Vector3.zero;

    virtual protected void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _initStartCameraPos = _startCameraPos;
        _startPos = transform.localPosition;
        _initStartPos = _startPos;

        _curPlusMultiplier = _plusMultiplier;

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

}