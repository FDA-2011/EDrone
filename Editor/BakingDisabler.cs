using UnityEngine;
using UnityEditor;

public class BakingDisabler : EditorWindow
{
    static void PerformDisable()
    {
        if (Lightmapping.lightingDataAsset != null)
        {
            Lightmapping.lightingDataAsset = null;
        }
        Light[] allLights = FindObjectsOfType<Light>(true);
        int lightsChanged = 0;
        foreach (Light light in allLights)
        {
            if (light.lightmapBakeType != LightmapBakeType.Realtime)
            {
                light.lightmapBakeType = LightmapBakeType.Realtime;
                lightsChanged++;
            }
        }
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(true);
        int renderersChanged = 0;
        foreach (Renderer renderer in allRenderers)
        {
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
            if ((flags & StaticEditorFlags.ContributeGI) != 0)
            {
                flags &= ~StaticEditorFlags.ContributeGI;
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, flags);
                renderersChanged++;
            }
        }

        Lightmapping.Clear();
        EditorUtility.SetDirty(FindFirstObjectByType<Light>());
        SceneView.RepaintAll();
    }
}