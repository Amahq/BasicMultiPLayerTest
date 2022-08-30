using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GlobalData
{
    private static string _username;
    public static MatchModes matchmode;
    public static MatchManager CurrentMM;

    public static event EventHandler OnUsernameChanged;

    public static string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            EventHandler handler = OnUsernameChanged;
            GlobalEvents args = new GlobalEvents();
            args.username = _username;
            handler?.Invoke(null, args);
        }
    }

}
