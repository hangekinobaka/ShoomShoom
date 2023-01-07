using UnityEngine;

public class ParallaxLayerFollow : MonoBehaviour
{
    [SerializeField] float _multiplier = 0.0f;

    Transform _cameraTransform;

    Vector3 _startCameraPos;
    Vector3 _origPos;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _origPos = transform.position;
    }


    private void LateUpdate()
    {
        Vector3 deltaPos = _cameraTransform.position - _startCameraPos;

        var position = _origPos;
        position.x += deltaPos.x;

        position.y += _multiplier * deltaPos.y;

        transform.position = position;
    }

}