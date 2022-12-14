using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetObject : InteractableObject
{

    [Header("Target Specific")]
    public GameObject ObjectToBeQueued;
    public float AmountOfItemsTobeQueued;

    public override event EventHandler evtOnClear;

    public override void OnClick()
    {
        VisualFeedback(TypesOfVisualFeedback.clicked);
        _timesclicked++;
        if (_timesclicked >= AmountOfClicksToClear)
        {
            OnClear();
        }
    }

    public override void OnClear()
    {
        base.VisualFeedback(TypesOfVisualFeedback.cleared);
        EventHandler handler = evtOnClear;
        TargetEvents args = new TargetEvents();
        args.ScoreModifier = ScoreModifierOnClear;
        args.AmountOfObjectsToBeQueued = AmountOfItemsTobeQueued;
        args.ObjectToBeQueued = ObjectToBeQueued;
        args.ID = ID;
        if(gameObject.tag == "LocalPlayer")
        {
            args.type = PlayerType.Local;
        }
        else
        {
            args.type = PlayerType.Remote;
        }
        handler?.Invoke(this, args);
    }
}
