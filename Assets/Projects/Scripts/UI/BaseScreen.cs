using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScreen : MonoBehaviour
{
    public ScreenState screenState;
    protected UiController uiController;
    public virtual void Initialize(UiController _uiController)
    {
        this.uiController = _uiController;
    }
    public virtual void Active()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
