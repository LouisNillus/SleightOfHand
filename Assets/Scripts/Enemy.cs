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

    [Header("Behaviour")]
    [Range(0, 10)]
    public float lookPlayerRotationSpeed;

    [HideInInspector] public UnityEvent OnDead;
    [HideInInspector] public UnityEvent OnTargetEnterInSight;
    [HideInInspector] public UnityEvent OnTargetEnterInRange;

    public GameObject slashPs;

    public GameObject target;
    public SkinnedMeshRenderer mr;
    public bool isAttacking = false;
    public bool canAttack = true;
    bool isChasing = false;

    NavMeshAgent agent;
    public Animator animator;

    Vector3 currentPosition;
    Vector3 lastPosition;

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

        //OnDead.AddListener(Death);

        StartCoroutine(UpdatePath(pathRefreshRate));
    }

    // Update
    void Update()
    {
        currentPosition = this.transform.position;

        Chase();
        Attack();
        Movement();
        Rotation();
        OnDeadTrigger();



        lastPosition = currentPosition;
    }

    public void Movement()
    {
        animator.SetFloat("Speed", (transform.position - lastPosition).magnitude * 100f);
    }

    public void Rotation()
    {
        if(target != null)
        {
            Vector3 _direction = (target.transform.position - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * lookPlayerRotationSpeed);
        }

    }

    #region Attack
    public void Attack()
    {
        if (canAttack && IsInAttackRange() && IsInTriggerRange() && isAttacking == false)
        {
            StartCoroutine(UpdateAttack());
        }
    }
    public IEnumerator UpdateAttack()
    {
        isAttacking = true;

        float timeToCast = 0f;
        float timeToCD = 0f;
        float timeToRotate = 0f;

        while(timeToCast < attackCastDelay)
        {
            
            agent.isStopped = true;
            if (IsInAttackRange() == false) //Cancel l'attaque si la target sort de la range
            { 
                agent.isStopped = false;
                isAttacking = false;
                yield break;
            }

            timeToCast += Time.deltaTime;
            yield return null;
        }

            agent.isStopped = false;




        while (timeToRotate < 0.5f)
        {


            timeToRotate += Time.deltaTime;
            yield return null;
        }


        animator.SetTrigger("Attack");

        if(slashPs != null)
        foreach(ParticleSystem ps in slashPs.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }


        if (IsInAttackRange() && target.GetComponent<Player>().HP > 0) target.GetComponent<Player>().TakeDamages(damages);

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

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(DissolveToDeath(2f));
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

    public void TakeDamages(int value)
    {
        animator.SetTrigger("Hit");
        HP -= value;
    }

    bool isdying;
    public void OnDeadTrigger()
    {
        if (HP <= 0 && isdying == false)
        {
            isdying = true;
            this.GetComponent<Collider>().enabled = false;
            StopAllCoroutines();
            animator.SetTrigger("Dead");
            CardThrowing.instance.enemies.Remove(this.gameObject);
            agent.isStopped = true;
            StartCoroutine(Death());
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Methods.ChangeY(transform.position, transform.position.y - (transform.localScale.y / 2)), Methods.ChangeX(Methods.ChangeY(transform.position, transform.position.y - (transform.localScale.y / 2)), transform.position.x + attackRange));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Methods.ChangeY(transform.position, transform.position.y - (transform.localScale.y / 2)), Methods.ChangeX(Methods.ChangeY(transform.position, transform.position.y - (transform.localScale.y / 2)), transform.position.x + triggerRange));
    }

    public IEnumerator DissolveToDeath(float duration)
    {
        float t = 0f;

        while( t < duration)
        {
            foreach (Material m in mr.materials)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                m.SetFloat("_alphaClipDissolve", Mathf.Lerp(0f, 1f, (t / duration)));
                mpb.SetFloat("_alphaClipDissolve", Mathf.Lerp(0f, 1f, (t / duration)));
                mr.SetPropertyBlock(mpb);
            }
            t += Time.deltaTime;
            yield return null;
        }
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
