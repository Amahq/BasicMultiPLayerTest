using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableObject : MonoBehaviour
{
    [HideInInspector]
    public int ID;
    public float ScoreModifierOnClear;
    public float ScoreModifierOnMiss;
    public float AmountOfClicksToClear = 1;
    public float DecayTime = 5;
    protected float _timesclicked;

    public virtual event EventHandler evtOnClear;
    public virtual event EventHandler evtOnDecayFinished;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DecayCounter());
    }

    protected void VisualFeedback(TypesOfVisualFeedback t)
    {
        //here goes the visual feedback for any given action;
        switch (t)
        {
            case TypesOfVisualFeedback.cleared:
                //Debug.Log("ObjectCleared");
                break;
            case TypesOfVisualFeedback.clicked:
                //Debug.Log("Object Clicked");
                break;
            case TypesOfVisualFeedback.missed:
                //Debug.Log("ObjectMissed");
                break;
            default:
                //Nothing
                break;
        }
    }


    public virtual void OnClick()
    {
        VisualFeedback(TypesOfVisualFeedback.clicked);
        _timesclicked++;
        if(_timesclicked >= AmountOfClicksToClear)
        {
            OnClear();
        }
    }

    public virtual void OnClear()
    {
        //Debug.Log("Normal Clear");
        VisualFeedback(TypesOfVisualFeedback.cleared);
        EventHandler handler = evtOnClear;
        ObjectEvents args = new ObjectEvents();
        args.ScoreModifier = ScoreModifierOnClear;
        args.ID = ID;
        if (gameObject.tag == "LocalPlayer")
        {
            args.type = PlayerType.Local;
        }
        else
        {
            args.type = PlayerType.Remote;
        }
        handler?.Invoke(this, args);
    }

    protected void OnDecayFinished()
    {
        VisualFeedback(TypesOfVisualFeedback.missed);
        EventHandler handler = evtOnDecayFinished;
        ObjectEvents args = new ObjectEvents();
        args.ScoreModifier = ScoreModifierOnMiss;
        args.ID = ID;
        if (gameObject.tag == "LocalPlayer")
        {
            args.type = PlayerType.Local;
        }
        else
        {
            args.type = PlayerType.Remote;
        }
        handler?.Invoke(this, args);
    }

    IEnumerator DecayCounter()
    {
        yield return new WaitForSeconds(DecayTime);

        OnDecayFinished();

        yield return null;
    }
}
