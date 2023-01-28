using System;
using UnityEngine;

[Serializable]
public class ParallaxRangeStructBase
{
    [Header("Activate Point")]
    [SerializeField] Transform _lActivePoint;
    [SerializeField] Transform _rActivePoint;
    public Transform LActivePoint => _lActivePoint;
    public Transform RActivePoint => _rActivePoint;

    public float MultiplierX { get; set; }

    [HideInInspector]
    public Vector3 RangeStartPos;
    [HideInInspector]
    public Vector3 RangeEndPos;

    public CameraRangeTester RangeTester { get; private set; }

    public float LCamStartX { get; set; }
    public float RCamStartX { get; set; }

    public CameraRangeTester InitRangeTester()
    {
        RangeTester = CameraRangeManager.Instance.AddRangeTester(_lActivePoint, _rActivePoint);

        return RangeTester;
    }
}