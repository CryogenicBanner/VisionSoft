using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoColorSlider2D : MonoBehaviour
{
    public enum State { Looping, Normal }

    public Color NormalColor = new Color(255, 255, 255);
    public Color loopStartColor, loopEndColor;

    public SpriteRenderer sprite;

    public State myState = State.Normal;
    public int interpolationSpeed = 1;

    private float startTime;
    private void Start()
    {
        if (sprite == null)
        {
            sprite = this.GetComponent<SpriteRenderer>();

        }
        if (loopStartColor == null || loopEndColor == null)
        {
            loopStartColor = new Color(255, 0, 0);
            loopEndColor = new Color(0, 255, 255);
        }

    }
    private void Update(){
        switch (myState)
        {
            case State.Normal:
                if (sprite.color != NormalColor){
                    sprite.color = NormalColor;
                }
                break;
            case State.Looping:
                sprite.color = Color.Lerp(loopStartColor, loopEndColor, Mathf.PingPong(Time.time, 1));
                break;

        }
    }

    public void NormalState(){myState = State.Normal;}
    public void LoopingState(){myState = State.Looping;}

}
