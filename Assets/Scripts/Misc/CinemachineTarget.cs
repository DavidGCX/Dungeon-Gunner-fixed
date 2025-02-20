using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour {
    private CinemachineTargetGroup targetGroup;
    [SerializeField] private Transform cursorTarget;

    private void Awake() {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start() {
        SetCineMachineTargetGroup();
    }

    private void OnEnable() {
        GameManager.Instance.GetPlayer().destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroyed;
    }
    private void OnDisable() {
        GameManager.Instance.GetPlayer().destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroyed;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2) {
        targetGroup.Targets = new List<CinemachineTargetGroup.Target>() { };
    }

    private void SetCineMachineTargetGroup() {
        if (GameManager.Instance.GetPlayer() == null) {
            Debug.LogError("Player not found in GameManager");
            return;
        }

        CinemachineTargetGroup.Target cinemachineTarget = new CinemachineTargetGroup.Target {
            Weight = 1f,
            Radius = 3f,
            Object = GameManager.Instance.GetPlayer().transform
        };

        CinemachineTargetGroup.Target cinemachineCursorTarget = new CinemachineTargetGroup.Target {
            Weight = 1f,
            Radius = 1f,
            Object = cursorTarget
        };

        var targets = new List<CinemachineTargetGroup.Target>() {
            cinemachineTarget,
            cinemachineCursorTarget
        };
        targetGroup.Targets = targets;
    }

    private void Update() {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
