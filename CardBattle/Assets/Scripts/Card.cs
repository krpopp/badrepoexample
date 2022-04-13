using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : CardBehavior
{

    public AudioClip flipSound;

    public AudioClip goodNewSound;

    public int cardScore;

    private List<AnimationClip> animations;

    public GameObject diamond;

    public DiamondPlayer diamondPlayer;

    protected override void OnMouseDown()
    {
       // base.OnMouseDown();
       // GetComponent<Animator>().SetTrigger("doBounce");
    }

    protected override void SelectedChanges()
    {
        base.SelectedChanges();
        GetComponent<AudioSource>().PlayOneShot(flipSound);
    }

    void FixedUpdate()
    {
        Debug.Log("ayo");
        float movementHorizontal = Input.GetAxis("Horizontal");
        float movementVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3 (movementHorizontal, 0f, movementVertical);
    }
}
