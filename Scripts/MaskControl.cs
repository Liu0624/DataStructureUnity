using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskControl : MonoBehaviour {
    public Transform introduce;
    public Transform mask;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (introduce.gameObject.activeSelf)
        {
            mask.gameObject.SetActive(true);
        }
        else
        {
            mask.gameObject.SetActive(false);
        }
	}
}
