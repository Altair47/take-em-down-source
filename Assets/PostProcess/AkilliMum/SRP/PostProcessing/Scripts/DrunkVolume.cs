using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Drunk")]
    public class DrunkVolume : VolumeBase
    {
        public FloatParameter Speed = new ClampedFloatParameter(1,0,30, false);
    }
}