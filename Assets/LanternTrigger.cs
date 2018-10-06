using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanternTrigger : NetworkBehaviour {

    Coroutine coLosingHealth;

    GameObject player;

    [SyncVar]
    public bool coroutineIsRunning;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   /* void OnTriggerStay(Collider other)
    {
        //tagTrigger = other.tag;

        if (other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            //triggeredLantern = true;
            //Debug.Log("Trig: "+triggeredLantern);

            this.gameObject.transform.root.GetComponent<ActiveLantern>().TriggerStay(other);

           /* if (coroutineIsRunning == false)
            {
                player = other.gameObject;
                CmdStartLosingHealth();
                Debug.Log("Start losing health...");
            }*/
       // }
    //}

   /* void OnTriggerExit(Collider other)
    {
        // tagTrigger = other.tag;
        if (other.gameObject.CompareTag("Player") &&
            other.gameObject.GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            //Si se estaba ejecutando alguna corutina pero ya no hay ningun trigger ejecutandose... se detiene la corutina
            /*if (GetComponent<Health>().coroutineIsRunning == true && triggeredLantern == false)
            {*/
           // this.gameObject.transform.root.GetComponent<ActiveLantern>().TriggerExit();

           /* CmdStopLosingHealth();
                Debug.Log("STOP losing health...");*/
            //}
       // }
   // }

/*
    [Command]
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
    }

    IEnumerator CmdLosingHealth()
    {
        yield return new WaitForSeconds(1f);
        int playerVit = player.GetComponent<Health>().GetVit();
        for (; ; )
        {
            if (playerVit > 0)
            {
                playerVit = playerVit - 1;
                player.GetComponent<Health>().CmdSetVit(playerVit);
                //RpcUpdateHealthBar();
                GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(player.GetComponent<Health>().GetVit());
            }
            else
            {
                //RpcUpdateHealthBar();
                GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(player.GetComponent<Health>().GetVit());
                StopCoroutine(CmdLosingHealth());
                print("Stopped " + Time.time);
                //coroutineIsRunning = false;

                break;
            }
            yield return new WaitForSeconds(1f);
        }
    }*/
}
