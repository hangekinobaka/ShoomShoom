using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalDisplayAttribute))]
public class ConditionalDisplayAttribute_Editor : PropertyDrawer
{
    ConditionalDisplayAttribute _attr;
    SerializedProperty _condPropertyName;

    bool _condMet = false;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The height of the property should be defaulted to the default height.
        return _condMet ? base.GetPropertyHeight(property, label) : 0;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Set the global variables.
        _attr = attribute as ConditionalDisplayAttribute;
        _condPropertyName = property.serializedObject.FindProperty(_attr.CondPropertyName);

        if (CompareProperty())
        {
            EditorGUI.PropertyField(position, property, label);
            _condMet = true;
        }
        else
        {
            _condMet = false;
        }
    }

    bool CompareProperty()
    {
        // get the value & compare based on types
        switch (_condPropertyName.type)
        { // Possible extend cases to support your own type
            case "bool":
                return _condPropertyName.boolValue.Equals(_attr.CondValue);
            case "Enum":
                return _condPropertyName.enumValueIndex.Equals((int)_attr.CondValue);
            default:
                Debug.LogError("Error: " + _condPropertyName.type + " is not supported");
                return true;
        }
    }
}