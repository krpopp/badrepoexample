using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBehavior : MonoBehaviour
{

    GameManager myManager;

    #region StateDeclaration
    public enum State{
        Deck,
        Hand,
        Selected,
        Discard
    }

    private State currentState;

    public State CurrentState{
        get{
            return currentState;
        }
        set{
            if(currentState == value) return;
            currentState = value;
            TransitionStates(value);
        }
    }
    #endregion

    #region DamageData
    private int _damageVal;
    public int DamageVal{
        get{
            return _damageVal;
        }
        set{
            _damageVal = value;
        }
    }
    #endregion

    #region CardVisuals
    SpriteRenderer myRend;
    public Sprite backSprite, frontSprite, valueSprite;
    public Color backColor, frontColor;
    GameObject childSprite;
    #endregion

    float time = 0;

    protected virtual void Start(){
        Initialize();
    }

    /// <summary>
    /// Sets all initial values
    /// </summary>
    protected virtual void Initialize(){ 
        myManager = GameManager.FindInstance();
        myRend = GetComponent<SpriteRenderer>();
        myRend.sprite = backSprite;
        myRend.color = backColor;
        childSprite = transform.Find("Value Sprite").gameObject;
        childSprite.GetComponent<SpriteRenderer>().sortingOrder = 20;
        childSprite.GetComponent<SpriteRenderer>().sprite = valueSprite;
        childSprite.SetActive(false);
    }

    void Update(){
        RunState();
    }

    protected virtual void OnMouseDown(){
        if(myManager.CurrentState == GameManager.State.SelectCard && currentState == State.Hand){
            CurrentState = State.Selected;
        }
    }

    /// <summary>
    /// Sets any one off values or method calls when we go to a new state
    /// </summary>
    private void TransitionStates(State newState){
        switch(newState){
            case State.Deck:
                break;
            case State.Hand:
                break;
            case State.Selected:
                SelectedChanges();
                break;
            case State.Discard:
                childSprite.SetActive(false);
                myRend.sprite = backSprite;
                myRend.color = backColor;
                break;
        }
    }

    /// <summary>
    /// Handles any continuous features
    /// </summary>
    private void RunState(){
        switch(currentState){
            case State.Hand:
                HandJuice();
                break;
        }
    }

    /// <summary>
    /// Visual changes made when the player selects a card from their hand
    /// </summary>
    protected virtual void SelectedChanges(){
        myRend.color = frontColor;
        myRend.sprite = frontSprite;
        myRend.sortingOrder = 6;
        transform.Find("Value Sprite").gameObject.SetActive(true);
        myManager.selectedCard = gameObject;
        myManager.CurrentState = GameManager.State.Resolve;
    }

    /// <summary>
    /// Moves card and checks to see if we've reached the destination
    /// </summary>
    public bool MoveCard(Vector3 endPos, float duration){
        SmoothStepLerp(endPos, duration);
        if(Vector3.Distance(transform.position, endPos) <= 0.05f){
            time = 0;
           return true;
        } else{
            return false;
        }
    }

    //lerp examples https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
    /// <summary>
    /// Lerps position with easing
    /// </summary>
    protected virtual void SmoothStepLerp(Vector3 endPos, float duration){
        Vector3 startPos = transform.position;
        if(time < duration){
            float t = time/duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            time += Time.deltaTime;
        } else{
            transform.position = endPos; 
        }
    }

    protected virtual void HandJuice(){

    }

}
