using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card : MonoBehaviour, IPlayable
{

    [ReadOnly] public CardType typeOfCard;
    [ReadOnly] public bool isAnAce;

    Vector3 playerPosWhenThrown;

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
                break;
            case CardType.Heart:
                en.StartCoroutine(en.Attract(0.5f, 0.1f, 20f));
                break;
            case CardType.Diamond:
                en.StartCoroutine(en.Freeze(3f));
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
                        Debug.Log("KABOOOOM");
                        en.StartCoroutine(en.Freeze(3f));
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
        float t = 0f;
        while (t < castDelay)
        {
            t += Time.deltaTime;
            yield return null;
        }


        t = 0f;

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

        while (t < pushDuration)
        {
            for (int i = 0; i < pos.Count; i++)
            {
                enemies[i].transform.position = Vector3.Lerp(pos[i], pos[i] + ((enemies[i].transform.position - launcher.transform.position).normalized.ChangeY(0f) * distance), (t / pushDuration));
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