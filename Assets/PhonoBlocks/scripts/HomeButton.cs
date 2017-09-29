﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIButtonMessage))]
[RequireComponent(typeof(UIImageButton))]
public class HomeButton : MonoBehaviour {
	[SerializeField] Mode mode;

	void Start(){

		gameObject.SetActive(false);
		SceneManager.sceneLoaded+=(Scene scene, LoadSceneMode arg1) =>{
			if(scene.name == "MainMenu"){
				gameObject.SetActive(false);
			}
		};
		UIButtonMessage messenger = GetComponent<UIButtonMessage> ();
		messenger.target = gameObject;
		messenger.functionName = "ReturnToMainMenu";
		messenger.trigger = UIButtonMessage.Trigger.OnClick;
		Transaction.Instance.ActivitySceneLoaded.Subscribe(() => {
			gameObject.SetActive(true);
		});
	}


	void ReturnToMainMenu(){
		Transaction.Instance.ReturnToMainMenu();
	}
}
