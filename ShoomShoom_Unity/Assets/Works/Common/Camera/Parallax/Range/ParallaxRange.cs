using UnityEngine;

/// <summary>
/// Define Parallax range by range
/// </summary>
public class ParallaxRange : ParallaxRangeBase
{
    [SerializeField] ParallaxRangeStruct[] _parallaxRangeStructs;

    private void Awake()
    {
        foreach (var range in _parallaxRangeStructs)
        {
            range.Init();
        }
        this._structs = _parallaxRangeStructs;
    }
}