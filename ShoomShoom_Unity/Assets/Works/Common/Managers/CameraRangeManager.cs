using System.Collections.Generic;
using UnityEngine;

public class CameraRangeManager : Singleton<CameraRangeManager>
{
    Dictionary<string, CameraRangeTester> _testerMap = new Dictionary<string, CameraRangeTester>();

    public CameraRangeTester AddRangeTester(Transform lActivePoint, Transform rActivePoint)
    {
        string rangeName = SleepyUtil.RangeUtil.GetRangeName(lActivePoint, rActivePoint);

        // Return the tester if there is already one same tester exist
        if (_testerMap.ContainsKey(rangeName))
        {
            return _testerMap[rangeName];
        }

        CameraRangeTester tester = gameObject.AddComponent<CameraRangeTester>();
        tester.Init(lActivePoint, rActivePoint);
        _testerMap.Add(rangeName, tester);

        return tester;
    }
}
