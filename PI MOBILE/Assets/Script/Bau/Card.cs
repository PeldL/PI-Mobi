using UnityEngine;

[System.Serializable]
public class Card
{
    public string cardName;
    public Sprite cardImage;
    public string description;

    public Card(string name, Sprite image, string desc)
    {
        cardName = name;
        cardImage = image;
        description = desc;
    }
}