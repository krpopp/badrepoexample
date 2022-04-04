using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region SingletonDeclaration
    private static GameManager instance;
    public static GameManager FindInstance()
    {
        return instance;
    }
    #endregion

    #region StateDeclartion
    [HideInInspector]
    public enum State
    {
        CreateCards,
        Deal,
        SelectCard,
        Resolve,
        Cleanup,
        Reshuffle
    }
    private State _currentState;
    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            TransitionStates(value);
        }
    }
    #endregion

    #region  DeckObjectLists
    List<GameObject> deck = new List<GameObject>();
    List<GameObject> hand = new List<GameObject>();
    List<GameObject> discard = new List<GameObject>();
    #endregion

    #region CardData
    [HideInInspector]
    public GameObject selectedCard;
    [Header("Card Data")]
    public GameObject cardObj;
    public int cardCount;
    public int handCount;
    #endregion

    #region CardPositions
    [Header("Card Positions")]
    public Vector3 handPos;
    public Vector3 attackPos;
    public Vector3 discardPos;
    #endregion

    #region EnemyInfo
    [Header("Enemy Info")]
    public Text enemyText;
    public int enemyHealth;
    #endregion

    #region CardVisuals
    [Header("Card Visuals")]
    public Sprite[] valueSprites;
    public Color minusColor;
    public Color plusColor;
    #endregion

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
    }

    void Start()
    {
        ChangeEnemyText();
        CurrentState = State.CreateCards;
    }

    void Update()
    {
        RunStates();
    }

    /// <summary>
    /// Sets any initial values or one off methods when we're moving between states
    /// </summary>
    private void TransitionStates(State newState)
    {
        switch (newState)
        {
            case State.CreateCards:
                CreateCards();
                break;
            case State.Deal:
                break;
            case State.SelectCard:
                break;
            case State.Resolve:
                break;
            case State.Cleanup:
                break;
        }
    }

    /// <summary>
    /// Runs an continuous features needed for a state to work
    /// </summary>
    private void RunStates()
    {
        switch (CurrentState)
        {
            case State.Deal:
                DealCards();
                break;
            case State.Resolve:
                UseCard();
                break;
            case State.Cleanup:
                CleanHand();
                break;
            case State.Reshuffle:
                ReturnToDeck();
                break;
        }
    }

    /// <summary>
    /// Creates deck and gives them their values
    /// </summary>
    private void CreateCards()
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newCard = Instantiate(cardObj);
            CardBehavior cS = newCard.GetComponent<CardBehavior>();
            int dmgCnt = deck.Count % 6;
            cS.CurrentState = CardBehavior.State.Deck;
            cS.valueSprite = valueSprites[dmgCnt];
            cS.DamageVal = SetDamageVal(dmgCnt);
            if (cS.DamageVal < 0)
            {
                cS.frontColor = minusColor;
            }
            else
            {
                cS.frontColor = plusColor;
            }
            cS.GetComponent<SpriteRenderer>().sortingOrder = -(cardCount - i);
            Vector3 newPos = new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y,
                0);
            newCard.transform.position = newPos;
            deck.Add(newCard);
        }
        CurrentState = State.Deal;
    }

    /// <summary>
    /// Finds the damange a card can do based on the deck
    /// </summary>
    int SetDamageVal(int dmgCnt)
    {
        switch (dmgCnt)
        {
            case 0:
                return -3;
            case 1:
                return -2;
            case 2:
                return -1;
            case 3:
                return 1;
            case 4:
                return 2;
            case 5:
                return 3;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Moves cards from the deck to the hand
    /// </summary>
    private void DealCards()
    {
        GameObject currentCard = deck[deck.Count - 1];
        currentCard.GetComponent<SpriteRenderer>().sortingOrder = hand.Count;
        CardBehavior cS = currentCard.GetComponent<CardBehavior>();
        Vector3 newPos = new Vector3(handPos.x + (hand.Count * 3), handPos.y, 0);
        if (cS.MoveCard(newPos, 3f))
        {
            hand.Add(currentCard);
            deck.RemoveAt(deck.Count - 1);
            cS.CurrentState = CardBehavior.State.Hand;
            if (hand.Count == handCount)
            {
                CurrentState = State.SelectCard;
            }
        }

    }

    /// <summary>
    /// Alters the enemy's health based on the chosen card
    /// </summary>
    private void UseCard()
    {
        CardBehavior cS = selectedCard.GetComponent<CardBehavior>();
        if (cS.MoveCard(attackPos, 5f))
        {
            enemyHealth = enemyHealth + selectedCard.GetComponent<CardBehavior>().DamageVal;
            ChangeEnemyText();
            CurrentState = State.Cleanup;
        }
    }

    /// <summary>
    /// Updates the enemy's health text
    /// </summary>
    private void ChangeEnemyText()
    {
        enemyText.text = "Health: " + enemyHealth;
    }

    /// <summary>
    /// Moves cards in the hand to the discard
    /// </summary>
    private void CleanHand(){
        GameObject currentCard = hand[hand.Count - 1];
        currentCard.GetComponent<SpriteRenderer>().sortingOrder = discard.Count;
        CardBehavior cS = currentCard.GetComponent<CardBehavior>();
        if(cS.MoveCard(discardPos, 3f)){
            discard.Add(currentCard);
            currentCard.GetComponent<CardBehavior>().CurrentState = CardBehavior.State.Discard;
            hand.RemoveAt(hand.Count - 1);
            if(hand.Count == 0){
                if(deck.Count == 0){
                    CurrentState = State.Reshuffle;
                } else{
                    CurrentState = State.Deal;
                }
            }
        }
    }

    /// <summary>
    /// Moves cards back to deck
    /// </summary>
    private void ReturnToDeck(){
        GameObject currentCard = discard[discard.Count - 1];
        CardBehavior cS = currentCard.GetComponent<CardBehavior>();
        if(cS.MoveCard(transform.position, 3f)){
            currentCard.GetComponent<SpriteRenderer>().sortingOrder = -(cardCount - deck.Count);
            deck.Add(currentCard);
            currentCard.GetComponent<CardBehavior>().CurrentState = CardBehavior.State.Deck;
            discard.RemoveAt(discard.Count - 1);
            if(discard.Count == 0){
                CurrentState = State.Deal;
            }
        }
    }

}
