using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigraphControl : MonoBehaviour {
	public Transform introduce;
	public Transform undigraph;

	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!this.gameObject.activeSelf  && !introduce.gameObject.activeSelf && !undigraph.gameObject.activeSelf )
        {
			this.gameObject.SetActive(true);
        }
	}
}
