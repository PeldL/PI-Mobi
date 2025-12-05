using UnityEngine;

public class DayActions : MonoBehaviour
{
    public GameObject[] objetosDia;
    public DayNightCycle dayNightCycle;

    private bool[] estadoSalvo;

    void Start()
    {
        if (PlayerPrefs.HasKey("ObjetosDia_count"))
        {
            int[] ativos = PlayerPrefsX.GetIntArray("ObjetosDia");
            estadoSalvo = new bool[ativos.Length];

            for (int i = 0; i < ativos.Length && i < objetosDia.Length; i++)
            {
                bool ativo = ativos[i] == 1;
                objetosDia[i].SetActive(ativo);
                estadoSalvo[i] = ativo;
            }
        }
        else
        {
            estadoSalvo = new bool[objetosDia.Length];
        }
    }

    void Update()
    {
        if (dayNightCycle == null) return;

        float hora = dayNightCycle.currentTime;
        bool deveEstarAtivo = hora >= 6f && hora < 18f;

        for (int i = 0; i < objetosDia.Length; i++)
        {
            if (objetosDia[i] != null)
            {
                bool ativo = objetosDia[i].activeSelf;
                if (ativo != deveEstarAtivo)
                {
                    objetosDia[i].SetActive(deveEstarAtivo);
                    estadoSalvo[i] = deveEstarAtivo;
                }
            }
        }

        int[] salvar = new int[estadoSalvo.Length];
        for (int i = 0; i < salvar.Length; i++)
        {
            salvar[i] = estadoSalvo[i] ? 1 : 0;
        }
        PlayerPrefsX.SetIntArray("ObjetosDia", salvar);
    }
}