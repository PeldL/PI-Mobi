using UnityEngine;

public class ObjetoQuebravel : MonoBehaviour, IDestrutivel
{
    [Header("ID único para salvar entre cenas")]
    public string objetoID = "objeto_quebrado_001";

    [Header("Efeitos visuais e sonoros")]
    public AudioClip somQuebrar;      
    public GameObject efeitoQuebrar;   

    void Start()
    {
      
        if (PlayerPrefs.GetInt(objetoID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    public void Destruir()
    {
        Debug.Log($"💥 {gameObject.name} foi destruído pelo machado!");

       
        if (somQuebrar != null)
        {
            AudioSource.PlayClipAtPoint(somQuebrar, transform.position);
        }

        
        if (efeitoQuebrar != null)
        {
            Instantiate(efeitoQuebrar, transform.position, Quaternion.identity);
        }

        
        PlayerPrefs.SetInt(objetoID, 1);
        PlayerPrefs.Save();

       
        Destroy(gameObject);
    }
}
