using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon {
    public WeaponDetailsSO weaponDetails;
    public int weaponListPosition;
    public float weaponReloadTimer;
    public int weaponClipRemainingAmmo;
    public int weaponRemainingAmmo;
    public bool isWeaponReloading;
    public GameObject owner;
    public override string ToString() {
        return weaponDetails.weaponName;
    }

    public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        Weapon weapon = (Weapon)obj;
        return weaponDetails == weapon.weaponDetails;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
