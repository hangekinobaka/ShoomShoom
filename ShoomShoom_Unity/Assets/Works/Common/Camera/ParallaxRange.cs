using System.Linq;
using UnityEngine;

/// <summary>
/// Define Parallax range by range
/// </summary>
public class ParallaxRange : ParallaxLayer
{
    [SerializeField] ParallaxRangeStruct[] _parallaxRangeStructs;

    Vector3 _initStartPos;

    int _curActiveRangeIndex = 0;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _startCameraPos = _cameraTransform.position;
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
        _parallaxRangeStructs = _parallaxRangeStructs.ToList()
                .OrderBy(range => range.LActivePoint.transform.position.x)
                .ToArray();
        for (int i = 0; i < _parallaxRangeStructs.Length; i++)
        {
            var range = _parallaxRangeStructs[i];
            if (i == 0)
            {
                range.RangeStartPos = _initStartPos;
                range.RangeEndPos = range.RangeStartPos;
                range.RangeEndPos.x += Mathf.Min(
                    (range.RActivePoint.position.x - _startCameraPos.x), (range.RActivePoint.position.x - _startCameraPos.x))
                    * range.MultiplierX;
                continue;
            }

            var preRange = _parallaxRangeStructs[i - 1];
            range.RangeStartPos = preRange.RangeEndPos;
            range.RangeEndPos = range.RangeStartPos;
            range.RangeEndPos.x += (range.RActivePoint.position.x - range.LActivePoint.position.x) * range.MultiplierX;
        }
    }

    void UpdateRangeIn(int i)
    {
        _curActiveRangeIndex = i;

        var range = _parallaxRangeStructs[i];
        if (range.RangeTester.CurRangeState == RangeState.LeftIn)
        {
            _startPos = range.RangeStartPos;
        }
        else
        {
            _startPos = range.RangeEndPos;
        }
        _startPos.y = transform.position.y;
        _startCameraPos = _cameraTransform.position;
        _multiplierX = range.MultiplierX;
    }

    void UpdateRangeOut(int i)
    {
        if (_curActiveRangeIndex != i) return;

        var range = _parallaxRangeStructs[i];
        if (range.RangeTester.CurRangeState == RangeState.RightOut)
        {
            _startPos = range.RangeEndPos;
        }
        else
        {
            _startPos = range.RangeStartPos;
        }
        _startPos.y = transform.position.y;
        _multiplierX = 0;
    }

    void InitRangeTester()
    {
        for (int i = 0; i < _parallaxRangeStructs.Length; i++)
        {
            var range = _parallaxRangeStructs[i];
            int index = i;
            range.InitRangeTester(gameObject,
               _cameraTransform,
               _ => UpdateRangeIn(index),
               _ => UpdateRangeOut(index)
               );
        }
    }
}