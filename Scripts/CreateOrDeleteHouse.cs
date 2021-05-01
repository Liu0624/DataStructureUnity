using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrDeleteHouse : MonoBehaviour {

	public static string[] housesName = new string[] { "A", "B", "C", "D", "E", "F", "G" };

	GameObject moxing;
	GameObject house;
	GameObject camera;

	GameObject[] houses = new GameObject[7];
	GameObject[] cameras = new GameObject[7];

	//use this for initialization
	void Start()
	{
		GetHouse();

		GetCameras();

		SetHouseNotEnable();

		SetCamerasNotEnable();
		cameras[0].SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{
		Control();
	}

	public void Control()
	{
		if (Input.GetKey(KeyCode.A))
		{
			SetCamerasNotEnable();
			cameras[0].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.B))
		{
			SetCamerasNotEnable();
			cameras[1].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.C))
		{
			SetCamerasNotEnable();
			cameras[2].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.D))
		{
			SetCamerasNotEnable();
			cameras[3].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.E))
		{
			SetCamerasNotEnable();
			cameras[4].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F))
		{
			SetCamerasNotEnable();
			cameras[5].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.G))
		{
			SetCamerasNotEnable();
			cameras[6].SetActive(true);

			Invoke("CreateHouse", 0.5f);
		}

		if (Input.GetKey(KeyCode.F1))
		{
			SetCamerasNotEnable();
			cameras[0].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F2))
		{
			SetCamerasNotEnable();
			cameras[1].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F3))
		{
			SetCamerasNotEnable();
			cameras[2].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F4))
		{
			SetCamerasNotEnable();
			cameras[3].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F5))
		{
			SetCamerasNotEnable();
			cameras[4].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F6))
		{
			SetCamerasNotEnable();
			cameras[5].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
		if (Input.GetKey(KeyCode.F7))
		{
			SetCamerasNotEnable();
			cameras[6].SetActive(true);

			Invoke("DeleteHouse", 0.5f);
		}
	}

	private void GetHouse()
	{
		moxing = GameObject.Find("moxing3");
		house = moxing.transform.Find("houses").gameObject;

		houses[0] = house.transform.Find("houseA").gameObject;
		houses[1] = house.transform.Find("houseB").gameObject;
		houses[2] = house.transform.Find("houseC").gameObject;
		houses[3] = house.transform.Find("houseD").gameObject;
		houses[4] = house.transform.Find("houseE").gameObject;
		houses[5] = house.transform.Find("houseF").gameObject;
		houses[6] = house.transform.Find("houseG").gameObject;
	}

	private void GetCameras()
	{
		camera = GameObject.Find("cameras");

		cameras[0] = camera.transform.Find("cameraA").gameObject;
		cameras[1] = camera.transform.Find("cameraB").gameObject;
		cameras[2] = camera.transform.Find("cameraC").gameObject;
		cameras[3] = camera.transform.Find("cameraD").gameObject;
		cameras[4] = camera.transform.Find("cameraE").gameObject;
		cameras[5] = camera.transform.Find("cameraF").gameObject;
		cameras[6] = camera.transform.Find("cameraG").gameObject;
	}

	private void SetHouseNotEnable()
	{
		for (int i = 0; i < 7; ++i)
		{
			houses[i].SetActive(false);
		}
	}

	private void SetCamerasNotEnable()
	{
		foreach (var camera in cameras)
		{
			camera.SetActive(false);
		}
	}

	private void CreateHouse()
	{
		string nameHouse = "";

		for (int i = 0; i < 7; ++i)
		{
			if (cameras[i].activeSelf == true)
			{
				nameHouse = housesName[i];
				break;
			}
		}

		if (nameHouse != "" && house.transform.Find(nameHouse) == null)
		{
			switch (nameHouse)
			{
				case "A":
					houses[0].SetActive(true);
					break;
				case "B":
					houses[1].SetActive(true);
					break;
				case "C":
					houses[2].SetActive(true);
					break;
				case "D":
					houses[3].SetActive(true);
					break;
				case "E":
					houses[4].SetActive(true);
					break;
				case "F":
					houses[5].SetActive(true);
					break;
				case "G":
					houses[6].SetActive(true);
					break;
			}
		}
	}

	private void DeleteHouse()
	{
		string nameHouse = "";

		for (int i = 0; i < 7; ++i)
		{
			if (cameras[i].activeSelf == true)
			{
				nameHouse = housesName[i];
				break;
			}
		}


		switch (nameHouse)
		{
			case "A":
				houses[0].SetActive(false);
				break;
			case "B":
				houses[1].SetActive(false);
				break;
			case "C":
				houses[2].SetActive(false);
				break;
			case "D":
				houses[3].SetActive(false);
				break;
			case "E":
				houses[4].SetActive(false);
				break;
			case "F":
				houses[5].SetActive(false);
				break;
			case "G":
				houses[6].SetActive(false);
				break;
		}

	}
}
