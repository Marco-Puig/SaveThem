using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsShower : MonoBehaviour
{
    [SerializeField] List<GameObject> menuPieces = new List<GameObject>();
    [SerializeField] List<GameObject> creditPieces = new List<GameObject>();

    public void OnShow()
    {
        for (int i = 0; i < menuPieces.Count; i++)
        {
            menuPieces[i].SetActive(!menuPieces[i].activeSelf);
            
        }

        for (int i = 0; i < creditPieces.Count; i++)
        {
            creditPieces[i].SetActive(!creditPieces[i].activeSelf);

        }
    }
}
