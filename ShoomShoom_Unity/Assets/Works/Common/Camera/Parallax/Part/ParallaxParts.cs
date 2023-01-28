using UnityEngine;

/// <summary>
/// ParallaxParts will only modify the localPosition
/// </summary>
public class ParallaxParts : ParallaxPartsRangeBase
{
    [Header("Ranges")]
    [SerializeField] ParallaxPartsRangeStruct[] _rangeStructs;
    [SerializeField] ParallaxPartsRangeProportionalStruct[] _rangeProportionalStructs;

    override protected void Start()
    {
        base.Start();

        // if there is a specific range definition then ignore the proportion range
        if (_rangeStructs.Length > 0)
        {
            InitRange(_rangeStructs);
        }
        else if (_rangeProportionalStructs.Length > 0)
        {
            InitRange(_rangeProportionalStructs);
        }

    }

}