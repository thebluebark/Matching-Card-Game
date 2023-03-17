using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtons : MonoBehaviour
{
    //Hold the grid to the memory cards
    [SerializeField]
    private GameObject cardGrid;

    //Hold the card prefab as a gameobject for reference
    [SerializeField]
    private GameObject memoryCard;

    //How many cards do we want to spawn?
    public int numberOfCards;

    //Awake calls before any other Starts from other scripts
    void Awake()
    {
        //Loop to create our buttons
        for(int i = 1; i <= numberOfCards; i++)
        {
            //Create clones of our card prefab
            GameObject cloneCard = Instantiate(memoryCard);

            //Give this card a name that will distinguish itself
            cloneCard.name = "Memory Card " + i;

            //Set the parent of this card to be the grid, while also being relative to the grid
            cloneCard.transform.SetParent(cardGrid.transform, false);
        }
    }
}
