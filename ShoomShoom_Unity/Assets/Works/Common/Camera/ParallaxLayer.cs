using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] float _multiplier = 0.0f;
    [SerializeField] bool _horizontalOnly = true;
    public float Multiplier => _multiplier;
    public bool HorizontalOnly => _horizontalOnly;
    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _startPos;
    public Vector3 StartCameraPos => _startCameraPos;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.position;
    }


    private void LateUpdate()
    {
        Vector3 position = _startPos;
        Vector3 delta = _cameraTransform.position - _startCameraPos;

        if (_horizontalOnly)
            position.x += _multiplier * delta.x;
        else
            position += _multiplier * delta;

        transform.position = position;
    }

}