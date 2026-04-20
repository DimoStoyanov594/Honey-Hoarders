using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [Header("Data")]
    public CharacterDatabase characterDB;

    [Header("Visual References")]
    public Transform visualRoot;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    void Start()
    {
        int selectedOption = PlayerPrefs.GetInt("SelectedCharacter", 0);
        Character character = characterDB.GetCharacter(selectedOption);

        if (character == null)
        {
            return;
        }

        if (spriteRenderer != null)
            spriteRenderer.sprite = character.characterSprite;

        if (animator != null && character.animatorController != null)
            animator.runtimeAnimatorController = character.animatorController;

        if (visualRoot != null)
            visualRoot.localScale = character.scale;
    }
}