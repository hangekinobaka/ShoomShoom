using System;
using UnityEngine;

public class RangeTester : MonoBehaviour
{
    [Header("Range Name")]
    public string RangeName;

    [Header("State Display:")]
    [SerializeField] protected RangeState _curRangeState = RangeState.LeftOut;

    RangeState _prevRangeState = RangeState.LeftOut;

    public Transform LActivePoint { get; set; }
    public Transform RActivePoint { get; set; }
    public Transform Target { get; set; }
    public ReactProps<RangeState> CurRangeState = new ReactProps<RangeState>();

    public Action<RangeState> OnInRangeHandler { get; set; }
    public Action<RangeState> OnOutRangeHandler { get; set; }

    public bool EnableRangeTest = true;

    public void Init(Transform target, Transform lActivePoint, Transform rActivePoint)
    {
        LActivePoint = lActivePoint;
        RActivePoint = rActivePoint;
        Target = target;
        EnableRangeTest = true;
        RangeName = SleepyUtil.RangeUtil.GetRangeName(lActivePoint, rActivePoint);

        _prevRangeState = TestPosInit();
        BeforeSetState(_prevRangeState);
        CurRangeState.SetState(_prevRangeState);
    }

    private void Update()
    {
        if (!EnableRangeTest) return;

        _curRangeState = TestPos();

        if (_curRangeState != _prevRangeState)
        {
            BeforeSetState(_curRangeState);
            CurRangeState.SetState(_curRangeState);
        }

        _prevRangeState = _curRangeState;
    }

    virtual protected void BeforeSetState(RangeState state) { }

    RangeState TestPosInit()
    {
        if (Target.position.x < LActivePoint.position.x)
        {
            return RangeState.LeftOut;
        }

        if (Target.position.x > RActivePoint.position.x)
        {
            return RangeState.RightOut;
        }

        if (Mathf.Abs(Target.position.x - LActivePoint.position.x) <=
            Mathf.Abs(Target.position.x - RActivePoint.position.x))
        {
            return RangeState.LeftIn;
        }

        return RangeState.RightIn;
    }
    RangeState TestPos()
    {
        if (Target.position.x < LActivePoint.position.x)
        {
            return RangeState.LeftOut;
        }

        if (Target.position.x > RActivePoint.position.x)
        {
            return RangeState.RightOut;
        }

        if (_prevRangeState == RangeState.LeftOut)
        {
            return RangeState.LeftIn;
        }

        if (_prevRangeState == RangeState.RightOut)
        {
            return RangeState.RightIn;
        }

        return _curRangeState;
    }
}
