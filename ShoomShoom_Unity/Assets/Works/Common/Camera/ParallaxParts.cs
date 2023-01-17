using UnityEngine;

/// <summary>
/// This part's multiplier is based on anoter layer
/// </summary>
public class ParallaxParts : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] ParallaxLayer _baseLayer;

    [Header("Local")]
    [SerializeField] float _plusMultiplier = 0.0f;
    [SerializeField] bool _hasLimit = false;
    [ConditionalDisplay("_hasLimit", true)]
    [SerializeField] float _maxDeltaX = 1.0f;
    [ConditionalDisplay("_hasLimit", true)]
    [SerializeField] float _maxDeltaY = 1.0f;

    float _multiplier = 0.0f;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _startPos;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.position;

        _multiplier = _baseLayer.Multiplier + _plusMultiplier;
    }


    private void LateUpdate()
    {
        float multiplierX = _multiplier,
            multiplierY = _multiplier;

        Vector3 delta = _cameraTransform.position - _startCameraPos;
        Vector3 position = _startPos;
        Vector3 plusPos = Vector3.zero;

        if (_hasLimit)
        {
            Vector3 posDelta = transform.position - _startPos;
            if (Mathf.Abs(posDelta.x) > _maxDeltaX)
            {
                multiplierX = _baseLayer.Multiplier;
                plusPos.x = Mathf.Sign(posDelta.x) * _maxDeltaX;
            }
            if (Mathf.Abs(posDelta.y) > _maxDeltaY)
            {
                multiplierY = _baseLayer.Multiplier;
                plusPos.y = Mathf.Sign(posDelta.y) * _maxDeltaY;
            }
            delta -= plusPos / _multiplier;
        }

        if (_baseLayer.HorizontalOnly)
            position.x += multiplierX * delta.x + plusPos.x;
        else
        {
            position.x += multiplierX * delta.x + plusPos.x;
            position.y += multiplierY * delta.y + plusPos.y;
        }

        transform.position = position;
    }

}