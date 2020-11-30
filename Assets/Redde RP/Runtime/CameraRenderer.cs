using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {
  static ShaderTagId
    unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
		litShaderTagId = new ShaderTagId("CustomLit");

  const string bufferName = "Render Camera";
	CommandBuffer buffer = new CommandBuffer {
		name = bufferName
	};
  Lighting lighting = new Lighting();

  ScriptableRenderContext context;
  Camera camera;
  ShadowSettings shadowSettings;

  CullingResults cullingResults;

  partial void DrawUnsupportedShaders();
  partial void DrawGizmos();
  partial void PrepareForSceneWindow();
  partial void PrepareBuffer ();

  public void Render(ScriptableRenderContext context, Camera camera, ShadowSettings shadowSettings) {
    this.context = context;
    this.camera = camera;
    this.shadowSettings = shadowSettings;

    PrepareBuffer();
    PrepareForSceneWindow();

    if (!Cull(shadowSettings.maxDistance))
			return;

    buffer.BeginSample(SampleName);

    ExecuteBuffer();

    lighting.Setup(context, cullingResults, shadowSettings);

    buffer.EndSample(SampleName);

    Setup();
    DrawVisibleGeometry();
    DrawUnsupportedShaders();
    DrawGizmos();
    lighting.Cleanup();
		Submit();
  }

  bool Cull (float maxShadowDistance) {
		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			p.shadowDistance = Mathf.Min(maxShadowDistance, camera.farClipPlane);;

      cullingResults = context.Cull(ref p);

      return true;
		}
		return false;
	}

  void Setup () {
		context.SetupCameraProperties(camera);
    CameraClearFlags flags = camera.clearFlags;
    // Do I really need this? Seems like I could save some performance by not using it
    buffer.ClearRenderTarget(
      flags <= CameraClearFlags.Depth,
      flags == CameraClearFlags.Color,
      flags == CameraClearFlags.Color ?
				camera.backgroundColor.linear
        :
        Color.clear
    );
    buffer.BeginSample(SampleName);
    ExecuteBuffer();
	}

  void DrawVisibleGeometry () {
    var sortingSettings = new SortingSettings(camera) {
			criteria = SortingCriteria.CommonOpaque
		};
    var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

    drawingSettings.SetShaderPassName(1, litShaderTagId);

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    context.DrawSkybox(camera);

    sortingSettings.criteria = SortingCriteria.CommonTransparent;
		drawingSettings.sortingSettings = sortingSettings;
		filteringSettings.renderQueueRange = RenderQueueRange.transparent;

		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	}

  void Submit () {
    buffer.EndSample(SampleName);
    ExecuteBuffer();
		context.Submit();
	}

  void ExecuteBuffer () {
		context.ExecuteCommandBuffer(buffer);
		buffer.Clear();
	}
}