﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class PlayerController : NetworkBehaviour
{
    private Animator animator;
    private Camera mainCamera;
    private TextMesh nameLabel;

    public GameObject win_panel;
    public GameObject loose_panel;

    private CustomNetworkManager networkManager;

    private bool panel_displayed;


    [SyncVar(hook = "LiveChanged")]
    bool alive = true;



    [SyncVar(hook = "SpeedChanged")]
    public float RUNNING_SPEED = 3.0f;

    [Command]
    void CmdChangeSpeed(float speed) { RUNNING_SPEED = speed; }
    void SpeedChanged(float speed) { RUNNING_SPEED = speed; }



    public float ROTATION_SPEED = 180.0f;

    // Name sync /////////////////////////////////////
    [SyncVar(hook = "SyncNameChanged")]
    string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }
    void SyncNameChanged(string name) { nameLabel.text = name; }

    // OnGUI /////////////////////////////////////////
  

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, Screen.height - 20));

            string prevPlayerName = playerName;
            playerName = GUILayout.TextField(playerName);
            if (playerName != prevPlayerName)
            {
                CmdChangeName(playerName);
            }

            GUILayout.EndArea();


            short newIndex = (short)GUILayout.SelectionGrid(
                networkManager.playerPrefabIndex, networkManager.playerNames, 3);

            if(newIndex != networkManager.playerPrefabIndex)
            {
                networkManager.playerPrefabIndex = newIndex;
                CmdChangePlayerPrefab(newIndex);
            }

        }
    }

   
    // Animation sync ////////////////////////////////
    [SyncVar(hook ="OnSetAnimation")]
    string animationName;


    void setAnimation(string animName)
    {
        OnSetAnimation(animName);
        CmdSetAnimation(animName);
    }

    [Command]
    void CmdSetAnimation(string animName)
    {
        animationName = animName;
    }


    void OnSetAnimation(string animName)
    {
        if (animationName == animName) return;
        animationName = animName;

        animator.SetBool("Idling", false);
        animator.SetBool("Running", false);
        animator.SetBool("Running backwards", false);
        animator.ResetTrigger("Jumping");
        animator.ResetTrigger("Kicking");

        if (animationName == "Idling") animator.SetBool("Idling", true);
        else if (animationName == "Running") animator.SetBool("Running", true);
        else if (animationName == "Running backwards") animator.SetBool("Running backwards", true);
        else if (animationName == "Jumping") animator.SetTrigger("Jumping");
        else if (animationName == "Kicking") animator.SetTrigger("Kicking");
    }


    // Lifecycle methods ////////////////////////////

    // Use this for initialization
    void Start ()
    {
        panel_displayed = false;
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        nameLabel = transform.Find("Label").gameObject.GetComponent<TextMesh>();
        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
        StartCoroutine("SpawOverTime");
    }

    // Update is called once per frame
    void Update()
    {


        if (isLocalPlayer)
        {
            Vector3 translation = new Vector3();
            float angle = 0.0f;

            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            translation += new Vector3(0.0f, 0.0f, 1 * RUNNING_SPEED * Time.deltaTime);
            transform.Translate(translation);

            if (verticalAxis > 0.0)
            {
                //setAnimation("Running");
                //translation += new Vector3(0.0f, 0.0f, verticalAxis * RUNNING_SPEED * Time.deltaTime);
                //transform.Translate(translation);
            }



            else if (verticalAxis < 0.0)
            {
                //setAnimation("Running backwards");
                //translation += new Vector3(0.0f, 0.0f, verticalAxis * RUNNING_SPEED * Time.deltaTime * 0.5f);
                //transform.Translate(translation);
            }
            else
            {
                setAnimation("Idling");
            }

            if (horizontalAxis > 0.0f)
            {
                angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
            }
            else if (horizontalAxis < 0.0f)
            {
                angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
                transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
            }

            if (Input.GetButtonDown("Jump"))
            {
                setAnimation("Jumping");
            }

            if (Input.GetButtonDown("Fire1"))
            {
                setAnimation("Kicking");
            }

            if (mainCamera)
            {
                //mainCamera.transform.SetPositionAndRotation(transform.position + new Vector3(0.0f, 4.0f, -3.0f), Quaternion.identity);
                //mainCamera.transform.LookAt(transform.position + new Vector3(0.0f, 2.0f, 0.0f), Vector3.up);
            }

            if (Input.GetMouseButtonDown(0))
            {
                //CmdAddProjectile();
            }

            if (nameLabel)
            {
                nameLabel.transform.rotation = Quaternion.identity;
            }
        }
    }


    [Command]
    void CmdChangePlayerPrefab(int prefabIndex)
    {
        networkManager.ChangePlayerPrefab(this, prefabIndex);
    }

    [Command]
    public void CmdAddProjectile()
    {
        Vector3 a_bit_behind = this.transform.position;
        float offset = 2;

        a_bit_behind = a_bit_behind - this.transform.forward * offset;
        networkManager.AddObject(4, a_bit_behind);
    }



    IEnumerator SpawOverTime()
    {
        while (true)
        {
            CmdAddProjectile();
            yield return new WaitForSeconds(0.1f);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
            return;

            Debug.Log("dying");
        CmdSetAlive(false);
        CmdChangeSpeed(0);
    }


    [Command]
    void CmdSetAlive(bool new_alive)
    {
        alive = new_alive;
        Instantiate<GameObject>(loose_panel, FindObjectOfType<Canvas>().gameObject.transform);
    }
    void LiveChanged(bool new_alive)
    {
        StopAllCoroutines();




        if (!isLocalPlayer)
        {
            Debug.Log(alive);
            Debug.Log(new_alive);
            Debug.Log("- hook-");
            if (FindObjectOfType<Image>() == null)
                Instantiate<GameObject>(win_panel, FindObjectOfType<Canvas>().gameObject.transform);

        }

    }
}
