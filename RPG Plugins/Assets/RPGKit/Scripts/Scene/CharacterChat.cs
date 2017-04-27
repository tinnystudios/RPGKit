using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterChat : MonoBehaviour
{
	public int id = 0;
	public int conversationId = 0;
	public ConversationList conversationList;
	public ConversationList.Characters characterInfo;

	void Start ()
	{
		characterInfo = conversationList.characters [id];
		StartChat ();
	}

	//=========START THE CHAT====== (CALL CONVERSATION MANAGER)
	public void StartChat ()
	{
		ConversationManager.singletonInstance.StartConversation (gameObject, characterInfo, characterInfo.conversations [conversationId]);
	}

	//===================SEND MESSAGES FUNCTIONS================//

	public void ExampleMessageStart ()
	{
		print ("Conversation Started: Message Sent");
	}

	public void ExampleMessageEnd ()
	{
		print ("Conversation Ended: Message Sent");
	}

	public void RewardGold (string s)
	{
		print (s);
	}

}
