using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CreateLobby : MonoBehaviour
{
    [SerializeField]
    TMP_InputField lobbyCode;

    [SerializeField] 
    TMP_InputField nameInput;

    public void LobbyOnConfirm(string sceneName)
    {
        if (!string.IsNullOrEmpty(lobbyCode.text) && !string.IsNullOrWhiteSpace(nameInput.text))
        {
            // create lobby using photon from lobbyCode.text

            PlayerPrefs.SetString("PlayerName", nameInput.text);
            PlayerPrefs.SetString("LobbyCode", lobbyCode.text);

            SceneManager.LoadScene(sceneName);
        }
    }
}
