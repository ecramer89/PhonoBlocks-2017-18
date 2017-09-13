﻿using UnityEngine;
using System.Collections;
using System;

public class CheckedWordImageController : MonoBehaviour
{
		public GameObject checkedWordImage;
		UITexture img;
		BoxCollider clickTrigger;
		long showTime = -1;
		bool disableTextureOnPress;
		public long defaultDisplayTime = 2000;
		bool caller_ends_display;

		void Start ()
		{
				img = checkedWordImage.GetComponent<UITexture> ();
				img.enabled = false;
				clickTrigger = checkedWordImage.GetComponent<BoxCollider> ();
				clickTrigger.enabled = false;
		        
				//remove the old image, if it's still there
				Events.Dispatcher.OnNewProblemBegun += () => {
					EndDisplay ();
				};
				//display the target word image
				Events.Dispatcher.OnCurrentProblemCompleted += () => {
					Texture2D newimg = (Texture2D)Resources.Load ($"{Parameters.FILEPATHS.RESOURCES_WORD_IMAGE_PATH}{State.Current.TargetWord}", typeof(Texture2D));
					if (!ReferenceEquals (newimg, null)) {
						ShowImage (newimg, false, true);
					}
				};
		}

		public void ShowImage (Texture2D newimg, long showTime)
		{
				this.showTime = showTime;
				SetAndEnableTexture (newimg);


		}

		public void ShowImage (Texture2D newimg, bool disableTextureOnPress, bool caller_ends_display=false)
		{
				this.caller_ends_display = caller_ends_display;
				if (newimg != null) {
						if (disableTextureOnPress) {
								this.disableTextureOnPress = disableTextureOnPress;
								SetAndEnableTexture (newimg);
								clickTrigger.enabled = true;
						} else
								ShowImage (newimg, defaultDisplayTime);
				}
		}

		void SetAndEnableTexture (Texture2D newImg)
		{
				img.mainTexture = newImg;
				img.enabled = true;
		}

		void OnPress (bool isPressed)
		{
				if (isPressed && disableTextureOnPress) {
						EndDisplay ();
				}
		}

		public void EndDisplay ()
		{

		    
				img.enabled = false;
				caller_ends_display = false;
				if (disableTextureOnPress) {
						disableTextureOnPress = false;
						clickTrigger.enabled = false;
				}
				if (showTime > 0) {
						showTime = -1;
				}

		}

		void Update ()
		{
				if (!caller_ends_display) {
						if (showTime > 0)
								showTime--;
						if (showTime == 0) {
								EndDisplay ();
						}
				}
				
		}

		public bool WordImageIsOnDisplay ()
		{
				return img.enabled;


		}


		




}
