using UnityEngine;
using System.Linq;
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

    [Header("Gizmos")]
    public bool mostrarAlcance = true;

    [Header("Constantes")]
    private const string MACHADO_PREF_KEY = "MachadoDesbloqueado";
    private const string REGADOR_PREF_KEY = "RegadorDesbloqueado";
    private const string ANIM_MACHADO = "AnimMachado";
    private const string ANIM_REGADOR = "AnimRegador";
    private const string OBJETOS_DESTRUÍDOS_KEY = "ObjetosRegador";

    void Start()
    {
        playerController = GetComponent<CharacterController2D>();
        machadoDesbloqueado = PlayerPrefs.GetInt(MACHADO_PREF_KEY, 0) == 1;
        regadorDesbloqueado = PlayerPrefs.GetInt(REGADOR_PREF_KEY, 0) == 1;

        Debug.Log($"🪓 Machado desbloqueado? {machadoDesbloqueado} | 💧 Regador desbloqueado? {regadorDesbloqueado}");

        // Destruir objetos que já foram regados
        string[] destruídos = PlayerPrefsX.GetStringArray(OBJETOS_DESTRUÍDOS_KEY);
        foreach (string id in destruídos)
        {
            GameObject obj = GameObject.Find(id);
            if (obj != null) Destroy(obj);
        }
    }

    void Update()
    {
        if (machadoDesbloqueado && Input.GetMouseButtonDown(0))
        {
            UsarMachado();
        }

        if (regadorDesbloqueado && Input.GetMouseButtonDown(1))
        {
            UsarRegador();
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
    }

    public void EncherRegador()
    {
        Debug.Log("🚰 Regador enchido!");
    }

    void UsarMachado()
    {
        if (animator != null)
        {
            animator.SetTrigger("usarMachado");
            playerController.SetSpeed(0f);
            float duracao = animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(c => c.name == ANIM_MACHADO)?.length ?? 0.5f;
            Invoke(nameof(VoltarMovimento), duracao);
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
        if (animator != null)
        {
            animator.SetTrigger("usarRegador");
            playerController.SetSpeed(0f);
            float duracao = animator.runtimeAnimatorController.animationClips
                .FirstOrDefault(c => c.name == ANIM_REGADOR)?.length ?? 0.5f;
            Invoke(nameof(VoltarMovimento), duracao);
        }

        Collider2D[] objetos = Physics2D.OverlapCircleAll(transform.position, alcanceRegador, camadaAlvoRegador);
        foreach (Collider2D col in objetos)
        {
            SpriteRenderer sr = col.GetComponent<SpriteRenderer>();
            if (sr != null)
                StartCoroutine(DesaparecerObjeto(sr.gameObject, sr));
        }
    }

    void VoltarMovimento()
    {
        playerController.SetSpeed(5f);
    }

    System.Collections.IEnumerator DesaparecerObjeto(GameObject obj, SpriteRenderer sr)
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
            var lista = PlayerPrefsX.GetStringArray(OBJETOS_DESTRUÍDOS_KEY).ToList();
            if (!lista.Contains(id))
            {
                lista.Add(id);
                PlayerPrefsX.SetStringArray(OBJETOS_DESTRUÍDOS_KEY, lista.ToArray());
            }

            Destroy(obj);
            Debug.Log("🌱 Objeto sumiu com regador e foi salvo!");
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
