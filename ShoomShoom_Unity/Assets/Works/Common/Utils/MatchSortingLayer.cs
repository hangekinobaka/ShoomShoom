using UnityEngine;

[ExecuteInEditMode]
public class MatchSortingLayer : MonoBehaviour, IIgnoreBySortingHelper
{

    private void Awake()
    {
        MatchOrder();
    }

    private void OnGUI()
    {
        MatchOrder();
    }

    void MatchOrder()
    {
        GetComponent<SpriteRenderer>().sortingOrder = transform.parent.GetComponent<SpriteRenderer>().sortingOrder;
    }
}
