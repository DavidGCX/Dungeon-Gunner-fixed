using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    private void SetCineMachineTargetGroup() {
        CinemachineTargetGroup.Target cinemachineTarget = new CinemachineTargetGroup.Target {
            weight = 1f,
            radius = 3f,
            target = GameManager.Instance.player.transform
        };

        CinemachineTargetGroup.Target cinemachineCursorTarget = new CinemachineTargetGroup.Target {
            weight = 1f,
            radius = 1f,
            target = cursorTarget
        };

        var targets = new[] { cinemachineTarget, cinemachineCursorTarget };
        targetGroup.m_Targets = targets;
    }

    private void Update() {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
