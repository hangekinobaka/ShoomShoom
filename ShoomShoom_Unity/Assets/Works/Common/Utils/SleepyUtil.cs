using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SleepyUtil
{
    public static class RangeUtil
    {
        public static string GetRangeName(Transform lRange, Transform rRange)
        {
            return $"{lRange.name}_to_{rRange.name}";
        }

    }

    public static class InputUtil
    {
        /// <summary>
        /// Test if the pointer is touching an UI element
        /// </summary>
        /// <returns></returns>
        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            foreach (RaycastResult r in results)
                if (r.gameObject.GetComponent<RectTransform>() != null)
                    return true;

            return false;
        }
    }
}