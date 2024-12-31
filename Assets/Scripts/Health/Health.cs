using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour {
    private int startingHealth;
    private int currentHealth;
    private bool isImmortal;
    public void SetStartingHealth(int startingHealth) {
        this.startingHealth = startingHealth;
        currentHealth = this.startingHealth;
    }

    public int GetStartingHealth() {
        return startingHealth;
    }

    public void SetImmortal(bool isImmortal) {
        this.isImmortal = isImmortal;
    }
}
