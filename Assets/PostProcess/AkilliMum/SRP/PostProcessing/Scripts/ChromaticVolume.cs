using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Chromatic")]
    public class ChromaticVolume : VolumeBase
    {
        public FloatParameter Chroma = new ClampedFloatParameter(8, -40, 40, false);

    }
}