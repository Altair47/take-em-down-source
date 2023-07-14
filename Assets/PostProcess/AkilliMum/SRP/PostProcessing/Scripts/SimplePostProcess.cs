using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// ReSharper disable once CheckNamespace
namespace AkilliMum.SRP.PostProcessing
{
    public class SimplePostProcess : ScriptableRendererFeature
    {
        class CustomRenderPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier _source;
            private VolumeBase _volume;
            const string Tag = "Akilli Mum URP Simple Post Process";

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                // Grab the color buffer from the renderer camera color target.
                _source = renderingData.cameraData.renderer.cameraColorTarget;

                RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
                // Set the number of depth bits we need for our temporary render texture.
                descriptor.depthBufferBits = 0;

                //ConfigureInput(ScriptableRenderPassInput.Depth);
            }

            public CustomRenderPass(VolumeBase volume)
            {
                _volume = volume;
            }

            private Material _activeMat;

            private Material _cartoonMat;
            private Material _fastBlurMat;
            private Material _chromaticMat;
            private Material _simpleEdgeMat;
            private Material _sketchMat;
            private Material _glitchMat;
            private Material _drunkMat;
            private Material _crtMat;
            private Material _monochromeMat;
            private Material _edgeGlowMat;
            private Material _vcrMat;
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (renderingData.cameraData.camera.cameraType == CameraType.Reflection ||
                    renderingData.cameraData.camera.cameraType == CameraType.SceneView)
                    return;

                CommandBuffer cmd = CommandBufferPool.Get(Tag);
                using (new ProfilingScope(cmd, new ProfilingSampler(Tag)))
                {
                    if (_volume is CartoonVolume cVolume && _volume != null)
                    {
                        if (_cartoonMat == null)
                            _cartoonMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Cartoon"));
                        //_cartoonMat.SetFloat("_Brightness", cVolume.Brightness.value);
                        //_cartoonMat.SetFloat("_Regions", cVolume.Regions.value);
                        _cartoonMat.SetFloat("_Lines", cVolume.Lines.value);
                        //_cartoonMat.SetFloat("_Base", cVolume.Base.value);
                        //_cartoonMat.SetFloat("_Bias", cVolume.Bias.value);
                        _cartoonMat.SetColor("_ColorLight", cVolume.ColorLight.value);
                        _cartoonMat.SetColor("_ColorDark", cVolume.ColorDark.value);
                        _activeMat = _cartoonMat;
                    }
                    else if (_volume is FastBlurVolume fVolume && _volume != null)
                    {
                        if (_fastBlurMat == null)
                            _fastBlurMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/FastBlur"));
                        _fastBlurMat.SetFloat("_Scale", fVolume.Scale.value);
                        _activeMat = _fastBlurMat;
                    }
                    else if (_volume is ChromaticVolume chVolume && _volume != null)
                    {
                        if (_chromaticMat == null)
                            _chromaticMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Chromatic"));
                        _chromaticMat.SetFloat("_Chroma", chVolume.Chroma.value);
                        _activeMat = _chromaticMat;
                    }
                    else if (_volume is SimpleEdgeVolume siVolume && _volume != null)
                    {
                        if (_simpleEdgeMat == null)
                            _simpleEdgeMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/SimpleEdge"));
                        _simpleEdgeMat.SetFloat("_Multiplier", siVolume.Multiplier.value);
                        _activeMat = _simpleEdgeMat;
                    }
                    else if (_volume is SketchVolume sVolume && _volume != null)
                    {
                        if (_sketchMat == null)
                            _sketchMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Sketch"));
                        _sketchMat.SetTexture("_Noise", sVolume.Noise.value);
                        //_sketchMat.SetFloat("_RANGE", sVolume.Range.value);
                        //_sketchMat.SetFloat("_STEP", sVolume.Step.value);
                        //_sketchMat.SetFloat("_ANGLE", sVolume.Angle.value);
                        //_sketchMat.SetFloat("_THRESHOLD", sVolume.Threshold.value);
                        _sketchMat.SetFloat("_SENSITIVITY", sVolume.Sensitivity.value);
                        _sketchMat.SetFloat("_COLOR", sVolume.Color.value);
                        _activeMat = _sketchMat;
                    }
                    else if (_volume is GlitchVolume gVolume && _volume != null)
                    {
                        if (_glitchMat == null)
                            _glitchMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Glitch"));
                        _glitchMat.SetTexture("_Noise", gVolume.Noise.value);
                        _glitchMat.SetFloat("_Size", gVolume.Size.value);
                        _glitchMat.SetFloat("_BlockDensity", gVolume.BlockDensity.value);
                        _glitchMat.SetFloat("_LineDensity", gVolume.LineDensity.value);
                        _glitchMat.SetFloat("_Speed", gVolume.Speed.value);
                        _activeMat = _glitchMat;
                    }
                    else if (_volume is DrunkVolume dVolume && _volume != null)
                    {
                        if (_drunkMat == null)
                            _drunkMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Drunk"));
                        _drunkMat.SetFloat("_Speed", dVolume.Speed.value);
                        _activeMat = _drunkMat;
                    }
                    else if (_volume is CRTVolume crtVolume && _volume != null)
                    {
                        if (_crtMat == null)
                            _crtMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/CRT"));
                        _crtMat.SetFloat("_SCAN", crtVolume.Scan.value);
                        _crtMat.SetFloat("_BRIGHTNESS", crtVolume.Brightness.value);
                        _crtMat.SetFloat("_OFFSET", crtVolume.Offset.value);
                        _activeMat = _crtMat;
                    }
                    else if (_volume is MonochromeVolume mValue && _volume != null)
                    {
                        if (_monochromeMat == null)
                            _monochromeMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/Monochrome"));
                        _monochromeMat.SetFloat("_Scale", mValue.Scale.value);
                        _monochromeMat.SetColor("_ColorLight", mValue.ColorLight.value);
                        _monochromeMat.SetColor("_ColorDark", mValue.ColorDark.value);
                        _activeMat = _monochromeMat;
                    }
                    else if (_volume is EdgeGlowVolume egVolume && _volume != null)
                    {
                        if (_edgeGlowMat == null)
                            _edgeGlowMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/EdgeGlow"));
                        _edgeGlowMat.SetFloat("_Scale", egVolume.Scale.value);
                        _edgeGlowMat.SetColor("_EdgeColor", egVolume.EdgeColor.value);
                        _activeMat = _edgeGlowMat;

                    }
                    else if (_volume is VCRVolume vcrVolume && _volume != null)
                    {
                        if (_vcrMat == null)
                            _vcrMat = new Material(Shader.Find("AkilliMum/SRP/PostProcessing/VCR"));
                        _vcrMat.SetTexture("_Noise", vcrVolume.Noise.value);
                        _vcrMat.SetFloat("_Stripes", vcrVolume.Stripes.value);
                        _vcrMat.SetFloat("_Noisy", vcrVolume.Noisy.value);
                        _vcrMat.SetFloat("_VShift", vcrVolume.VerticalShift.value);
                        _vcrMat.SetFloat("_HShift", vcrVolume.HorizontalShift.value);
                        _activeMat = _vcrMat;
                    }

                    if (_volume != null && _volume.Active.value)
                        cmd.Blit(_source, _source, _activeMat);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
            }
        }

        private CustomRenderPass _cartoonPass;
        private CustomRenderPass _fastBlurPass;
        private CustomRenderPass _chromaticPass;
        private CustomRenderPass _simpleEdgePass;
        private CustomRenderPass _drunkPass;
        private CustomRenderPass _monochromePass;
        private CustomRenderPass _edgeGlowPass;
        private CustomRenderPass _sketchPass;
        private CustomRenderPass _crtPass;
        private CustomRenderPass _glitchPass;
        private CustomRenderPass _vcrPass;

        private CartoonVolume _cartoonVolume;
        private FastBlurVolume _fastBlurVolume;
        private ChromaticVolume _chromaticVolume;
        private SimpleEdgeVolume _simpleEdgeVolume;
        private DrunkVolume _drunkVolume;
        private MonochromeVolume _monochromeVolume;
        private EdgeGlowVolume _edgeGlowVolume;
        private SketchVolume _sketchVolume;
        private CRTVolume _crtVolume;
        private GlitchVolume _glitchVolume;
        private VCRVolume _vcrVolume;

        public override void Create()
        {
            _cartoonVolume = VolumeManager.instance.stack.GetComponent<CartoonVolume>();
            _fastBlurVolume = VolumeManager.instance.stack.GetComponent<FastBlurVolume>();
            _chromaticVolume = VolumeManager.instance.stack.GetComponent<ChromaticVolume>();
            _simpleEdgeVolume = VolumeManager.instance.stack.GetComponent<SimpleEdgeVolume>();
            _drunkVolume = VolumeManager.instance.stack.GetComponent<DrunkVolume>();
            _monochromeVolume = VolumeManager.instance.stack.GetComponent<MonochromeVolume>();
            _edgeGlowVolume = VolumeManager.instance.stack.GetComponent<EdgeGlowVolume>();
            _sketchVolume = VolumeManager.instance.stack.GetComponent<SketchVolume>();
            _crtVolume = VolumeManager.instance.stack.GetComponent<CRTVolume>();
            _glitchVolume = VolumeManager.instance.stack.GetComponent<GlitchVolume>();
            _vcrVolume = VolumeManager.instance.stack.GetComponent<VCRVolume>();

            _cartoonPass = new CustomRenderPass(_cartoonVolume);
            _fastBlurPass = new CustomRenderPass(_fastBlurVolume);
            _chromaticPass = new CustomRenderPass(_chromaticVolume);
            _simpleEdgePass = new CustomRenderPass(_simpleEdgeVolume);
            _drunkPass = new CustomRenderPass(_drunkVolume);
            _monochromePass = new CustomRenderPass(_monochromeVolume);
            _edgeGlowPass = new CustomRenderPass(_edgeGlowVolume);
            _sketchPass = new CustomRenderPass(_sketchVolume);
            _crtPass = new CustomRenderPass(_crtVolume);
            _glitchPass = new CustomRenderPass(_glitchVolume);
            _vcrPass = new CustomRenderPass(_vcrVolume);

            _cartoonPass.renderPassEvent = _cartoonVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _fastBlurPass.renderPassEvent = _fastBlurVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _chromaticPass.renderPassEvent = _chromaticVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _simpleEdgePass.renderPassEvent = _simpleEdgeVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _drunkPass.renderPassEvent = _drunkVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _monochromePass.renderPassEvent = _monochromeVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _edgeGlowPass.renderPassEvent = _edgeGlowVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _sketchPass.renderPassEvent = _sketchVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _crtPass.renderPassEvent = _crtVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _glitchPass.renderPassEvent = _glitchVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;
            _vcrPass.renderPassEvent = _vcrVolume?.When?.value ?? RenderPassEvent.AfterRenderingTransparents;

        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_cartoonPass);
            renderer.EnqueuePass(_fastBlurPass);
            renderer.EnqueuePass(_chromaticPass);
            renderer.EnqueuePass(_simpleEdgePass);
            renderer.EnqueuePass(_drunkPass);
            renderer.EnqueuePass(_monochromePass);
            renderer.EnqueuePass(_edgeGlowPass);
            renderer.EnqueuePass(_sketchPass);
            renderer.EnqueuePass(_crtPass);
            renderer.EnqueuePass(_glitchPass);
            renderer.EnqueuePass(_vcrPass);
        }
    }
}