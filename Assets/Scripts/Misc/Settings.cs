using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings {
    #region Dungeon Build Settings

    public const int MaxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int MaxDungeonBuildAttempts = 10;

    #endregion

    #region Room settings

    public const float fadeInTime = 0.5f;
    public const int MaxChildCorridors = 3;
    public const float doorUnlockDelay = 1f;

    #endregion

    #region Units

    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;

    #endregion

    #region Animator parameters

    // Player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");

    public static float baseSpeedForPlayerAnimations = 8f;

    // Enemy
    public static float baseSpeedForEnemyAnimations = 3f;

    // Door
    public static int open = Animator.StringToHash("open");

    #endregion

    #region gameTags

    // Object tags
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    #endregion

    #region FIRING CONTROL

    public const float useAimAngleDistance = 3.5f;

    #endregion

    #region Astar Pathfinding Parameters

    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;

    public const int targetFrameRateToSpreadPathfinding = 60;

    #endregion

    #region Enemy Parameters

    public const int defaultEnemyHealth = 20;
    

    #endregion

    #region UI Parameters

    public const float uiAmmoIconSpacing = 4f;
    public const float uiHeartSpacing = 16f;

    #endregion

    #region Contact Damage Parameters

    public const float contactDamageCooldown = 0.5f;

    #endregion
}
