using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
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
    CardsRules cr;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(CardType cardType = CardType.Any)
    {
        GenerateCard(cardType);
        playerPosWhenThrown = CardThrowing.instance.transform.position;
        camPosWhenThrown = Camera.main.transform.forward;
    }

    public void GenerateCard(CardType cardType = CardType.Any)
    {
        mr = this.GetComponent<MeshRenderer>();
        cr = CardThrowing.instance.rules;
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

        if (cardType != CardType.Any)
        {
            mr.sharedMaterial = new Material(mr.sharedMaterial);
            mr.sharedMaterial.SetColor("_BaseColor", Color.yellow);

            typeOfCard = cardType;
            isAnAce = true;
        }


        //Debug.Log(typeOfCard);

    }


    public void Play(Enemy en)
    {
        //GameManager.instance.StartCoroutine(GameManager.instance.TimedSlowMotion(0.75f));
        switch (typeOfCard)
        {
            case CardType.Any:
                en.TakeDamages(cr.normalCardDamages);
                break;
            case CardType.Spades:
                GameManager.instance.StartCoroutine(Spades(en.gameObject, cr.spadesCastDelay, cr.spadesDistance, cr.spadesEffectDuration, cr.spadesRange, cr.spadesDamages, -1f));
                break;
            case CardType.Heart:
                GameManager.instance.StartCoroutine(Heart(en.gameObject, cr.heartCastDelay, cr.heartEffectDuration, cr.heartRange, -1.4f));
                break;
            case CardType.Diamond:
                GameManager.instance.StartCoroutine(Diamond(en.gameObject, cr.diamondEffectDuration, cr.diamondDamages, -1.4f));
                break;
            case CardType.Clubs:
                GameManager.instance.StartCoroutine(Clubs(en.gameObject, cr.clubsCastDelay, cr.clubsEffectDuration, cr.clubsRange, cr.clubsPushingDistance));
                break;
        }
    }

    public void Combo((CardType, CardType) combo, Enemy en)
    {
        switch (combo.Item1)
        {
            case (CardType.Spades):

                switch (combo.Item2)
                {
                    case CardType.Spades:
                        break;
                    case CardType.Heart:
                        break;
                    case CardType.Diamond:
                        GameManager.instance.StartCoroutine(SpadesDiamond(en.gameObject, cr.spadesCastDelay, cr.spadesDistance, cr.spadesEffectDuration, cr.spadesRange, cr.spadesDamages, -1f));
                        break;
                    case CardType.Clubs:
                        break;
                }
                break;

            case (CardType.Heart):
                switch (combo.Item2)
                {
                    case CardType.Spades:
                        break;
                    case CardType.Heart:
                        DoubleHeart(en);
                        break;
                    case CardType.Diamond:
                        break;
                    case CardType.Clubs:
                        HeartClubs(en);
                        break;
                }
                break;

            case (CardType.Diamond):
                switch (combo.Item2)
                {
                    case CardType.Spades:
                        GameManager.instance.StartCoroutine(SpadesDiamond(en.gameObject, cr.spadesCastDelay, cr.spadesDistance, cr.spadesEffectDuration, cr.spadesRange, cr.spadesDamages, -1f));
                        break;
                    case CardType.Heart:
                        break;
                    case CardType.Diamond:
                        GameManager.instance.StartCoroutine(DoubleDiamond(en.gameObject, cr.diamondEffectDuration, cr.diamondDoubleRange, cr.diamondDamages, -1.4f));
                        break;
                    case CardType.Clubs:
                        break;
                }
                break;

            case (CardType.Clubs):
                switch (combo.Item2)
                {
                    case CardType.Spades:
                        break;
                    case CardType.Heart:
                        HeartClubs(en);
                        break;
                    case CardType.Diamond:
                        GameManager.instance.StartCoroutine(Diamond(en.gameObject, cr.diamondEffectDuration, cr.diamondDamages));
                        GameManager.instance.StartCoroutine(Clubs(en.gameObject, cr.clubsCastDelay, cr.clubsEffectDuration, cr.clubsRange, cr.clubsPushingDistance));
                        break;
                    case CardType.Clubs:
                        DoubleClubs(en);
                        break;
                }
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
    public IEnumerator Diamond(GameObject target, float duration, int damages, float heightOffset = 0f)
    {
        float time = 0f;

        GameObject go = Instantiate(CardThrowing.instance.aceOfDiamond, target.transform.position.ChangeY(target.transform.position.y + heightOffset), Quaternion.identity);
        
        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();

        target.GetComponent<Enemy>().TakeDamages(damages);

        while (time < duration)
        {
            if (target == null)
            {
                go.GetComponent<ParticleSystem>().Stop();
                yield break;
            }

            target.GetComponent<Enemy>().canAttack = false;
            agent.isStopped = true;
            time += Time.deltaTime;
            yield return null;
        }
        agent.isStopped = false;
        target.GetComponent<Enemy>().canAttack = true;
        go.GetComponent<ParticleSystem>().Stop();
    }
    public IEnumerator Spades(GameObject target, float castDelay, float length, float duration, float damageRange, int damages, float heightOffset = 0f)
    {
        float time = 0f;

        GameObject go = Instantiate(CardThrowing.instance.aceOfSpades, target.transform.position.ChangeY(target.transform.position.y + heightOffset), Quaternion.identity);

        Vector3 initPos = go.transform.position;

        Vector3 direction = Camera.main.transform.forward;
        go.transform.rotation = Quaternion.LookRotation(direction);
        go.transform.localEulerAngles = go.transform.localEulerAngles.ChangeX(90);

        while (time < castDelay)
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

            foreach (var hitCollider in hitColliders)
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
                enemies[i].transform.position = Vector3.Lerp(pos[i], finalPos[i], CardThrowing.instance.easing.Evaluate(t / attractionDuration));
            }
            yield return null;
            t += Time.deltaTime;
        }

        go.GetComponent<ParticleSystem>().Stop();
    }

    public void DoubleClubs(Enemy en)
    {
        GameManager.instance.StartCoroutine(Clubs(en.gameObject, cr.clubsCastDelay, cr.clubsEffectDuration, cr.clubsRange*2, cr.clubsPushingDistance));
    }

    public IEnumerator DoubleDiamond(GameObject target, float duration, float range, int damages, float heightOffset = 0f)
    {
        float time = 0f;

        List<GameObject> hit = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, range);


        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null && hit.Contains(hitCollider.gameObject) == false && hitCollider.GetComponent<Enemy>())
            {
                hit.Add(hitCollider.gameObject);
                GameManager.instance.StartCoroutine(Diamond(hitCollider.gameObject, duration, damages, heightOffset));
            }
        }

        yield return null;
    }

    public void DoubleSpades(Enemy en)
    {
        GameManager.instance.StartCoroutine(Spades(en.gameObject, cr.spadesCastDelay, cr.spadesDistance, cr.spadesEffectDuration, cr.spadesRange*2, cr.spadesDamages, -1f));
    }

    public void DoubleHeart(Enemy en)
    {
        GameManager.instance.StartCoroutine(Heart(en.gameObject, cr.heartCastDelay, cr.heartEffectDuration, cr.heartRange*2, -1.4f));
    }

    public IEnumerator SpadesHeart()
    {

        yield return null;
    }
    public IEnumerator SpadesDiamond(GameObject target, float castDelay, float length, float duration, float damageRange, int damages, float heightOffset = 0f)
    {
        float time = 0f;

        GameObject go = Instantiate(CardThrowing.instance.aceOfSpades, target.transform.position.ChangeY(target.transform.position.y + heightOffset), Quaternion.identity);

        Vector3 initPos = go.transform.position;

        Vector3 direction = Camera.main.transform.forward;
        go.transform.rotation = Quaternion.LookRotation(direction);
        go.transform.localEulerAngles = go.transform.localEulerAngles.ChangeX(90);

        while (time < castDelay)
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

            foreach (var hitCollider in hitColliders)
            {
                if (hit.Contains(hitCollider.gameObject) == false && hitCollider.GetComponent<Enemy>())
                {
                    hit.Add(hitCollider.gameObject);
                    GameManager.instance.StartCoroutine(Diamond(hitCollider.gameObject, cr.diamondEffectDuration, cr.diamondDamages));
                    hitCollider.GetComponent<Enemy>().TakeDamages(damages);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        go.GetComponent<UnityEngine.VFX.VisualEffect>().Stop();
    }
    public IEnumerator SpadesClubs()
    {
        yield return null;

    }

    public IEnumerator HeartDiamond(GameObject launcher, float castDelay, float attractionDuration, float range, float heightOffset = 0f)
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
                enemies[i].transform.position = Vector3.Lerp(pos[i], finalPos[i], CardThrowing.instance.easing.Evaluate(t / attractionDuration));
            }
            yield return null;
            t += Time.deltaTime;
        }

        go.GetComponent<UnityEngine.VFX.VisualEffect>().Stop();
    }

    public IEnumerator HeartClubs(Enemy en)
    {
        GameManager.instance.StartCoroutine(Heart(en.gameObject, cr.heartCastDelay, cr.heartEffectDuration, cr.heartRange));
        yield return new WaitForSeconds(cr.heartEffectDuration + cr.heartCastDelay);
        GameManager.instance.StartCoroutine(Clubs(en.gameObject, cr.clubsCastDelay, cr.clubsEffectDuration, cr.clubsRange, cr.clubsPushingDistance));
    }

    public IEnumerator DiamondClubs()
    {
        yield return null;

    }

}

public enum CardType { Spades, Heart, Diamond, Clubs, Any }

public interface IPlayable
{
    void Play(Enemy en);
    void Combo((CardType, CardType) combo, Enemy en);
}