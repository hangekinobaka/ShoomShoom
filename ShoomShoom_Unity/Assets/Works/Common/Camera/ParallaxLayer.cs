using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] float _multiplier = 0.0f;
    [SerializeField] bool _horizontalOnly = true;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _startPos;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.position;
    }


    private void LateUpdate()
    {
        var position = _startPos;
        if (_horizontalOnly)
            position.x += _multiplier * (_cameraTransform.position.x - _startCameraPos.x);
        else
            position += _multiplier * (_cameraTransform.position - _startCameraPos);

        transform.position = position;
    }

}