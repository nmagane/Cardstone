using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoader : MonoBehaviour
{
    public MonoBehaviour loader;
    void Start()
    {
        loader.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {

        }

    }
}
