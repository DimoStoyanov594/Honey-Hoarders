using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string characterName;
    public Sprite characterSprite;
    public RuntimeAnimatorController animatorController;
    public Vector3 scale = new Vector3(1, 1, 1);
}