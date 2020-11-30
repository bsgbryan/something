using UnityEngine;
using UnityEngine.Rendering;

public class ReddeRenderPipeline : RenderPipeline {
  CameraRenderer renderer = new CameraRenderer();
  ShadowSettings shadowSettings;

  public ReddeRenderPipeline(ShadowSettings shadowSettings) {
    GraphicsSettings.useScriptableRenderPipelineBatching = true;
    GraphicsSettings.lightsUseLinearIntensity = true;

    this.shadowSettings = shadowSettings;
  }

  protected override void Render (ScriptableRenderContext context, Camera[] cameras) {
    foreach (Camera camera in cameras) {
			renderer.Render(context, camera, shadowSettings);
		}
  }
}