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
    Vector3 _initStartCameraPos;

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
        _parallaxRangeStructs = _parallaxRangeStructs.ToList()
                .OrderBy(range => range.LActivePoint.transform.position.x)
                .ToArray();
        for (int i = 0; i < _parallaxRangeStructs.Length; i++)
        {
            var range = _parallaxRangeStructs[i];

            // init cam pos
            range.LCamStartX = range.LActivePoint.position.x > _initStartCameraPos.x ? range.LActivePoint.position.x : _initStartCameraPos.x;
            range.RCamStartX = range.RActivePoint.position.x > _initStartCameraPos.x ? range.RActivePoint.position.x : _initStartCameraPos.x;

            if (i == 0)
            {
                range.RangeStartPos = _initStartPos;
                range.RangeEndPos = range.RangeStartPos;
                range.RangeEndPos.x += Mathf.Min(
                    Mathf.Abs(range.RActivePoint.position.x - _startCameraPos.x), Mathf.Abs(range.RActivePoint.position.x - range.LActivePoint.position.x))
                    * range.MultiplierX;
                continue;
            }

            var preRange = _parallaxRangeStructs[i - 1];
            range.RangeStartPos = preRange.RangeEndPos;
            range.RangeEndPos = range.RangeStartPos;
            range.RangeEndPos.x += (range.RActivePoint.position.x - range.LActivePoint.position.x) * range.MultiplierX;

        }
    }

    void UpdateRangeIn(RangeState state, int i)
    {
        _curActiveRangeIndex = i;

        _startCameraPos = _cameraTransform.position;
        var range = _parallaxRangeStructs[i];
        if (state == RangeState.LeftIn)
        {
            _startPos = range.RangeStartPos;
            _startCameraPos.x = range.LCamStartX;

        }
        else
        {
            _startPos = range.RangeEndPos;
            _startCameraPos.x = range.RCamStartX;
        }
        _startPos.y = transform.position.y;
        _multiplierX = range.MultiplierX;
    }

    void UpdateRangeOut(RangeState state, int i)
    {
        if (_curActiveRangeIndex != i) return;

        _startCameraPos = _cameraTransform.position;
        var range = _parallaxRangeStructs[i];
        if (state == RangeState.RightOut)
        {
            _startPos = range.RangeEndPos;
            _startCameraPos.x = range.RCamStartX;
        }
        else
        {
            _startPos = range.RangeStartPos;
            _startCameraPos.x = range.LCamStartX;
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
               state => UpdateRangeIn(state, index),
               state => UpdateRangeOut(state, index)
               );
        }
    }
}