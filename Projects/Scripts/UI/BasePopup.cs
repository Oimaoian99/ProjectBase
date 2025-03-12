using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePopup : MonoBehaviour
{
    public PopupState popupState;
    protected UiController uiController;
    public virtual void Initialize(UiController _uiController)
    {
        this.uiController = _uiController;
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
