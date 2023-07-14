using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/Sketch")]
    public class SketchVolume : VolumeBase
    {
        public TextureParameter Noise = new TextureParameter(null, false);
        //public FloatParameter Range = new FloatParameter(16, false);
        //public FloatParameter Step = new FloatParameter(2, false);
        //public FloatParameter Angle = new FloatParameter(1.65, false);
        //public FloatParameter Threshold = new FloatParameter(0.01f, false);
        public FloatParameter Sensitivity = new ClampedFloatParameter(1,0,1, false);
        public FloatParameter Color = new ClampedFloatParameter(0.5f, 0, 1, false);
    }
}