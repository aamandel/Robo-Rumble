using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputInitializer : MonoBehaviour
{
    private PlayerInput p1Input, p2Input;
    public GameObject playerPrefab;
    public MultiTargetCamera camTop, camBottom;
    public PlayerUIManager playerUI;
    void Awake()
    {
        if (StaticData.player1Input != null)
        {
            GameObject player1 = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p1Input = player1.GetComponent<PlayerInput>();
            p1Input.SwitchCurrentControlScheme(StaticData.player1Input);
            ColorAssigner CA = player1.GetComponent<ColorAssigner>();
            if (CA && StaticData.p1Color != null)
            {
                CA.SetColor(StaticData.p1Color);
            }
            // pass player to multi target camera
            if (camTop)
            {
                camTop.AddTarget(p1Input);

                // add to player list
                //PlayerHandler.players.Add(new Player("Player 1", 1, player1, StaticData.p1Color, null, camTop.gameObject.GetComponent<Camera>()));
                WeaponController WC = player1.GetComponent<WeaponController>();
                if (WC)
                {
                    WC.SetCam(camTop.gameObject.GetComponent<Camera>());
                }
            }
            StaticData.p1GO = player1;
        }
        if(StaticData.player2Input != null)
        {
            GameObject player2 = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            p2Input = player2.GetComponent<PlayerInput>();
            p2Input.SwitchCurrentControlScheme(StaticData.player2Input);
            ColorAssigner CA = player2.GetComponent<ColorAssigner>();
            if (CA && StaticData.p2Color != null)
            {
                CA.SetColor(StaticData.p2Color);
            }
            // pass player to multi target camera
            if (camBottom)
            {
                camBottom.AddTarget(p2Input);

                // add to player list
                //PlayerHandler.players.Add(new Player("Player 2", 2, player2, StaticData.p2Color, null, camBottom.gameObject.GetComponent<Camera>()));
                WeaponController WC = player2.GetComponent<WeaponController>();
                if (WC)
                {
                    WC.SetCam(camBottom.gameObject.GetComponent<Camera>());
                }
            }
            StaticData.p2GO = player2;
        }
        if (playerUI)
        {
            playerUI.connectToPlayers();
        }
        
    }
}
