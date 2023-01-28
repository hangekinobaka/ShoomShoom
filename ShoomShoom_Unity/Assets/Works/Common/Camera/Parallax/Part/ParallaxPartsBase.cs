using UnityEngine;


public class ParallaxPartsBase : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected float _plusMultiplierX = 0.0f;
    [SerializeField] protected float _plusMultiplierY = 0.0f;

    [Header("Limits")]
    [SerializeField] bool _hasLimitY = false;
    [ConditionalDisplay("_hasLimitY", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    protected Transform _cameraTransform;

    protected Vector3 _startCameraPos;
    protected Vector3 _startPos;
    protected Vector3 _plusPos = Vector3.zero;

    virtual protected void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        // Calculate the plus pos(with the _plusMultiplier)
        Vector3 plusDelta = _cameraTransform.position - _startCameraPos;
        _plusPos.x = plusDelta.x * _plusMultiplierX;
        _plusPos.y = plusDelta.y * _plusMultiplierY;

        // Limit the Y axis
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