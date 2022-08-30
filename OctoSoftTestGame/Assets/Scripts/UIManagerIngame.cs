using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManagerIngame : MonoBehaviour
{

    public TMP_Text txtUsername;
    public TMP_Text txtScore;
    public Player player;

    private void Awake()
    {
        //GlobalData.OnUsernameChanged += OnUsernameChanged;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnUsernameChanged(object sender, System.EventArgs e)
    {
        GlobalEvents ge = (GlobalEvents)e;
        updateUsername(ge.username);
    }

    public void updateUsername(string username)
    {
        txtUsername.text = username;
    }

    public void UpdateScore(float score)
    {
        txtScore.text = score.ToString();
    }

    public void Refresh()
    {
        txtUsername.text = player.Username;
        txtScore.text = player.Score.ToString();

    }
}
