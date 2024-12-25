using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour {
    private CinemachineTargetGroup targetGroup;

    private void Awake() {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start() {
        SetCineMachineTargetGroup();
    }

    private void SetCineMachineTargetGroup() {
        CinemachineTargetGroup.Target cinemachineTarget = new CinemachineTargetGroup.Target {
            weight = 1f,
            radius = 1f,
            target = GameManager.Instance.player.transform
        };

        var targets = new[] { cinemachineTarget };
        targetGroup.m_Targets = targets;
    }
}
