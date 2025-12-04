using UnityEngine;
using UnityEngine.SceneManagement;

public class TrocaCena : MonoBehaviour
{
    // Nome da cena que será carregada
    public string nomeDaCena;

    // Função chamada pelo botão
    public void TrocarCena()
    {
        SceneManager.LoadScene(nomeDaCena);
    }
}
