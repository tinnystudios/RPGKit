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
	public string s = "Scene: ConversationManager";

	public int pageIndex = 0;
	public ConversationList.Conversations currentConversation;
	public ConversationList.Characters currentCharacter;

	public void StartConversation (GameObject sender, ConversationList.Characters character, ConversationList.Conversations conversation)
	{
		Reset ();
		currentCharacter = character;
		currentConversation = conversation;
		StartCoroutine (_ActiveConversation (sender, character, conversation));
	}

	public void RunEventMesage (List<ConversationList.MessageEvent> e, GameObject sender)
	{
		for (int i = 0; i < e.Count; i++) {

			string message = e [i].message;
			string param = e [i].parameter;
			GameObject receiver = sender;

			if (param.Length <= 0)
				receiver.SendMessage (message);
			else
				receiver.SendMessage (message, param);

		}
	}

	IEnumerator _ActiveConversation (GameObject sender, ConversationList.Characters character, ConversationList.Conversations conversation)
	{
		while (pageIndex < currentConversation.pages.Count) {

			textBox.text = "";

			yield return null;
		}

		CloseConversation ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			//Next page
			pageIndex += 1;
		}
	}

	public void Reset ()
	{
		chatDisplay.SetActive (true);
		textBox.text = "";
	}

	public void CloseConversation ()
	{
		chatDisplay.SetActive (false);
		pageIndex = 0;
	}


	public static ConversationManager singletonInstance {

		get { 
		
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<ConversationManager> ();
		
			return _instance;
		}

	}

}
