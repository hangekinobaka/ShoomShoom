using UnityEngine.Events;

public interface IDamagable
{
    public event UnityAction OnHurt, OnDead, OnCriticalHurt;
    public abstract void TakeDamage(float damage, bool isCritical = false);
}