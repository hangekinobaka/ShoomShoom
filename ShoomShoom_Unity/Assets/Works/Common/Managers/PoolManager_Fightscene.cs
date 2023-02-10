using UnityEngine;
using UnityEngine.Pool;

public class PoolManager_Fightscene : Singleton<PoolManager_Fightscene>
{
    // Gun smoke pool
    [SerializeField] private GameObject _gunEffectPrefab;
    public ObjectPool<GameObject> GunEffectPool { get; set; }


    private void Start()
    {
        CreateGunEffectPool();
    }

    private void CreateGunEffectPool()
    {
        GunEffectPool = new ObjectPool<GameObject>(
            () => Instantiate(_gunEffectPrefab, GlobalEffectsContainer.Instance.transform), // Create function
            shape => shape.gameObject.SetActive(true), // OnGet function, when init
            shape => shape.gameObject.SetActive(false), // OnRelease function, when give it back to pool
            shape => Destroy(shape.gameObject), // OnDestory function, when give back to the pool but the pool is filled
            false, // Collection check
            10, // pool normal size, Default 10
            20 // pool Max size, Default 10000
        );
    }
}
