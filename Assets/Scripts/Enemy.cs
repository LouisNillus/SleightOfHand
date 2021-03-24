using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Enemy : MonoBehaviour
{
    public bool debug;

    [Header("Data")]
    [Range(0,500)]
    public int HP;
    [Range(0,100)]
    public int damages;
    
    [Header("Pathfinding")]
    public float pathRefreshRate = 0f;

    [Header("Ranges")]
    [Range(0,50)]
    public float chaseRange = 0f;
    [Range(0,15)]
    public float attackRange = 0f;
    [Range(0,15)]
    public float triggerRange = 0f;


    [Header("Attack")]
    public bool attackRangeFromEnemySize;
    [Range(0,5)]
    public float attackCastDelay = 0f;
    [Range(0,5)]
    public float attackCD = 0f;

    [HideInInspector] public UnityEvent OnDead;
    [HideInInspector] public UnityEvent OnTargetEnterInSight;
    [HideInInspector] public UnityEvent OnTargetEnterInRange;


    public GameObject target;
    public bool isAttacking = false;
    bool isChasing = false;

    NavMeshAgent agent;

    IEnumerator attack;
    IEnumerator updatePath;


    // Start
    void Start()
    {
        CardThrowing.instance.enemies.Add(this.gameObject);

        agent = this.GetComponent<NavMeshAgent>();

        target = Player.instance.gameObject;

        attack = UpdateAttack();
        updatePath = UpdatePath(pathRefreshRate);

        if (attackRangeFromEnemySize) attackRange = this.GetComponent<Collider>().bounds.size.x;

        //agent.stoppingDistance = attackRange * 1.5f;

        OnDead.AddListener(Death);

        StartCoroutine(UpdatePath(pathRefreshRate));
    }

    // Update
    void Update()
    {
        Chase();
        Attack();

        if(Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(Freeze(2f));
        }

        OnDeadTrigger();
    }


    #region Attack
    public void Attack()
    {
        if (IsInAttackRange() && IsInTriggerRange() && isAttacking == false)
        {
            StartCoroutine(UpdateAttack());
        }
    }
    public IEnumerator UpdateAttack()
    {
        isAttacking = true;

        Debug.Log("Attack !");
        float timeToCast = 0f;
        float timeToCD = 0f;

        while(timeToCast < attackCastDelay)
        {
            if (IsInAttackRange() == false) //Cancel l'attaque si la target sort de la range
            { 
                Debug.Log("Cancel Attack");
                yield break;
            }

            timeToCast += Time.deltaTime;
            yield return null;

        }

        if (IsInAttackRange()) target.GetComponent<Player>().TakeDamages(damages);

        while(timeToCD <= attackCD)
        {
            timeToCD += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    public bool IsInAttackRange()
    {
        if (target == null) return false;
        else return Methods.FlatDistanceTo(this.transform.position, target.transform.position) <= attackRange;
    }

    public bool IsInTriggerRange()
    {
        if (target == null) return false;
        else return Methods.FlatDistanceTo(this.transform.position, target.transform.position) <= triggerRange;
    }

    #endregion

    #region Chase
    public void Chase()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, chaseRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Player>() && hitCollider.gameObject != target) //Lag ici
            {
                target = hitCollider.gameObject;
                StartCoroutine(updatePath);
                return;
            }
            else
            {
                StopCoroutine(updatePath);
                isChasing = false;
                target = null;
            }
        }

    }
    public IEnumerator UpdatePath(float refreshRate)
    {

        while(target != null && agent.transform.position != target.transform.position)
        {
            isChasing = true;

            agent.destination = target.transform.position;
            yield return new WaitForSeconds(refreshRate);
        }
    }
    #endregion

    public void Death()
    {
        CardThrowing.instance.enemies.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    public void TakeDamages(int value)
    {
        HP -= value;
    }
    public void OnDeadTrigger()
    {
        if (HP <= 0)
        {
            OnDead.Invoke();
        }
    }

    public IEnumerator Freeze(float duration)
    {
        float time = 0f;

        while(time < duration)
        {
            Debug.Log("frozen");
            agent.isStopped = true;
            time += Time.deltaTime;
            yield return null;
        }
            agent.isStopped = false;
    }
}


 [CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{

    private Enemy e;

    public void OnSceneGUI()
    {
        e = this.target as Enemy;

        if(e.debug)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = Color.blue;
            Handles.DrawWireDisc(Methods.ChangeY(e.transform.position, e.transform.position.y - (e.transform.localScale.y/2)), e.transform.up, e.chaseRange);
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(Methods.ChangeY(e.transform.position, e.transform.position.y - (e.transform.localScale.y / 2)), e.transform.up, e.attackRange);
            Handles.color = Color.red;
            Handles.DrawWireDisc(Methods.ChangeY(e.transform.position, e.transform.position.y - (e.transform.localScale.y / 2)), e.transform.up, e.triggerRange);
        }
    }
}
