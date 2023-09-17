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
    public float logStartFrequency = -2.0f;

    public float startFrequency
    {
        get
        {
            return Mathf.Pow(10, logStartFrequency); // Convert linear value to base-10 logarithmic value.
        }
    }

    [Range(0.0f, 1.0f)]
    public float persistance = 0.6f;


    [Range(1, 8)]
    public int biomeOctaves = 6;

    [Range(-3.0f, -1.0f)] // Linear range from 0 to 3 for our "fake" logarithmic slider.
    public float logBiomeStartFrequency = -2.0f;

    public float biomeStartFrequency
    {
        get
        {
            return Mathf.Pow(10, logBiomeStartFrequency); // Convert linear value to base-10 logarithmic value.
        }
    }

    [Range(0.0f, 1.0f)]
    public float biomePersistance = 0.6f;
}