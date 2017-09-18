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

		Events.Dispatcher.OnActivitySelected += (Activity activity) => {
		  gameObject.SetActive(activity == Activity.SYLLABLE_DIVISION);
		};

		Events.Dispatcher.OnUIInputLocked += () => {
			gameObject.SetActive(false);
		};
		Events.Dispatcher.OnUIInputUnLocked += () => {
			gameObject.SetActive(true);
		};
	}

	void ToggleSyllableDivisionShow(){

		if (State.Current.UIInputLocked)
			return;
		Events.Dispatcher.RecordSyllableDivisionShowStateToggled ();


	}
}
