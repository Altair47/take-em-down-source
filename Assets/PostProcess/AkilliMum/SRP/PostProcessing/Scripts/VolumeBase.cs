using System;

// ReSharper disable once CheckNamespace
namespace UnityEngine.Rendering.Universal
{
    public abstract class VolumeBase : VolumeComponent, IPostProcessComponent
    {
        public BoolParameter Active = new BoolParameter(false, false);
        public WhenParameter When = new WhenParameter(RenderPassEvent.AfterRenderingTransparents, false);

        public bool IsActive()
        {
            return Active.value;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

    [Serializable]
    public sealed class WhenParameter : VolumeParameter<RenderPassEvent>
    {
        public WhenParameter(RenderPassEvent value, bool overrideState = false) : base(value, overrideState) { }
    }
}