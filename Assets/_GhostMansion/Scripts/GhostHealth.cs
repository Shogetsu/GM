using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GhostHealth : NetworkBehaviour
{

    public Text HealthText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateHealthGhost(int vit)
    {
        RpcUpdateHealth(vit);
    }


    [ClientRpc]
    public void RpcUpdateHealth(int vit)
    {
        HealthText.text = "Ghost: " + vit.ToString();
    }

   /* [ClientRpc]
    public void RpcChangeLayerGhost()
    {
        Transform[] children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform go in children)
        {
            go.gameObject.layer = 10;
        }
    }

    GameObject GetGhostGO()
    {
        GameObject go = null;
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<SetupLocalPlayer>().colorString == "White")
            {
                go = GameObject.FindGameObjectsWithTag("Player")[i];
                break;
            }
        }
        return go;
    }*/
}
