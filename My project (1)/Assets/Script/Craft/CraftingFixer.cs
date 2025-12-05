using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingFixer : MonoBehaviour
{
    [Header("Referências OBRIGATÓRIAS")]
    public CraftingRecipe receitaMachado;
    public ItemData madeira;
    public ItemData pedra;

    [Header("Debug Visual")]
    public bool mostrarDebug = true;

    private void Start()
    {
        Debug.Log("🛠 INICIANDO CRAFTING FIXER...");
        Invoke("ConfigurarSistemaCompleto", 0.5f);
    }

    private void Update()
    {
        // Teste rápido com teclas
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TesteCompleto();
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            VerificarStatus();
        }
    }

    void ConfigurarSistemaCompleto()
    {
        Debug.Log("🔧 CONFIGURANDO SISTEMA COMPLETO...");

        // 1. Garantir que tem itens no inventário
        AdicionarMateriais();

        // 2. Garantir que receita está desbloqueada
        DesbloquearReceitas();

        // 3. Forçar atualização da UI
        AtualizarUI();

        Debug.Log("✅ SISTEMA CONFIGURADO! Pressione F5 para teste completo");
    }

    void AdicionarMateriais()
    {
        if (InventorySystem.Instance != null)
        {
            // Adiciona materiais generosos para testar
            if (madeira != null)
            {
                InventorySystem.Instance.AddItem(madeira, 10);
                Debug.Log($"📦 Adicionado 10x {madeira.itemName}");
            }

            if (pedra != null)
            {
                InventorySystem.Instance.AddItem(pedra, 8);
                Debug.Log($"📦 Adicionado 8x {pedra.itemName}");
            }
        }
    }

    void DesbloquearReceitas()
    {
        if (CraftingSystem.Instance != null && receitaMachado != null)
        {
            if (!receitaMachado.isUnlocked)
            {
                receitaMachado.isUnlocked = true;
                CraftingSystem.Instance.OnRecipesUpdated?.Invoke();
                Debug.Log($"🔓 Receita desbloqueada: {receitaMachado.recipeName}");
            }
        }
    }

    void AtualizarUI()
    {
        if (CraftingUI.Instance != null)
        {
            CraftingUI.Instance.UpdateUI();
            Debug.Log("🔄 UI atualizada forçadamente");
        }
    }

    void TesteCompleto()
    {
        Debug.Log("🎯 INICIANDO TESTE COMPLETO DO CRAFTING");

        // 1. Verificar sistemas
        VerificarSistemas();

        // 2. Verificar inventário
        VerificarInventario();

        // 3. Verificar receitas
        VerificarReceitas();

        // 4. Testar craft
        TestarCraft();

        Debug.Log("🎯 TESTE COMPLETO FINALIZADO!");
    }

    void VerificarSistemas()
    {
        Debug.Log("=== 🔍 VERIFICAÇÃO DE SISTEMAS ===");
        Debug.Log($"InventorySystem: {InventorySystem.Instance != null}");
        Debug.Log($"CraftingSystem: {CraftingSystem.Instance != null}");
        Debug.Log($"CraftingUI: {CraftingUI.Instance != null}");
        Debug.Log($"GameData: {GameData.Instance != null}");
    }

    void VerificarInventario()
    {
        if (InventorySystem.Instance != null)
        {
            Debug.Log("=== 📦 VERIFICAÇÃO DE INVENTÁRIO ===");

            if (madeira != null)
            {
                int count = InventorySystem.Instance.GetItemCount(madeira);
                Debug.Log($"📊 {madeira.itemName}: {count} unidades");
            }

            if (pedra != null)
            {
                int count = InventorySystem.Instance.GetItemCount(pedra);
                Debug.Log($"📊 {pedra.itemName}: {count} unidades");
            }
        }
    }

    void VerificarReceitas()
    {
        if (CraftingSystem.Instance != null)
        {
            Debug.Log("=== 📋 VERIFICAÇÃO DE RECEITAS ===");

            foreach (var recipe in CraftingSystem.Instance.allRecipes)
            {
                if (recipe != null)
                {
                    bool podeCraftar = CraftingSystem.Instance.CanCraft(recipe);
                    Debug.Log($"🔧 {recipe.recipeName}: " +
                             $"Desbloqueada={recipe.isUnlocked} | " +
                             $"PodeCraftar={podeCraftar}");

                    // Mostrar materiais necessários
                    foreach (var material in recipe.materials)
                    {
                        if (material.item != null)
                        {
                            int noInventario = InventorySystem.Instance.GetItemCount(material.item);
                            Debug.Log($"   - {material.item.itemName}: {noInventario}/{material.amount} " +
                                     $"{(noInventario >= material.amount ? "✅" : "❌")}");
                        }
                    }
                }
            }
        }
    }

    void TestarCraft()
    {
        if (CraftingSystem.Instance != null && receitaMachado != null)
        {
            Debug.Log("=== 🛠 TESTANDO CRAFT ===");

            bool sucesso = CraftingSystem.Instance.CraftItem(receitaMachado);

            if (sucesso)
            {
                Debug.Log($"✅ CRAFT BEM-SUCEDIDO: {receitaMachado.resultAmount}x {receitaMachado.resultItem.itemName}");

                // Verificar se item foi adicionado
                int countResultado = InventorySystem.Instance.GetItemCount(receitaMachado.resultItem);
                Debug.Log($"📦 {receitaMachado.resultItem.itemName} no inventário: {countResultado}");
            }
            else
            {
                Debug.Log("❌ FALHA NO CRAFT - Verifique materiais ou se receita está desbloqueada");
            }
        }
    }

    void VerificarStatus()
    {
        Debug.Log("=== 📊 STATUS RÁPIDO ===");

        // Verificar UI
        if (CraftingUI.Instance != null && CraftingUI.Instance.craftingPanel != null)
        {
            Debug.Log($"🖥 Crafting UI: {(CraftingUI.Instance.craftingPanel.activeInHierarchy ? "ABERTO" : "FECHADO")}");
        }

        // Verificar uma receita específica
        if (receitaMachado != null)
        {
            bool podeCraftar = CraftingSystem.Instance.CanCraft(receitaMachado);
            Debug.Log($"🔧 {receitaMachado.recipeName}: Pode craftar = {podeCraftar}");
        }
    }

    // Método para forçar aparecer itens na UI
    [ContextMenu("🔧 Forçar Configuração Completa")]
    public void ForcarConfiguracao()
    {
        ConfigurarSistemaCompleto();
    }

    [ContextMenu("🎯 Executar Teste Completo")]
    public void ExecutarTesteCompleto()
    {
        TesteCompleto();
    }
}