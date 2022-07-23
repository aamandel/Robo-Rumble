using UnityEngine;
using UnityEngine.InputSystem;

public class StaticData : MonoBehaviour
{
    public static PlayerUIManager playerUI;
    public static InputDevice[] player1Input, player2Input;
    public static Color p1Color, p2Color;
    public static GameObject p1GO, p2GO;
    public static int p1Lives, p2Lives;
    public int numStartLives = 3;
    public static bool roundOver = false;

    public void setPlayers(PlayerInput p)
    {
        if (player1Input == null)
        {
            SetDevices(ref player1Input, p);
            p1GO = p.gameObject;
        }else if(player2Input == null)
        {
            SetDevices(ref player2Input, p);
            p2GO = p.gameObject;
        }
        if (playerUI)
        {
            playerUI.connectToPlayers();
        }
    }

    private void SetDevices(ref InputDevice[] deviceArray, PlayerInput pInput)
    {
        var readOnlyDevices = pInput.devices;
        deviceArray = new InputDevice[readOnlyDevices.Count];
        int index = 0;
        foreach (InputDevice inputDevice in readOnlyDevices)
        {
            deviceArray[index] = inputDevice;
            index++;
        }
    }

    private void Awake()
    {
        playerUI = gameObject.GetComponent<PlayerUIManager>();
        p1Lives = numStartLives;
        p2Lives = numStartLives;
    }
}
