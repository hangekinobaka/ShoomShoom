#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;


public class SpriteRendererGeneral
{
    SpriteRenderer _spriteRenderer;
    SpriteShapeRenderer _shapeRenderer;

    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
        set => _spriteRenderer = value;
    }
    public SpriteShapeRenderer ShapeRenderer
    {
        get => _shapeRenderer;
        set => _shapeRenderer = value;
    }

    public float ZIndex => _spriteRenderer == null ? _shapeRenderer.transform.position.z : _spriteRenderer.transform.position.z;
    public int SortingOrder
    {
        get { return _spriteRenderer == null ? (int)_shapeRenderer.sortingOrder : (int)_spriteRenderer.sortingOrder; }
        set
        {
            if (_spriteRenderer == null) _shapeRenderer.sortingOrder = value;
            else _spriteRenderer.sortingOrder = value;
        }
    }
    public SpriteRendererGeneral(SpriteRenderer r)
    {
        SetRenderer(r);
    }
    public SpriteRendererGeneral(SpriteShapeRenderer r)
    {
        SetRenderer(r);
    }

    public void SetRenderer(SpriteRenderer r)
    {
        _spriteRenderer = r;
    }
    public void SetRenderer(SpriteShapeRenderer r)
    {
        _shapeRenderer = r;
    }
    public System.Type GetRendererType()
    {
        if (_spriteRenderer != null) return typeof(SpriteRenderer);
        return typeof(SpriteShapeRenderer);
    }
}

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
    Dictionary<string, List<SpriteRendererGeneral>> _spriteMap;
    int _selectedLayerIndex = 0;

    void OnEnable()
    {
        _target = target as LevelDesignHelper;

        _displayLevelDesign = _target.DisplayLevelDesign;
        ToggleLevelDesignDisplay();

        _spriteMap = new Dictionary<string, List<SpriteRendererGeneral>>();
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
        _sortingLayerNames = SleepyEditorUtils.GetSortingLayerNames();
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
            _selectedLayerIndex = EditorGUILayout.Popup(_selectedLayerIndex, _sortingLayerNames);
            if (GUILayout.Button("Sort this"))
            {
                SortLayer(_sortingLayerNames[_selectedLayerIndex]);
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
            SortLayerBasedOnZIndex(layerName);
        }
    }
    void SortLayer(string layerName)
    {
        if (_sortingLayerNames == null) return;

        GetAllGameSprites();
        SortLayerBasedOnZIndex(layerName);
    }

    void GetAllGameSprites()
    {
        List<SpriteRendererGeneral> renderers = new List<SpriteRendererGeneral>();
        GameObject.FindObjectsOfType<SpriteRenderer>()
            .ToList()
                .ForEach((System.Action<SpriteRenderer>)(s =>
                {
                    if (s.GetComponent<global::MatchSortingLayer>() == null)
                        renderers.Add(new SpriteRendererGeneral(s));
                }));
        GameObject.FindObjectsOfType<SpriteShapeRenderer>()
            .ToList()
                .ForEach((System.Action<SpriteShapeRenderer>)(s =>
                {
                    if (s.GetComponent<global::MatchSortingLayer>() == null)
                        renderers.Add(new SpriteRendererGeneral(s));
                }));

        renderers.ForEach(x =>
        {
            string sortingLayerName;
            if (x.GetRendererType() == typeof(SpriteRenderer))
            {
                sortingLayerName = x.SpriteRenderer.sortingLayerName;
            }
            else
            {
                sortingLayerName = x.ShapeRenderer.sortingLayerName;
            }

            if (!_target.ExcludeLayer.ToList<string>().Any(e => e == sortingLayerName))
            {
                if (_spriteMap.ContainsKey(sortingLayerName))
                {
                    _spriteMap[sortingLayerName].Add(x);
                }
                else
                {
                    _spriteMap.Add(sortingLayerName, new List<SpriteRendererGeneral> { x });
                }
            }
        });
    }

    void SortLayerBasedOnZIndex(string layerName)
    {
        if (!_spriteMap.ContainsKey(layerName)) return;

        var renderers = _spriteMap[layerName];

        // Find the 0 layer
        SpriteRendererGeneral zeroRenderer = renderers
            .OrderBy(x => Mathf.Abs(0 - x.ZIndex)).First();
        zeroRenderer.SortingOrder = 0;
        renderers.Remove(zeroRenderer);

        // Find all the layers in front(and behind) of the 0 layer and sort it.
        var frontRenderers = new List<SpriteRendererGeneral>();
        renderers.RemoveAll(x =>
        {
            if (x.ZIndex <= zeroRenderer.ZIndex)
            {
                frontRenderers.Add(x);
                return true;
            }
            return false;
        });
        frontRenderers = frontRenderers.OrderByDescending(x => x.ZIndex)
            .ToList();
        var behindRenderers = renderers.OrderBy(x => x.ZIndex)
            .ToList();
        // Assign sorting order for all the layers
        for (int i = 0, order = 0; i < frontRenderers.Count; i++)
        {
            var renderer = frontRenderers[i];
            if (i == 0) // If this is the first one
            {
                if (renderer.ZIndex != zeroRenderer.ZIndex)
                {
                    order++;
                }
            }
            else
            {
                var prevRenderer = frontRenderers[i - 1];
                if (renderer.ZIndex != prevRenderer.ZIndex)
                {
                    order++;
                }
            }

            renderer.SortingOrder = order;
        }
        for (int i = 0, order = 0; i < behindRenderers.Count; i++)
        {
            var renderer = behindRenderers[i];
            if (i == 0) // If this is the first one
            {
                if (renderer.ZIndex != zeroRenderer.ZIndex)
                {
                    order--;
                }
            }
            else
            {
                var prevRenderer = behindRenderers[i - 1];
                if (renderer.ZIndex != prevRenderer.ZIndex)
                {
                    order--;
                }
            }

            renderer.SortingOrder = order;
        }
    }
}
#endif