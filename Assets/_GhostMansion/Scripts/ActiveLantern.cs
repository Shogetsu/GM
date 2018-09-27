using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActiveLantern : NetworkBehaviour {

    [SyncVar(hook = "UpdateLanternLight")]
    bool active;

    // Use this for initialization
    void Start () {
        if (isServer)
            active = false;
    }

    void UpdateLanternLight(bool act)
    {
        this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.SetActive(act);
    }

    [Command]
    public void CmdActiveLantern(bool act)
    {
        active = act;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
