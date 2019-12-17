﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;


public class MatchMaking : MonoBehaviour
{
    public Text newMatchName;


    public void OnJoinMatchClicked(UnityEngine.Networking.Types.NetworkID networkId)
    {
        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.JoinMatch(networkId, "", "", "", 0, 0, OnMatchJoin);
    }

    public void OnMatchJoin(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        NetworkManager.singleton.StopMatchMaker();

        if (success)
        {
            NetworkManager.singleton.StartClient(matchInfo);
        }
        else
        {
            Debug.Log("OnMatchJoin Failed");
        }

    }


    public void OnCreateMatchClicked()
    {
        Debug.Log("OnCreateMatchCicked" + newMatchName.text);
        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.CreateMatch(newMatchName.text, 2, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        NetworkManager.singleton.StopMatchMaker();

        if (success)
        {
            NetworkManager.singleton.StartHost(matchInfo);
        }
        else
        {
            Debug.Log("OnMatchCreate failed");
        }
    }

   

    float time = 9999.0f;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 5.0f)
        {
            time = 0.0f;

            NetworkManager.singleton.StartMatchMaker();
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
        }

    }

    public RectTransform listOfMatches;
    public GameObject matchEntryPrefab;

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success) {

            int childCount = listOfMatches.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Destroy(listOfMatches.transform.GetChild(i).gameObject);

            }

            for (int i = 0; i < matches.Count; ++i)
            {
                MatchInfoSnapshot match = matches[i];
                string matchName = match.name;
                UnityEngine.Networking.Types.NetworkID networkId = match.networkId;

                GameObject gameObject = Instantiate(matchEntryPrefab, listOfMatches);
                RectTransform rect = gameObject.GetComponent<RectTransform>();

                rect.position = new Vector2(10, listOfMatches.position.y - (float)i * 50.0f);

                Text text = gameObject.GetComponentInChildren<Text>();
                text.text = "Match: " + matchName;
                Button button = gameObject.GetComponentInChildren<Button>();
                
                button.onClick.AddListener(delegate { OnJoinMatchClicked(networkId); });

            }


        }
        else
        {
            Debug.Log("OnMatchList failed");
        }
    }
}