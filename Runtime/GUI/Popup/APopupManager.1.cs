using System;
using DG.Tweening;
using UnityEngine;

namespace SisyphusFramework.GUI.Popup
{
    public abstract partial class APopupManager
    {
        public  T GetScreen<T>(string id) where T : Popup
        {
            Popup popup = GetScreen(id);
            return (T)popup;
        }

        public  void SetVisibility(bool visible)
        {
            var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rootCanvas.gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = visible ? 1.0f : 0.0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        public bool IsVisible()
        {
            var canvasGroup = rootCanvas.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                return true;
            }

            bool isVisible = canvasGroup.alpha > 0.0f &&
                             canvasGroup.interactable == true &&
                             canvasGroup.blocksRaycasts == true;

            return isVisible;
        }

    }
}