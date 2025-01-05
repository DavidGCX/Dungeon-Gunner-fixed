using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MessageType {
    Event,
    Normal,
    PickUp,
    Warning
}

public class MessageStack : MonoBehaviour {
    [SerializeField] private GameObject EventMessagePrefab;
    [SerializeField] private GameObject NormalMessagePrefab;
    [SerializeField] private GameObject PickUpMessagePrefab;
    [SerializeField] private GameObject WarningMessagePrefab;
    private List<TextMeshProUGUI> messages = new List<TextMeshProUGUI>();
    [SerializeField] private GameObject content;
    private CanvasGroup canvasGroup;

    public void Start() {
        ClearAllMessages();
    }

    public void ClearAllMessages() {
        foreach (TextMeshProUGUI message in messages) {
            Destroy(message.gameObject);
        }

        messages.Clear();
    }

    // TODO: Using DoTween for fading in and out could be faster
    public IEnumerator FadeInMessages() {
        while (canvasGroup.alpha < 1) {
            canvasGroup.alpha += Time.deltaTime * 5;
            yield return null;
        }
    }

    public IEnumerator FadeOutMessages() {
        while (canvasGroup.alpha > 0) {
            canvasGroup.alpha -= Time.deltaTime * 2;
            yield return null;
        }
    }

    public void AddMessage(string message, MessageType messageType, float duration = 3) {
        if (!content) {
            return;
        }

        if (!canvasGroup) {
            canvasGroup = content.GetComponent<CanvasGroup>();
        }

        StartCoroutine(AddMessageRoutine(message, messageType, duration));
    }

    private IEnumerator AddMessageRoutine(string message, MessageType messageType, float duration) {
        StartCoroutine(FadeInMessages());
        GameObject prefab = null;
        switch (messageType) {
            case MessageType.Event:
                prefab = EventMessagePrefab;
                break;
            case MessageType.Normal:
                prefab = NormalMessagePrefab;
                break;
            case MessageType.PickUp:
                prefab = PickUpMessagePrefab;
                break;
            case MessageType.Warning:
                prefab = WarningMessagePrefab;
                break;
        }

        GameObject messageObject = Instantiate(prefab, content.transform);
        TextMeshProUGUI textMeshProUGUI = messageObject.GetComponent<TextMeshProUGUI>();
        textMeshProUGUI.text = message;
        messages.Add(textMeshProUGUI);
        yield return new WaitForSeconds(duration);
        StartCoroutine(FadeOutMessages());
    }
}
