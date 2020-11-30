using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Redde Render Pipeline")]
public class ReddeRenderPipelineAsset : RenderPipelineAsset {
  [SerializeField]
	ShadowSettings shadows = default;

  protected override RenderPipeline CreatePipeline() {
    return new ReddeRenderPipeline(shadows);
  }
}