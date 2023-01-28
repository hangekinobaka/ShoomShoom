using System.Linq;
using UniRx;
using UnityEngine;

public class ParallaxRangeBase : ParallaxLayer
{
    protected ParallaxRangeStructBase[] _structs;

    Vector3 _initStartPos;
    Vector3 _initStartCameraPos;

    protected int _curActiveRangeIndex = 0;
    public int CurActiveRangeIndex => _curActiveRangeIndex;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
        _initStartCameraPos = _startCameraPos;

        _startPos = transform.position;
        _initStartPos = transform.position;

        InitRanges();
        InitRangeTester();
    }

    private void LateUpdate()
    {
        UpdatePos();
    }

    void InitRanges()
    {
        // The program will always follow the left priority principal, 
        // so set the scene this way!!!!!!!!

        // sort the ranges in case the definition is not correct
        _structs = _structs.ToList()
                .OrderBy(range => range.LActivePoint.transform.position.x)
                .ToArray();

        for (int i = 0; i < _structs.Length; i++)
        {
            var range = _structs[i];

            if (i == 0)
            {
                range.RangeStartPos = _initStartPos;
                range.RangeEndPos = range.RangeStartPos;
                range.RangeEndPos.x += Mathf.Min(
                        Mathf.Abs(range.RActivePoint.position.x - _initStartCameraPos.x),
                        Mathf.Abs(range.RActivePoint.position.x - range.LActivePoint.position.x)
                    ) * range.MultiplierX;
                continue;
            }

            var preRange = _structs[i - 1];
            range.RangeStartPos = preRange.RangeEndPos;
            range.RangeEndPos = range.RangeStartPos;
            range.RangeEndPos.x += (range.RActivePoint.position.x - range.LActivePoint.position.x) * range.MultiplierX;

        }
    }

    void UpdateRangeIn(RangeState state, int i)
    {
        _curActiveRangeIndex = i;

        var range = _structs[i];
        if (state == RangeState.LeftIn)
        {
            _startPos = range.RangeStartPos;
        }
        else
        {
            _startPos = range.RangeEndPos;
        }
        _startCameraPos = range.RangeTester.StartCameraPos;
        _startPos.y = transform.position.y;
        _multiplierX = range.MultiplierX;
    }

    void UpdateRangeOut(RangeState state, int i)
    {
        if (_curActiveRangeIndex != i) return;

        var range = _structs[i];
        if (state == RangeState.RightOut)
        {
            _startPos = range.RangeEndPos;
        }
        else
        {
            _startPos = range.RangeStartPos;
        }
        _startCameraPos = range.RangeTester.StartCameraPos;
        _startPos.y = transform.position.y;
        _multiplierX = 0;
    }

    void InitRangeTester()
    {
        for (int i = 0; i < _structs.Length; i++)
        {
            var range = _structs[i];
            int index = i;
            CameraRangeTester tester = range.InitRangeTester();

            tester.CurRangeState.State.Subscribe(state =>
            {
                switch (state)
                {
                    case RangeState.LeftIn:
                    case RangeState.RightIn:
                        UpdateRangeIn(state, index);
                        break;
                    case RangeState.LeftOut:
                    case RangeState.RightOut:
                        UpdateRangeOut(state, index);
                        break;
                    default:
                        break;
                }
            }).AddTo(this);
        }
    }

}