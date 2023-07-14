using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Glitch")]
    public class GlitchVolume : VolumeBase
    {
        public TextureParameter Noise = new TextureParameter(null, false);
        public FloatParameter Size = new ClampedFloatParameter(16, 0, 50, false);
        public FloatParameter BlockDensity = new ClampedFloatParameter(0.2f, 0, 1, false);
        public FloatParameter LineDensity = new ClampedFloatParameter(0.7f, 0, 1, false);
        public FloatParameter Speed = new ClampedFloatParameter(5f, 0, 20, false);

    }
}