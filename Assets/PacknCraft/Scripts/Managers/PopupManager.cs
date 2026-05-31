using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private string addressableGroup = "popup";

    private Stack<GameObject> popupStack = new();

    public static PopupManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public async UniTask<GameObject> PushPopup(string address)
    {
        if (string.IsNullOrEmpty(address))
            return null;

        var popup = await AssetLoader.InstantiateAsync(address, addressableGroup, container);

        if (popup != null)
            popupStack.Push(popup);

        return popup;
    }

    public void PopPopup()
    {
        if (popupStack.Count == 0)
            return;

        var topPopup = popupStack.Pop();

        if (topPopup != null)
            Destroy(topPopup);
    }

    public void ClearAll()
    {
        while (popupStack.Count > 0)
        {
            var popup = popupStack.Pop();
            if (popup != null)
                Destroy(popup);
        }
    }
}