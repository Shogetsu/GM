using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SetupLocalPlayer : NetworkBehaviour {

    [SyncVar]
    public string pname = "player";

    [SyncVar]
    public Color playerColor;

    public string colorString;

    [SyncVar]
    int layerGO;

   /* GameObject cam;
    GameObject camGhost;*/

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
           /* pname = GUI.TextField(new Rect(25, Screen.height - 40, 100, 30), pname);
            if(GUI.Button(new Rect(130, Screen.height - 40, 80, 30), "Change"))
            {
                CmdChangeName(pname);
            }*/
        }
    }

    /*Esto es para cambiar de nombre dentro de la partida.... NO INTERESA (de momento)*/
    [Command]
    void CmdChangeName(string newName)
    {
        pname = newName;
        //this.GetComponentsInChildren<TextMesh>()
    }

    private void Start()
    {
       /* cam = GameObject.Find("Map").transform.Find("Camera").gameObject;
        camGhost = GameObject.Find("Map").transform.Find("CameraGhost").gameObject;

        cam.SetActive(false);
        camGhost.SetActive(false);*/

        Renderer[] rends = GetComponentsInChildren<Renderer>(); // Se obtienen todos los renders del GameObject
        foreach (Renderer r in rends)//Se recorren todos los renders y se les asigna el color del jugador en cuestion
        {
            if(r.name.Equals("Object001") || r.name.Equals("Object002"))
            {
                r.material.color = playerColor;
            }
        }
            

        SetColorString(playerColor);

        CmdSetLayerGO();
       // UpdateCamera();
    }


    void SetColorString(Color playerColor)
    {
        if (playerColor == Color.magenta)
            colorString = "Magenta";
        if (playerColor == Color.red)
            colorString = "Red";
        if (playerColor == Color.cyan)
            colorString = "Cyan";
        if (playerColor == Color.blue)
            colorString = "Blue";
        if (playerColor == Color.green)
            colorString = "Green";
        if (playerColor == Color.yellow)
            colorString = "Yellow";
        if (playerColor == Color.white)
            colorString = "White";


    }

    [Command]
    void CmdSetLayerGO()
    {
        if (colorString == "White")
            layerGO = 10; //ghost
        else
            layerGO = 8; //player

        //this.gameObject.layer = layerGO;
        RpcUpdateLayerGO();
    }

    [ClientRpc]
    void RpcUpdateLayerGO()
    {
        Debug.Log("Soy el jugador de color: " + colorString);

        Transform[] children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform go in children)
        {
            go.gameObject.layer = layerGO;
        }
    }
    /* void UpdateCamera()
     {
         if (colorString == "White") //Si es el fantasma
             camGhost.SetActive(true);
         else
             cam.SetActive(true);
     }*/

    public GameObject GetGameObjectGhost()
    {
        if (colorString == "White")
            return this.gameObject;
        else
            return null;
    }
}
