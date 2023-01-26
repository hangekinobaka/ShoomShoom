using System;
using UnityEngine;
[Serializable]
public class ParallaxRangeStruct
{
    [Header("Activate Point")]
    [SerializeField] Transform _lActivePoint;
    [SerializeField] Transform _rActivePoint;
    public Transform LActivePoint => _lActivePoint;
    public Transform RActivePoint => _rActivePoint;

    [Header("Range multiplier")]
    public float MultiplierX = 0.0f;

    [HideInInspector]
    public Vector3 RangeStartPos;
    [HideInInspector]
    public Vector3 RangeEndPos;

    public RangeTester RangeTester { get; private set; }

    public float LCamStartX { get; set; }
    public float RCamStartX { get; set; }

    public void InitRangeTester(GameObject go, Transform camPos, Action<RangeState> callbackIn, Action<RangeState> callbackOut)
    {
        RangeTester = go.AddComponent<RangeTester>();
        RangeTester.OnInRangeHandler = callbackIn;
        RangeTester.OnOutRangeHandler = callbackOut;
        RangeTester.Init(camPos, _lActivePoint, _rActivePoint);
    }

}