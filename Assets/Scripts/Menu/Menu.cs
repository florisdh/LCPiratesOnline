using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    #region Vars

    private Menu _previousMenu;
    
    #endregion

    #region Methods

    public void Show(Menu caller = null)
    {
        if (caller != null)
        {
            _previousMenu = caller;
            _previousMenu.Hide();
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowPrevious()
    {
        Hide();
        _previousMenu.Show();
    }

    #endregion
}
