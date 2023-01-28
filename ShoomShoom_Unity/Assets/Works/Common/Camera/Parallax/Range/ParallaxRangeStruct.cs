using System;
using UnityEngine;

[Serializable]
public class ParallaxRangeStruct : ParallaxRangeStructBase
{
    [Header("Range multiplier")]
    [SerializeField] float _multiplierX = 0.0f;

    public void Init()
    {
        this.MultiplierX = _multiplierX;
    }
}