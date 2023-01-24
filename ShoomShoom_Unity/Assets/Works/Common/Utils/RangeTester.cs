using System;
using UnityEngine;

public class RangeTester : MonoBehaviour
{
    [Header("State Display:")]
    [SerializeField] RangeState _curRangeState = RangeState.LeftOut;
    RangeState _prevRangeState = RangeState.LeftOut;

    public Transform LActivePoint { get; set; }
    public Transform RActivePoint { get; set; }
    public Transform Target { get; set; }
    public RangeState CurRangeState => _curRangeState;

    public Action<RangeState> OnInRangeHandler { get; set; }
    public Action<RangeState> OnOutRangeHandler { get; set; }

    public bool EnableRangeTest = true;

    public void Init(Transform target, Transform lActivePoint, Transform rActivePoint)
    {
        LActivePoint = lActivePoint;
        RActivePoint = rActivePoint;
        Target = target;
        EnableRangeTest = true;

        _prevRangeState = TestPosInit();
        switch (_prevRangeState)
        {
            case RangeState.LeftIn:
            case RangeState.RightIn:
                OnInRangeHandler.Invoke(_curRangeState);
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!EnableRangeTest) return;

        _curRangeState = TestPos();

        switch (_curRangeState)
        {
            case RangeState.LeftOut:
                if (_prevRangeState == RangeState.RightIn || _prevRangeState == RangeState.LeftIn)
                    OnOutRangeHandler?.Invoke(_curRangeState);
                break;
            case RangeState.LeftIn:
                if (_prevRangeState == RangeState.LeftOut)
                    OnInRangeHandler?.Invoke(_curRangeState);
                break;
            case RangeState.RightOut:
                if (_prevRangeState == RangeState.RightIn || _prevRangeState == RangeState.LeftIn)
                    OnOutRangeHandler?.Invoke(_curRangeState);
                break;
            case RangeState.RightIn:
                if (_prevRangeState == RangeState.RightOut)
                    OnInRangeHandler?.Invoke(_curRangeState);
                break;
            default:
                break;
        }

        _prevRangeState = _curRangeState;
    }

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
