﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class UserStarGridController : PhonoBlocksSubscriber
{
	//invoked only when a new scene begins, from the on scene loaded handler.
	public override void SubscribeToAll(PhonoBlocksScene scene){
		
	
		if(scene == PhonoBlocksScene.Activity) {

		if(Transaction.Instance.State.Mode == Mode.STUDENT){

				Transaction.Instance.ActivitySceneLoaded.Subscribe(this,() => {
					gameObject.SetActive(true);
					userStarGrid = GameObject.Find("UserStarGrid");
					MatchStarImageToGridCellDimensions (); //but if nothing is specified it defaults to make it the same size as the grid cells.

					PlaceUserStarOutlinesInGrid (); 
				});

			Transaction.Instance.CurrentProblemCompleted.Subscribe(this,() => {
				if (Transaction.Instance.Selector.SolvedOnFirstTry){
					AddNewUserStar (true, ProblemsRepository.Instance.ProblemsCompleted-1);
				}
			});
		}
		}

	}

		public GameObject userStarGrid;
		public Texture2D userStarImg;
		public Texture2D userStarOutlineImg;
		int starWidth;
		int starHeight;
		int timesToFlash = 4;
		int flashCounter;
		float secondsDelayBetweenFlashes = .20f;
		UITexture toFlash;

		/*public void Start ()
		{
			gameObject.SetActive(false);
		Transaction.Instance.ActivitySceneLoaded.Subscribe(this,() => {
					if(Transaction.Instance.State.Mode == Mode.STUDENT){
						gameObject.SetActive(true);
						userStarGrid = GameObject.Find("UserStarGrid");
						MatchStarImageToGridCellDimensions (); //but if nothing is specified it defaults to make it the same size as the grid cells.

						PlaceUserStarOutlinesInGrid (); 

				Transaction.Instance.CurrentProblemCompleted.Subscribe(this,() => {
								if (Transaction.Instance.Selector.SolvedOnFirstTry){
									AddNewUserStar (true, ProblemsRepository.Instance.ProblemsCompleted-1);
								}
						});
					}
			});
			
		}*/

		void PlaceUserStarOutlinesInGrid ()
		{
				int numStars = Parameters.StudentMode.PROBLEMS_PER_SESSION;
				for (int i=0; i<numStars; i++) {
						CreateStarCellInGrid ();

				}
	
				userStarGrid.GetComponent<UIGrid> ().Reposition ();
		}

		public void AddNewUserStar (bool flash, int at)
	{       	
				UITexture newCellTexture = userStarGrid.transform.GetChild (at).GetComponent<UITexture> ();
				newCellTexture.mainTexture = userStarImg;
				userStarGrid.GetComponent<UIGrid> ().Reposition ();
				if (flash) {
						toFlash = newCellTexture;
						StartCoroutine ("Flash");
				}

		}

		void MatchStarImageToGridCellDimensions ()
		{
				UIGrid grid = userStarGrid.GetComponent<UIGrid> ();
				starWidth = (int)grid.cellWidth;
				starHeight = (int)grid.cellHeight;
		
		}

		public UITexture CreateStarCellInGrid ()
		{      
				Texture2D tex2dCopy = CopyAndScaleTexture (starWidth, starHeight, userStarOutlineImg);
				UITexture ut = NGUITools.AddChild<UITexture> (userStarGrid);
				ut.material = new Material (Shader.Find ("Unlit/Transparent Colored"));
				ut.shader = Shader.Find ("Unlit/Transparent Colored");
				ut.mainTexture = tex2dCopy;
			    
				ut.MakePixelPerfect ();
				return ut;
			
		}

		Texture2D CopyAndScaleTexture (float w, float h, Texture tex2D)
		{
				Texture2D tex2dCopy = Instantiate (tex2D) as Texture2D;
				TextureScale.Bilinear (tex2dCopy, (int)w, (int)h);
				return tex2dCopy;
		}

		public IEnumerator Flash ()
		{
		
				int mod_To_end_on = (timesToFlash % 2 == 0 ? 1 : 0);
		
				while (flashCounter<timesToFlash) {
			
						if (flashCounter % 2 == mod_To_end_on) {
								toFlash.color = Color.white;
				
				
						} else {
								toFlash.color = Color.red;
				
						}
						flashCounter++;
			
						yield return new WaitForSeconds (secondsDelayBetweenFlashes);
				}
		
				flashCounter = 0;
		
		
		
		
		}
}
