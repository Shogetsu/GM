using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour {

    public ParticleSystem particleHit;

    [SerializeField]
    [SyncVar]
    int vit;

    [SerializeField]
    [SyncVar]
    int def = 0;

    [SyncVar]
    public bool coroutineIsRunning;

    public int vitMAX = 100;

    List<Coroutine> coroutineList = new List<Coroutine>();
    Coroutine coLosingHealth;

    [SyncVar]
    bool triggeredLantern;

    //public RectTransform healthBar;



    void Start () {
        if (!isLocalPlayer)
            return;
        CmdSetVITMAX();
        // ghostHealthText = GameObject.Find("Canvas").transform.Find("GhostHealth") as Text;
        if (isServer)
            triggeredLantern = false;
    }
	
    [Command]
    public void CmdSetVITMAX()
    {
        vit = vitMAX;
    }
    

    [Command]
    public void CmdSetDef(int newDef)
    {
        def = newDef;
    }
    
    public int GetVit()
    {
        return vit;
    }


    [Command]
    public void CmdSetVit(int newVit)
    {
        vit = newVit;
    }


    public void TakeDamage(int damage)
    {
        if (!isServer) return;

       // particleHit.Emit(15);
        //Este metodo siempre se ejecutara en el lado del servidor (command o con if(isServer))

        //int newDamage = damage - def; //Se aplica la defensa adicional del jugador debido a la armadura equipada (si def=0 el damage sera el mismo, en caso de plantas y animales siempre sera asi)

        //Este metodo solo se ejecuta a traves de un Command que envia un jugador con autoridad
       // Debug.Log("Bajando la vida... damage: "+newDamage);
        //Debug.Log("Bajando la vida... damage: " + damage);
        Debug.Log("Bajando la vida...");

        vit = vit - damage;
        GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(vit);
        //ghostHealthText.text = "Ghost: " + vit.ToString();
        /* if (GetComponent<Inventory>() != null) //Solo los jugadores pueden tener inventario y, por tanto, armaduras, en caso contrario se trata de un animal o planta
         {
             GetComponent<Inventory>().LosingColorLevelArmor(damage); //Si el jugador que sufre danyo lleva armadura equipada, esta se vera danyada disminuyendo su colorLevel
         }

         if (healthBar != null) //healthBar NO es null cuando se trata de un jugador
         {
             RpcUpdateHealthBar();
         }

         if (vit <= 0 && healthBar==null) //healthBar es null cuando se trata de cualquier elemento que no sea un jugador
         {
             StartCoroutine(Die());
         }

         if (GetComponent<AnimalIA>() != null) //Si se trata de un animal, este entrara en el estado de huida al sufrir danyo
         {
             GetComponent<AnimalIA>().SetState("runAway");
         }*/
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.3f);
        //soltaran objetos y se destruiran posteriormente
        GetComponent<Drop>().DropItem();
        NetworkServer.Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcUpdateHealthBar()
    {
        //Debug.Log("Me queda: " + vit + " de vida");
        //healthBar.sizeDelta = new Vector2(vit, healthBar.sizeDelta.y);
    }


    /*void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        // tagTrigger = other.tag;

        if (other.gameObject.CompareTag("Lantern") &&
        GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            triggeredLantern = true;
            //Debug.Log(triggeredLantern);

            if (coroutineIsRunning == false)
            {
                //CmdStartLosingHealth();
               // Debug.Log("QUE ESTA PASANDO JODER ARRIBA LAS MANOS YA");
                coroutineIsRunning = true;
                TargetDoRpcStartCoroutine(connectionToClient);
                //CmdStartLosingHealth();
            }
        }
    }*/

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        if (other.gameObject.CompareTag("Lantern") &&
             GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            if(coroutineIsRunning && other.gameObject.GetComponent<Collider>().enabled)
            {
                other.gameObject.GetComponent<Collider>().enabled = false;
            }

        }
    }

    void OnTriggerStay(Collider other)
      {
        if (!isServer)
            return;

        // tagTrigger = other.tag;

        if (other.gameObject.CompareTag("Lantern") &&
              GetComponent<SetupLocalPlayer>().colorString == "White")
          {
              triggeredLantern = true;
            //  Debug.Log("Trig: "+triggeredLantern+"/"+Time.deltaTime);

            if (coroutineIsRunning == false)
            {
                coroutineIsRunning = true;
                TargetDoRpcStartCoroutine(connectionToClient);
            }
          /*  else if(other.gameObject.GetComponent<Collider>().enabled)
            {
                other.gameObject.GetComponent<Collider>().enabled = false;
            }*/
          }
      }

    void OnTriggerExit(Collider other)
    {
        if (!isServer)
            return;

        // tagTrigger = other.tag;
        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            //Si se estaba ejecutando alguna corutina pero ya no hay ningun trigger ejecutandose... se detiene la corutina
            if (coroutineIsRunning == true && triggeredLantern == false)
            {
                // Debug.Log("TrigEx: " + triggeredLantern + "/" + Time.deltaTime);
                //CmdStopLosingHealth();

                /*coroutineIsRunning = false;
                StopCoroutine(coLosingHealth);*/
                coroutineIsRunning = false;
                // CmdStopLosingHealth();
                TargetDoRpcStopCoroutine(connectionToClient);

                Debug.Log("STOP losing health...");
            }
        }
    }

    [Command]
    public void CmdStartLosingHealth()
    {
        coroutineIsRunning = true;
        //RpcStartCoroutine();
        coLosingHealth= StartCoroutine(LosingHealth());
    }

    [Command]
    public void CmdStopLosingHealth()
    {
        coroutineIsRunning = false;
        StopCoroutine(coLosingHealth);
        //RpcStopCoroutine();
    }

    [Command]
    void CmdCalculeVit()
    {
        vit = vit - 1;
        GameObject.Find("GameManager").GetComponent<GhostHealth>().RpcUpdateHealth(vit);
        Debug.Log("PUM!" + vit);
    }

    [TargetRpc]
    void TargetDoRpcStartCoroutine(NetworkConnection target)
    {
        coLosingHealth= StartCoroutine(LosingHealth());
    }

    [TargetRpc]
    void TargetDoRpcStopCoroutine(NetworkConnection target)
    {
        StopCoroutine(coLosingHealth);
    }

    IEnumerator LosingHealth()
    {

        yield return new WaitForSeconds(1f);
        while(true)
        {
            if (vit > 0)
            {
                CmdCalculeVit();
            }
            else
            {
                //RpcUpdateHealthBar();
                //GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(vit);
                StopCoroutine(LosingHealth());
                print("Stopped " + Time.time);
                //coroutineIsRunning = false;
                break;
            }
        yield return new WaitForSeconds(1f);
        }
    }

    [Command]
    public void CmdHeal(int heal)
    {
        vit = vit + heal;
        RpcUpdateHealthBar();
    }

    /*public bool CheckCoroutine()
    {
        return coroutineIsRunning;
    }
    */

    void FixedUpdate()
    {
        if (isServer)
            triggeredLantern = false;

      //  Debug.Log("Quedan: " + coroutineList.Count + " corutinas en lista");

       // Debug.Log("Update: "+triggeredLantern+"/"+Time.deltaTime);

        /*  if (!triggeredLantern)
          {
              if (GetComponent<SetupLocalPlayer>().colorString == "White")
              {
                  if (GetComponent<Health>().coroutineIsRunning == true)
                  {
                      GetComponent<Health>().CmdStopLosingHealth();
                      Debug.Log("Stop losing health...");
                  }
              }
          }*/
    }


}
