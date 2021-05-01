using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl: MonoBehaviour {
	public GameObject RenzhiLight;
	public GameObject DjLight;
	public GameObject FloydLight;
	public GameObject BellManLight;

	private GameObject[] dj;
	private GameObject[] floyed;
	private GameObject[] bellman;

	private int Choose = 1;

	// Use this for initialization
	void Start () {
		dj = new GameObject[5];
		floyed = new GameObject[4];
		bellman = new GameObject[7];

		FindCameras();

		DjLight.SetActive(false);
		FloydLight.SetActive(false);
		BellManLight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(!IsActive(dj) && !IsActive(floyed) && !IsActive(bellman))
        {
			Choose = 1;
			SetActive(true, false, false, false);
        }

		if(Choose == 1)
        {
			if (IsActive(dj))
			{
				SetActive(false, true, false, false);
				Choose = 2;
			}
			else if (IsActive(floyed))
			{
				SetActive(false, false, true, false);
				Choose = 3;
			}
			else if (IsActive(bellman))
			{
				SetActive(false, false, false, true);
				Choose = 4;
			}
		}else if(Choose == 2)
        {
			if (RenzhiLight.activeSelf == true)
			{
				SetActive(true, false, false, false);
				Choose = 1;
			}
			else if (IsActive(floyed))
			{
				SetActive(false, false, true, false);
				Choose = 3;
			}
			else if (IsActive(bellman))
			{
				SetActive(false, false, false, true);
				Choose = 4;
			}
		}else if(Choose == 3)
        {
			if (IsActive(dj))
			{
				SetActive(false, true, false, false);
				Choose = 2;
			}
			else if (RenzhiLight.activeSelf == true)
			{
				SetActive(true, false, false, false);
				Choose = 1;
			}
			else if (IsActive(bellman))
			{
				SetActive(false, false, false, true);
				Choose = 4;
			}
		}else if(Choose == 4)
        {
			if (IsActive(dj))
			{
				SetActive(false, true, false, false);
				Choose = 2;
			}
			else if (IsActive(floyed))
			{
				SetActive(false, false, true, false);
				Choose = 3;
			}
			else if (RenzhiLight.activeSelf == true)
			{
				SetActive(true, false, false, false);
				Choose = 1;
			}
		}
		
	}

	private void FindCameras()
    {
		dj[0] = GameObject.Find("target").transform.Find("djMainCamera").gameObject;
		dj[1]= GameObject.Find("target").transform.Find("2/2Camera").gameObject;
		dj[2] = GameObject.Find("target").transform.Find("3/3Camera").gameObject;
		dj[3] = GameObject.Find("target").transform.Find("4/4Camera").gameObject;
		dj[4] = GameObject.Find("target").transform.Find("5/5Camera").gameObject;

		floyed[0] = GameObject.Find("cameras").transform.Find("site1").gameObject;
		floyed[1] = GameObject.Find("cameras").transform.Find("site2").gameObject;
		floyed[2] = GameObject.Find("cameras").transform.Find("site3").gameObject;
		floyed[3] = GameObject.Find("cameras").transform.Find("site4").gameObject;

		bellman[0] = GameObject.Find("BellmanCameras").transform.Find("cameraA").gameObject;
		bellman[1] = GameObject.Find("BellmanCameras").transform.Find("cameraB").gameObject;
		bellman[2] = GameObject.Find("BellmanCameras").transform.Find("cameraC").gameObject;
		bellman[3] = GameObject.Find("BellmanCameras").transform.Find("cameraD").gameObject;
		bellman[4] = GameObject.Find("BellmanCameras").transform.Find("cameraE").gameObject;
		bellman[5] = GameObject.Find("BellmanCameras").transform.Find("cameraF").gameObject;
		bellman[6] = GameObject.Find("BellmanCameras").transform.Find("cameraG").gameObject;
	}

	private bool IsActive(GameObject[] gameObjects)
    {
		foreach(var item in gameObjects)
        {
			if (item.activeSelf)
				return true;
        }

		return false;
    }

	private void SetActive(bool renzhi, bool dj, bool floyed, bool bellMan)
    {
		RenzhiLight.SetActive(renzhi);
		DjLight.SetActive(dj);
		FloydLight.SetActive(floyed);
		BellManLight.SetActive(bellMan);
    }
}
