using UnityEngine;

public class ParallaxRangeProportional : ParallaxRangeBase
{
    [SerializeField] ParallaxRangeProportionalStruct[] _parallaxRangeStructs;

    public ParallaxRangeProportionalStruct[] ParallaxRangeStructs => _parallaxRangeStructs;

    private void Awake()
    {
        foreach (var range in _parallaxRangeStructs)
        {
            range.Init(_multiplierX);
        }
        this._structs = _parallaxRangeStructs;
    }
}