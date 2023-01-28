using System;
using UnityEngine;

[Serializable]
public class ParallaxPartsRangeProportionalStruct : ParallaxRangeStructBase
{
    [Header("Range plus multiplier proportion")]
    [SerializeField] float _plusRangeProportion = 0.0f;

    public void Init(float baseMultiplier)
    {
        this.MultiplierX = baseMultiplier * _plusRangeProportion;
    }
}