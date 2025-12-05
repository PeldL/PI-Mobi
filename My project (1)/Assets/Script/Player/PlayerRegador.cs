using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerRegador : MonoBehaviour
{
    public Animator animator;
    private CharacterController2D playerController;

    [Header("Desbloqueios")]
    public bool machadoDesbloqueado = false;
    public bool regadorDesbloqueado = false;

    [Header("Regador Config")]
    public float alcanceRegador = 1.5f;
    public LayerMask camadaAlvoRegador;
    public float tempoDesaparecimento = 0.5f;
    public int usosMaximos = 3;
    private int usosAtuais = 0;

    [Header("Gizmos")]
    public bool mostrarAlcance = true;

    [Header("Constantes")]
    private const string MACHADO_PREF_KEY = "MachadoDesbloqueado";
    private const string REGADOR_PREF_KEY = "RegadorDesbloqueado";
    private const string REGADOR_USOS_KEY = "RegadorUsos";
    private const string OBJETOS_DESTRUÍDOS_KEY = "ObjetosRegador";

    [Header("Referências UI")]
    public UnityEngine.UI.Text usosText; // Opcional: mostrar usos na UI

    void Start()
    {
        playerController = GetComponent<CharacterController2D>();
        machadoDesbloqueado = PlayerPrefs.GetInt(MACHADO_PREF_KEY, 0) == 1;
        regadorDesbloqueado = PlayerPrefs.GetInt(REGADOR_PREF_KEY, 0) == 1;
        usosAtuais = PlayerPrefs.GetInt(REGADOR_USOS_KEY, usosMaximos);

        Debug.Log($"🪓 Machado: {machadoDesbloqueado} | 💧 Regador: {regadorDesbloqueado} | Usos: {usosAtuais}/{usosMaximos}");

        // Destruir objetos que já foram regados
        string[] destruídos = PlayerPrefsX.GetStringArray(OBJETOS_DESTRUÍDOS_KEY);
        foreach (string id in destruídos)
        {
            GameObject obj = GameObject.Find(id);
            if (obj != null) Destroy(obj);
        }

        AtualizarUI();
    }

    void Update()
    {
        if (machadoDesbloqueado && Input.GetMouseButtonDown(0))
        {
            UsarMachado();
        }

        if (regadorDesbloqueado && Input.GetMouseButtonDown(1) && PodeUsarRegador())
        {
            UsarRegador();
        }
    }

    // MÉTODO PARA BOTÃO MOBILE (OnClick)
    public void UsarRegadorMobile()
    {
        if (regadorDesbloqueado && PodeUsarRegador())
        {
            Debug.Log("📱 Regador usado via botão mobile!");
            UsarRegador();
        }
        else if (!regadorDesbloqueado)
        {
            Debug.Log("❌ Regador não desbloqueado!");
        }
        else if (!PodeUsarRegador())
        {
            Debug.Log("💧 Regador vazio! Encha primeiro.");
        }
    }

    public void DesbloquearMachado()
    {
        machadoDesbloqueado = true;
        PlayerPrefs.SetInt(MACHADO_PREF_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("🪓 Machado desbloqueado!");
    }

    public void DesbloquearRegador()
    {
        regadorDesbloqueado = true;
        PlayerPrefs.SetInt(REGADOR_PREF_KEY, 1);
        PlayerPrefs.Save();
        Debug.Log("💧 Regador desbloqueado!");
        AtualizarUI();
    }

    public void EncherRegador()
    {
        usosAtuais = usosMaximos;
        PlayerPrefs.SetInt(REGADOR_USOS_KEY, usosAtuais);
        PlayerPrefs.Save();
        Debug.Log($"🚰 Regador enchido! Usos: {usosAtuais}/{usosMaximos}");
        AtualizarUI();
    }

    public bool RegadorTemAgua()
    {
        return usosAtuais > 0;
    }

    void UsarMachado()
    {
        if (animator != null)
        {
            animator.SetTrigger("usarMachado");
            playerController.SetSpeed(0f);
            Invoke(nameof(VoltarMovimento), 0.6f);
        }

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (Collider2D col in objetos)
        {
            IDestrutivel destrutivel = col.GetComponent<IDestrutivel>();
            if (destrutivel != null)
            {
                destrutivel.Destruir();
            }
        }
    }

    void UsarRegador()
    {
        if (!PodeUsarRegador()) return;

        // Consome um uso
        usosAtuais--;
        PlayerPrefs.SetInt(REGADOR_USOS_KEY, usosAtuais);
        PlayerPrefs.Save();

        if (animator != null)
        {
            animator.SetTrigger("usarRegador");
            playerController.SetSpeed(0f);
            Invoke(nameof(VoltarMovimento), 0.6f);
        }

        // Detecta objetos no alcance
        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, alcanceRegador, camadaAlvoRegador);

        bool regouAlgo = false;
        foreach (Collider2D col in objetos)
        {
            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                StartCoroutine(DesaparecerObjeto(sr.gameObject, sr));
                regouAlgo = true;
            }
        }

        if (regouAlgo)
        {
            Debug.Log($"💧 Regador usado! Restam {usosAtuais} usos");
        }
        else
        {
            Debug.Log("💧 Nada para regar na área");
        }

        AtualizarUI();
    }

    bool PodeUsarRegador()
    {
        return regadorDesbloqueado && usosAtuais > 0;
    }

    void VoltarMovimento()
    {
        playerController.SetSpeed(5f);
    }

    IEnumerator DesaparecerObjeto(GameObject obj, SpriteRenderer sr)
    {
        float tempo = 0f;
        Color corInicial = sr.color;

        while (tempo < tempoDesaparecimento)
        {
            if (sr == null) yield break;

            float alpha = Mathf.Lerp(1f, 0f, tempo / tempoDesaparecimento);
            sr.color = new Color(corInicial.r, corInicial.g, corInicial.b, alpha);
            tempo += Time.deltaTime;
            yield return null;
        }

        if (obj != null)
        {
            // Salvar ID destruído
            string id = obj.name;
            List<string> lista = new List<string>(PlayerPrefsX.GetStringArray(OBJETOS_DESTRUÍDOS_KEY));
            if (!lista.Contains(id))
            {
                lista.Add(id);
                PlayerPrefsX.SetStringArray(OBJETOS_DESTRUÍDOS_KEY, lista.ToArray());
            }

            Destroy(obj);
            Debug.Log("🌱 Objeto sumiu com regador e foi salvo!");
        }
    }

    void AtualizarUI()
    {
        if (usosText != null)
        {
            if (regadorDesbloqueado)
            {
                usosText.text = $"💧 {usosAtuais}/{usosMaximos}";
                usosText.color = usosAtuais > 0 ? Color.white : Color.red;
            }
            else
            {
                usosText.text = "💧 Bloqueado";
                usosText.color = Color.gray;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (mostrarAlcance)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, alcanceRegador);
        }
    }
}