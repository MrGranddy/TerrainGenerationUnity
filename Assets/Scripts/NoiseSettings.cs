using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    [Min(1)]
    public int octaves = 6;
    [Min(0.001f)]
    public float startFrequency = 0.02f;
    [Min(0)]
    public float persistance = 0.6f;

    public int biomeOctaves = 1;

    public float biomePersistance = 1.0f;
    public float biomeStartFrequency = 0.01f;


}
