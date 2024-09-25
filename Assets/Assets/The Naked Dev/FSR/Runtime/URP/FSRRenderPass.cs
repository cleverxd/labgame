using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace TND.FSR
{
    public class FSRRenderPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;
        private FSR3_URP m_upscaler;

        private readonly Vector4 flipVector = new Vector4(1, -1, 0, 1);
        private Vector4 _scaleBias;

        public FSRRenderPass(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;

            renderPassEvent = RenderPassEvent.AfterRendering + 2;
            _scaleBias = SystemInfo.graphicsUVStartsAtTop ? flipVector : Vector4.one;
        }

        public void OnSetReference(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            try
            {
                cmd = CommandBufferPool.Get("[FSR 3] Final Blit");

                CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store, ClearFlag.None, Color.clear);
                cmd.SetViewport(renderingData.cameraData.camera.pixelRect);
                Blitter.BlitTexture(cmd, m_upscaler.m_fsrOutput, _scaleBias, 0, false);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            catch { }
        }
    }

    public class FSRBufferPass : ScriptableRenderPass
    {
        private FSR3_URP m_upscaler;
        private CommandBuffer cmd;
        private int multipassId = 0;

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_MotionVectorTexture");

        public FSRBufferPass(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;

            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

#if UNITY_2022_1_OR_NEWER
#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public void Setup(ScriptableRenderer renderer)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (m_upscaler == null)
            {
                return;
            }

            m_upscaler.m_dispatchDescription.Depth = new FidelityFX.ResourceView(renderer.cameraDepthTargetHandle, RenderTextureSubElement.Depth);
            m_upscaler.m_dispatchDescription.MotionVectors = new FidelityFX.ResourceView(Shader.GetGlobalTexture(motionTexturePropertyID));
        }
#endif

        public void OnSetReference(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            cmd = CommandBufferPool.Get("[FSR 3] Upscaler");

#if UNITY_2022_1_OR_NEWER
            m_upscaler.m_dispatchDescription.Color = new FidelityFX.ResourceView(renderingData.cameraData.renderer.cameraColorTargetHandle, RenderTextureSubElement.Color);
#else
            m_upscaler.m_dispatchDescription.Color = new FidelityFX.ResourceView(renderingData.cameraData.renderer.cameraColorTarget, RenderTextureSubElement.Color);
            m_upscaler.m_dispatchDescription.Depth = new FidelityFX.ResourceView(Shader.GetGlobalTexture(depthTexturePropertyID), RenderTextureSubElement.Depth);
            m_upscaler.m_dispatchDescription.MotionVectors = new FidelityFX.ResourceView(Shader.GetGlobalTexture(motionTexturePropertyID));

            try
            {
                m_upscaler.m_dispatchDescription.DepthFormat = Shader.GetGlobalTexture(depthTexturePropertyID).graphicsFormat == UnityEngine.Experimental.Rendering.GraphicsFormat.None;
            }
            catch
            {
                m_upscaler.m_dispatchDescription.DepthFormat = true;
            }
#endif

            //Stereo
            if (XRSettings.enabled)
            {
                multipassId++;
                if (multipassId >= 2)
                {
                    multipassId = 0;
                }
            }

            if (m_upscaler.generateReactiveMask)
            {
                m_upscaler.m_context[multipassId].GenerateReactiveMask(m_upscaler.m_genReactiveDescription, cmd);
            }
            m_upscaler.m_context[multipassId].Dispatch(m_upscaler.m_dispatchDescription, cmd);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public class FSROpaqueOnlyPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;
        private FSR3_URP m_upscaler;

        public FSROpaqueOnlyPass(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;

            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }

        public void OnSetReference(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            cmd = CommandBufferPool.Get();

#if UNITY_2022_1_OR_NEWER
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, m_upscaler.m_opaqueOnlyColorBuffer);
#else
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, m_upscaler.m_opaqueOnlyColorBuffer);
#endif

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public class FSRTransparentPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;
        private FSR3_URP m_upscaler;

        public FSRTransparentPass(FSR3_URP _upscaler)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            m_upscaler = _upscaler;
        }

        public void OnSetReference(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            cmd = CommandBufferPool.Get();

#if UNITY_2022_1_OR_NEWER
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, m_upscaler.m_afterOpaqueOnlyColorBuffer);
#else
            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, m_upscaler.m_afterOpaqueOnlyColorBuffer);
#endif

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
