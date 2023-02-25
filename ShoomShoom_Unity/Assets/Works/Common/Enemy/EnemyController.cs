using UnityEngine;

public class EnemyController : MonoBehaviour
{
    readonly Vector3 RIGHT_DIR_SCALE = new Vector3(-1, 1, 1);
    readonly Vector3 LEFT_DIR_SCALE = Vector3.one;


    [Header("Basics")]
    [Tooltip("moving speed")]
    [SerializeField] protected float _speed = 5.0f;
    [SerializeField] protected float _accSpeed = 10f;
    [SerializeField] protected float _health = 100.0f;

    [Header("Patrol")]
    [SerializeField] protected float _patrolRange = 5.0f;
    [SerializeField] protected float _chaseRange = 18.0f;
    [SerializeField] protected float _chaseStopRange = 10.0f;

    [Header("Attacks")]
    [Tooltip("Basic attack damage")]
    [SerializeField] protected float _attackDamage = 10.0f;

    [Header("Helpers")]
    [SerializeField] bool _showChaseRange = true;

    [Header("Refs")]
    [SerializeField] Transform _myModelTrans;
    Rigidbody2D _rigidbody;
    Transform _playerTrans;

    // local states
    protected bool _chasing = false;
    protected bool _inAttackRange = false;
    protected Vector3 _velocity;

    void Start()
    {
        _playerTrans = LevelManager_Fightscene.Instance.PlayerTrans;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        DetectAndChase();
        UpdateDirection();
        UpdatePhysics();
    }

    void DetectAndChase()
    {
        _velocity = _rigidbody.velocity;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTrans.position);

        _chasing = false;
        _inAttackRange = false;

        if (distanceToPlayer < _chaseRange && distanceToPlayer > _chaseStopRange)
        {
            Vector3 moveDir = (_playerTrans.position - transform.position).normalized;
            moveDir.y = 0;

            _velocity += moveDir * _accSpeed * Time.fixedDeltaTime;

            // Clamp horizontal speed.
            _velocity.x = Mathf.Clamp(_velocity.x, -_speed, _speed);

            _chasing = true;
        }
        else if (distanceToPlayer <= _chaseStopRange)
        {
            // if it is moving then stop it first
            if (_velocity.magnitude > 0)
            {
                // Smoothly reduce the velocity
                Vector3.SmoothDamp(_velocity, Vector3.zero, ref _velocity, .8f);
            }
            // Player is in the attack range
            _inAttackRange = true;
        }
    }

    void UpdateDirection()
    {

        if (_velocity.x > 0)
        {
            _myModelTrans.transform.localScale = RIGHT_DIR_SCALE;
        }
        else if (_velocity.x < 0)
        {
            _myModelTrans.transform.localScale = LEFT_DIR_SCALE;
        }
        else
        {
            // if in sttack range, then always face the player
            if (_inAttackRange)
            {
                if (_playerTrans.position.x <= _myModelTrans.position.x)
                {
                    _myModelTrans.transform.localScale = LEFT_DIR_SCALE;
                }
                else
                {
                    _myModelTrans.transform.localScale = RIGHT_DIR_SCALE;
                }
            }
        }
    }
    void UpdatePhysics()
    {
        _rigidbody.velocity = _velocity;
    }

    private void OnDrawGizmos()
    {
        if (_showChaseRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _chaseRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseStopRange);
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    public void Die() { }

    public virtual void Attack() { }
    public virtual void DistanceAttack() { }
}
