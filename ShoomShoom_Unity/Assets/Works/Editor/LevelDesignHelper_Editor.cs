#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDesignHelper))]
public class LevelDesignHelper_Editor : Editor
{
    LevelDesignHelper _target = null;

    // Toggle level design display
    bool _displayLevelDesign;
    List<SpriteRenderer> _levelDesignRenderers;

    // Auto sort function
    SerializedProperty _excludeLayerProp;
    string[] _sortingLayerNames;
    Dictionary<string, List<SpriteRenderer>> _spriteMap;
    int index = 0;

    void OnEnable()
    {
        _target = target as LevelDesignHelper;

        _displayLevelDesign = _target.DisplayLevelDesign;
        ToggleLevelDesignDisplay();

        _spriteMap = new Dictionary<string, List<SpriteRenderer>>();
        _excludeLayerProp = serializedObject.FindProperty("ExcludeLayer");
    }

    public override void OnInspectorGUI()
    {
        //  ------------------------------------------------------------------
        GUILayout.BeginVertical("BOX");
        EditorGUILayout.LabelField("Level Design Sprites", EditorStyles.boldLabel);
        _target.LevelDesign = (Transform)EditorGUILayout.ObjectField("Level Design Root",
            _target.LevelDesign,
            typeof(Transform),
            true);
        bool displayLevelDesign = EditorGUILayout.Toggle("Display level design?", _displayLevelDesign);
        if (_displayLevelDesign != displayLevelDesign)
        {
            _displayLevelDesign = displayLevelDesign;
            ToggleLevelDesignDisplay();
        }
        GUILayout.EndVertical();

        //  ------------------------------------------------------------------
        EditorGUILayout.Space();

        GUILayout.BeginVertical("BOX");
        EditorGUILayout.LabelField("Layer Sorting", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_excludeLayerProp, true);
        _sortingLayerNames = MyUtils.GetSortingLayerNames();
        _sortingLayerNames = _sortingLayerNames.ToList<string>()
            .Where(layer => !_target.ExcludeLayer.ToList<string>().Any(e => e == layer))
            .ToArray<string>();

        if (_sortingLayerNames != null)
        {

            EditorGUILayout.HelpBox("Sort all the sorting layers based on the z index", MessageType.Info);
            if (GUILayout.Button("Sort All Layers"))
            {
                SortAllLayers();
            }

            EditorGUILayout.LabelField("Sort this layer only: ");
            EditorGUILayout.BeginHorizontal();
            index = EditorGUILayout.Popup(index, _sortingLayerNames);
            if (GUILayout.Button("Sort this"))
            {

            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Toggle the level design objects' sprite display
    /// </summary>
    void ToggleLevelDesignDisplay()
    {
        _levelDesignRenderers = _target.LevelDesign.GetComponentsInChildren<SpriteRenderer>().ToList<SpriteRenderer>();
        _levelDesignRenderers.ForEach(x =>
        {
            x.enabled = _displayLevelDesign;
        });
        _target.DisplayLevelDesign = _displayLevelDesign;
    }

    /// <summary>
    /// Sort all the sorting layers based on the z index
    /// </summary>
    void SortAllLayers()
    {
        if (_sortingLayerNames == null) return;

        GetAllGameSprites();

        foreach (string layerName in _sortingLayerNames)
        {
            if (!_spriteMap.ContainsKey(layerName)) continue;
            var renderers = _spriteMap[layerName];
            //_spriteMap[layerName] = _spriteMap[layerName].OrderBy(x => x.transform.position.z).ToList();
            // Find the 0 layer
            SpriteRenderer zeroRenderer = renderers.OrderBy(x => Mathf.Abs(0 - x.transform.position.z)).First();
            zeroRenderer.sortingOrder = 0;
            // Find all the layers in front(and behind) of the 0 layer and sort it.
            var frontRenderers = new List<SpriteRenderer>();
            renderers.RemoveAll(x =>
            {
                if (x.transform.position.z <= zeroRenderer.transform.position.z)
                {
                    frontRenderers.Add(x);
                    return true;
                }
                return false;
            });
            frontRenderers = frontRenderers.OrderByDescending(x => x.transform.position.z)
                .ToList();
            var behindRenderers = renderers.OrderBy(x => x.transform.position.z)
                .ToList();
            // Assign sorting order for all the layers
            for (int i = 0, order = 0; i < frontRenderers.Count; i++)
            {
                var renderer = frontRenderers[i];
                if (i == 0) // If this is the first one
                {
                    if (renderer.transform.position.z != zeroRenderer.transform.position.z)
                    {
                        order++;
                    }
                }
                else
                {
                    var prevRenderer = frontRenderers[i - 1];
                    if (renderer.transform.position.z != prevRenderer.transform.position.z)
                    {
                        order++;
                    }
                }

                renderer.sortingOrder = order;
            }
            for (int i = 0, order = 0; i < renderers.Count; i++)
            {
                var renderer = frontRenderers[i];
                if (i == 0) // If this is the first one
                {
                    if (renderer.transform.position.z != zeroRenderer.transform.position.z)
                    {
                        order--;
                    }
                }
                else
                {
                    var prevRenderer = frontRenderers[i - 1];
                    if (renderer.transform.position.z != prevRenderer.transform.position.z)
                    {
                        order--;
                    }
                }

                renderer.sortingOrder = order;
            }
        }
    }

    void GetAllGameSprites()
    {
        List<SpriteRenderer> renderers = GameObject.FindObjectsOfType<SpriteRenderer>().ToList();
        renderers.ForEach(x =>
        {
            if (!_target.ExcludeLayer.ToList<string>().Any(e => e == x.sortingLayerName))
            {
                if (_spriteMap.ContainsKey(x.sortingLayerName))
                {
                    _spriteMap[x.sortingLayerName].Add(x);
                }
                else
                {
                    _spriteMap.Add(x.sortingLayerName, new List<SpriteRenderer> { x });
                }
            }
        });
    }
}
#endif