using UnityEngine;

public class UIMobileController : Singleton<UIMobileController>
{
    [SerializeField] GameObject _mobileUI;
    private void Start()
    {
        if (GameManager.Instance.CurPlatformType == PlatformType.Mobile)
            _mobileUI.SetActive(true);
        else
            _mobileUI.SetActive(false);
    }
}
