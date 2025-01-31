using UnityEditor;

using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Profiling;

public partial class CameraRenderer {
  #if UNITY_EDITOR
    static Material errorMaterial;
    static ShaderTagId[] legacyShaderTagIds = {
      new ShaderTagId("Always"),
      new ShaderTagId("ForwardBase"),
      new ShaderTagId("PrepassBase"),
      new ShaderTagId("Vertex"),
      new ShaderTagId("VertexLMRGBM"),
      new ShaderTagId("VertexLM")
    };

    string SampleName { get; set; }

    partial void DrawUnsupportedShaders () {
      if (errorMaterial == null)
        errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));

      var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera)) {
        overrideMaterial = errorMaterial
      };

      var filteringSettings = FilteringSettings.defaultValue;

      for (int i = 1; i < legacyShaderTagIds.Length; i++)
        drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);

      context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    partial void DrawGizmos () {
      if (Handles.ShouldRenderGizmos()) {
        context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
        context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
      }
    }

    partial void PrepareForSceneWindow () {
      if (camera.cameraType == CameraType.SceneView)
        ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
    }

    partial void PrepareBuffer () {
      Profiler.BeginSample("Editor Only");

      buffer.name = SampleName = camera.name;

      Profiler.EndSample();
    }
  #else
    const string SampleName = bufferName;
  #endif
}