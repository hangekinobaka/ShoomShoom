using System;
using UnityEngine;

[Serializable]
public class ParallaxRangeProportionalStruct : ParallaxRangeStructBase
{
    [Header("Range proportion")]
    [SerializeField] float _rangeProportion = 0.0f;

    public float RangeProportion => _rangeProportion;
    public void Init(float baseMultiplierX)
    {
        this.MultiplierX = baseMultiplierX * _rangeProportion;
    }
}