using UnityEngine;

/// <summary>
/// ParallaxParts will only modify the localPosition
/// </summary>
public class ParallaxParts : ParallaxPartsRangeBase
{
    [Header("Ranges")]
    [SerializeField] ParallaxPartsRangeStruct[] _rangeStructs;

    override protected void Start()
    {
        base.Start();
        if (_rangeStructs.Length > 0)
        {
            InitRange(_rangeStructs);
        }

    }

}