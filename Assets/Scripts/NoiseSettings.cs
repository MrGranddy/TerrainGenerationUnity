using System;
using UnityEngine;

[Serializable]
public class NoiseSettings
{
    [Range(1, 8)]
    public int elevationOctaves = 6;

    [Range(-3.0f, -1.0f)] // Linear range from 0 to 3 for our "fake" logarithmic slider.
    public float elevationLogStartFrequency = -2.0f;

    public float elevationStartFrequency
    {
        get
        {
            return Mathf.Pow(10, elevationLogStartFrequency); // Convert linear value to base-10 logarithmic value.
        }
    }

    [Range(0.0f, 1.0f)]
    public float elevationPersistance = 0.6f;

    [Range(-50f, 50f)]
    public float elevationOffset = 0.0f;



    [Range(1, 8)]
    public int decreaseOctaves = 6;

    [Range(-3.0f, -1.0f)] // Linear range from 0 to 3 for our "fake" logarithmic slider.
    public float decreaseLogStartFrequency = -2.0f;

    public float decreaseStartFrequency
    {
        get
        {
            return Mathf.Pow(10, decreaseLogStartFrequency); // Convert linear value to base-10 logarithmic value.
        }
    }

    [Range(0.0f, 1.0f)]
    public float decreasePersistance = 0.6f;

    [Range(-50f, 50f)]
    public float decreaseOffset = 0.0f;

    [Range(0.0f, 1.0f)]
    public float decreaseScale = 0.4f;


    [Range(-1.0f, 1.0f)]
    public float mountainThreshold = 0.34f;

    [Range(-1.0f, 1.0f)]
    public float seaThreshold = 0.27f;

}