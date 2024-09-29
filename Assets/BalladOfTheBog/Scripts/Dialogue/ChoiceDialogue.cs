using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Choice Dialogue Container")]
public class ChoiceDialogue : ScriptableObject
{
    [TextArea(2,8)]
    public string choiceText;

    public Dialogue nextDialogue;
}
