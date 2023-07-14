using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    [Serializable, VolumeComponentMenu("Akilli Mum/SRP/Post Processing/CRT")]
    public class CRTVolume : VolumeBase
    {
        public FloatParameter Scan = new ClampedFloatParameter(-0.18f,-1,1, false);
        public FloatParameter Brightness= new ClampedFloatParameter(1,0,5, false);
        public FloatParameter Offset= new ClampedFloatParameter(1, 0, 5, false);
    }
}