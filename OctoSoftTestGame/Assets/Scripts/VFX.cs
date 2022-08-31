using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VFX : MonoBehaviour
{
    public float duration;
    public Vector3 movespeed;
    public GameObject TextGO;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer());
        //GetComponent<AudioSource>().Play();
    }

    private void FixedUpdate()
    {
        transform.Translate(movespeed * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(duration);

        Destroy(gameObject);
        yield return null;
    }
}
