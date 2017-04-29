using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;


public class ConversationManager : MonoBehaviour
{
	public static ConversationManager _instance;
	public GameObject chatDisplay;

	public ConversationList conversationList;

	public Text textBox;
	public int currentChapter = 0;
	public bool isConversationActive;

	public CharacterChat characterChat;

	public float charSpeed = 0.01F;

	IEnumerator _ActiveConversation (GameObject sender, ConversationList.AssetBase character, ConversationList.Conversations conversation)
	{
		isConversationActive = true;
		Reset (true);
		int pageIndex = 0;

		while (pageIndex < conversation.pages.Count) {

			textBox.text = "";
			string textToDisplay = conversation.pages [pageIndex].outputText;
			bool isReadingPage = true;
			bool canGoNextPage = false;

			Coroutine c = StartCoroutine (AddText (textToDisplay));

			while (isReadingPage) {
				if (textBox.text.Length >= conversation.pages [pageIndex].outputText.Length) {
					isReadingPage = false;
				}

				if (Input.GetKeyDown (KeyCode.Space)) {
					isReadingPage = false;
					StopCoroutine (c);
					textBox.text = conversation.pages [pageIndex].outputText;
				}

				yield return null;
			}

			while (!canGoNextPage) {
				if (Input.GetKeyDown (KeyCode.Space)) {
					canGoNextPage = true;
				}

				yield return null;
			}

			pageIndex += 1;


			yield return null;
		}

		isConversationActive = false;
		CloseConversation ();
	}


	IEnumerator AddText (string textToDisplay)
	{
		while (textToDisplay.Length > 0) {
			textBox.text += textToDisplay [0];
			textToDisplay = textToDisplay.Substring (1);
			yield return new WaitForSeconds (charSpeed);
		}	
	}

	public void StartConversation (GameObject sender, ConversationList.AssetBase character, ConversationList.Conversations conversation)
	{
		if (!isConversationActive)
			StartCoroutine (_ActiveConversation (sender, character, conversation));
	}

	void Update ()
	{

		if (isConversationActive)
			return;

		//Start the main story
		if (Input.GetKeyDown (KeyCode.S)) {
			//Story runs in order


			if (conversationList.stories [0].chapters.Count > currentChapter) {

				ConversationList.AssetBase chapter = conversationList.stories [0].chapters [currentChapter];

				if (chapter.conversations.Count > 0) {
					StartConversation (gameObject, chapter, chapter.conversations [0]);
					StartCoroutine (WaitForConversationToBeDone ());
				} else {
					print ("No conversation exist");
				}
	
			} else {
				print ("No chapter exist");
			}

		}

		//Start a character story
		if (Input.GetKeyDown (KeyCode.C)) {
			characterChat.StartChat ();
		}
			
	}

	IEnumerator WaitForConversationToBeDone ()
	{
		while (isConversationActive)
			yield return null;

		//Do stuff
		currentChapter++;

	}

	public void Reset (bool state)
	{
		chatDisplay.SetActive (state);
		textBox.text = "";
	}

	public void CloseConversation ()
	{
		Reset (false);
	}


	public static ConversationManager singletonInstance {

		get { 
		
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<ConversationManager> ();
		
			return _instance;
		}

	}

}
