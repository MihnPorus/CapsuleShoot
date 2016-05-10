using UnityEngine;
using System.Collections;
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State
    {
        Idle,Chasing,Attacking
    };
    public static event System.Action OnDeathStatic;

    State currentState;


    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinmaterial;

    Color originalColour;

    float attackDistanceThreshold = 1.5f;
    float timeBetweenAttack = 1;
    float damage = 1;

    float nextAttackTime;
    float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();


        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = GetComponent<CapsuleCollider>().radius;
        }
    }

    protected override void Start () {
        base.Start();
        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTergetDeath;

            StartCoroutine(UpdatePath());
        }
	}

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHeathl, Color skinColour)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
            
        }
        startingHealth = enemyHeathl;
        deathEffect.startColor = new Color(skinColour.r, skinColour.g, skinColour.b, 1);
        skinmaterial = GetComponent<Renderer>().material;
        skinmaterial.color = skinColour;
        originalColour = skinmaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        AudioManager.intance.PlaySound("Impacts", transform.position);
        if (damage >= health)
        {
            if (OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            AudioManager.intance.PlaySound("Enemy Death", transform.position);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
            
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTergetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void Update() {
        if (hasTarget)
        {

            if (Time.time > nextAttackTime)
            {
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, deathEffect.startLifetime))
                {
                    nextAttackTime = Time.time + timeBetweenAttack;
                    AudioManager.intance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }
	}

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originaPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius+targetCollisionRadius/2);
        
        
        float attackSpeed = 3f;
        float precent = 0;

        skinmaterial.color = Color.red;
        bool hasApplieDamage = false;

        while (precent <= 1)
        {
            if (precent >= .5f && !hasApplieDamage) {
                hasApplieDamage = true;
                targetEntity.TakeDamage(damage);
            }
            precent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(precent, 2) + precent) * 4;
            transform.position = Vector3.Lerp(originaPosition, attackPosition, interpolation);

            yield return null;
        }
        skinmaterial.color = originalColour;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius+targetCollisionRadius +attackDistanceThreshold/4);
                
                if (!dead) {
                    pathfinder.SetDestination(targetPosition);
                }
                
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
