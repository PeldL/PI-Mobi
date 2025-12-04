using UnityEngine;
using UnityEngine.UI;

public class VidaUIEnemy : MonoBehaviour
{
    [Header("Elementos UI")]
    public Image[] coracoes;          // Array de imagens dos corações
    public Sprite cheio, metade, vazio;

    [Header("Configuração")]
    public int vidaPorCoracao = 2;    // Quantos HP cada coração representa (2 = cheio, 1 = metade)

    public void Inicializar(int vidaMaxima)
    {
        // Calcula quantos corações são necessários
        int numCoracoes = Mathf.CeilToInt((float)vidaMaxima / vidaPorCoracao);

        // Opcional: Criar corações dinamicamente se necessário
        // (Implemente conforme sua UI)
    }

    public void AtualizarVida(int vidaAtual)
    {
        for (int i = 0; i < coracoes.Length; i++)
        {
            int vidaRestante = vidaAtual - (i * vidaPorCoracao);

            if (vidaRestante >= vidaPorCoracao)
            {
                coracoes[i].sprite = cheio;
            }
            else if (vidaRestante >= 1)
            {
                coracoes[i].sprite = metade;
            }
            else
            {
                coracoes[i].sprite = vazio;
            }
        }
    }
}