using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] float _maxHealth = 100.0f;
    float _health;

    public event UnityAction OnDead;

    public void Init()
    {
        _health = _maxHealth;
    }
    public void Init(float maxHealth)
    {
        _maxHealth = maxHealth;
        _health = maxHealth;
    }

    public void Heal(float dose = 1f)
    {
        if (_health >= _maxHealth) return;

        _health += dose;

        if (_health > _maxHealth) _health = _maxHealth;
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (OnDead != null) OnDead();
    }

}
