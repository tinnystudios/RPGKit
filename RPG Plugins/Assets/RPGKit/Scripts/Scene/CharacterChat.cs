using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChat : MonoBehaviour
{
	public int id = 0;
	public int conversationIndex = 0;
	public ConversationList conversationList;
	public ConversationList.Characters characterInfo;

	void Start ()
	{
		characterInfo = conversationList.characters [id];
	}

	//=========START THE CHAT======
	public void StartChat ()
	{
		List<ConversationList.Conversations> conversations = characterInfo.conversations;
		List<ConversationList.Conversations> conversationsInChapter = new List<ConversationList.Conversations> ();
		List<ConversationList.Conversations> conChapPrev = new List<ConversationList.Conversations> ();

		int prevChapter = 0;

		for (int i = 0; i < conversations.Count; i++) {

			//Add latest
			if (conversations [i].playOnIndex == ConversationManager.singletonInstance.currentChapter)
				conversationsInChapter.Add (conversations [i]);
			
			//Find out which conversations we're used previously
			if (conversations [i].playOnIndex < ConversationManager.singletonInstance.currentChapter) {
				if (conversations [i].playOnIndex > prevChapter)
					prevChapter = conversations [i].playOnIndex;
			}

		}

		//Check if current conversation exists //Alternatively, on the character script, it should know which was the 'latest' and 'second' latest selected chapter!
		if (conversationsInChapter.Count == 0) {
			//Use prev prevChapter value
			for (int i = 0; i < conversations.Count; i++) {
				if (conversations [i].playOnIndex == prevChapter) {
					conversationsInChapter.Add (conversations [i]);
				}
			}
		}

		if (conversationsInChapter.Count == 0) {
			//Cannot run a conversation without a valid conversation
			print ("This character does not contain any conversations!");
			return;
		}

		ConversationManager.singletonInstance.StartConversation (gameObject, characterInfo, conversationsInChapter [conversationIndex]);
		StartCoroutine (WaitForConversationToBeDone (conversationsInChapter.Count - 1));
	}

	IEnumerator WaitForConversationToBeDone (int max)
	{
		while (ConversationManager.singletonInstance.isConversationActive)
			yield return null;

		if (conversationIndex < max)
			conversationIndex++;
	}

}
