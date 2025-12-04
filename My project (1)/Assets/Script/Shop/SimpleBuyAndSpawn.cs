using UnityEngine;
using UnityEngine.UI;

public class SimpleBuyAndActivate : MonoBehaviour
{
    public string idCompra = "compra_unica_001"; // ID única pra salvar o estado
    public int preco = 10;
    public GameObject objetoParaAtivar;
    public Button botaoCompra;
    public MonoBehaviour scriptParaDesativar; // script que será deletado

    [Header("Painel Final - Após COMPRAR este terreno")]
    public GameObject painelFinal; // Painel que aparece quando COMPRAR este terreno

    void Start()
    {
        // Verificar se JÁ comprou antes
        if (PlayerPrefs.GetInt(idCompra, 0) == 1)
        {
            if (objetoParaAtivar != null)
                objetoParaAtivar.SetActive(true);

            if (botaoCompra != null)
                botaoCompra.gameObject.SetActive(false);

            if (scriptParaDesativar != null)
                Destroy(scriptParaDesativar);

            return;
        }

        // Se ainda não comprou, configurar o botão
        if (botaoCompra != null)
            botaoCompra.onClick.AddListener(TentarComprar);

        if (objetoParaAtivar != null)
            objetoParaAtivar.SetActive(false);

        // Garantir que painel não aparece no início
        if (painelFinal != null)
            painelFinal.SetActive(false);
    }

    void TentarComprar()
    {
        if (GameData.Instance.coins >= preco)
        {
            GameData.Instance.coins -= preco;

            if (objetoParaAtivar != null)
                objetoParaAtivar.SetActive(true);

            PlayerPrefs.SetInt(idCompra, 1);
            PlayerPrefs.Save();

            if (botaoCompra != null)
                botaoCompra.gameObject.SetActive(false);

            if (scriptParaDesativar != null)
                Destroy(scriptParaDesativar);

            // ⭐⭐ MOSTRAR PAINEL FINAL quando comprar ESTE terreno ⭐⭐
            if (painelFinal != null)
            {
                painelFinal.SetActive(true);
            }

            Debug.Log("✅ Compra feita - TERREMO FINAL COMPRADO!");
        }
        else
        {
            Debug.Log("❌ Moedas insuficientes!");
        }
    }
}