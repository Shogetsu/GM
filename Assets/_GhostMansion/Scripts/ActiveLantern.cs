using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActiveLantern : NetworkBehaviour {

    [SyncVar(hook = "UpdateLanternLight")]
    bool active;

    [SyncVar(hook = "UpdateLanternGO")]
    bool lanternGOenabled;

   /* Vector3 v3Act;
    Vector3 v3NoAct;*/

    // Use this for initialization
    void Start () {
        if (isServer)
        {
            active = false;
            if(GetComponent<SetupLocalPlayer>().colorString == "White")
            {
                lanternGOenabled = false;
            }
        }

    /*    v3Act = new Vector3(
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.x,
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.y,
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.z
            );

        v3NoAct = new Vector3(
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.x,
            5,
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.z
            );*/
    }

    void UpdateLanternLight(bool act)
    {
        if (act==false)
        {
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition = new Vector3(
                 this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.x,
            5,
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.z
            );



            /*this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition = Vector3.MoveTowards(
                this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition,
                new Vector3(
                    this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.x,
                    5,
                    this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.z
                    ),
                Time.deltaTime*100
                );*/
        }
        else
        {
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition = new Vector3(
                 this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.x,
            0.35f,
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.localPosition.z
            );
        }
            

        //this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.SetActive(act);
        this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.Find("Cone").GetComponent<Renderer>().enabled = act;
        this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.Find("Spot light").GetComponent<Light>().enabled = act;

    }

    [Command]
    public void CmdActiveLantern(bool act)
    {
        active = act;
    }

    void UpdateLanternGO(bool act)
    {
        this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.SetActive(act);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
