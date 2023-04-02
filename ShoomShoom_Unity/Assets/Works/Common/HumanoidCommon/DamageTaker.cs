using UnityEngine;

public class DamageTaker : MonoBehaviour
{
    [Header("The damage target instance")]
    [SerializeField, RequireInterface(typeof(IDamagable))]
    private Object _damagableObj;
    IDamagable _damagable => _damagableObj as IDamagable;

    [Header("Who can hurt you")]
    [SerializeField] LayerMask _attackerLayer;

    [Header("Basics")]
    [SerializeField] float _basicDamage = 10f;
    [SerializeField] bool _isCritical = false;

    const string WEAPON_NAME = "Weapon";

    private void OnEnable()
    {
        _damagable.OnDead += DisableMe;
    }

    private void OnDisable()
    {
        _damagable.OnDead -= DisableMe;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // e.g. If the trigger collider is a Player(layer) Weapon(tag), then get hit
        if (collision.CompareTag(WEAPON_NAME) && _attackerLayer == (_attackerLayer | (1 << collision.gameObject.layer)))
        {
            // if what hit us is a bullet
            BulletController bulletController;
            if (collision.TryGetComponent<BulletController>(out bulletController))
            {
                bulletController.TriggerHitEffect(BulletHitType.Metal, collision.transform.position);

                _damagable.TakeDamage(_basicDamage + bulletController.PlusDamage);
            }
        }
    }

    void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
