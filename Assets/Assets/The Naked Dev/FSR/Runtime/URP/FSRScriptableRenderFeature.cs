using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace TND.FSR
{
    //Not allowed to be in a namespace
    public class FSRScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;

        private FSR3_URP m_upscaler;

        private FSRBufferPass fsrBufferPass;
        private FSRRenderPass fsrRenderPass;
        private FSROpaqueOnlyPass fsrReactiveMaskPass;
        private FSRTransparentPass fsrReactiveMaskTransparentPass;

        private CameraData cameraData;

        public void OnSetReference(FSR3_URP _upscaler)
        {
            m_upscaler = _upscaler;
            fsrBufferPass.OnSetReference(m_upscaler);
            fsrRenderPass.OnSetReference(m_upscaler);
            fsrReactiveMaskPass.OnSetReference(m_upscaler);
            fsrReactiveMaskTransparentPass.OnSetReference(m_upscaler);
        }

        public override void Create()
        {
            name = "FSRRenderFeature";

            // Pass the settings as a parameter to the constructor of the pass.
            fsrBufferPass = new FSRBufferPass(m_upscaler);
            fsrRenderPass = new FSRRenderPass(m_upscaler);
            fsrReactiveMaskPass = new FSROpaqueOnlyPass(m_upscaler);
            fsrReactiveMaskTransparentPass = new FSRTransparentPass(m_upscaler);

            fsrBufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        public void OnDispose()
        {
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            fsrBufferPass.Setup(renderer);
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (!IsEnabled)
            {
                return;
            }

            cameraData = renderingData.cameraData;
            if (cameraData.cameraType != CameraType.Game)
            {
                return;
            }
            if (!cameraData.resolveFinalTarget)
            {
                return;
            }

            m_upscaler.m_autoHDR = cameraData.isHdrEnabled;

            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(fsrBufferPass);
            renderer.EnqueuePass(fsrRenderPass);
            if (m_upscaler.generateReactiveMask)
            {
                renderer.EnqueuePass(fsrReactiveMaskPass);
                renderer.EnqueuePass(fsrReactiveMaskTransparentPass);
            }
        }
    }
}
