using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [SerializeField] string _activatorTag = null;
    [SerializeField] bool _deactivateOnExit = false;
    [SerializeField] GameObject[] _activeObjs = null;
    [SerializeField] GameObject[] _deasctiveObjs = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_activatorTag))
        {
            foreach (var obj in _deasctiveObjs)
                obj.SetActive(false);
            foreach (var obj in _activeObjs)
                obj.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_deactivateOnExit && collision.CompareTag(_activatorTag))
        {
            foreach (var obj in _activeObjs)
                obj.SetActive(false);
            foreach (var obj in _deasctiveObjs)
                obj.SetActive(true);
        }
    }
}
