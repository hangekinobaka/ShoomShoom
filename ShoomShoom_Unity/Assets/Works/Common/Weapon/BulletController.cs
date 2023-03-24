using UniRx;
using UnityEngine;

public enum BulletHitType
{
    Normal,
    Hard,
    Water,
    Metal
}

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
    [SerializeField] GameObject _bulletHitEffect_Metal;
    BulletHitType _bulletHitType = BulletHitType.Normal;

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

            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                _bulletHitType = BulletHitType.Normal;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("GroundHard"))
            {
                _bulletHitType = BulletHitType.Hard;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("GroundWater"))
            {
                _bulletHitType = BulletHitType.Water;
            }

            PlayHitEffect(transform.position);
        }
    }

    void PlayHitEffect(Vector3 pos)
    {
        GameObject prefab = null;
        switch (_bulletHitType)
        {
            case BulletHitType.Normal:
                prefab = _bulletHitEffect_Normal;
                break;
            case BulletHitType.Hard:
                prefab = _bulletHitEffect_Hard;
                break;
            case BulletHitType.Water:
                prefab = _bulletHitEffect_Water;
                break;
            case BulletHitType.Metal:
                prefab = _bulletHitEffect_Metal;
                break;
            default:
                break;
        }

        if (prefab != null)
        {
            GameObject effect = Instantiate(prefab, GlobalEffectsContainer.Instance.transform);
            pos.z = -1;
            effect.transform.position = pos;
        }
    }

    public void TriggerHitEffect(BulletHitType type, Vector3 hitPos)
    {
        _bulletHitType = type;
        PlayHitEffect(hitPos);

        PoolManager_Fightscene.Instance.PistolBulletPool.Release(gameObject);
    }
}
