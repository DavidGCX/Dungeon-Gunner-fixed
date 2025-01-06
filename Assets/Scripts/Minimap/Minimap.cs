using System;
using UnityEngine;
using Unity.Cinemachine;

public class Minimap : MonoBehaviour {
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerTransform;

    private void Start() {
        playerTransform = GameManager.Instance.GetPlayer().transform;

        CinemachineCamera cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
        cinemachineCamera.Follow = playerTransform;

        SpriteRenderer playerSpriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer) {
            playerSpriteRenderer.sprite = GameManager.Instance.GetPlayerMinimapIcon();
        }
    }

    private void Update() {
        if (playerTransform && minimapPlayer) {
            minimapPlayer.transform.position = playerTransform.position;
        }
    }

    #region validation

#if UNITY_EDITOR
    private void OnValidate() {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }

#endif

    #endregion
}
