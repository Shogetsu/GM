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

}
