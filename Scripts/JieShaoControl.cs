using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JieShaoControl : MonoBehaviour {
	public Transform introduce;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!introduce.gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
	}
}
