using UnityEngine;
using System.Collections.Generic;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    [System.Serializable]
    public class PocaoQuest
    {
        public string nomeQuest;
        public List<CraftingRecipe> pocoesRequeridas;
        public int pocoesEntregues;
        public bool questCompleta;
        public string localEntrega;
    }

    [Header("Missão de Poções")]
    public PocaoQuest missaoPocoes;
    public bool missaoAtiva = false;

    public System.Action OnQuestUpdated;
    public System.Action OnQuestCompleted;

    // 🔥 ALTERADO: Agora são 2 poções em vez de 3
    private int pocoesNecessarias = 2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CarregarProgressoQuest();

            // 🔥 VERIFICA SE JÁ TEM 2 POÇÕES E COMPLETA AUTOMATICAMENTE
            if (missaoAtiva && missaoPocoes != null && missaoPocoes.pocoesEntregues >= 2 && !missaoPocoes.questCompleta)
            {
                Debug.Log("🔔 Missão atualizada: 2/2 poções já feitas! Completando automaticamente...");
                CompletarMissao();
            }

            Debug.Log("✅ QuestSystem inicializado!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarMissaoPocoes()
    {
        if (!missaoAtiva)
        {
            missaoAtiva = true;
            missaoPocoes = new PocaoQuest()
            {
                nomeQuest = "Aprendiz de Alquimista",
                pocoesRequeridas = new List<CraftingRecipe>(),
                pocoesEntregues = 0,
                questCompleta = false,
                localEntrega = "AltarAlquimia"
            };

            Debug.Log($"🎯 MISSÃO INICIADA: Faça {pocoesNecessarias} poções diferentes!");
            OnQuestUpdated?.Invoke();
            SalvarProgressoQuest();
        }
    }

    public void OnPocaoCraftada(CraftingRecipe poção)
    {
        if (!missaoAtiva)
        {
            Debug.Log($"🔔 Poção '{poção?.recipeName}' craftada, mas missão não está ativa. Iniciando missão automaticamente...");
            IniciarMissaoPocoes();
        }

        if (missaoPocoes.questCompleta)
        {
            Debug.Log($"🔔 Poção '{poção?.recipeName}' craftada, mas missão já está completa.");
            return;
        }

        Debug.Log($"🧪 VERIFICANDO POÇÃO: {poção?.recipeName}");
        Debug.Log($"📊 Antes: {missaoPocoes.pocoesEntregues}/{pocoesNecessarias} poções | Lista: {missaoPocoes.pocoesRequeridas.Count}");

        if (!missaoPocoes.pocoesRequeridas.Contains(poção))
        {
            missaoPocoes.pocoesRequeridas.Add(poção);
            missaoPocoes.pocoesEntregues = missaoPocoes.pocoesRequeridas.Count;

            Debug.Log($"🎯 NOVA POÇÃO REGISTRADA: {poção.recipeName}");
            Debug.Log($"📊 Agora: {missaoPocoes.pocoesEntregues}/{pocoesNecessarias} poções");

            // 🔥 ALTERADO: Agora verifica com 2 poções
            if (missaoPocoes.pocoesEntregues >= pocoesNecessarias)
            {
                Debug.Log($"🏆 {pocoesNecessarias}/{pocoesNecessarias} Poções feitas! Vá até o altar!");
            }

            OnQuestUpdated?.Invoke();
            SalvarProgressoQuest();
        }
        else
        {
            Debug.Log($"ℹ️ Poção {poção.recipeName} já foi registrada antes");
        }
    }

    public void EntregarPocoes()
    {
        // 🔥 ALTERADO: Agora verifica com 2 poções
        if (missaoAtiva && missaoPocoes.pocoesEntregues >= pocoesNecessarias && !missaoPocoes.questCompleta)
        {
            CompletarMissao();
        }
        else if (missaoAtiva)
        {
            // 🔥 ALTERADO: Agora mostra 2 poções
            Debug.Log($"❌ Ainda faltam {pocoesNecessarias - missaoPocoes.pocoesEntregues} poções! ({missaoPocoes.pocoesEntregues}/{pocoesNecessarias})");
        }
    }

    void CompletarMissao()
    {
        missaoPocoes.questCompleta = true;
        missaoAtiva = false;

        if (GameData.Instance != null)
        {
            GameData.Instance.AddCoins(500);
            Debug.Log("🏆 MISSÃO COMPLETA! +500 moedas!");
        }
        else
        {
            Debug.Log("🏆 MISSÃO COMPLETA! (GameData não encontrado)");
        }

        OnQuestCompleted?.Invoke();
        SalvarProgressoQuest();
    }

    void SalvarProgressoQuest()
    {
        PlayerPrefs.SetInt("MissaoAtiva", missaoAtiva ? 1 : 0);
        if (missaoPocoes != null)
        {
            PlayerPrefs.SetInt("PocoesEntregues", missaoPocoes.pocoesEntregues);
            PlayerPrefs.SetInt("QuestCompleta", missaoPocoes.questCompleta ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    void CarregarProgressoQuest()
    {
        missaoAtiva = PlayerPrefs.GetInt("MissaoAtiva", 0) == 1;
        if (missaoAtiva)
        {
            missaoPocoes = new PocaoQuest()
            {
                nomeQuest = "Aprendiz de Alquimista",
                pocoesRequeridas = new List<CraftingRecipe>(),
                pocoesEntregues = PlayerPrefs.GetInt("PocoesEntregues", 0),
                questCompleta = PlayerPrefs.GetInt("QuestCompleta", 0) == 1,
                localEntrega = "AltarAlquimia"
            };

            if (missaoPocoes.questCompleta)
            {
                missaoAtiva = false;
            }
        }
    }

    [ContextMenu("Resetar Missão")]
    public void ResetarMissao()
    {
        missaoAtiva = false;

        if (missaoPocoes == null)
        {
            missaoPocoes = new PocaoQuest()
            {
                nomeQuest = "Aprendiz de Alquimista",
                pocoesRequeridas = new List<CraftingRecipe>(),
                pocoesEntregues = 0,
                questCompleta = false,
                localEntrega = "AltarAlquimia"
            };
        }
        else
        {
            missaoPocoes.pocoesRequeridas.Clear();
            missaoPocoes.pocoesEntregues = 0;
            missaoPocoes.questCompleta = false;
        }

        PlayerPrefs.DeleteKey("MissaoAtiva");
        PlayerPrefs.DeleteKey("PocoesEntregues");
        PlayerPrefs.DeleteKey("QuestCompleta");
        PlayerPrefs.Save();

        // 🔥 ALTERADO: Agora mostra 2 poções
        Debug.Log($"🔄 MISSÃO RESETADA! Progresso: 0/{pocoesNecessarias} poções");
        OnQuestUpdated?.Invoke();
    }

    [ContextMenu("DEBUG - Completar Missão Agora")]
    public void DebugCompletarMissaoAgora()
    {
        if (missaoPocoes != null && !missaoPocoes.questCompleta)
        {
            // 🔥 FORÇA COMPLETAR COM 2 POÇÕES
            missaoPocoes.pocoesEntregues = 2;
            CompletarMissao();
            Debug.Log("🔧 DEBUG: Missão completada com 2/2 poções!");
        }
        else if (missaoPocoes?.questCompleta == true)
        {
            Debug.Log("✅ Missão já está completa!");
        }
        else
        {
            Debug.Log("❌ Missão não inicializada!");
        }
    }

    [ContextMenu("DEBUG - Ver Status")]
    public void DebugVerStatus()
    {
        Debug.Log($"🔍 STATUS MISSÃO:");
        Debug.Log($"- Ativa: {missaoAtiva}");
        // 🔥 ALTERADO: Agora mostra 2 poções
        Debug.Log($"- Poções: {missaoPocoes?.pocoesEntregues ?? 0}/{pocoesNecessarias}");
        Debug.Log($"- Completa: {missaoPocoes?.questCompleta ?? false}");
        Debug.Log($"- Salvo: MissaoAtiva={PlayerPrefs.GetInt("MissaoAtiva", 0)}, PocoesEntregues={PlayerPrefs.GetInt("PocoesEntregues", 0)}");

        if (missaoPocoes?.pocoesRequeridas != null)
        {
            Debug.Log($"- Poções únicas: {missaoPocoes.pocoesRequeridas.Count}");
            foreach (var pocao in missaoPocoes.pocoesRequeridas)
            {
                Debug.Log($"  - {pocao?.recipeName ?? "NULL"}");
            }
        }
    }

    public bool IsMissaoCompleta()
    {
        return missaoPocoes?.questCompleta ?? false;
    }

    public int GetProgressoAtual()
    {
        return missaoPocoes?.pocoesEntregues ?? 0;
    }

    public int GetProgressoNecessario()
    {
        // 🔥 ALTERADO: Agora retorna 2
        return pocoesNecessarias;
    }
}