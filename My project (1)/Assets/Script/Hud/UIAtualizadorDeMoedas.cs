using UnityEngine;
using UnityEngine.UI;

public class UIAtualizadorDeMoedas : MonoBehaviour
{
    [Header("Texto da UI que mostrará a quantidade de moedas")]
    public Text textoMoedas;

    void Update()
    {
        if (GameData.Instance != null && textoMoedas != null)
        {
            textoMoedas.text = " " + GameData.Instance.coins;
        }
    }

}