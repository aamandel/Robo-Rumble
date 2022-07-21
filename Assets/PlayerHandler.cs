using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerHandler : MonoBehaviour
{
    public static List<Player> players = new List<Player>();
    public Color[] playerColors;
    public GameObject Player1Slot, Player2Slot;
    private static int numPlayers;

    private void Start()
    {
        numPlayers = 0;
    }

    public static int GetNumPlayers()
    {
        return numPlayers;
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        numPlayers++;
        var readOnlyDevices = input.gameObject.GetComponent<PlayerInput>().devices;
        InputDevice[] devices = new InputDevice[readOnlyDevices.Count];
        int index = 0;
        foreach (InputDevice device in readOnlyDevices)
        {
            devices[index] = device;
            index++;
        }
        if (numPlayers == 1)
        {
            input.gameObject.transform.SetParent(Player1Slot.transform);
            StaticData.player1Input = devices;
            StaticData.p1Color = playerColors[0];
        }
        else
        {
            input.gameObject.transform.SetParent(Player2Slot.transform);
            StaticData.player2Input = devices;
            StaticData.p2Color = playerColors[1];
        }
        input.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        // TODO: set player color

        string name = "player " + (numPlayers);
        Debug.Log(name + " joined");
    }

    public void OnPlayerLeft(PlayerInput i_val)
    {
        players.RemoveAll(p => p.gObject == i_val.gameObject);
    }

    //The static function ClearPlayerUI() clears the current UI selection for all 
    //players and should be called before displaying any player specific UI elements
    public static void ClearPlayerUI()
    {
        foreach (Player p in players)
        {
            var bs = p.GetGO().GetComponent<MultiplayerEventSystem>();
            bs.playerRoot = null;
            bs.SetSelectedGameObject(null);
        }
    }
}

public class Player
{
    public string name;
    public int ID;
    public GameObject gObject;
    public Color playerColor;
    public GameObject UI;
    private bool dead;
    public Camera camera;

    public Player(string _name, int _id, GameObject _gObject, Color _playerColor, GameObject _UI, Camera _camera)
    {
        name = _name;
        ID = _id;
        gObject = _gObject;
        playerColor = _playerColor;
        dead = false;
        UI = _UI;
        camera = _camera;

        if (_gObject)
        {
            _gObject.name = name;
        }

        if (UI)
        {
            UI.SetActive(true);
        }
    }

    public GameObject GetGO()
    {
        return gObject;
    }
    public bool isDead()
    {
        return dead;
    }

    public void setDead(bool _isDead)
    {
        dead = _isDead;
    }
}
