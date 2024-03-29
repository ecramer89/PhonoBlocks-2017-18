using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

public class StudentActivityController : PhonoBlocksSubscriber
{

		private static StudentActivityController instance;
		public static StudentActivityController Instance{
			get {
				return instance;
			}

		}
	    //save references to frequently used audio clips 
		AudioClip excellent;
		AudioClip incorrectSoundEffect;
		AudioClip notQuiteIt;
		AudioClip offerHint;
		AudioClip youDidIt;
		AudioClip correctSoundEffect;
		AudioClip removeAllLetters;
		AudioClip triumphantSoundForSessionDone;


		void Start ()
		{      instance = this;
		}


	public override void SubscribeToAll(PhonoBlocksScene nextToLoad){
		if(nextToLoad == PhonoBlocksScene.Activity){
			
			if(Transaction.Instance.State.Mode != Mode.STUDENT) {
				gameObject.SetActive (false);	
				return;
			}

			ProblemsRepository.Instance.Initialize (Transaction.Instance.State.Session);
			CacheAudioClips ();
			Transaction.Instance.InteractiveLettersCreated.Subscribe(this,(List<InteractiveLetter> letters)=>SetUpNextProblem());
			Transaction.Instance.UserEnteredNewLetter.Subscribe(this,HandleNewArduinoLetter);
			Transaction.Instance.UserSubmittedTheirLetters.Subscribe(this,HandleSubmittedAnswer);
		

			Transaction.Instance.WordColorShowStateSet.Subscribe(this, (WordColorShowStates newWordState)=>{
				AudioClip clip;

				if(Transaction.Instance.Selector.CurrentStateOfInputMatchesTarget){
					clip = Transaction.Instance.State.WordColorShowState == WordColorShowStates.SHOW_TARGET_UNITS ? 
						AudioSourceController.GetSoundedOutWordFromResources(Transaction.Instance.State.TargetWord) :
						AudioSourceController.GetWordFromResources(Transaction.Instance.State.TargetWord);
				} else clip = InstructionsAudio.instance.yourWordIsntQuiteRightYet;

				AudioSourceController.PushClip(clip);

			});
		
		
		}


	}
		


	void CacheAudioClips(){
		excellent = InstructionsAudio.instance.excellent;
		incorrectSoundEffect = InstructionsAudio.instance.incorrectSoundEffect;
		notQuiteIt = InstructionsAudio.instance.notQuiteIt;
		offerHint = InstructionsAudio.instance.offerHint;
		youDidIt = InstructionsAudio.instance.youDidIt;
		correctSoundEffect = InstructionsAudio.instance.correctSoundEffect;
		removeAllLetters = InstructionsAudio.instance.removeAllLetters;

		triumphantSoundForSessionDone = InstructionsAudio.instance.allDoneSession;
	}


	bool AllUserControlledLettersAreBlank(){
		return Transaction.Instance.State.UserInputLetters.Aggregate(true,(bool result, char nxt)=>result && nxt == ' ');
	}

	void HandleNewArduinoLetter(char newLetter, int atPosition){
		switch (Transaction.Instance.State.StudentModeState) {
		case StudentModeStates.MAIN_ACTIVITY:
			MainActivityNewLetterHandler(newLetter, atPosition);
			break;
		case StudentModeStates.FORCE_CORRECT_LETTER_PLACEMENT:
			ForceCorrectPlacementNewLetterHandler(newLetter, atPosition);
			break;
		case StudentModeStates.REMOVE_ALL_LETTERS:
			RemoveAllLettersNewLetterHandler(newLetter, atPosition);
			break;
		}
	}

	//if the user removes a letter at a position of a place holder letter,
	//then restore the dashed letter outline for the placeholder.
	void RestorePlaceholderLetterOutlineIfPlaceHolderRemoved(char newLetter, int atPosition){
		if(newLetter!=' ' || atPosition >= Transaction.Instance.State.PlaceHolderLetters.Length) return;
		char placeholderLetter = Transaction.Instance.State.PlaceHolderLetters[atPosition];
		if(placeholderLetter == ' ') return;

		ArduinoLetterController.instance.ChangeTheImageOfASingleCell(
			atPosition, 
			LetterImageTable.instance.GetLetterOutlineImageFromLetter(placeholderLetter));
	}


	void MainActivityNewLetterHandler(char letter, int atPosition){
		ArduinoLetterController.instance.ChangeTheLetterOfASingleCell (atPosition, letter, LetterImageTable.instance.GetLetterImageFromLetter);
		Colorer.Instance.ReColor ();
		RestorePlaceholderLetterOutlineIfPlaceHolderRemoved(letter, atPosition);

	}

	void ForceCorrectPlacementNewLetterHandler(char letter, int atPosition){
		InteractiveLetter asInteractiveLetter = Transaction.Instance.State.UILetters[atPosition];
		if (Transaction.Instance.Selector.IsCorrectlyPlaced(atPosition)){
			//in case the user removed and then replaced a letter correctly.
			asInteractiveLetter.UpdateInputDerivedAndDisplayColor (Transaction.Instance.State.TargetWordColors[atPosition]);
			return;
		}
		//otherwise, don't update the UI letters but do flash the error to indicate that child didn't place the right letter.	
		asInteractiveLetter.UpdateInputDerivedAndDisplayColor (Parameters.Colors.DEFAULT_OFF_COLOR);
		asInteractiveLetter.ConfigureFlashParameters (
			Parameters.Colors.DEFAULT_OFF_COLOR, Parameters.Colors.DEFAULT_ON_COLOR,
			Parameters.Flash.Durations.ERROR_OFF, Parameters.Flash.Durations.ERROR_ON,
			Parameters.Flash.Times.TIMES_TO_FLASH_ERRORNEOUS_LETTER
		);
		asInteractiveLetter.StartFlash ();

	}

	void RemoveAllLettersNewLetterHandler(char letter, int atPosition){
		if (letter != ' ') //don't bother updating the letter unless the user removed it
			return;

		ArduinoLetterController.instance.ChangeTheLetterOfASingleCell (atPosition, letter, LetterImageTable.instance.GetLetterImageFromLetter);
		//once the user removes all letters from the current problem; automatically turn off the display image and go to the next activity.
		if(AllUserControlledLettersAreBlank()){ 
			HandleEndOfActivity ();
		}
	}
		
	 void SetUpNextProblem ()
	{       ProblemData nextProblem = ProblemsRepository.Instance.GetNextProblem ();
				Transaction.Instance.NewProblemBegun.Fire (nextProblem);
				Transaction.Instance.StudentModeMainActivityEntered.Fire ();
	
			if(Transaction.Instance.State.Activity == Activity.SYLLABLE_DIVISION){
					Transaction.Instance.TargetWordSyllablesSet.Fire(
						SpellingRuleRegex.Syllabify(nextProblem.targetWord));
				}

				AudioSourceController.PushClips (nextProblem.instructions);

		}

		void HandleEndOfActivity ()
		{
				if (ProblemsRepository.Instance.AllProblemsDone ()) {
						Transaction.Instance.SessionCompleted.Fire ();
						AudioSourceController.PushClip (triumphantSoundForSessionDone);
						
				} else {
						SetUpNextProblem ();
				}

		}
				

		
		public void HandleSubmittedAnswer ()
		{     
		       			
				Transaction.Instance.TimesAttemptedCurrentProblemIncremented.Fire ();
		
				if (Transaction.Instance.Selector.CurrentStateOfInputMatchesTarget) {
					HandleCorrectAnswer ();
				} else {
					HandleIncorrectAnswer ();				
					
				}


		}

		void HandleCorrectAnswer(){
			AudioSourceController.PushClip (correctSoundEffect);
		if (Transaction.Instance.State.TimesAttemptedCurrentProblem > 1)
				AudioSourceController.PushClip (youDidIt);
			else
				AudioSourceController.PushClip (excellent);
			CurrentProblemCompleted ();
		}

		void HandleIncorrectAnswer ()
		{
				Transaction.Instance.UserSubmittedIncorrectAnswer.Fire ();
				AudioSourceController.PushClip (incorrectSoundEffect);
				AudioSourceController.PushClip (notQuiteIt);
		if(Transaction.Instance.State.StudentModeState == StudentModeStates.MAIN_ACTIVITY) 
					AudioSourceController.PushClip (offerHint);
		}

	    
		void CurrentProblemCompleted ()
		{
				Transaction.Instance.CurrentProblemCompleted.Fire ();
		        //require user to remove all of the tangible letters from the platform before advancing to the next problem.
		        //don't want the letters still on platform from problem n being interpreted as input for problem n+1.
				Transaction.Instance.StudentModeForceRemoveAllLettersEntered.Fire();
			
      
		}
		



}
