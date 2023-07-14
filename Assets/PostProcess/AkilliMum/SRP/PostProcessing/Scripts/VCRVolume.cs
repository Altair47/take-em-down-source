using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/VCR")]
    public class VCRVolume : VolumeBase
    {
        public TextureParameter Noise = new TextureParameter(null, false);
        public FloatParameter Stripes = new ClampedFloatParameter(4, 0 ,40, false);
        public FloatParameter Noisy = new ClampedFloatParameter(1, 0, 5, false);
        public FloatParameter VerticalShift = new ClampedFloatParameter(0.25f,0 ,40, false);
        public FloatParameter HorizontalShift = new ClampedFloatParameter(1.8f, 0, 20, false);
    }
}