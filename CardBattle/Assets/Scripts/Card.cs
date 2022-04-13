using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : CardBehavior
{

    public AudioClip flipSound;

    public AudioClip goodNewSound;

    public int cardScore;

    private List<AnimationClip> animations;

    private void FixedUpdate()
    {
        Debug.Log("ayo");
    }

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
}
