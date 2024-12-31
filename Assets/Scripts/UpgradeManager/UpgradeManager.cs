using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;


public enum UpgradeType {
    PLAYER_HEALTH,
    PLAYER_MOVE_SPEED,
    PLYAER_ROLL_SPEED,
    // TO be added
}

public enum UpgradeOperation {
    SET,
    SET_BASE,
    ADD,
    ADD_BASE,
    MULTIPLY,
    STACKING_MULTIPLY,
}

public class Upgrade {
    public UpgradeType upgradeType;
    public UpgradeOperation upgradeOperation;
    public float value;
    public Weapon targetWeapon = null;
    public string identifier = "";
    public bool isStackable = true;

    public Player targetPlayer = null;
    // Can add more type specific checkings here
}

public class UpgradeManager : MonoBehaviour {
    private List<List<Upgrade>> totalUpgradeList;

    private void Awake() {
        totalUpgradeList = new List<List<Upgrade>>();
        for (int i = 0; i < Enum.GetValues(typeof(UpgradeType)).Length; i++) {
            totalUpgradeList.Add(new List<Upgrade>());
        }
    }

    public void AddUpgrade(Upgrade upgrade) {
        if (!upgrade.isStackable) {
            RemoveUpgrade(upgrade);
        }

        totalUpgradeList[(int)upgrade.upgradeType].Add(upgrade);
    }

    public void RemoveUpgrade(Upgrade upgrade) {
        List<Upgrade> upgradeList = totalUpgradeList[(int)upgrade.upgradeType];
        List<Upgrade> upgradeListToRemove = new List<Upgrade>();
        // copy the upgrade list to avoid concurrent modification
        foreach (Upgrade u in upgradeList) {
            upgradeListToRemove.Add(u);
        }

        if (upgrade.identifier != "") {
            upgradeListToRemove = upgradeList.FindAll(u => u.identifier == upgrade.identifier);
        }

        if (upgrade.targetPlayer != null) {
            upgradeListToRemove = upgradeList.FindAll(u => Equals(upgrade.targetPlayer, u.targetPlayer));
        }

        if (upgrade.targetWeapon != null) {
            upgradeListToRemove = upgradeList.FindAll(u => Equals(upgrade.targetWeapon, u.targetWeapon));
        }

        if (upgradeListToRemove.Count == 0) {
            return;
        }

        // This will remove in the total list since it is a reference
        upgradeList.Remove(upgradeListToRemove[0]);
    }
}
