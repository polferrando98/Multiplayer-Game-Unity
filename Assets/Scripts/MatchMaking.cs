using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;


public class MatchMaking : MonoBehaviour
{
    public Text newMatchName;
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

    public void OnJoinMatchClicked(UnityEngine.Networking.Types.NetworkID networkId)
    {
        NetworkManager.singleton.StartMatchMaker();
        NetworkManager.singleton.matchMaker.JoinMatch(networkId, "", "", "", 0, 0, OnMatchJoin);
    }

    public void OnMatchJoin(bool success, string extendedIngo, MatchInfo matchInfo)
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
