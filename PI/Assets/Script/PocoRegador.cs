using UnityEngine;

public class PocoRegador : MonoBehaviour
{
    public GameObject indicadorE;
    public ParticleSystem particulasRecompensa;
    public AudioClip somDesbloqueio;

    private bool jogadorPerto = false;
    private bool regadorJaDado = false;

    void Start()
    {
        if (indicadorE != null)
            indicadorE.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E) && !regadorJaDado)
        {
            TentarEntregarPocoes();
        }
    }

    void TentarEntregarPocoes()
    {
        // 🔥 CORRIGIDO: Agora verifica 2 poções em vez de 3
        if (QuestSystem.Instance != null && QuestSystem.Instance.missaoPocoes.pocoesEntregues >= 2)
        {
            DarRegador();
        }
        else
        {
            // 🔥 CORRIGIDO: Mensagem atualizada para 2 poções
            int atual = QuestSystem.Instance?.missaoPocoes.pocoesEntregues ?? 0;
            Debug.Log($"❌ Precisa fazer 2 poções! Atual: {atual}/2");
        }
    }

    void DarRegador()
    {
        var player = FindFirstObjectByType<PlayerRegador>();
        if (player != null)
        {
            player.DesbloquearRegador();
            regadorJaDado = true;

            Debug.Log("🎁 REGADOR DESBLOQUEADO! Área secreta liberada!");

            // Efeitos visuais e sonoros
            if (particulasRecompensa != null)
                particulasRecompensa.Play();

            if (somDesbloqueio != null)
                AudioSource.PlayClipAtPoint(somDesbloqueio, transform.position);

            if (indicadorE != null)
                indicadorE.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !regadorJaDado)
        {
            jogadorPerto = true;
            if (indicadorE != null)
                indicadorE.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (indicadorE != null)
                indicadorE.SetActive(false);
        }
    }
}