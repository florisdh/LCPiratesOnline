using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
    #region Vars

    public Menu PrevousMenu;
    
    #endregion

    #region Methods

    public void Show(Menu caller = null)
    {
        if (caller != null)
        {
            PrevousMenu = caller;
            PrevousMenu.Hide();
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
        PrevousMenu.Show();
    }

    #endregion
}
