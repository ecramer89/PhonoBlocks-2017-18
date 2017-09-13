using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WordHistoryController : MonoBehaviour
{
		private static WordHistoryController instance;
		public static WordHistoryController Instance{
			get {
				return instance;
			}

		}

		public GameObject wordHistoryPanelBackground;
		LetterImageTable letterImageTable;


		public GameObject wordHistoryGrid;
		LetterGridController lettersOfWordInHistory;
		List<Word> words; //words in the word history.
		Word psuedoWord; //a dummy value to return in case there is some kind of error.

		public void Start(){
			instance = this;

		}

		public int showImageTime = 60 * 8;

		public void Initialize ()
		{
				lettersOfWordInHistory = wordHistoryGrid.gameObject.GetComponent<LetterGridController> ();
				wordHistoryGrid.GetComponent<UIGrid> ().maxPerLine = Parameters.UI.ONSCREEN_LETTER_SPACES;
				letterImageTable = GameObject.Find ("DataTables").GetComponent<LetterImageTable> ();
				InteractiveLetter.LetterPressed += PlayWordOfPressedLetter;

		        //subscribe to events
				Events.Dispatcher.OnCurrentProblemCompleted += AddCurrentWordToHistory;
				Events.Dispatcher.OnUserAddedWordToHistory += AddCurrentWordToHistory;
		}

		public void AddCurrentWordToHistory ()
		{
				Word newWord = CreateNewWordAndAddToList (AddLettersOfNewWordToHistory ());
				AudioSourceController.PushClip (newWord.Sound);
			
		}

		string AddLettersOfNewWordToHistory ()
		{ 
				StringBuilder currentWordAsString = new StringBuilder ();
				int position = words.Count * Parameters.UI.ONSCREEN_LETTER_SPACES;
				foreach (InteractiveLetter l in State.Current.UILetters) {
					
						
			     GameObject letterInWord = lettersOfWordInHistory.CreateLetterBarCell (l.InputLetter (), l.CurrentDisplayImage (), (position++) + "", (SessionsDirector.IsSyllableDivisionActivity?l.SelectColour:l.ColorDerivedFromInput));
				

						letterInWord.AddComponent<BoxCollider> ();
						letterInWord.AddComponent<UIDragPanelContents> ();
				
						UIDragPanelContents drag = letterInWord.GetComponent<UIDragPanelContents> ();
						drag.draggablePanel = gameObject.GetComponent<UIDraggablePanel> ();
						currentWordAsString.Append (l.InputLetter ());
						
				}
				wordHistoryGrid.GetComponent<UIGrid> ().Reposition ();
				return currentWordAsString.ToString ().Trim ().ToLower ();


		}

		public void ClearWordHistory ()
		{
				words.Clear ();
				//set the letter and display color of each word in history to blank
				List<InteractiveLetter> letters = lettersOfWordInHistory.GetLetters (false);
				foreach (InteractiveLetter letter in letters) {
					letter.UpdateInputLetterAndInputDerivedColor (" ", 
				    lettersOfWordInHistory.GetAppropriatelyScaledImageForLetter(" "), Color.white);
				}
		}

		Word CreateNewWordAndAddToList (string newWordAsString)
		{
				Word newWord = new Word (newWordAsString);

				newWord.Sound = AudioSourceController.GetWordFromResources (newWordAsString);
				words.Add (newWord);
				return newWord;

		}
	
		public void PlayWordOfPressedLetter (GameObject pressedLetterCell)
		{
				InteractiveLetter l = pressedLetterCell.GetComponent<InteractiveLetter> ();
				if (l.IsBlank ()) 
						return;
				Word wordThatLettersBelongTo = RetrieveWordGivenLetterAndIndex (l, IndexOfWordThatLetterBelongsTo (pressedLetterCell));
				AudioSourceController.PushClip (wordThatLettersBelongTo.Sound);
			
		}

		int IndexOfWordThatLetterBelongsTo (GameObject pressedLetterCell)
		{
				return (Int32.Parse (pressedLetterCell.name)) / Parameters.UI.ONSCREEN_LETTER_SPACES;

		}
	
		Word RetrieveWordGivenLetterAndIndex (InteractiveLetter pressedLetter, int idx)
		{
				if (idx > -1 && idx < words.Count)
						return words [idx];
				return psuedoWord;

		}

	//private inner class; just a convenient package for all the data pertaining to a specific word.
	class Word : MonoBehaviour
	{

		string asString;

		public string AsString {
			get {
				return this.asString;

			}


		}

		Texture2D image;

		public Texture2D Image {
			get {
				return this.image;

			}

			set {
				this.image = value;

			}

		}

		AudioClip sound;

		public AudioClip Sound {
			get {
				return this.sound;

			}

			set {
				this.sound = value;

			}



		}

		public Word (string asString_)
		{
			asString = asString_;
			sound = (AudioClip)Resources.Load ("audio/words/" + asString, typeof(AudioClip));
		}

	}


}
