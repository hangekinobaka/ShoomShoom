#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Member;

[CustomEditor(typeof(LevelDesignHelper))]
public class LevelDesignHelper_Editor : Editor
{
    LevelDesignHelper _levelDesignHelper = null;

    // Toggle level design
    bool _displayLevelDesign;
    List<SpriteRenderer> _levelDesignRenderers;

    void OnEnable()
    {
        _levelDesignHelper = target as LevelDesignHelper;

        _displayLevelDesign = _levelDesignHelper.DisplayLevelDesign;
        ToggleLevelDesignDisplay();
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical("BOX");
        _levelDesignHelper.LevelDesign = (Transform)EditorGUILayout.ObjectField("Level Design",
            _levelDesignHelper.LevelDesign,
            typeof(Transform),
            true);
        bool displayLevelDesign = EditorGUILayout.Toggle("Display level design?", _displayLevelDesign);
        if (_displayLevelDesign != displayLevelDesign)
        {
            _displayLevelDesign = displayLevelDesign;
            ToggleLevelDesignDisplay();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Toggle the level design objects' sprite display
    /// </summary>
    void ToggleLevelDesignDisplay()
    {
        _levelDesignRenderers = _levelDesignHelper.LevelDesign.GetComponentsInChildren<SpriteRenderer>().ToList<SpriteRenderer>();
        _levelDesignRenderers.ForEach(x =>
        {
            x.enabled = _displayLevelDesign;
        });
        _levelDesignHelper.DisplayLevelDesign = _displayLevelDesign;
    }
}
#endif