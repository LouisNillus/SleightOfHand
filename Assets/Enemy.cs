using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{


    public float pathRefreshRate = 0f;
    [Range(0,5)]
    public float attackRange = 0f;
    [Range(0,5)]
    public float attackCastDelay = 0f;
    [Range(0,5)]
    public float attackCD = 0f;

    GameObject target;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        agent.stoppingDistance = attackRange / 2;
        target = Player.instance.gameObject;

        StartCoroutine(UpdatePath(pathRefreshRate));
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator UpdatePath(float refreshRate)
    {

        while(agent.transform.position != target.transform.position)
        {
            agent.destination = target.transform.position;
            yield return new WaitForSeconds(refreshRate);
        }

    }


    public IEnumerator Attack()
    {
        while(target.activeSelf)
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                if(hitCollider.gameObject == target)
                {
                    yield return new WaitForSeconds(attackCastDelay);
                    Debug.Log("hit !");
                }
            }
            yield return new WaitForSeconds(attackCD);
        }
    }
}
