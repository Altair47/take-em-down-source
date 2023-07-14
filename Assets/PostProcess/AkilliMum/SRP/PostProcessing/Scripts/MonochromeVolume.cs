using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Monochrome")]
    public class MonochromeVolume : VolumeBase
    {
        public FloatParameter Scale = new ClampedFloatParameter(0,0,5, false);
        public ColorParameter ColorLight = new ColorParameter(Color.white, false);
        public ColorParameter ColorDark = new ColorParameter(Color.black, false);
    }
}