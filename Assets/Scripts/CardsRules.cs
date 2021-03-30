using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rules", menuName = "Cards Rules")]
public class CardsRules : ScriptableObject
{
    [Header("General")]
    [Range(0, 4)]
    public int comboRangeMultiplier;
    [Range(0, 50)]
    public int normalCardDamages;

    [Header("Ace of Spades")]
    [Range(0, 2)]
    public float spadesCastDelay;
    [Range(0, 10)]
    public float spadesRange;
    [Range(0, 10)]
    public float spadesDistance;
    [Range(0, 10)]
    public float spadesEffectDuration;
    [Range(0, 100)]
    public int spadesDamages;

    [Header("Ace of Heart")]
    [Range(0, 2)]
    public float heartCastDelay;
    [Range(0, 10)]
    public float heartRange;
    [Range(0, 10)]
    public float heartDistance;
    [Range(0, 10)]
    public float heartEffectDuration;
    [Range(0, 100)]
    public int heartDamages;

    [Header("Ace of Diamond")]
    [Range(0, 10)]
    public float diamondRange;
    [Range(0, 100)]
    public int diamondDamages;
    [Range(0, 10)]
    public float diamondEffectDuration;

    [Header("Ace of Clubs")]
    [Range(0, 2)]
    public float clubsCastDelay;
    [Range(0, 10)]
    public float clubsRange;
    [Range(0, 10)]
    public float clubsPushingDistance;
    [Range(0, 10)]
    public float clubsEffectDuration;
    [Range(0, 100)]
    public int clubsDamages;

    [Header("Double Diamond")]
    [Range(0, 10)]
    public float diamondDoubleRange;
}
