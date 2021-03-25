using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card : MonoBehaviour, IPlayable
{

    [ReadOnly] public CardType typeOfCard;
    [ReadOnly] public bool isAnAce;

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

        if (CardThrowing.instance.forceAce != CardType.Any) typeOfCard = CardThrowing.instance.forceAce;
        
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
                break;
        }
    }

    public void Combo(CardType lastCardType)
    {

        int a = (int)lastCardType;
        int b = (int)typeOfCard;

        switch (a + b)
        {
            case 0: //Spades + Spades
                break;
            case 1: //Spades + Heart
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
        }
    }

}

public enum CardType {Spades, Heart, Diamond, Clubs, Any}

public interface IPlayable
{
    void Play(Enemy en);
    void Combo(CardType lastCardType);
}