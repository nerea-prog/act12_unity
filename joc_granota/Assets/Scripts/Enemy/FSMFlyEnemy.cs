using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FSMFlyEnemy : MonoBehaviour
{
    private enum EFly
    {
        Move,
        Attack
    }

    [Header("Movement")]
    public float speed = 1.2f;
    [SerializeField] private Vector2 direction = new Vector2(1f, 0.25f);

    [Header("Wall Detection")]
    [SerializeField] private LayerMask ground;
    [SerializeField] private Transform upPoint;
    [SerializeField] private Transform lateralPoint;
    [SerializeField] private Transform downPoint;
    [SerializeField] private float radiusDetectWalls = 0.25f;

    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private float attackDistance = 5f;

    [Header("Shoot")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private float shootInterval = 1.0f;

    private Rigidbody2D rb;
    private FSM<EFly> brain;

    private bool upHit;
    private bool lateralHit;
    private bool downHit;
    private float shootTimer;
    private bool warnedMissingBullet;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitFSM();
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }

        brain.Update();
    }

    private void InitFSM()
    {
        brain = new FSM<EFly>(EFly.Move);
        brain.SetOnEnter(EFly.Move, () => shootTimer = 0f);
        brain.SetOnEnter(EFly.Attack, () => shootTimer = 0f);
        brain.SetOnStay(EFly.Move, MoveUpdate);
        brain.SetOnStay(EFly.Attack, AttackUpdate);
    }

    private void MoveUpdate()
    {
        rb.linearVelocity = direction.normalized * speed;

        if (DetectCollision())
        {
            ChangeDirection();
        }

        if (IsPlayerClose())
        {
            brain.ChangeState(EFly.Attack);
        }
    }

    private void AttackUpdate()
    {
        if (player == null)
        {
            brain.ChangeState(EFly.Move);
            return;
        }

        direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            if (bulletPrefab == null)
            {
                if (!warnedMissingBullet)
                {
                    warnedMissingBullet = true;
                    Debug.LogWarning("FSMFlyEnemy: falta asignar Bullet Prefab en el Inspector.", this);
                }
                return;
            }

            EnemyBullet currentBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            currentBullet.dir = direction;
        }

        if (!IsPlayerClose())
        {
            brain.ChangeState(EFly.Move);
        }
    }

    private bool DetectCollision()
    {
        if (upPoint == null || lateralPoint == null || downPoint == null)
        {
            return false;
        }

        upHit = Physics2D.OverlapCircle(upPoint.position, radiusDetectWalls, ground);
        lateralHit = Physics2D.OverlapCircle(lateralPoint.position, radiusDetectWalls, ground);
        downHit = Physics2D.OverlapCircle(downPoint.position, radiusDetectWalls, ground);
        return upHit || lateralHit || downHit;
    }

    private void ChangeDirection()
    {
        if (lateralHit)
        {
            direction.x = -direction.x;
        }

        if (upHit && direction.y > 0f)
        {
            direction.y = -direction.y;
        }

        if (downHit && direction.y < 0f)
        {
            direction.y = -direction.y;
        }
    }

    private bool IsPlayerClose()
    {
        if (player == null)
        {
            return false;
        }

        return Vector2.Distance(transform.position, player.position) <= attackDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
