﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DayNightCycle : NetworkBehaviour {

    private AudioManager audioManager;

    [SyncVar]
    public float time;
    public TimeSpan currentTime; //Hora actual en el juego
    public Transform sunTransform; //Transform del Sol
    public Light sun; //Sol. Lo necesitamos para cambiar su intensidad
    public Text timeText; //Texto en el que se ve reflejada la hora del juego

    [SyncVar (hook = "OnUpdateDaysScreen")]
    public int days; //Contador de dias que han transcurrido en el juego

    public float intensity;
    public Color fogDay = Color.grey;
    public Color fogNight = Color.red;
    Color dayAmbientLight = new Color(0.45f, 0.45f, 0.45f);
    Color nightAmbientLight =  new Color(0, 0, 0);

    Color eveningAmbientLight = new Color(0.45f, 0.45f, 0.45f);

    Color currentAmbientLight;

    public int speed;

    float auxTime;

    public Text gameOverDaysText;
    public Text daysText;

    [SyncVar (hook = "OnGameOverTrue")]
    public bool gameOver;

    [SyncVar]
    bool rainyDay;

    [SyncVar]
    int rainHour;

    [SyncVar]
    int rainDuration;

    bool playingMusic;
    string currentMusic;

    void Start()
    {
        /*  sunTransform = GameObject.Find("Directional Light").transform;
          sun = GameObject.Find("Directional Light").GetComponent<Light>();
          timeText = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Text>();*/
        if (isServer)
        {
            time = HourToSeconds(9); //Las partidas empiezan con el reloj a las 9:00
            rainyDay = ItsRainToday();
            if (rainyDay)
                CalculeRainHour();
        }

        daysText.text = "Day " + days.ToString();
        currentAmbientLight = NewAmbientLight(0.45f); //Esta luz ambiental es con la que empieza la partida, con sincronizar la variable time, se sincronizara la luz ambiental al mismo tiempo
        gameOverDaysText.text = "You have survived "+days.ToString()+" days";
        //RenderSettings.ambientLight = NewAmbientLight(0.45f); //pa probar

        //audioManager
        audioManager = AudioManager.instance;
        playingMusic = false;

        audioManager.PlaySound("Sea");


    }

    // Update is called once per frame
    void Update()
    {
        ChangeTime();

        if (rainyDay)
            Rain();


        //gameOverDaysText.text.Replace("X", "0");
        UpdateMusic();
    }

    void UpdateMusic()
    {
        StopCurrentMusic();
        ChangeMusic();
    }

    void StopCurrentMusic()
    {
        if (currentMusic == null) return;

        if (time >= HourToSeconds(12) && currentMusic.Equals("Morning"))
        {
            audioManager.StopSound("Morning");
            playingMusic = false;
        }

        if (time >= HourToSeconds(18) && currentMusic.Equals("Afternoon"))
        {
            audioManager.StopSound("Afternoon");
            playingMusic = false;
        }

        if (time >= HourToSeconds(23) && currentMusic.Equals("Evening"))
        {
            audioManager.StopSound("Evening");
            playingMusic = false;
        }

        if (time >= HourToSeconds(6) && currentMusic.Equals("Night"))
        {
            audioManager.StopSound("Night");
            playingMusic = false;
        }
    }

    void ChangeMusic()
    {
        if ((time >= HourToSeconds(6) && time < HourToSeconds(12)) && !playingMusic)
        {
            audioManager.PlaySound("Morning");
            currentMusic = "Morning";
            playingMusic = true;
        }

        if ((time >= HourToSeconds(12) && time < HourToSeconds(18)) && !playingMusic)
        {
            audioManager.PlaySound("Afternoon");
            currentMusic = "Afternoon";
            playingMusic = true;
        }

        if ((time >= HourToSeconds(18) && time < HourToSeconds(23)) && !playingMusic)
        {
            audioManager.PlaySound("Evening");
            currentMusic = "Evening";
            playingMusic = true;
        }

        if ((time >= HourToSeconds(0) && time < HourToSeconds(6)) && !playingMusic)
        {
            audioManager.PlaySound("Night");
            currentMusic = "Night";
            playingMusic = true;
        }
    }

    bool ItsRainToday()
    {
        //50% de probabilidad de llover

        bool rain = false;

        int num = UnityEngine.Random.Range(0, 2);
        if (num == 0)
            rain = false;
        else
            rain = true;

        /*TRUCO!!!!!!*/
       rain = true;


        Debug.Log("Va a llover hoy? " + rain);
        return rain;
    }

    void OnUpdateDaysScreen(int d)
    {
        gameOverDaysText.text = "You have survived " + d.ToString() + " days";
    }

    void CalculeRainHour()
    {
        rainHour = UnityEngine.Random.Range(0, 24); //Se obtiene la hora de comienzo de la lluvia (entre 00:00 y 23:00)
        rainDuration = UnityEngine.Random.Range(1, 4); //1 a 3h

        /*TRUCO!!!!!!*/
       rainHour = 15;


        Debug.Log("Va a llover a las: " + rainHour + " y va a durar "+rainDuration);
    }

    void OnGameOverTrue(bool value)
    {
        if (value) speed = 0;
    }

    void Rain()
    {
        if (GetLocalPlayer() == null) return;

        if (time > HourToSeconds(rainHour) && time < HourToSeconds(rainHour + rainDuration))
        {
            if (GetLocalPlayer().transform.Find("Rain").gameObject.activeSelf == false)
            {
                GetLocalPlayer().transform.Find("Rain").gameObject.SetActive(true);
                audioManager.PlaySound("Rain");
            }
        }
        else if(time > HourToSeconds(rainHour + rainDuration))
        {
            if (GetLocalPlayer().transform.Find("Rain").gameObject.activeSelf == true)
            {
                GetLocalPlayer().transform.Find("Rain").gameObject.SetActive(false);
                audioManager.StopSound("Rain");
            }
        }
    }

    [Command]
    void CmdSetTime(int t)
    {
        time = HourToSeconds(t);
    }

    GameObject GetLocalPlayer()
    {
        GameObject localPlayer = null;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = players[i];
            }
        }

        return localPlayer;
    }

    public void ChangeTime()
    {

        // if(timeText==null) GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Text>();

        //time += Time.deltaTime * speed;
        if (isServer)
        {
            time += Time.deltaTime * speed;

            if (time > HourToSeconds(24))
            {
                time = 0; //Importante resetear la variable time a 0
                          //Cada 24h se avanza 1 día
                days += 1;
                daysText.text = "Day "+days.ToString();
                rainyDay = ItsRainToday();

                if(rainyDay)
                    CalculeRainHour();

            }
        }

        UpdateAmbientLightByHour();

      /*  if (time > HourToSeconds(24))
        {
            time = 0; //Importante resetear la variable time a 0
            //Cada 24h se avanza 1 día
            days += 1; 
            rainyDay=ItsRainToday();
        }*/

        currentTime = TimeSpan.FromSeconds(time);
        string[] temptime = currentTime.ToString().Split(":"[0]);
        timeText.text = temptime[0] + ":" + temptime[1][0]+"0";

        sunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 360, 0, 0));
        if (time < HourToSeconds(12))
        {
            intensity = 1 - (HourToSeconds(12) - time) / HourToSeconds(12); //La intensidad del Sol va en aumento conforme se acercan las 12h (de 00:00 a 12:00)
        }
        else
        {
            intensity = 1 - ((HourToSeconds(12) - time) / HourToSeconds(12) * (-1)); //La intensidad del Sol va disminuyendo conforme han pasado las 12h (de 12:00 a 00:00)
        }

        RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, intensity * intensity);
        sun.intensity = intensity;
    }

    public int HourToSeconds(int h)
    {
        return h * 3600;
    }

    void UpdateAmbientLightByHour()
    {
        if (time>HourToSeconds(19) && time<HourToSeconds(24)) //19h-22h
        {
            auxTime += Time.deltaTime * speed;
            if (ChangeAmbientLight(5, currentAmbientLight, NewAmbientLight(0f), auxTime)) //Si ha terminado de hacerse el cambio de AmbientLight, se reinicia auxTime
                auxTime = 0;
        }

        if(time>HourToSeconds(5) && time < HourToSeconds(9)) //05h-09h
        {
            auxTime += Time.deltaTime * speed;
            if (ChangeAmbientLight(4, currentAmbientLight, NewAmbientLight(0.45f), auxTime)) //Si ha terminado de hacerse el cambio de AmbientLight, se reinicia auxTime
                auxTime = 0;
        }

        if (time > HourToSeconds(9) && time < HourToSeconds(12)) //05h-09h
        {
            auxTime += Time.deltaTime * speed;
            if (ChangeAmbientLight(3, currentAmbientLight, NewAmbientLight(0.6f), auxTime)) //Si ha terminado de hacerse el cambio de AmbientLight, se reinicia auxTime
                auxTime = 0;
        }

        if (time > HourToSeconds(17) && time < HourToSeconds(19)) //05h-09h
        {
            auxTime += Time.deltaTime * speed;
            if (ChangeAmbientLight(2, currentAmbientLight, NewAmbientLight(0.45f), auxTime)) //Si ha terminado de hacerse el cambio de AmbientLight, se reinicia auxTime
                auxTime = 0;
        }

        //Debug.Log(RenderSettings.ambientLight+", hora: "+(int)time/3600);
       
    }

    Color NewAmbientLight(float newColor)
    {
        return new Color(newColor, newColor, newColor);
    }

    bool ChangeAmbientLight(int hourDuration, Color currentAmbientLight, Color newAmbientLight, float p)
    {
        bool finish = false;
        RenderSettings.ambientLight = Color.Lerp(currentAmbientLight, newAmbientLight, p / HourToSeconds(hourDuration));
       // Debug.Log("Deseado: "+ newAmbientLight.ToString() +" / Actual:"+RenderSettings.ambientLight.ToString()+"==>"+ RenderSettings.ambientLight.ToString().Equals(newAmbientLight.ToString()));
        if (RenderSettings.ambientLight.ToString().Equals(newAmbientLight.ToString()) )
        {
            finish = true;
            this.currentAmbientLight = newAmbientLight;
        }
       // Debug.Log("**currentAmbientLight**-->" + this.currentAmbientLight);
        return finish;
    }
}
