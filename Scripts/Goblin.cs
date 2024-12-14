using System;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour, IDamagable
{
    public Action OnTakingDamage { get; set; }
    public bool isDead { get; set; }

    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private Animator animator;
    [SerializeField] private GoblinData data;

    [SerializeField] private Transform scanPoint;
    [SerializeField] private float scanRange;
    [SerializeField] private float attackRange;

    [SerializeField] bool isDamagableDetected;
    [SerializeField] bool isDamagableInAttackRange;
    [SerializeField] GoblinAnimatorManager animatorManager;
    [SerializeField] Transform hammerDealer;
    [SerializeField] List<Collider2D> damagableColliders;
    [SerializeField] Collider2D damagableTarget;

    private static Vector2 diedColliderOffset = new Vector2(-1.8f, 0.7766428f);
    private static Vector2 diedColliderSize = new Vector2(3.657529f, 1.474371f);
    private static Vector2 initialColliderOffset;
    private static Vector2 initialColliderSize;
    private float offSet = 5f;
    private IState idle;
    private IState run;
    private IState attack;
    private IState hurt;
    private IState died;
    private Vector2 initialPoint;

    private void Start()
    {
        idle = new Idle(data);
        run = new Run(data);
        attack = new Attack(data);
        hurt = new Hurt(data);
        died = new Died(data);

        StateTransition toIdle = new StateTransition(idle, () => { return !isDamagableDetected; }, 0);
        StateTransition toRun = new StateTransition(run, () => { return isDamagableDetected; }, 1);
        StateTransition toAttack = new StateTransition(attack, () => { return isDamagableInAttackRange && isDamagableDetected; }, 2);

        StateTransition toDied = new StateTransition(died, () => { return data.hp <= 0; }, 3);
        toDied.SetOnTransition(() => { animator.SetTrigger("Died"); });

        StateTransition toHurt = new StateTransition(hurt, () => { return !data.isDied; });
        toHurt.SetOnTransition(() => { animator.SetTrigger("TakeDamage"); });


        stateMachine.AddAnyTransition(toIdle);
        stateMachine.AddAnyTransition(toRun);
        stateMachine.AddAnyTransition(toAttack);
        stateMachine.AddAnyTransition(toDied);

        Action onTakingDamageAction = null;
        OnTakingDamage += () => onTakingDamageAction?.Invoke();
        stateMachine.AddAnyTransitionTrigger(ref onTakingDamageAction, toHurt);

        animatorManager.OnAttacked += dealDamage;

        initialColliderOffset = data.collider.offset;
        initialColliderSize = data.collider.size;

        initialPoint = gameObject.transform.position;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void dealDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(hammerDealer.position, 2f);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                if (!damagable.Equals(this))
                {
                    damagable.takeDamage();
                }
            }
        }
    }

    private void Update()
    {
        if (!data.isDied)
        {
            damagableTarget = FindClosestDamagableCollider(scanTarget());

            if (damagableTarget == null)
            {
                isDamagableDetected = false;
            }

            else
            {
                data.target = damagableTarget.transform;
                isDamagableDetected = true;

                Vector3 towardsTarget = (damagableTarget.transform.position - data.goblinModel.transform.position).normalized;
                towardsTarget.y = 0;

                data.goblinModel.transform.rotation = Quaternion.LookRotation(towardsTarget);

                if (Vector2.Distance(scanPoint.position, damagableTarget.transform.position) < attackRange)
                {
                    isDamagableInAttackRange = true;
                }

                else
                {
                    isDamagableInAttackRange = false;
                }
            }
        }


        stateMachine.Update();

        handleAnimations();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(scanPoint.position, scanRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scanPoint.position, attackRange);
    }

    private void restartGoblin()
    {
        isDead = false;

        data.hp = 10;

        data.rb.mass = 200;
        data.collider.offset = initialColliderOffset;
        data.collider.size = initialColliderSize;
        data.goblin.gameObject.layer = LayerMask.NameToLayer("Default");
        transform.position = initialPoint;

        stateMachine.SetState(idle);
    }

    private List<Collider2D> scanTarget()
    {
        damagableColliders = new List<Collider2D>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(scanPoint.position, scanRange);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
            {
                if (!damagable.Equals(this) && !damagable.isDead)
                {
                    damagableColliders.Add(collider);
                }
            }
        }

        return damagableColliders;
    }

    private Collider2D FindClosestDamagableCollider(List<Collider2D> colliders)
    {
        Collider2D closestDamagableCollider = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider != null)
            {
                float distance = Vector2.Distance(scanPoint.position, collider.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamagableCollider = collider;
                }
            }
        }

        return closestDamagableCollider;
    }

    private void handleAnimations()
    {
        animator.SetBool("Idling", data.isIdling);
        animator.SetBool("Running", data.isRunning);
        animator.SetBool("Attacking", data.isAttacking);
    }

    public void takeDamage()
    {
        OnTakingDamage?.Invoke();
        data.hp--;

        if (data.hp <= 0)
        {
            isDead = true;
        }
    }

    public void AttackEnded()
    {
        data.isAttacking = false;
    }

    class Idle : IState
    {
        public string type { get { return "Idle"; } set { } }
        GoblinData _data;
        public Idle(GoblinData data)
        {
            _data = data;
        }
        public void Enter()
        {
            _data.isIdling = true;
        }

        public void Exit()
        {
            _data.isIdling = false;
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    class Run : IState
    {
        public string type { get { return "Move"; } set { } }
        Rigidbody2D _rb;
        float moveSpeed = 10f;
        GoblinData _data;
        public Run(GoblinData data)
        {
            _data = data;
            _rb = _data.rb;
        }

        public void Enter()
        {
            _data.isRunning = true;
        }

        public void Exit()
        {
            _data.isRunning = false;
            _rb.velocity = Vector3.zero;
        }

        public void Tick()
        {
        }

        public void Update()
        {
            if (_data.target != null)
            {
                Vector3 moveDirection = (_data.target.transform.position - _rb.transform.position).normalized;
                Vector2 velocity = new Vector2(moveDirection.x, moveDirection.y).normalized * moveSpeed;

                _rb.velocity += velocity * Time.deltaTime;
            }
        }
    }

    class Attack : IState
    {
        public string type { get { return "Attack"; } set { } }
        GoblinData _data;
        public Attack(GoblinData data)
        {
            _data = data;
        }
        public void Enter()
        {
            _data.isAttacking = true;
        }

        public void Exit()
        {
            _data.isAttacking = false;
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    class Hurt : IState
    {
        public string type { get { return "Hurt"; } set { } }
        GoblinData _data;
        public Hurt(GoblinData data)
        {
            _data = data;
        }
        public void Enter()
        {
            _data.isHurting = true;
        }

        public void Exit()
        {
            _data.isHurting = false;
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    class Died : IState
    {
        public string type { get { return "Died"; } set { } }
        GoblinData _data;
        Vector3 offset;
        public Died(GoblinData data)
        {
            _data = data;
        }

        public void Enter()
        {
            _data.isDied = true;

            _data.rb.mass = 50;

            if (Vector3.Dot(_data.goblinModel.transform.forward, Vector3.right) < 0)
            {
                diedColliderOffset.x *= -1;
            }

            _data.collider.offset = diedColliderOffset;
            _data.collider.size = diedColliderSize;
            _data.goblin.gameObject.layer = LayerMask.NameToLayer("Platforms");
        }

        public void Exit()
        {
            _data.isDied = false;
        }

        public void Tick()
        {
        }

        public void Update()
        {
            float clampedX = Mathf.Clamp(_data.rb.position.x, _data.goblinLeftBound.position.x, _data.goblinRightBound.position.x);
            _data.rb.position = new Vector2(clampedX, _data.rb.position.y);
        }
    }


    [System.Serializable]
    class GoblinData
    {
        public bool isIdling;
        public bool isRunning;
        public bool isAttacking;
        public bool isHurting;
        public bool isDied;

        public Transform head;
        public Transform foot;

        public float hp;

        public Transform goblinModel;
        public Transform target;
        public Rigidbody2D rb;
        public BoxCollider2D collider;
        public GameObject goblin;
        public Transform goblinRightBound;
        public Transform goblinLeftBound;
    }
}