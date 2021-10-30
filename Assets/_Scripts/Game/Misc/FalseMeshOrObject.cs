using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseMeshOrObject : MonoBehaviour
{
    public enum Tipo_Select { Mesh,Object}
    public Tipo_Select Tipo;

    private void Start()
    {
        switch (Tipo)
        {
            case Tipo_Select.Mesh:
                GetComponent<MeshRenderer>().enabled = false;
                break;
            case Tipo_Select.Object:
                gameObject.SetActive(false);
                break;
        }
    }
}
