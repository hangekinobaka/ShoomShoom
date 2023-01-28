using UnityEngine;

public class CameraRangeTester : RangeTester
{
    Transform _cameraPos;

    private void Awake()
    {
        _cameraPos = Camera.main.transform;
    }

    public void Init(Transform lActivePoint, Transform rActivePoint)
    {
        base.Init(_cameraPos, lActivePoint, rActivePoint);
    }
}