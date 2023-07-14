using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Cartoon")]
    public class CartoonVolume : VolumeBase
    {
        //public FloatParameter Brightness = new FloatParameter(0.65f,false);
        //public FloatParameter Regions = new FloatParameter(5,false);
        public FloatParameter Lines = new ClampedFloatParameter(1, 0.01f, 30,false);
        //public FloatParameter Base = new FloatParameter(2.5f,false);
        //public FloatParameter Bias = new FloatParameter(0.9f,false);
        public ColorParameter ColorLight = new ColorParameter(Color.white, false);
        public ColorParameter ColorDark = new ColorParameter(Color.black, false);

    }
}