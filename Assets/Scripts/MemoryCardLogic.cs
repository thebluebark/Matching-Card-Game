using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryCardLogic : MonoBehaviour
{
    //Hold a list for our cards that will be the buttons in our grid
    public List<Button> cards = new List<Button>();

    //Hold a list for our memory sprite pairs
    public List<Sprite> memories = new List<Sprite>();

    //Hold the deafult image for our cards when they are unflipped
    [SerializeField]
    private Sprite unflippedImage;

    //Hold the sprites of our memory images
    public Sprite[] memoryImages;

    //Text objects to adjust
    [SerializeField]
    private TMP_Text guessesText;
    [SerializeField]
    private TMP_Text matchesText;

    //Gameplay logic variables
    private bool hasFlippedFirst, hasFlippedSecond;     //Keeping track of flips per guess
    private int guessAmount, correctAmount;             //Keeping track of guess amounts with correct guesses
    private int numPairs;                               //Hold the amount of pairs possible
    private int firstIndex, secondIndex;                //Which guess corresponds to which index in the card set
    private string firstName, secondName;               //Names of the sprites to compare with


    // Start is called before the first frame update
    void Start()
    {
        //Add all of our memory images from our sprites folder to the sprite array
        memoryImages = Resources.LoadAll<Sprite>("Sprites/MemoryImages");

        FindButtons();
        AddListeners();
        CreateSpritePairs();

        //Variable initializations
        numPairs = cards.Count / 2;
        guessAmount = correctAmount = 0;

        //Reset our saved score
        PlayerPrefs.SetInt("guesses", guessAmount);
        PlayerPrefs.SetInt("matches", correctAmount);

        //Update our textboxes
        UpdateTextBoxes();
    }

    //Function to call when we need to find all the cards in our grid
    void FindButtons()
    {
        //Use the "card" tag of our buttons to find all of the buttons in the grid
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Card");

        //Now we go through each of these cards and add them to our card list
        for(int i = 0; i < buttons.Length; i++)
        {
            //We get the button component of each card in the grid and add it to our list of cards
            cards.Add(buttons[i].GetComponent<Button>());

            //We can also set the default unflipped image as well
            cards[i].image.sprite = unflippedImage;
        }
    }

    //Function to call when we want to add the listeners to our cards
    void AddListeners()
    {
        //Go through each of our cards in the card list
        foreach(Button card in cards)
        {
            //When clicking each card, make them call these functions
            card.onClick.AddListener(() => OnClickCard());
        }
    }

    //Function to create two sets of our sprites within our memory list
    void CreateSpritePairs()
    {
        //Hold how many cards we currently have
        int cardCount = cards.Count;
        //Hold an index for the current sprite in our sprite array
        int spriteIndex = 0;

        //Go through our card count to add two sets of sprites in our memory list
        for(int i = 0; i < cardCount; i++)
        {
            //When we reach half the cards of our grid, we want to reset the sprite index
            //That way we can ensure that we have two sets of sprites in pairs
            if (spriteIndex == cardCount / 2) spriteIndex = 0;

            //Add the sprite to our memory list
            memories.Add(memoryImages[spriteIndex]);

            //Increase our sprite index
            spriteIndex++;
        }

        //Now we need to shuffle our pairs in order to create new games everytime
        ShuffleSpriteList(memories);
    }

    //Function to shuffle elements in a list of type Sprite
    void ShuffleSpriteList(List<Sprite> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            //Create a temporary copy of the current index
            Sprite tempSprite = list[i];

            //Get a random index between the current index and the elements in a list
            int randomIndex = Random.Range(i, list.Count);

            //Place the sprite from this random index and place it into the sprite of the current iteration index
            list[i] = list[randomIndex];

            //Now we can place our temporary copy into the random index of the list
            list[randomIndex] = tempSprite;
        }
    }

    //Function to call when you are clicking a card to deal with matching logic
    public void OnClickCard()
    {
        //Get the name and index of the currently selected card that the user has clicked
        string currentName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        string currentIndex = currentName.Remove(0, 12);

        //Check if this is either the first or second flip of the guess
        if (!hasFlippedFirst)
        {
            //Now we have flipped for the first time
            hasFlippedFirst = true;

            //Get the index for our guess
            firstIndex = int.Parse(currentIndex) - 1;

            //Get the name of the sprite
            firstName = memories[firstIndex].name;

            //Set the sprite for the current card we clicked
            cards[firstIndex].image.sprite = memories[firstIndex];

        } else if (!hasFlippedSecond && (int.Parse(currentIndex)-1 != firstIndex))
        {
            //We also check if the second guess isn't just the first card again because you can still click on it
            //I've tried just disabling the card but it makes it turn grey after clicking it.

            //Now we have flipped for the first time
            hasFlippedSecond = true;

            //Get the index for our guess
            secondIndex = int.Parse(currentIndex) - 1;

            //Get the name of the sprite
            secondName = memories[secondIndex].name;

            //Set the sprite for the current card we clicked
            cards[secondIndex].image.sprite = memories[secondIndex];

            //Increment our amount of guesses
            guessAmount++;
            //Save the guess score
            PlayerPrefs.SetInt("guesses", guessAmount);
            //Update textboxes
            UpdateTextBoxes();

            //Check our matches with our match checker coroutine
            StartCoroutine(CheckForMatch());
        }
    }

    //Allow a coroutine to check our matches [And also give a delay]
    IEnumerator CheckForMatch()
    {
        //Add a delay so the player can view their guesses
        yield return new WaitForSeconds(0.70f);

        //Now to compare our guesses
        if (firstName == secondName)
        {
            //We have a match!

            //First, let's disable our guesses so we won't click them again
            cards[firstIndex].interactable = false;
            cards[secondIndex].interactable = false;
            //Then, make them slightly invisible
            cards[firstIndex].image.color = new Color(255, 255, 255, 137);
            cards[secondIndex].image.color = new Color(255, 255, 255, 137);

            //Now check for our win condition
            WinConditionCheck();
        }
        else
        {
            //These do not match...

            //Let's return these images back to their flipped side
            cards[firstIndex].image.sprite = unflippedImage;
            cards[secondIndex].image.sprite = unflippedImage;
        }

        //Reset our flipped booleans
        hasFlippedFirst = hasFlippedSecond = false;
    }

    //Function to check for our win condition, when all memories have been matched
    void WinConditionCheck()
    {
        //Increment our correct guess amount
        correctAmount++;
        //Save our correct guesses
        PlayerPrefs.SetInt("matches", correctAmount);
        UpdateTextBoxes();

        //Now check if the amount of correct guesses match the amount of pairs possible
        if (correctAmount == numPairs)
        {
            //If so, we won the game!
            SceneManager.LoadScene("GameOver");
        }
    }

    //Function to update our text boxes
    void UpdateTextBoxes()
    {
        guessesText.text = "Guesses: " + guessAmount;
        matchesText.text = "Matches: " + correctAmount;
    }
}
