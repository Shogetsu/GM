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

    [Command]
    public void CmdStartLosingHealth()
    {
        coroutineIsRunning = true;
        coLosingHealth= StartCoroutine(CmdLosingHealth());
    }

    [Command]
    public void CmdStopLosingHealth()
    {
        coroutineIsRunning = false;
        StopCoroutine(coLosingHealth);
    }

    /*public void StopLosingHealth()
    {
        StopCoroutine(CmdLosingHealth());
    }*/

    IEnumerator CmdLosingHealth()
    {
        for (; ; )
        {
            if (vit > 0)
            {
                vit = vit - 1;
                //RpcUpdateHealthBar();
                GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(vit);
            }
            else
            {
                //RpcUpdateHealthBar();
                GameObject.Find("GameManager").GetComponent<GhostHealth>().UpdateHealthGhost(vit);
                StopCoroutine(CmdLosingHealth());
                print("Stopped " + Time.time);
                //coroutineIsRunning = false;

               /* if(GetComponent<ColorLevel>().GetColorLevel()>0)
                    GetComponent<ColorLevel>().CmdStartCoroutine();*/

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

    /*void OnTriggerStay(Collider other)
    {
        // tagTrigger = other.tag;
        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            if (GetComponent<Health>().coroutineIsRunning == false)
            {
                GetComponent<Health>().CmdStartLosingHealth();
                Debug.Log("Start losing health...");
                triggeredLantern = true;
            }
        }
    }*/

    /*void OnTriggerEnter(Collider other)
    {
        // tagTrigger = other.tag;

        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            triggeredLantern = true;
            Debug.Log(triggeredLantern);

            if (GetComponent<Health>().coroutineIsRunning == false)
            {
                GetComponent<Health>().CmdStartLosingHealth();
                Debug.Log("Start losing health...");
            }
        }
    }*/

    void OnTriggerStay(Collider other)
    {
        // tagTrigger = other.tag;

        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            triggeredLantern = true;
            //Debug.Log("Trig: "+triggeredLantern);

            if (GetComponent<Health>().coroutineIsRunning == false)
            {
                GetComponent<Health>().CmdStartLosingHealth();
                Debug.Log("Start losing health...");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // tagTrigger = other.tag;
        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            //Si se estaba ejecutando alguna corutina pero ya no hay ningun trigger ejecutandose... se detiene la corutina
            if (GetComponent<Health>().coroutineIsRunning == true && triggeredLantern == false)
            {
                GetComponent<Health>().CmdStopLosingHealth();
                Debug.Log("STOP losing health...");
            }
        }
    }

    void Update()
    {
        if (isServer)
            triggeredLantern = false;

        //Debug.Log("Fix: "+triggeredLantern);

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
