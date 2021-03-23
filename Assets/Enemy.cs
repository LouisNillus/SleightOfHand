using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public int HP;


    public float pathRefreshRate = 0f;

    public bool attackRangeFromEnemySize;
    [Range(0,50)]
    public float chaseRange = 0f;
    [Range(0,5)]
    public float attackRange = 0f;
    [Range(0,5)]
    public float attackCastDelay = 0f;
    [Range(0,5)]
    public float attackCD = 0f;

    [HideInInspector] public UnityEvent OnDead;

    public int damages;

    public GameObject target;
    GameObject targetToChase;
    bool targetInRange = false;

    NavMeshAgent agent;


    IEnumerator attack;
    IEnumerator updatePath;

    // Start
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        target = Player.instance.gameObject;
        targetToChase = Player.instance.gameObject;

        attack = UpdateAttack();
        updatePath = UpdatePath(pathRefreshRate);

        if (attackRangeFromEnemySize) attackRange = this.GetComponent<Collider>().bounds.size.x;

        agent.stoppingDistance = attackRange * 1.5f;


        StartCoroutine(UpdatePath(pathRefreshRate));
    }

    // Update
    void Update()
    {
        //target = GetTarget();

        Chase();
        Attack();
        /*if (target != null && target != GetTarget())
        {
            StopCoroutine(attack);
            StartCoroutine(attack);
        }*/

        if (target == null) StopCoroutine(attack);
        
        OnDeadTrigger();
    }


    #region Attack
    public void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == targetToChase && targetInRange == false)
            {
                targetInRange = true;
                StartCoroutine(attack);
                return;
            }
        }

        targetInRange = false;
        StopCoroutine(attack);
    }
    public IEnumerator UpdateAttack()
    {
        while(true)
        {
            yield return new WaitForSeconds(attackCastDelay);
            target.GetComponent<Player>().TakeDamages(damages);
            yield return new WaitForSeconds(attackCD);
        }
    }
    #endregion

    #region Chase
    public void Chase()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, chaseRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Player>() && hitCollider.gameObject != targetToChase)
            {
                targetToChase = hitCollider.gameObject;
                StartCoroutine(updatePath);
                return;
            }
        }

        StopCoroutine(updatePath);
        targetToChase = null;
    }
    public IEnumerator UpdatePath(float refreshRate)
    {       
        while(targetToChase != null && agent.transform.position != targetToChase.transform.position)
        {
            agent.destination = targetToChase.transform.position;
            yield return new WaitForSeconds(refreshRate);
        }

    }
    #endregion

    public void OnDeadTrigger()
    {
        if (HP <= 0) OnDead.Invoke();
    }

    public void OnTargetEnter()
    {

    }
}
