using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Card : MonoBehaviour, IPlayable
{

    [ReadOnly] public CardType typeOfCard;
    [ReadOnly] public bool isAnAce;

    Vector3 playerPosWhenThrown;
    Vector3 camPosWhenThrown;

    public (CardType, CardType) combo;

    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        GenerateCard();
        playerPosWhenThrown = CardThrowing.instance.transform.position;
        camPosWhenThrown = Camera.main.transform.forward;
    }

    public void GenerateCard()
    {
        mr = this.GetComponent<MeshRenderer>();
        int rand = Random.Range(0, 100);


        if (rand > CardThrowing.instance.aceProbability)
        {
            typeOfCard = CardType.Any;
        }
        else
        {
            typeOfCard = (CardType)(Random.Range(0, 4));

            mr.sharedMaterial = new Material(mr.sharedMaterial);
            mr.sharedMaterial.SetColor("_BaseColor", Color.yellow);

            isAnAce = true;
        }

        if (CardThrowing.instance.forceAceLeftClick != CardType.Any) typeOfCard = CardThrowing.instance.forceAceLeftClick;
        
        Debug.Log(typeOfCard);

    }


    public void Play(Enemy en)
    {

        //GameManager.instance.StartCoroutine(GameManager.instance.TimedSlowMotion(0.75f));
        switch(typeOfCard)
        {
            case CardType.Any:
                break;
            case CardType.Spades:
                GameManager.instance.StartCoroutine(Spades(en.gameObject, 1f, 10f, 2f, 4f, 15, -1f));
                break;
            case CardType.Heart:
                GameManager.instance.StartCoroutine(Heart(en.gameObject, 0.5f, 0.35f, 20f));
                break;
            case CardType.Diamond:
                GameManager.instance.StartCoroutine(Diamond(en.gameObject, (3f)));
                break;
            case CardType.Clubs:
                GameManager.instance.StartCoroutine(Clubs(en.gameObject, 0.5f, 0.25f, 10f, 5f));
                break;
        }
    }

    public void Combo((CardType, CardType) combo, Enemy en)
    {

        switch(combo.Item1)
        {
            case (CardType.Clubs):
                switch(combo.Item2)
                {
                    case CardType.Diamond:
                        GameManager.instance.StartCoroutine(Diamond(en.gameObject, (3f)));
                        GameManager.instance.StartCoroutine(Clubs(en.gameObject, 0.5f, 0.25f, 10f, 5f));                                             
                    break;
                }
                break;
            case (CardType.Heart):

                break;
        }
    }

    public IEnumerator Clubs(GameObject launcher, float castDelay, float pushDuration, float range, float distance)
    {
        yield return new WaitForSeconds(castDelay);

        Collider[] hitColliders = Physics.OverlapSphere(launcher.transform.position, range);

        List<Vector3> pos = new List<Vector3>();
        List<GameObject> enemies = new List<GameObject>();      

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Enemy>() && hitCollider.gameObject != launcher.gameObject)
            {
                pos.Add(hitCollider.transform.position);
                enemies.Add(hitCollider.gameObject);
            }
        }

        float t = 0f;
        while (t < pushDuration)
        {
            for (int i = 0; i < pos.Count; i++)
            {
                enemies[i].transform.position = Vector3.Lerp(pos[i], pos[i] + ((enemies[i].transform.position - launcher.transform.position).normalized.ChangeY(0f) * distance), CardThrowing.instance.easing.Evaluate(t / pushDuration));
            }
            yield return null;
            t += Time.deltaTime;
        }
    }
    public IEnumerator Diamond(GameObject target, float duration)
    {
        float time = 0f;

        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();

        while (time < duration)
        {
            agent.isStopped = true;
            time += Time.deltaTime;
            yield return null;
        }
        agent.isStopped = false;
    }

    public IEnumerator Spades(GameObject target, float castDelay, float length, float duration, float damageRange, int damages, float heightOffset = 0f)
    {
        float time = 0f;

        GameObject go = Instantiate(CardThrowing.instance.aceOfSpades, target.transform.position.ChangeY(target.transform.position.y + heightOffset), Quaternion.identity);

        Vector3 initPos = go.transform.position;

        Vector3 direction = Camera.main.transform.forward;
        go.transform.rotation = Quaternion.LookRotation(direction);
        go.transform.localEulerAngles = go.transform.localEulerAngles.ChangeX(90);

        while(time < castDelay)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        direction = Camera.main.transform.forward;
        go.transform.rotation = Quaternion.LookRotation(direction);
        go.transform.localEulerAngles = go.transform.localEulerAngles.ChangeX(90);

        List<GameObject> hit = new List<GameObject>();



        while (time < duration)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, initPos + (direction).ChangeY(0f) * length, (time / duration));

            Collider[] hitColliders = Physics.OverlapSphere(go.transform.position, damageRange);

            foreach(var hitCollider in hitColliders)
            {
                if (hit.Contains(hitCollider.gameObject) == false && hitCollider.GetComponent<Enemy>())
                {
                    hit.Add(hitCollider.gameObject);
                    hitCollider.GetComponent<Enemy>().TakeDamages(damages);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        go.GetComponent<UnityEngine.VFX.VisualEffect>().Stop();
    }

    public IEnumerator Heart(GameObject launcher, float castDelay, float attractionDuration, float range, float heightOffset = 0f)
    {
        GameObject go = Instantiate(CardThrowing.instance.aceOfHeart, launcher.transform.position.ChangeY(launcher.transform.position.y + heightOffset), Quaternion.identity);

        Vector3 direction = Camera.main.transform.forward;
        go.transform.rotation = Quaternion.LookRotation(direction);
        go.transform.localEulerAngles = go.transform.localEulerAngles.ChangeX(0);

            yield return new WaitForSeconds(castDelay);

        Collider[] hitColliders = Physics.OverlapSphere(launcher.transform.position, range);

        List<Vector3> pos = new List<Vector3>();
        List<Vector3> finalPos = new List<Vector3>();
        List<GameObject> enemies = new List<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.GetComponent<Enemy>() && hitCollider.gameObject != launcher.gameObject)
            {
                pos.Add(hitCollider.transform.position);
                Vector3 vec = (hitCollider.transform.position + (launcher.transform.position - hitCollider.transform.position).normalized * 2f);
                finalPos.Add(vec);
                enemies.Add(hitCollider.gameObject);
            }
        }

        float t = 0f;



        while (t < attractionDuration)
        {
            for (int i = 0; i < pos.Count; i++)
            {
                Debug.Log(finalPos[i]);
                Debug.DrawLine(finalPos[i], pos[i], Color.yellow);
                Debug.Break();

                enemies[i].transform.position = Vector3.Lerp(pos[i], finalPos[i], CardThrowing.instance.easing.Evaluate(t / attractionDuration));
            }
            yield return null;
            t += Time.deltaTime;
        }
    }

}

public enum CardType {Spades, Heart, Diamond, Clubs, Any}

public interface IPlayable
{
    void Play(Enemy en);
    void Combo((CardType, CardType) combo, Enemy en);
}