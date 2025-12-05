using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [Header("Configuração dos Corações")]
    public Image[] coracoes;          // Array de imagens dos corações (3 no total)
    public Sprite coracaoCheio;       // Sprite do coração cheio
    public Sprite coracaoMetade;      // Sprite do coração metade
    public Sprite coracaoVazio;       // Sprite do coração vazio

    [Header("Configuração de Vida")]
    public int maxHealth = 5;         // Vida máxima (5)

    public void AtualizarVida(int vidaAtual)
    {
        // Cada coração representa ~1.66 de vida (5/3)
        float vidaPorCoracao = (float)maxHealth / coracoes.Length;

        for (int i = 0; i < coracoes.Length; i++)
        {
            float vidaMinimaParaCheio = (i + 1) * vidaPorCoracao;
            float vidaMinimaParaMetade = i * vidaPorCoracao + (vidaPorCoracao / 2f);

            if (vidaAtual >= vidaMinimaParaCheio)
            {
                coracoes[i].sprite = coracaoCheio;
            }
            else if (vidaAtual >= vidaMinimaParaMetade)
            {
                coracoes[i].sprite = coracaoMetade;
            }
            else
            {
                coracoes[i].sprite = coracaoVazio;
            }
        }
    }
}