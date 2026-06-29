using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class BlurShader : ScriptableRendererFeature {
    [Serializable]
    public class BlurSettings {
        public Shader shader;

        [Range(0f, 10f)]
        public float blurSize = 2f;

        [Range(0f, 1f)]
        public float darken = 0.15f;

        public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    [SerializeField] private BlurSettings settings = new();
    public static BlurShader Instance;
    public float BlurSize {
        get => settings.blurSize;
        set => settings.blurSize = value;
    }

    public float Darken {
        get => settings.darken;
        set => settings.darken = value;
    }

    public bool Enabled { get; set; } = false;

    private Material material;
    private SettingsGaussianBlurPass blurPass;

    public override void Create() {

        Instance = this;

        if (settings.shader == null)
            return;

        material = CoreUtils.CreateEngineMaterial(settings.shader);

        blurPass = new SettingsGaussianBlurPass(material, settings) {
            renderPassEvent = settings.passEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (!Enabled) return;

        if (blurPass == null || material == null)
            return;

        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;

        renderer.EnqueuePass(blurPass);
    }

    protected override void Dispose(bool disposing) {
        CoreUtils.Destroy(material);
    }

    private class SettingsGaussianBlurPass : ScriptableRenderPass {
        private readonly Material material;
        private readonly BlurSettings settings;

        private static readonly int BlurSizeID = Shader.PropertyToID("_BlurSize");
        private static readonly int DarkenID = Shader.PropertyToID("_Darken");

        private const string TempTextureName = "_SettingsBlurTemp";
        private const string HorizontalPassName = "Settings Gaussian Blur Horizontal";
        private const string VerticalPassName = "Settings Gaussian Blur Vertical";

        public SettingsGaussianBlurPass(Material material, BlurSettings settings) {
            this.material = material;
            this.settings = settings;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
            if (material == null)
                return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            TextureHandle source = resourceData.activeColorTexture;

            if (!source.IsValid())
                return;

            material.SetFloat(BlurSizeID, settings.blurSize);
            material.SetFloat(DarkenID, settings.darken);

            TextureDesc desc = source.GetDescriptor(renderGraph);
            desc.name = TempTextureName;
            desc.depthBufferBits = 0;

            TextureHandle temp = renderGraph.CreateTexture(desc);

            RenderGraphUtils.BlitMaterialParameters horizontal =
                new(source, temp, material, 0);

            renderGraph.AddBlitPass(horizontal, HorizontalPassName);

            RenderGraphUtils.BlitMaterialParameters vertical =
                new(temp, source, material, 1);

            renderGraph.AddBlitPass(vertical, VerticalPassName);
        }
    }
}