using UnityEngine;
using UnityEngine.Events;

public enum EnemyState
{
    Idle,
    TurnLeft,
    TurnRight,
    Run,
    Jump,
    Fall,
    Land
}

public enum PatrolType
{
    FromCenter,
    LeftToRight,
    RightToLeft
}

public class EnemyController : MonoBehaviour
{
    readonly Vector3 RIGHT_DIR_SCALE = new Vector3(-1, 1, 1);
    readonly Vector3 LEFT_DIR_SCALE = Vector3.one;

    [Header("Basics")]
    [Tooltip("moving speed")]
    [SerializeField] protected float _speed = 5.0f;
    [SerializeField] protected float _accSpeed = 10f;
    [Tooltip("if set to null, will get this object's starting position")]
    [SerializeField] protected Transform _startPointTrans;
    protected Vector3 _startPoint;

    [Header("Patrol")]
    [SerializeField] protected bool _isStationary = false;
    [ConditionalDisplay("_isStationary", false)]
    [SerializeField] bool _showPatrolRange = true;
    [ConditionalDisplay("_isStationary", false)]
    [SerializeField] protected float _patrolRange = 5.0f;
    [SerializeField] protected PatrolType _patrolType = PatrolType.FromCenter;

    [Header("Chase")]
    [SerializeField] protected bool _willChase = false;
    [ConditionalDisplay("_willChase", false)]
    [SerializeField] bool _showChaseRange = true;
    [ConditionalDisplay("_willChase", false)]
    [SerializeField] protected float _chaseRange = 18.0f;
    [ConditionalDisplay("_willChase", false)]
    [SerializeField] protected float _chaseStopRange = 10.0f;

    [Header("Attacks")]
    [Tooltip("Basic attack damage")]
    [SerializeField] protected float _attackDamage = 10.0f;

    [Header("Refs")]
    [SerializeField] protected Transform _myModelTrans;
    protected Rigidbody2D _rigidbody;
    protected Transform _playerTrans;
    protected HealthController _healthController;

    // local states
    protected bool _isChasing = false;
    protected bool _inAttackRange = false;
    protected Vector3 _velocity;
    public ReactProps<EnemyState> CurEnemyState = new ReactProps<EnemyState>(EnemyState.Idle);

    //events
    public event UnityAction OnAttack, OnDistanceAttack;

    void Start()
    {
        _playerTrans = LevelManager_Fightscene.Instance.PlayerTrans;
        _rigidbody = GetComponent<Rigidbody2D>();

        if (_healthController == null)
            _healthController = GetComponent<HealthController>();
        _healthController.Init();

        if (_startPointTrans == null) _startPointTrans = transform;
        _startPoint = _startPointTrans.position;
    }

    private void OnEnable()
    {
        if (_healthController == null)
            _healthController = GetComponent<HealthController>();
        _healthController.Init();
        _healthController.OnDead += Die;
    }

    private void OnDisable()
    {
        _healthController.OnDead -= Die;
    }

    private void FixedUpdate()
    {
        if (!_willChase) DetectAndChase();
        if (!_isStationary && !_isChasing && !_inAttackRange) UpdatePatrol();
        UpdateDirection();
        UpdatePhysics();
        UpdateState();
    }

    void DetectAndChase()
    {
        _velocity = _rigidbody.velocity;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTrans.position);

        _isChasing = false;
        _inAttackRange = false;

        // Chase if player enter the detection range
        if (distanceToPlayer < _chaseRange && distanceToPlayer > _chaseStopRange)
        {
            Vector3 moveDir = (_playerTrans.position - transform.position).normalized;
            moveDir.y = 0;

            _velocity += moveDir * _accSpeed * Time.fixedDeltaTime;

            // Clamp horizontal speed.
            _velocity.x = Mathf.Clamp(_velocity.x, -_speed, _speed);

            _isChasing = true;
        }
        // Stop when the player enter the attack range(and attack)
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

    void UpdatePatrol()
    {
        // Get init vals
        _velocity = _rigidbody.velocity;
        float curX = transform.position.x;

        // First generate the patrol range
        float leftX = 0f, rightX = 0f;
        switch (_patrolType)
        {
            case PatrolType.FromCenter:
                leftX = _startPoint.x - _patrolRange / 2;
                rightX = _startPoint.x + _patrolRange / 2;
                break;
            case PatrolType.LeftToRight:
                leftX = _startPoint.x;
                rightX = _startPoint.x + _patrolRange;
                break;
            case PatrolType.RightToLeft:
                leftX = _startPoint.x - _patrolRange;
                rightX = _startPoint.x;
                break;
            default:
                break;
        }

        // Go back to the patrol range if is currently out of range
        if (curX < leftX)
        {
            _velocity += Vector3.right * _accSpeed * Time.fixedDeltaTime;
        }
        else if (curX > rightX)
        {
            _velocity += Vector3.left * _accSpeed * Time.fixedDeltaTime;
        }
        // Clamp horizontal speed.
        _velocity.x = Mathf.Clamp(_velocity.x, -_speed, _speed);

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
    void UpdateState()
    {
        if (Mathf.Abs(_velocity.x) > 0)
        {
            CurEnemyState.SetState(EnemyState.Run);
        }
        else
        {
            CurEnemyState.SetState(EnemyState.Idle);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_willChase && _showChaseRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _chaseRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseStopRange);
        }

        if (!_isStationary && _showPatrolRange)
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (_startPointTrans == null) _startPointTrans = transform;
                _startPoint = _startPointTrans.position;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_startPoint, _patrolRange);
        }
    }
#endif

    public virtual void Attack() { }
    public virtual void DistanceAttack() { }
    public virtual void Die() { }
    public virtual void TakeDamage(float damage)
    {
        _healthController.TakeDamage(damage);
    }
}
