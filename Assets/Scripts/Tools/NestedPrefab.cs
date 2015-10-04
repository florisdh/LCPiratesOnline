using UnityEngine;
using System.Collections;

public class NestedPrefab : MonoBehaviour
{
    #region Vars

    public GameObject Prefab;

    #endregion

    #region Methods

    private void Awake()
    {
        GameObject obj = (GameObject)Instantiate(Prefab, transform.position, transform.rotation);
        obj.transform.parent = transform.parent;
        Destroy(gameObject);
    }

    #endregion
}
