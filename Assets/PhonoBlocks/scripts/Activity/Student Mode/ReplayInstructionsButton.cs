﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIButtonMessage))]
public class ReplayInstructionsButton : MonoBehaviour {

	void Start(){
			if (Transaction.State.Mode == Mode.STUDENT) {
				gameObject.SetActive(true);
				UIButtonMessage messenger= GetComponent<UIButtonMessage> ();
				messenger.target = gameObject;
				messenger.functionName = "ReplayInstructions";
				messenger.trigger = UIButtonMessage.Trigger.OnClick;


				Transaction.Instance.UIInputLocked.Subscribe(() => {
						gameObject.SetActive(false);
				});
				Transaction.Instance.UIInputUnLocked.Subscribe(() => {
						gameObject.SetActive(true);
				});
			} else {
				gameObject.SetActive(false);
			}

	}

	void ReplayInstructions(){
		if (Transaction.State.UIInputLocked)
			return;
		AudioSourceController.PushClips (Transaction.State.CurrentProblemInstructions);

	}
}
