using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    [Range(1, 8)]
    public int octaves = 6;

    [Range(-3.0f, -1.0f)] // Linear range from 0 to 3 for our "fake" logarithmic slider.
    public float logStartFrequency = 2.0f;

    public float startFrequency
    {
        get
        {
            return Mathf.Pow(10, logStartFrequency); // Convert linear value to base-10 logarithmic value.
        }
    }

    [Range(0.0f, 1.0f)]
    public float persistance = 0.6f;

    public int biomeOctaves = 1;

    public float biomePersistance = 1.0f;
    public float biomeStartFrequency = 0.01f;
}
