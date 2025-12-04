using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CraftingButtons : MonoBehaviour
{
    [Header("🎯 BOTÕES DE RECEITAS")]
    public List<CraftingRecipe> receitasDisponiveis;
    public List<Button> botoesReceitas;

    [Header("📦 MATERIAIS - POSIÇÕES FIXAS")]
    public List<GameObject> posicoesMateriais;
    public GameObject slotMaterialPrefab;

    [Header("📝 DETALHES DA RECEITA")]
    public TextMeshProUGUI textoNomeReceita;

    [Header("🔨 BOTÃO DE CRAFTAR")]
    public Button botaoCraftar;
    public TextMeshProUGUI textoBotaoCraftar;

    [Header("🎮 CONFIGURAÇÕES")]
    public bool pausarJogo = true;

    private CraftingRecipe receitaSelecionada;
    private List<GameObject> slotsMateriaisAtuais = new List<GameObject>();

    void Start()
    {
        Debug.Log("🎯 CraftingButtons Iniciado!");

        ConfigurarBotoes();

        if (botaoCraftar != null)
        {
            botaoCraftar.onClick.AddListener(CraftarReceita);
        }

        LimparDetalhes();
    }

    void ConfigurarBotoes()
    {
        Debug.Log($"🔧 Configurando {botoesReceitas.Count} botões para {receitasDisponiveis.Count} receitas");

        for (int i = 0; i < botoesReceitas.Count; i++)
        {
            Button botao = botoesReceitas[i];
            int index = i;

            if (botao != null)
            {
                botao.onClick.RemoveAllListeners();
                botao.onClick.AddListener(() => SelecionarReceita(index));

                if (i < receitasDisponiveis.Count && receitasDisponiveis[i] != null)
                {
                    botao.interactable = true;

                    TextMeshProUGUI textoBotao = botao.GetComponentInChildren<TextMeshProUGUI>();
                    if (textoBotao != null)
                    {
                        textoBotao.text = receitasDisponiveis[i].recipeName;
                    }

                    Debug.Log($"✅ Botão {i} configurado: {receitasDisponiveis[i].recipeName}");
                }
                else
                {
                    botao.interactable = false;
                    Debug.Log($"❌ Botão {i} desativado - Sem receita");
                }
            }
        }
    }

    void SelecionarReceita(int indexReceita)
    {
        if (indexReceita < receitasDisponiveis.Count && receitasDisponiveis[indexReceita] != null)
        {
            receitaSelecionada = receitasDisponiveis[indexReceita];
            Debug.Log($"🎯 Receita selecionada: {receitaSelecionada.recipeName}");

            MostrarDetalhesReceita(receitaSelecionada);
            AtualizarBotoesSelecao(indexReceita);
        }
    }

    void AtualizarBotoesSelecao(int indexSelecionado)
    {
        for (int i = 0; i < botoesReceitas.Count; i++)
        {
            Button botao = botoesReceitas[i];
            if (botao != null)
            {
                var cores = botao.colors;
                if (i == indexSelecionado)
                {
                    cores.normalColor = Color.green;
                    Debug.Log($"🟢 Botão {i} SELECIONADO");
                }
                else
                {
                    cores.normalColor = Color.white;
                }
                botao.colors = cores;
            }
        }
    }

    void MostrarDetalhesReceita(CraftingRecipe receita)
    {
        if (receita == null) return;

        Debug.Log($"📖 Mostrando detalhes: {receita.recipeName}");

        // ✅ AGORA MOSTRA O NOME DA RECEITA NO TEXTO PRINCIPAL
        if (textoNomeReceita != null)
            textoNomeReceita.text = receita.recipeName;

        MostrarMateriais(receita);
        AtualizarBotaoCraftar(receita);
    }

    void MostrarMateriais(CraftingRecipe receita)
    {
        LimparMateriais();

        if (slotMaterialPrefab == null || posicoesMateriais.Count == 0)
        {
            Debug.LogError("❌ Prefab ou posições de materiais não configurados!");
            return;
        }

        Debug.Log($"📦 Mostrando {receita.materials.Length} materiais");

        for (int i = 0; i < receita.materials.Length; i++)
        {
            if (i < posicoesMateriais.Count && posicoesMateriais[i] != null)
            {
                RecipeMaterial material = receita.materials[i];

                if (material.item != null)
                {
                    GameObject slotMaterial = Instantiate(slotMaterialPrefab, posicoesMateriais[i].transform);
                    slotMaterial.transform.position = posicoesMateriais[i].transform.position;

                    MaterialSlotUI slotUI = slotMaterial.GetComponent<MaterialSlotUI>();
                    if (slotUI != null)
                    {
                        slotUI.Initialize(material, i, receita.materials.Length);
                        Debug.Log($"✅ Material {i}: {material.item.itemName} x{material.amount}");
                    }

                    slotsMateriaisAtuais.Add(slotMaterial);
                }
            }
        }
    }

    void AtualizarBotaoCraftar(CraftingRecipe receita)
    {
        if (botaoCraftar == null || textoBotaoCraftar == null) return;

        bool podeCraftar = VerificarSePodeCraftar(receita);

        botaoCraftar.interactable = podeCraftar;

        // ✅✅✅ CORREÇÃO: O TEXTO DO BOTÃO AGORA É SEMPRE O NOME DA RECEITA
        if (podeCraftar)
        {
            textoBotaoCraftar.text = $"CRAFTAR {receita.recipeName.ToUpper()}!";
            Debug.Log($"🎯 Pode craftar: {receita.recipeName}");
        }
        else
        {
            textoBotaoCraftar.text = $"MATERIAIS INSUFICIENTES PARA {receita.recipeName.ToUpper()}";
            Debug.Log($"❌ Não pode craftar: {receita.recipeName}");
        }
    }

    bool VerificarSePodeCraftar(CraftingRecipe receita)
    {
        if (receita == null) return false;

        if (CraftingSystem.Instance == null) return false;

        foreach (RecipeMaterial material in receita.materials)
        {
            if (!InventorySystem.Instance.HasItem(material.item, material.amount))
            {
                Debug.Log($"❌ Falta: {material.item.itemName} ({InventorySystem.Instance.GetItemCount(material.item)}/{material.amount})");
                return false;
            }
        }
        return true;
    }

    void CraftarReceita()
    {
        if (receitaSelecionada == null)
        {
            Debug.LogError("❌ Nenhuma receita selecionada para craftar!");
            return;
        }

        Debug.Log($"🔨 Tentando craftar: {receitaSelecionada.recipeName}");

        if (CraftingSystem.Instance != null)
        {
            bool sucesso = CraftingSystem.Instance.CraftItem(receitaSelecionada);

            if (sucesso)
            {
                Debug.Log($"✅ Craftado com sucesso: {receitaSelecionada.recipeName}");
                AtualizarBotaoCraftar(receitaSelecionada);
            }
            else
            {
                Debug.LogError($"❌ Falha ao craftar: {receitaSelecionada.recipeName}");
            }
        }
        else
        {
            Debug.LogError("❌ CraftingSystem não disponível!");
        }
    }

    void LimparMateriais()
    {
        foreach (GameObject slot in slotsMateriaisAtuais)
        {
            if (slot != null) Destroy(slot);
        }
        slotsMateriaisAtuais.Clear();
        Debug.Log("🧹 Materiais limpos");
    }

    void LimparDetalhes()
    {
        LimparMateriais();

        // ✅ QUANDO NÃO TEM RECEITA SELECIONADA, MOSTRA "SELECIONE UMA RECEITA"
        if (textoNomeReceita != null)
            textoNomeReceita.text = "Selecione uma Receita";

        if (botaoCraftar != null) botaoCraftar.interactable = false;

        // ✅ QUANDO NÃO TEM RECEITA SELECIONADA, BOTÃO FICA "SELECIONE UMA RECEITA"
        if (textoBotaoCraftar != null)
            textoBotaoCraftar.text = "Selecione uma Receita";

        Debug.Log("🧹 Detalhes limpos");
    }

    public void AbrirCrafting()
    {
        gameObject.SetActive(true);

        if (pausarJogo)
        {
            Time.timeScale = 0f;
            Debug.Log("⏸ Crafting aberto - Jogo pausado");
        }

        ConfigurarBotoes();
        LimparDetalhes();

        Debug.Log("🎯 Crafting Manual Aberto!");
    }

    public void FecharCrafting()
    {
        gameObject.SetActive(false);

        if (pausarJogo && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
            Debug.Log("⏩ Crafting fechado - Jogo despausado");
        }

        Debug.Log("🎯 Crafting Manual Fechado!");
    }

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            FecharCrafting();
        }
    }
}