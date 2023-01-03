using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [SerializeField] string activatorTag = null;
    [SerializeField] bool deactivateOnExit = false;
    [SerializeField] GameObject[] activeObjs = null;
    [SerializeField] GameObject[] deasctiveObjs = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(activatorTag))
        {
            foreach (var obj in deasctiveObjs)
                obj.SetActive(false);
            foreach (var obj in activeObjs)
                obj.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (deactivateOnExit && collision.CompareTag(activatorTag))
        {
            foreach (var obj in activeObjs)
                obj.SetActive(false);
            foreach (var obj in deasctiveObjs)
                obj.SetActive(true);
        }
    }
}
