using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BotaoVenderItem : MonoBehaviour
{
    [System.Serializable]
    public class ItemVenda
    {
        public ItemData item;
        public int precoPorUnidade = 3;
    }

    [Header("Configuração")]
    [SerializeField] private List<ItemVenda> itensAVenda = new List<ItemVenda>();

    [Header("UI - TextMeshPro")]
    public TextMeshProUGUI textoStatusTMP; // Esta é a propriedade que estava faltando

    // Método público para configurar os itens dinamicamente
    public void ConfigurarItensVenda(ItemData[] itens, int precoPadrao = 3)
    {
        itensAVenda.Clear();
        foreach (ItemData item in itens)
        {
            if (item != null)
            {
                itensAVenda.Add(new ItemVenda
                {
                    item = item,
                    precoPorUnidade = precoPadrao
                });
            }
        }
    }

    public void VenderTodos()
    {
        Debug.Log("VenderTodos chamado!");

        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance está nulo!");
            return;
        }
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData.Instance está nulo!");
            return;
        }

        bool vendeuAlgo = false;
        string mensagemFinal = "";
        int totalMoedasGanhas = 0;

        foreach (ItemVenda itemVenda in itensAVenda)
        {
            if (itemVenda == null || itemVenda.item == null)
                continue;

            int quantidade = InventorySystem.Instance.GetItemCount(itemVenda.item);
            Debug.Log($"Tentando vender {itemVenda.item.itemName}, quantidade no inventário: {quantidade}");

            if (quantidade > 0)
            {
                int totalValor = quantidade * itemVenda.precoPorUnidade;

                bool removido = InventorySystem.Instance.RemoveItem(itemVenda.item, quantidade);
                Debug.Log($"Removido do inventário? {removido}");

                if (removido)
                {
                    GameData.Instance.AddCoins(totalValor);
                    totalMoedasGanhas += totalValor;

                    mensagemFinal += $"✓ Vendeu <color=green>{quantidade}x</color> {itemVenda.item.itemName} por <color=yellow>{totalValor} moedas</color>\n";
                    vendeuAlgo = true;
                }
            }
        }

        // Atualizar TextMeshPro
        if (textoStatusTMP != null)
        {
            if (vendeuAlgo)
            {
                textoStatusTMP.text = mensagemFinal + $"\n<size=120%><color=yellow>Total ganho: {totalMoedasGanhas} moedas!</color></size>";
                textoStatusTMP.alignment = TextAlignmentOptions.Left;
            }
            else
            {
                textoStatusTMP.text = "<color=red>Você não tem nenhum dos itens para vender.</color>";
                textoStatusTMP.alignment = TextAlignmentOptions.Center;
            }
        }

        // Forçar atualização da UI
        if (vendeuAlgo)
        {
            Invoke("AtualizarUIExterna", 0.1f);
        }
    }

    private void AtualizarUIExterna()
    {
        // Chamar método de atualização em outros scripts se necessário
        NPCVendaFlores npc = GetComponent<NPCVendaFlores>();
        if (npc != null)
        {
            npc.Invoke("AtualizarUI", 0.1f);
        }
    }

    // Método para obter informações sobre os itens disponíveis para venda
    public string ObterInfoItensVenda()
    {
        string info = "<color=green>Itens que posso comprar:</color>\n";
        foreach (ItemVenda itemVenda in itensAVenda)
        {
            if (itemVenda != null && itemVenda.item != null)
            {
                info += $"- {itemVenda.item.itemName}: <color=yellow>{itemVenda.precoPorUnidade} moedas/unidade</color>\n";
            }
        }
        return info;
    }
}