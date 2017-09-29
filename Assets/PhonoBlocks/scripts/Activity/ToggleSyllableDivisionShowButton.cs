﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIButtonMessage))]
public class ToggleSyllableDivisionShowButton : MonoBehaviour {

	void Start(){
		UIButtonMessage messenger= GetComponent<UIButtonMessage> ();
		messenger.target = gameObject;
		messenger.functionName = "ToggleSyllableDivisionShow";
		messenger.trigger = UIButtonMessage.Trigger.OnClick;
	
	
		Transaction.Instance.ActivitySelected.Subscribe((Activity activity) => {
		  gameObject.SetActive(activity == Activity.SYLLABLE_DIVISION);
		});

		Transaction.Instance.UIInputLocked.Subscribe(() => {
			gameObject.SetActive(false);
		});
		Transaction.Instance.UIInputUnLocked.Subscribe(() => {
			gameObject.SetActive(true);
		});
	}

	void ToggleSyllableDivisionShow(){

		if (Transaction.Instance.State.UIInputLocked)
			return;
		Transaction.Instance.SyllableDivisionShowStateToggled.Fire ();


	}
}
