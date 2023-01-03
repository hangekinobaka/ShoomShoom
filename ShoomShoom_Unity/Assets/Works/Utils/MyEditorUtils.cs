#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditorInternal;

public static class MyEditorUtils
{
    public static string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        var sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
        return sortingLayers;
    }
}
#endif