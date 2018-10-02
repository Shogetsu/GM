﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PjControl : NetworkBehaviour
{
    private AudioManager audioManager;

    static Animator anim;
    public float rotationSpeed = 100.0f;

    public float speed;
    public float normalSpeed;
    // public Rigidbody RB;
    public float jumpForce;
    public CharacterController controller;
    public Rigidbody rb;
    private Vector3 moveDirection;
    public bool isGrounded = true;
    //public ThirdPersonCamera _thirdPCam;
    public float gravityScale;

    GameObject cam;

    public Transform pivot;

    public GameObject playerModel;

    CursorLockMode wantedMode;

    bool triggeredLantern = false;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer) //Si no se trata del jugador... return
        {
           // transform.GetChild(0).gameObject.SetActive(false); //camara

            return;
        }

        if (isServer) // host runs
        {
            Debug.Log("JUGADOR 1 AL ATAQUE");
        }
        else if (isClient) // client runs
        {
            Debug.Log("JUGADOR 2 AL ATAQUE");
        }


        /*  if (isServer)
              gameOver = false;*/

        //RB = GetComponent<Rigidbody>();
        //controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //GetComponent<Health>().CmdSetVITMAX();

        //transform.GetChild(0).GetComponent<Camera>().enabled = false;

        //audioManager
        audioManager = AudioManager.instance;
        normalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {


       /* if (!isLocalPlayer || GameObject.Find("GameManager").GetComponent<DayNightCycle>().gameOver)
            return;*/

        if (!isLocalPlayer)
            return;

        /* Vector3 pos = transform.position;
         Vector3 dir = Vector3.down;
         float dis = 1.0f;*/

        //isGrounded = Physics.Raycast(pos, dir, dis, LayerMask.NameToLayer("Ground"));
        // Debug.DrawRay((new Vector3(rb.transform.position.x, rb.transform.position.y + 1f, rb.transform.position.z)), Vector3.down, Color.green, 5);

        // isGrounded = CheckGround();

        while (cam == null)
        {
            Debug.Log(GetComponent<SetupLocalPlayer>().colorString);

            if ((GetComponent<SetupLocalPlayer>().colorString != ""))
            {
                if (GetComponent<SetupLocalPlayer>().colorString == "White")
                {
                    cam = transform.Find("CameraGhost").gameObject;
                    cam.SetActive(true);
                }
                else
                {
                    cam = transform.Find("Camera").gameObject;
                    cam.SetActive(true);
                }
            }
        }


        /* if (_thirdPCam.target != transform.GetChild(0)) //Se obtiene la posicion del gameobject target dentro del body del modelo
             _thirdPCam.target = transform.GetChild(0); //Se le asigna al target del script de la camara en tercera persona

         if (GameObject.FindObjectOfType<ThirdPersonCamera>() != null && _thirdPCam == null)
         {
             _thirdPCam = GameObject.FindObjectOfType<ThirdPersonCamera>();
             Debug.Log("Camara establecida");
         }*/

        ShowMouse();

        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * speed;
        /*translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);*/
        /*mover en la misma direccion*/
        /*Vector3 movement = new Vector3(rotation, 0.0f, translation);
         transform.rotation = Quaternion.LookRotation(movement);
         transform.Translate(movement, Space.World);*/

        /* float translation = Input.GetAxis("Vertical") * speed;
         float rotation = Input.GetAxis("Horizontal") * speed; 

         RB.velocity = new Vector3(rotation, RB.velocity.y, translation);
         */

        // moveDirection = new Vector3(rotation, moveDirection.y, translation);

        float yStore = moveDirection.y;

        moveDirection = (transform.forward * translation) + (transform.right * rotation);
        //Si el jugador corre en diagonal corre mas PERO ASI correra normal
        if (translation != 0 && rotation != 0)
        {
            moveDirection = moveDirection.normalized * speed;
        }

        moveDirection.y = yStore;

        /* moveDirection = (transform.forward * translation) + (transform.right * rotation);
         moveDirection = moveDirection.normalized * speed;*/

        /*  if (isGrounded)
        {*/
        moveDirection.y = 0f;
       /* if (Input.GetButtonDown("Jump")) //La tecla "espacio" es por defecto Jump
        {
            // RB.velocity = new Vector3(RB.velocity.x, jumpForce, RB.velocity.z);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            // moveDirection.y = jumpForce;
            GetComponent<NetworkAnimator>().SetTrigger("isJumping");
            audioManager.PlaySound("Jump");

            if (isServer) //El host ejecuta las animaciones Trigger 2 veces, esto lo soluciona
            {
                GetComponent<NetworkAnimator>().animator.ResetTrigger("isJumping");
            }

            //  anim.SetTrigger("isJumping");
        }*/
        

        if (Input.GetMouseButton(0) && GetComponent<SetupLocalPlayer>().colorString != "White")
        {
            GetComponent<ActiveLantern>().CmdActiveLantern(true);
            speed = normalSpeed / 2;
            CmdUpdateAnimSpeed(0.5f);
            //this.gameObject.GetComponent<NetworkAnimator>().animator.speed = 0.5f;
            /* 
            //(Input.GetMouseButtonDown(0) && Input.GetButton("ShowMouse") == false


            //El jugador golpea si presiona clic izquierdo y el raton no se esta mostrando en pantalla
            GetComponent<NetworkAnimator>().SetTrigger("isHitting");

            if (isServer) //El host ejecuta las animaciones Trigger 2 veces, esto lo soluciona
            {
                GetComponent<NetworkAnimator>().animator.ResetTrigger("isHitting");
            }

            */
        }

        else if (Input.GetMouseButton(0) == false)
        {
           GetComponent<ActiveLantern>().CmdActiveLantern(false);
           speed = normalSpeed;
           CmdUpdateAnimSpeed(1);
           //this.gameObject.GetComponent<NetworkAnimator>().animator.speed = 1;
        }
        //}

        /*  if (Input.GetButtonDown("Jump")) //La tecla "espacio" es por defecto JUMP
          {
              rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

              anim.SetTrigger("isJumping");
          }*/

        if (GetComponent<Health>().GetVit() <= 0 
            //&& GameObject.Find("GameManager").GetComponent<DayNightCycle>().gameOver == false
            )
        {
            anim.SetBool("isDead", true);

            //ACTIVAR VENTANA DE GAMEOVER AQUI!!!
            CmdGameOver();
            audioManager.StopSound("Footstep");
            // GameObject.Find("LobbyManager").transform.Find("TopPanel").GetComponent<Lobby>
        }

        if (translation != 0 || rotation != 0)
        {
            if (!anim.GetBool("isRunning"))
            {
                audioManager.PlaySound("Footstep");
            }
            anim.SetBool("isRunning", true);
            anim.SetBool("isIdle", false);
            
        }
        else
        {
            if (anim.GetBool("isRunning"))
            {
                audioManager.StopSound("Footstep");
            }
            anim.SetBool("isRunning", false);
            anim.SetBool("isIdle", true);
        }

        //moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);

        //controller.Move(moveDirection * Time.deltaTime );


        //Vector3 mov = new Vector3(rotation, 0, translation) * speed * Time.deltaTime;
        rb.MovePosition(transform.position + moveDirection);


        //Mover al jugador en diferentes direcciones basadas en la direccion de la camara
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // !=0 ---> si esta siendo pulsada la tecla
        {
           // transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            //slerp rotacion suave
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }


    [Command]
    void CmdUpdateAnimSpeed(float sp)
    {

    }

    [Command]
    void CmdGameOver()
    {
       /* GameObject.Find("GameManager").GetComponent<DayNightCycle>().gameOver = true;
        RpcGameOver();*/
    }

    [ClientRpc]
    void RpcGameOver()
    {
        GameObject.Find("Canvas").transform.Find("GameOver").gameObject.SetActive(true);
        Cursor.lockState = wantedMode = CursorLockMode.None;
        SetCursorState();
        //Debug.Log("Holacaracola: "+GameObject.Find("Canvas").transform.Find("GameOver").gameObject.activeSelf);
    }
    // Apply requested cursor state
    void SetCursorState()
    {
        Cursor.lockState = wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != wantedMode);
    }

    void ShowMouse()
    {
        if (Input.GetButton("ShowMouse"))
        {
            Cursor.lockState = wantedMode = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = wantedMode = CursorLockMode.Locked;
        }

        SetCursorState();
    }


    public Animator GetAnimator()
    {
        return anim;
    }

    /* void OnGUI()
     {
         GUILayout.BeginVertical();
         // Release cursor on escape keypress
         if (Input.GetButton("ShowMouse"))
         {
             Cursor.lockState = wantedMode = CursorLockMode.None;
         }
         else
         {
             Cursor.lockState = wantedMode = CursorLockMode.Locked;
         }

         switch (Cursor.lockState)
         {
             case CursorLockMode.None:
                 GUILayout.Label("Cursor is normal");
                 if (GUILayout.Button("Lock cursor"))
                     wantedMode = CursorLockMode.Locked;
                 if (GUILayout.Button("Confine cursor"))
                     wantedMode = CursorLockMode.Confined;
                 break;
             case CursorLockMode.Confined:
                 GUILayout.Label("Cursor is confined");
                 if (GUILayout.Button("Lock cursor"))
                     wantedMode = CursorLockMode.Locked;
                 if (GUILayout.Button("Release cursor"))
                     wantedMode = CursorLockMode.None;
                 break;
             case CursorLockMode.Locked:
                 GUILayout.Label("Cursor is locked");
                 if (GUILayout.Button("Unlock cursor"))
                     wantedMode = CursorLockMode.None;
                 if (GUILayout.Button("Confine cursor"))
                     wantedMode = CursorLockMode.Confined;
                 break;
         }

         GUILayout.EndVertical();

         SetCursorState();
     }*/
   
    void OnCollisionEnter(Collision collision)
     {
        // Debug.Log("Entered");
         if (collision.gameObject.CompareTag("ground"))
         {
             isGrounded = true;
         }
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Lantern") &&
            GetComponent<SetupLocalPlayer>().colorString == "White")
        {
            if (GetComponent<Health>().coroutineIsRunning == false)
            {
                GetComponent<Health>().CmdStartLosingHealth();
                Debug.Log("Start losing health...");
                
                
            }
        }
    }*/



    /* void OnTriggerExit(Collider other)
     {
         if (other.gameObject.CompareTag("Lantern") &&
             GetComponent<SetupLocalPlayer>().colorString == "White")
         {
             if (GetComponent<Health>().coroutineIsRunning == true)
             {
                 GetComponent<Health>().CmdStopLosingHealth();
                 Debug.Log("Stop losing health...");
             }
         }
     }*/

    void OnCollisionExit(Collision collision)
     {
        // Debug.Log("Exited");
         if (collision.gameObject.CompareTag("ground"))
         {
             isGrounded = false;
         }
     }

    /*   void CheckGrounded()
       {
           if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
           {
               m_IsGrounded = true;
           }
       }*/

    /*public bool CheckGround()
    {
        Vector3 position = transform.position;
        position.y = GetComponent<CapsuleCollider>().bounds.min.y + 0.1f;
        float length = isGroundedRayLength + 0.1f;
        Debug.DrawRay(position, Vector3.down * length);
        bool grounded = Physics.Raycast(position, Vector3.down, length, 1 << LayerMask.NameToLayer("Ground"));
        return grounded;
    }*/

}
