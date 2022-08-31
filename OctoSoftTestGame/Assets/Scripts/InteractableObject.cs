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

    [Header("VFX")]
    public GameObject fxAddPoints;
    public GameObject fxLosePoints;
    public GameObject fxOnClick;

    public virtual event EventHandler evtOnClear;
    public virtual event EventHandler evtOnDecayFinished;

    // Start is called before the first frame update
    protected void Start()
    {
        StartCoroutine(DecayCounter());
    }

    protected void VisualFeedback(TypesOfVisualFeedback t)
    {
        if (GlobalData.CurrentMM.isgamefinished)
        {
            return;
        }
        GameObject spawnedfx;
        //here goes the visual feedback for any given action;
        switch (t)
        {
            case TypesOfVisualFeedback.cleared:
                if(ScoreModifierOnClear == 0)
                {
                    //Do nothing
                }
                else if(ScoreModifierOnClear > 0)
                {
                    spawnedfx = (GameObject)Instantiate(fxAddPoints, transform.position, transform.rotation);
                    spawnedfx.GetComponent<VFX>().text.text = ScoreModifierOnClear.ToString();
                }
                else // If < 0
                {
                    spawnedfx = (GameObject)Instantiate(fxLosePoints, transform.position, transform.rotation);
                    spawnedfx.GetComponent<VFX>().text.text = ScoreModifierOnClear.ToString();
                }
                break;
            case TypesOfVisualFeedback.clicked:
                spawnedfx = (GameObject)Instantiate(fxOnClick, transform.position, transform.rotation);
                break;
            case TypesOfVisualFeedback.missed:
                if(ScoreModifierOnMiss < 0)
                {
                    spawnedfx = (GameObject)Instantiate(fxLosePoints, transform.position, transform.rotation);
                    spawnedfx.GetComponent<VFX>().text.text = ScoreModifierOnMiss.ToString();
                }
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

    protected IEnumerator DecayCounter()
    {
        yield return new WaitForSeconds(DecayTime);

        OnDecayFinished();

        yield return null;
    }
}
