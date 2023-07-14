using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/FastBlur")]
    public class FastBlurVolume : VolumeBase
    {
        public FloatParameter Scale = new ClampedFloatParameter(5,1,20, false);
    }
}