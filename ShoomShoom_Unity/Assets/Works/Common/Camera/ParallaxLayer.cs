using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected float _multiplierX = 0.0f;
    [SerializeField] protected float _multiplierY = 0.0f;

    protected Transform _cameraTransform;

    protected Vector3 _startCameraPos;
    public Vector3 StartCameraPos => _startCameraPos;
    protected Vector3 _startPos;

    protected Vector3 _delta;
    public Vector3 Delta => _delta;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _startPos = transform.position;
    }

    private void LateUpdate()
    {
        _delta = Vector3.zero;
        UpdatePos();
    }

    protected void UpdatePos()
    {
        Vector3 position = _startPos;
        _delta = _cameraTransform.position - _startCameraPos;

        _delta.x = _multiplierX * _delta.x;
        _delta.y = _multiplierY * _delta.y;

        position += _delta;
        transform.position = position;
    }

}