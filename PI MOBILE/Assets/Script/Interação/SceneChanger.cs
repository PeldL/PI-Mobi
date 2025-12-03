using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Nome da cena que você quer carregar
    public string nomeDaCena;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TrocarCena();
        }
    }

    void TrocarCena()
    {
        if (!string.IsNullOrEmpty(nomeDaCena))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogWarning("Nome da cena não foi definido!");
        }
    }
}