using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
   [Header("Contact Damage Amount to Receive")]
   [SerializeField] private int contactDamageAmount;
   private Health health;

   private void Awake() {
      health = GetComponent<Health>();
   }

   public void TakeContactDamage(int damageAmount = 0, GameObject damageSource = null) {
      if (contactDamageAmount > 0) {
         damageAmount = contactDamageAmount;
      }
      health.TakeDamage(damageAmount, damageSource);
   }

   #region validation

   private void OnValidate() {
      HelperUtilities.ValidateCheckPositiveValue(this, nameof(contactDamageAmount), contactDamageAmount, true);
   }

   #endregion
}
