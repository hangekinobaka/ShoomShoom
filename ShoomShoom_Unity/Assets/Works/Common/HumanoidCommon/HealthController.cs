using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] float _maxHealth = 100.0f;
    float _health;

    public event UnityAction OnDead;

    public HealthBar _healthBar;

    public void Init()
    {
        _health = _maxHealth;
        _healthBar.Init();
        _healthBar.SetMaxHealth(_health);
    }
    public void Init(float maxHealth)
    {
        _maxHealth = maxHealth;
        Init();
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

        _healthBar.DisplayHealthBar();
        _healthBar.SetHealth(_health);
    }

    public void Die()
    {
        if (OnDead != null) OnDead();
    }

}
