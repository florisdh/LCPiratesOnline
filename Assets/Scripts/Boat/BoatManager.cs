using UnityEngine;
using System.Collections;

public class BoatManager : MonoBehaviour
{
    #region Vars

    public Cannon[] Cannons;

    #endregion

    #region Methods

    void Start()
    {
        Cannons = GetComponentsInChildren<Cannon>();
    }

    void Update()
    {

    }

    private void setAngles(float angle)
    {

    }

    #endregion
}
