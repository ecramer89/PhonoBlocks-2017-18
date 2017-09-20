﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(UIButtonMessage))]
public class SessionButton : MonoBehaviour {

	void Start(){
		gameObject.SetActive(false);
		Transaction.Instance.StudentDataRetrieved.Subscribe(() => {
			if (Transaction.State.Mode == Mode.STUDENT) {
					UIButtonMessage messenger = GetComponent<UIButtonMessage> ();
					messenger.target = gameObject;
					messenger.functionName = "SelectSession";
					messenger.trigger = UIButtonMessage.Trigger.OnClick;
					gameObject.SetActive(true);
					//need -every- session selection button to deactivate when a session is selected.
					//as such, just subscribe to the event here.
					Transaction.Instance.SessionSelected.Subscribe((int session) => {
						gameObject.SetActive(false);
					});
			} else {
				gameObject.SetActive(false);
			}
		});


	}


	void SelectSession(){
		int session;
		if (Int32.TryParse (gameObject.name, out session)) {
			session--; //Unity seems to force indexing of grid children to start at 1, so just need to sub 1 to accommodate 0 based indexing of problem data in arrays.
			Transaction.Instance.SessionSelected.Fire (session);
			Transaction.Instance.ActivitySelected.Fire (Parameters.StudentMode.ActivityForSession (session));
		} else throw new Exception($"Check the names of the session buttons. Each should be an integer corresponding to the session number.");

	}
}
