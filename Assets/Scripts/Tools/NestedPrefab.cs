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

        if(this.transform.tag == "RowOne")
        {
            obj.GetComponent<Cannon>().row = 1;
        }
        else if(this.transform.tag == "RowTwo")
        {
            obj.GetComponent<Cannon>().row = 2;
        }

        Destroy(gameObject);
    }

    #endregion
}
