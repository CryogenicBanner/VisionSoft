using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public bool enableCombos;
    public float maxComboMultiplier;
    public float comboModifier;
    
    private float currentPoints;
    private float currentCombo;

    // Start is called before the first frame update
    void Start()
    {
        if (enableCombos)
        {
            if (maxComboMultiplier == 0) { maxComboMultiplier = 5; }
            if(comboModifier == 0) { comboModifier = 0.1f; }
        }
        currentPoints = 0;  currentCombo = 0;
        
    }

    public void ScorePoints(int points)
    {
        currentPoints += points * currentCombo;
    }

    public void ResetCombo() { currentCombo = 1; }

    public void ModifyCombo(bool increase)
    {
        if (increase)
        {
            currentCombo = (currentCombo + comboModifier > maxComboMultiplier) ? maxComboMultiplier : currentCombo + comboModifier;           
        }
        else
        {
            currentCombo = (currentCombo - comboModifier < 1) ? 1 : currentCombo - comboModifier;
        }
    }

    public float GetFloatCurrentPoints() { return currentPoints; }
    public int GetIntCurrentPoints() { return ((int)currentPoints); }

    public float GetFloatCurrentCombo() { return currentCombo; }
    public float GetIntCurrentCombo() { return (int)currentCombo; }

}
