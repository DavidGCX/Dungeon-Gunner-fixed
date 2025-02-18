using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour {
    [Header("Deal Damage")]
    [SerializeField] private int contactDamageAmount;
    [SerializeField] private LayerMask layerMask;
    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (isColliding) return;
        ContactDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (isColliding) return;
        ContactDamage(other);
    }

    private void ContactDamage(Collider2D other) {
        int collisionObjectLayerMask = 1 << other.gameObject.layer;

        if ((layerMask.value & collisionObjectLayerMask) == 0) return;

        ReceiveContactDamage receiveContactDamage = other.GetComponent<ReceiveContactDamage>();
        if (receiveContactDamage) {
            isColliding = true;
            // TODO: Can add some impulse here to demonstrate contact damage
            Invoke("ResetContactCollision", Settings.contactDamageCooldown);
            receiveContactDamage.TakeContactDamage(contactDamageAmount);
        }
    }

    private void ResetContactCollision() {
        isColliding = false;
    }

    #region validation

    private void OnValidate() {
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
    }

    #endregion
}
