using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class NameInput : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;

    public void OnConfirm(string sceneName)
    {
        string name = nameInput.text;

        if (!(string.IsNullOrWhiteSpace(name)))
        {
            PlayerPrefs.SetString(name, name);
            SceneManager.LoadScene(sceneName);
        }
        
    }
}
