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

    GameObject ghost;

    [SyncVar]
    public bool coroutineIsRunning;

    Coroutine coLosingHealth;

    // Use this for initialization
    void Start () {
        if (isServer)
        {
            active = false;
            if(GetComponent<SetupLocalPlayer>().colorString == "White")
            {
                lanternGOenabled = false;
            }
            //coroutineIsRunning = false;
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


            //this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.Find("Cone").GetComponent<Collider>().enabled = true;
            //HACER ESTO DE OTRA FORMA: En el Update, SI el fantasma NO tiene activa la corrutina, activar el collider de la linterna otra vez


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
        //Debug.Log(coroutineIsRunning);

        if (!isServer)
            return;

        //TODO cambiar cuando la layer de Ghost funcione
        /*  if (GameObject.Find("Player(Clone)").GetComponent<SetupLocalPlayer>().GetGameObjectGhost().GetComponent<Health>().coroutineIsRunning==false)
          {
              this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.Find("Cone").GetComponent<Collider>().enabled = true;
              Debug.Log("KEEEEEEE");
          }*/

        if (GetGhostGO().GetComponent<Health>().coroutineIsRunning == false)
        {
            this.gameObject.transform.GetChild(0).gameObject.transform.Find("LanternLight").gameObject.transform.Find("Cone").GetComponent<Collider>().enabled = true;
            //Debug.Log("KEEEEEEE");
        }

	}

    GameObject GetGhostGO()
    {
        GameObject go = null;
        for(int i=0; i< GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<SetupLocalPlayer>().colorString=="White")
            {
                go = GameObject.FindGameObjectsWithTag("Player")[i];
                break;
            }
        }
        return go;
    }

    /*  public void TriggerStay(Collider other)
      {
          if (coroutineIsRunning == false)
          {
              ghost = other.gameObject;
              CmdStartLosingHealth();
              Debug.Log("Start losing health...");
          }
      }

      public void TriggerExit()
      {
          if (coroutineIsRunning == true)
          {
              CmdStopLosingHealth();
              Debug.Log("STOP losing health...");
          }
      }*/



    /* [Command]
     void CmdStartLosingHealth()
     {
         coroutineIsRunning = true;
         coLosingHealth = StartCoroutine(CmdLosingHealth());
     }

     [Command]
     public void CmdStopLosingHealth()
     {
         coroutineIsRunning = false;
         StopCoroutine(coLosingHealth);
     }*/


    /* IEnumerator CmdLosingHealth()
     {
         yield return new WaitForSeconds(1f);
         int ghostVit = ghost.GetComponent<Health>().GetVit();
         for (; ; )
         {
             if (ghostVit > 0)
             {
                 ghostVit = ghostVit - 1;
                 ghost.GetComponent<Health>().CmdSetVit(ghostVit);
                 //RpcUpdateHealthBar();
                 GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(ghost.GetComponent<Health>().GetVit());
             }
             else
             {
                 //RpcUpdateHealthBar();
                 GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(ghost.GetComponent<Health>().GetVit());
                 StopCoroutine(CmdLosingHealth());
                 print("Stopped " + Time.time);
                 //coroutineIsRunning = false;

                 /* if(GetComponent<ColorLevel>().GetColorLevel()>0)
                      GetComponent<ColorLevel>().CmdStartCoroutine();*/

    //    break;
    //  }
    //  yield return new WaitForSeconds(1f);
    // }
    //}

}
