using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public CharacterDatabase characterDB;

    public TMP_Text nameText;
    public SpriteRenderer artworkSprite;
    public GameObject equippedBadge;
    public GameObject equippedBadgeImage;

    private int selectedOption = 0;

    void Start()
    {
        selectedOption = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateCharacter(selectedOption);
    }

    public void NextOption()
    {
        selectedOption++;
        if (selectedOption >= characterDB.CharacterCount) selectedOption = 0;
        UpdateCharacter(selectedOption);
    }

    public void BackOption()
    {
        selectedOption--;
        if (selectedOption < 0) selectedOption = characterDB.CharacterCount - 1;
        UpdateCharacter(selectedOption);
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = characterDB.GetCharacter(selectedOption);

        artworkSprite.sprite = character.characterSprite;
        nameText.text = character.characterName;

        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        bool isEquipped = selectedOption == savedIndex;

        if (equippedBadge != null)
            equippedBadge.SetActive(isEquipped);

        if (equippedBadgeImage != null)
            equippedBadgeImage.SetActive(isEquipped);
    }

    public void EquipSelection()
    {
        PlayerPrefs.SetInt("SelectedCharacter", selectedOption);
        PlayerPrefs.Save();
        UpdateCharacter(selectedOption);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game-Scene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }
}