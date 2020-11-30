#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 GetIncomingLight (Surface surface, Light light) {
  return saturate(dot(surface.normal, light.direction) * light.attenuation) * light.color;
}

float3 GetLighting (Surface surface, BRDF brdf, Light light) {
  return GetIncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting (Surface surfaceWS, BRDF brdf) {
  ShadowData shadowData = GetShadowData(surfaceWS);
  float3 color = 0.0;

  for (int i = 0; i < GetDirectionalLightCount(); i++) {
    Light light = GetDirectionalLight(i, surfaceWS, shadowData);

    color += GetLighting(surfaceWS, brdf, light);

    #if defined(_DEBUG_CASCADE_CULLING_SPHERES)
      if (shadowData.cascadeIndex == 0)
        color += float3(0.1 * shadowData.cascadeBlend, 0, 0);
      else if (shadowData.cascadeIndex == 1)
        color += float3(0, 0.1 * shadowData.cascadeBlend, 0);
      else if (shadowData.cascadeIndex == 2)
        color += float3(0, 0, 0.1 * shadowData.cascadeBlend);
      else if (shadowData.cascadeIndex == 3)
        color += float3(0.1 * shadowData.cascadeBlend, 0, 0.1 * shadowData.cascadeBlend);
    #endif
  }

  return color;
}

#endif