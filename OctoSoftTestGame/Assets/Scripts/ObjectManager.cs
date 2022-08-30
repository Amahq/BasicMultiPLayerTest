using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

[RequireComponent(typeof(Player))]
public class ObjectManager : MonoBehaviour
{
    [Tooltip("This represents how much time in seconds will pass before spawning the next object")]
    public float SpawnRate = 5;
    [Tooltip("This is the amount of time that will be deducted from SpawnRate every time an object is Spawned. Once the MinimumSpawnRate has been reached the SpawnRate will no longer be reduced.")]
    public float SpawnRateModifierPerObject = 0.1f;
    [Tooltip("This value represents the absolute minimum of time that will pass before spawning the next object. This value cannot be further reduced by the SpawnRateModifierOPerObject value.")]
    public float MinimumSpawnRate = 0.5f;
    public int MaxObjectAmount = 20;
    public GameObject[] SpawnableObjects;
    private Player _p;
    private Dictionary<int, GameObject> SpawnedObjects = new Dictionary<int, GameObject>();
    private int _amountofspawnedobjects = 0;
    private MatchManager mm;
    private Camera cam;
    private Queue queuedobjects = new Queue();
    private bool _isnextspawnspecial = false;
    public DifficultySetting diff;

    private PhotonView PV;

    private void LoadGameDifficulty()
    {
        diff = PTRoomManager.Instance.CurrentDifficulty;
        SpawnRate = diff.spawnRate;
        SpawnRateModifierPerObject = diff.SpawnRateModifierPerObject;
        MinimumSpawnRate = diff.MinimumSpawnRate;
        MaxObjectAmount = diff.MaxObjectAmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        _p = GetComponent<Player>();
        PV = GetComponent<PhotonView>();
        mm = _p.mm;
        cam = _p.cam;
        LoadGameDifficulty();
        StartCoroutine(SpawnCoolDown());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnObjectMissed(object sender, System.EventArgs e)
    {
        if (GlobalData.CurrentMM.isgamefinished)
        {
            return;
        }
        if(GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            ObjectEvents oe = (ObjectEvents)e;
            mm.ModifyScore(oe.ScoreModifier, oe.type);
            GameObject missedobject;
            SpawnedObjects.TryGetValue(oe.ID, out missedobject);
            GameObject.DestroyImmediate(missedobject);
            SpawnedObjects.Remove(oe.ID);
        }
        else
        {
            if (!PV.IsMine)
            {
                return;
            }
            ObjectEvents oe = (ObjectEvents)e;
            mm.ModifyScore(oe.ScoreModifier, oe.type);
            GameObject missedobject;
            SpawnedObjects.TryGetValue(oe.ID, out missedobject);
            //GameObject.DestroyImmediate(missedobject);
            PhotonNetwork.Destroy(missedobject);
            SpawnedObjects.Remove(oe.ID);
        }
    }

    private void OnObjectCleared(object sender, System.EventArgs e)
    {
        if (GlobalData.CurrentMM.isgamefinished)
        {
            return;
        }

        //Debug.Log("Object Cleared");
        GameObject clearedobject;
        if(GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            ObjectEvents re = (ObjectEvents)e;
            mm.ModifyScore(re.ScoreModifier, PlayerType.Local);

            try
            {
                TargetEvents te = (TargetEvents)e;
                //Debug.Log("Target ID (" + te.ID + ")");

                if (te.ObjectToBeQueued != null)
                {
                    for (int i = 0; i < te.AmountOfObjectsToBeQueued; i++)
                    {
                        queuedobjects.Enqueue(te.ObjectToBeQueued);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
            //Debug.Log("Object ID (" + re.ID+")");
            SpawnedObjects.TryGetValue(re.ID, out clearedobject);
            GameObject.DestroyImmediate(clearedobject);
            SpawnedObjects.Remove(re.ID);
            //clearedobject.GetComponent<InteractableObject>().evtOnClear -= OnObjectCleared;
        }
        else
        {
            if (!PV.IsMine)
            {
                return;
            }
            ObjectEvents re = (ObjectEvents)e;
            mm.ModifyScore(re.ScoreModifier, PlayerType.Local);

            try
            {
                TargetEvents te = (TargetEvents)e;
                //Debug.Log("Target ID (" + te.ID + ")");

                if (te.ObjectToBeQueued != null)
                {
                    for (int i = 0; i < te.AmountOfObjectsToBeQueued; i++)
                    {
                        queuedobjects.Enqueue(te.ObjectToBeQueued);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
            //Debug.Log("Object ID (" + re.ID+")");
            SpawnedObjects.TryGetValue(re.ID, out clearedobject);
            //GameObject.DestroyImmediate(clearedobject);
            PhotonNetwork.Destroy(clearedobject);
            SpawnedObjects.Remove(re.ID);
            //clearedobject.GetComponent<InteractableObject>().evtOnClear -= OnObjectCleared;
        }
    }

    private void SpawnNextObject()
    {
        if (GlobalData.CurrentMM.isgamefinished)
        {
            return;
        }

        GameObject _tempobject;
        if (GlobalData.matchmode == MatchModes.SinglePlayer)
        {
            if (SpawnRate > MinimumSpawnRate)
            {
                SpawnRate -= SpawnRateModifierPerObject;
            }
            _tempobject = Instantiate(GetNextObjectToBeSpawned(), GetNextSpawnLocation(), LookAtCamera());
            _amountofspawnedobjects++;
            _tempobject.GetComponent<InteractableObject>().ID = _amountofspawnedobjects;
            _tempobject.tag = _p.tag;
            foreach (Transform go in _tempobject.transform.GetComponentsInChildren<Transform>())
            {
                go.tag = _p.tag;
            }

            _tempobject.GetComponent<InteractableObject>().evtOnClear += OnObjectCleared;
            _tempobject.GetComponent<InteractableObject>().evtOnDecayFinished += OnObjectMissed;
            SpawnedObjects.Add(_tempobject.GetComponent<InteractableObject>().ID, _tempobject);
            StartCoroutine(SpawnCoolDown());

        }
        else
        {
            if (!PV.IsMine)
            {
                return;
            }
            if (SpawnRate > MinimumSpawnRate)
            {
                SpawnRate -= SpawnRateModifierPerObject;
            }
            _tempobject = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", GetNextObjectToBeSpawned().name), GetNextSpawnLocation(), LookAtCamera());// Instantiate(GetNextObjectToBeSpawned(), GetNextSpawnLocation(), LookAtCamera());
            _amountofspawnedobjects++;
            _tempobject.GetComponent<InteractableObject>().ID = _amountofspawnedobjects;
            _tempobject.tag = _p.tag;
            foreach (Transform go in _tempobject.transform.GetComponentsInChildren<Transform>())
            {
                go.tag = _p.tag;
            }

            _tempobject.GetComponent<InteractableObject>().evtOnClear += OnObjectCleared;
            _tempobject.GetComponent<InteractableObject>().evtOnDecayFinished += OnObjectMissed;
            SpawnedObjects.Add(_tempobject.GetComponent<InteractableObject>().ID, _tempobject);
            StartCoroutine(SpawnCoolDown());
        }

    }

IEnumerator SpawnCoolDown()
    {
        if (GlobalData.CurrentMM.isgamefinished)
        {
           yield return null;
        }

        yield return new WaitForSeconds(SpawnRate);
        SpawnNextObject();
        yield return null;
    }

    private Quaternion LookAtCamera()
    {
        Quaternion returnedvalue = Quaternion.identity;
        
        return returnedvalue;
    }

    private Vector3 GetNextSpawnLocation()
    {
        Vector3 returnedvalue = Vector3.zero;
        float x = UnityEngine.Random.Range(0.2f,0.81f);
        float y = UnityEngine.Random.Range(0.2f, 0.81f);

        returnedvalue = cam.ViewportToWorldPoint(new Vector3(x, y, 10));
        return returnedvalue;
    }

    private GameObject GetNextObjectToBeSpawned()
    {
        if(queuedobjects.Count > 0)
        {
            return (GameObject)queuedobjects.Dequeue();
        }
        GameObject returnedvalue = null;
        int l = Random.Range(0,SpawnableObjects.Length);
        if (SpawnableObjects[l].GetComponent<TargetObject>())
        {
            _isnextspawnspecial = true;
        }
        else
        {
            _isnextspawnspecial = false;
        }
        returnedvalue = SpawnableObjects[l];
        return returnedvalue;
    }

}
