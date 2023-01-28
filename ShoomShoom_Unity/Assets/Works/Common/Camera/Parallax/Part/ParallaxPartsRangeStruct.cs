using System;
using UnityEngine;

[Serializable]
public class ParallaxPartsRangeStruct : ParallaxRangeStructBase
{
    [Header("Range plus multiplier")]
    [SerializeField] float _plusMultiplierX = 0.0f;

    public void Init()
    {
        this.MultiplierX = _plusMultiplierX;
    }
}