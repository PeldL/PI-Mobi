using UnityEngine;

public class DayNightActions : MonoBehaviour
{
    public GameObject[] objetosNoite;
    public DayNightCycle dayNightCycle;

    private bool[] estadoSalvo;

    void Start()
    {
        if (PlayerPrefs.HasKey("ObjetosNoite_count"))
        {
            int[] ativos = PlayerPrefsX.GetIntArray("ObjetosNoite");
            estadoSalvo = new bool[ativos.Length];

            for (int i = 0; i < ativos.Length && i < objetosNoite.Length; i++)
            {
                bool ativo = ativos[i] == 1;
                objetosNoite[i].SetActive(ativo);
                estadoSalvo[i] = ativo;
            }
        }
        else
        {
            estadoSalvo = new bool[objetosNoite.Length];
        }
    }

    void Update()
    {
        if (dayNightCycle == null) return;

        float hora = dayNightCycle.currentTime;
        bool deveEstarAtivo = hora >= 18f || hora < 6f;

        for (int i = 0; i < objetosNoite.Length; i++)
        {
            if (objetosNoite[i] != null)
            {
                bool ativo = objetosNoite[i].activeSelf;
                if (ativo != deveEstarAtivo)
                {
                    objetosNoite[i].SetActive(deveEstarAtivo);
                    estadoSalvo[i] = deveEstarAtivo;
                }
            }
        }

        int[] salvar = new int[estadoSalvo.Length];
        for (int i = 0; i < salvar.Length; i++)
        {
            salvar[i] = estadoSalvo[i] ? 1 : 0;
        }
        PlayerPrefsX.SetIntArray("ObjetosNoite", salvar);
    }
}