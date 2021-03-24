using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card : MonoBehaviour, IPlayable
{

    [ReadOnly] public CardType typeOfCard;


    // Start is called before the first frame update
    void Start()
    {
        GenerateCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCard()
    {
        int rand = Random.Range(0, 101);



        switch(rand)
        {
            case int n when rand < 49:
                typeOfCard = CardType.Any;
                break;
            case 49:
                typeOfCard = CardType.Spades;
                break;
            case 50:
                typeOfCard = CardType.Heart;
                break;
            case 51:
                typeOfCard = CardType.Diamond;
                break;
            case 52:
                typeOfCard = CardType.Clubs;
                break;
        }
    }


    public void Play()
    {

    }

}

public enum CardType {Any, Spades, Heart, Diamond, Clubs}

public interface IPlayable
{
    void Play();
}