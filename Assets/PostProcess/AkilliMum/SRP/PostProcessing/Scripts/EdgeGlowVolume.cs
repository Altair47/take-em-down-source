using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Edge Glow")]
    public class EdgeGlowVolume : VolumeBase
    {
        public FloatParameter Scale = new ClampedFloatParameter(1,0.01f,5, false);
        public ColorParameter EdgeColor = new ColorParameter(Color.white, true, true, true, false);
    }
}