using UniRx;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Basic parameter")]
    [SerializeField] float _bulletSpeed = 60f;
    [Tooltip("millisecond")]
    [SerializeField] float _bulletLifeTime = 500f;

    // These bullet effects has its own destruction methods, 
    // not included in our effect pool system.
    [Header("effects")]
    [SerializeField] GameObject _bulletHitEffect_Normal;
    [SerializeField] GameObject _bulletHitEffect_Hard;
    [SerializeField] GameObject _bulletHitEffect_Water;

    Rigidbody2D _rigidbody;
    CompositeDisposable _disposable;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        _disposable.Dispose();
    }

    public void Init(Vector3 dir)
    {
        _rigidbody.AddForce(
            dir * _bulletSpeed,
            ForceMode2D.Impulse
            );
        // Destroy the bullet when its lifetime comes to the end
        _disposable = new CompositeDisposable();
        Observable.Timer(System.TimeSpan.FromMilliseconds(_bulletLifeTime))
            .Subscribe(_ => PoolManager_Fightscene.Instance.PistolBulletPool.Release(gameObject))
            .AddTo(_disposable);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            PoolManager_Fightscene.Instance.PistolBulletPool.Release(gameObject);

            GameObject prefab = null;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                prefab = _bulletHitEffect_Normal;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("GroundHard"))
            {
                prefab = _bulletHitEffect_Hard;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("GroundWater"))
            {
                prefab = _bulletHitEffect_Water;
            }

            if (prefab != null)
            {
                GameObject effect = Instantiate(prefab, GlobalEffectsContainer.Instance.transform);
                effect.transform.position = transform.position;
            }
        }
    }
}
