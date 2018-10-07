using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SetupLayerPlayer : NetworkBehaviour {

    [SyncVar]
    public int layerGO;

    // Use this for initialization
    void Start () {
        CmdSetLayer();

        if(layerGO==10)
            UpdateLayer();
    }

    void CmdSetLayer()
    {
        if (this.gameObject.GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            layerGO = 10; //ghost
        }
    }

    void UpdateLayer()
    {
        Transform[] children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform go in children)
        {
            go.gameObject.layer = layerGO;
        }
    }
}
