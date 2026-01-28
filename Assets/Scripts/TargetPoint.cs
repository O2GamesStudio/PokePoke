using UnityEngine;
using System.Collections;
using Lean.Pool;

public class TargetPoint : MonoBehaviour, IPoolable
{
    [Header("Rotation Settings")]
    [SerializeField] float minRotationSpeed = 30f;
    [SerializeField] float maxRotationSpeed = 120f;

    [Header("Collision Settings")]
    [SerializeField] float initialCollisionCheckDelay = 0.1f;

    private bool isCompleted = false;
    private bool isDespawning = false;
    private bool canPlayAnimation = false;
    private Collider2D pointCollider;
    private WaitForFixedUpdate waitFixed;
    private float rotationSpeed;
    private float spawnTime;

    void Awake()
    {
        pointCollider = GetComponent<Collider2D>();
        if (pointCollider == null)
        {
            pointCollider = gameObject.AddComponent<CircleCollider2D>();
            ((CircleCollider2D)pointCollider).radius = 0.3f;
        }
        pointCollider.isTrigger = true;
        waitFixed = new WaitForFixedUpdate();
    }

    void IPoolable.OnSpawn()
    {
        isCompleted = false;
        isDespawning = false;
        canPlayAnimation = false;
        spawnTime = Time.time;
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.value > 0.5f) rotationSpeed *= -1f;

        if (pointCollider != null)
        {
            pointCollider.enabled = true;
        }

        StartCoroutine(CheckOverlapNextFrame());
        StartCoroutine(EnableAnimationAfterDelay());
    }

    void IPoolable.OnDespawn()
    {
        StopAllCoroutines();
        isCompleted = false;
        isDespawning = false;
        canPlayAnimation = false;
    }

    void Update()
    {
        if (!isCompleted && !isDespawning)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator EnableAnimationAfterDelay()
    {
        yield return new WaitForSeconds(initialCollisionCheckDelay);
        canPlayAnimation = true;
    }

    IEnumerator CheckOverlapNextFrame()
    {
        yield return waitFixed;
        CheckOverlapWithObstacles();
    }

    void CheckOverlapWithObstacles()
    {
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        for (int i = 0; i < overlaps.Length; i++)
        {
            if (overlaps[i] == null || overlaps[i] == pointCollider) continue;
            if (!overlaps[i].CompareTag("StuckObj")) continue;

            StuckObj stuckObj = overlaps[i].GetComponent<StuckObj>();
            if (stuckObj != null && stuckObj.IsStuckToTarget())
            {
                CompletePoint();
                return;
            }
        }
    }

    public void CompletePoint()
    {
        if (isCompleted || isDespawning) return;

        isCompleted = true;
        isDespawning = true;

        TargetPointManager.Instance?.OnPointCompleted(this);

        SpriteAnimator animator = GetComponent<SpriteAnimator>();
        if (animator != null && canPlayAnimation)
        {
            animator.PlayAnimation(() =>
            {
                if (gameObject != null)
                {
                    LeanPool.Despawn(gameObject);
                }
            });
        }
        else
        {
            LeanPool.Despawn(gameObject);
        }
    }

    public bool IsCompleted => isCompleted;
}