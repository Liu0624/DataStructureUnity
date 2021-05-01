using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroduceControl : MonoBehaviour {
	public Transform digraph;
	public Transform bottom;

	// Use this for initialization
	void Start() {
		this.gameObject.SetActive(true);
		digraph.gameObject.SetActive(false);
		bottom.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update() {
        if (gameObject.activeSelf)
        {
			digraph.gameObject.SetActive(false);
        }

		Invoke("SetDigraphActive", 5f);
	}

	private void SetDigraphActive()
    {
		this.gameObject.SetActive(false);
		digraph.gameObject.SetActive(true);
		bottom.gameObject.SetActive(true);
    }
	
}
