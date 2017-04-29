using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor (typeof(ChatEventObject))] [CanEditMultipleObjects]
public class ChatInspectorDrawer : Editor
{
	public ConversationList conversationList;

	void OnEnable ()
	{
		conversationList = AssetDatabase.LoadAssetAtPath (MainEditor.pathCharacterList, typeof(ConversationList)) as ConversationList;
	}

	public override void OnInspectorGUI ()
	{
		
		//Check if other of same type exist
		//ChatEventObject[] allEventCharacters = GameObject.FindObjectsOfType<ChatEventObject> ();
		//List<ChatEventObject> listOfSameCharacters = new List<ChatEventObject> ();

		Draw ();
	}

	public ChatEventObject GetChatObject ()
	{
		return (ChatEventObject)target;
	}

	public int GetCharacterIndex ()
	{
		ChatEventObject myTarget = (ChatEventObject)target;
		return myTarget.characterIndex;
	}

	public void Draw ()
	{

		if (MainEditor.singletonInstance == null) {
			//GUILayout.Box ("Editting required the RPG Story editor to be opened");
			//return;
		}

		if (Application.isPlaying)
			return;

		ChatEventObject myTarget = (ChatEventObject)target;

		myTarget.assetType = (AssetType)EditorGUILayout.EnumPopup ("Asset Type", myTarget.assetType);


		//MainEditor.singletonInstance.GetCharacterStrings ();
		string[] characterStrings = MainEditor.GetCharacterStrings (conversationList.characters, true, myTarget.assetType);

		if (myTarget.assetType == AssetType.story) {
			characterStrings = MainEditor.GetChaptersStrings (conversationList.stories [0].chapters, true);
		}

		myTarget.lastCharIndex = myTarget.characterIndex;

		myTarget.characterIndex++;
		myTarget.characterIndex = EditorGUILayout.Popup ("", myTarget.characterIndex, characterStrings, GUILayout.Width (200));
		myTarget.characterIndex--;

		//Debug.Log (myTarget.characterIndex);

		if (myTarget.lastCharIndex != myTarget.characterIndex) {

			//Make sure your conversation is different now.
			Debug.Log ("Character changed");

			myTarget.conversationEvents = new List<ChatEventObject.ConversationEvents> ();


			//You should instead have a library of characters

			ChatEventObject[] allEventCharacters = GameObject.FindObjectsOfType<ChatEventObject> ();

			List<ChatEventObject> listOfSameCharacters = new List<ChatEventObject> ();

			for (int i = 0; i < allEventCharacters.Length; i++) {
				if (allEventCharacters [i].characterIndex == myTarget.characterIndex)
					listOfSameCharacters.Add (allEventCharacters [i]);
			}

			if (myTarget.characterIndex != -1) {

				if (myTarget.assetType == AssetType.story)
					myTarget.name = myTarget.characterIndex.ToString () + conversationList.stories [0].chapters [myTarget.characterIndex].name;

				if (myTarget.assetType == AssetType.character)
					myTarget.name = conversationList.characters [myTarget.characterIndex].name;

				//Debug.Log (listOfSameCharacters.Count + " " + conversationList.characters [myTarget.characterIndex].name + " in scene");
			} else
				myTarget.name = "none";

			if (myTarget.assetType == AssetType.character) {
				//This is the second more later one
				if (listOfSameCharacters.Count >= 2) {
					for (int i = 0; i < listOfSameCharacters.Count; i++) {
						if (listOfSameCharacters [i] != myTarget) {
							myTarget.conversationEvents = listOfSameCharacters [i].conversationEvents;
							break;
						}
					}
				}
			}

		}


		//Check if character has changed and has updated (Copy the other ones)

		if (myTarget != null) {
			if (Selection.activeGameObject != myTarget.gameObject)
				return;
		}

		try {

			this.serializedObject.Update ();

			#region CONVERSATION EVENTS
			List<ConversationList.Conversations> conversations = conversationList.characters [myTarget.characterIndex].conversations;

			//EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("eventPages").GetArrayElementAtIndex (p), true);


			int count = this.serializedObject.FindProperty ("conversationEvents").arraySize;

			//this.serializedObject.FindProperty ("conversationEvents").DeleteArrayElementAtIndex (0);





			if (conversations.Count > count) {
				
				ChatEventObject.ConversationEvents conversationEvent = new ChatEventObject.ConversationEvents ();
				conversationEvent.name = "Conversation Events ";
				myTarget.conversationEvents.Add (conversationEvent);
			}

			if (count > conversations.Count) {
				myTarget.conversationEvents.RemoveAt (0);
			}

			for (int i = 0; i < count; i++) {


				SerializedProperty pageProp = this.serializedObject.FindProperty ("conversationEvents").GetArrayElementAtIndex (i);
				SerializedProperty it = pageProp.Copy ();

				//EditorGUILayout.PropertyField (pageProp);

				#region PAGE EVENTS
				List<ConversationList.Pages> pages = conversationList.characters [myTarget.characterIndex].conversations [i].pages;


				while (myTarget.conversationEvents [i].eventPages.Count > pages.Count) {
					myTarget.conversationEvents [i].eventPages.RemoveAt (myTarget.conversationEvents [i].eventPages.Count - 1);
				}


				while (it.Next (true)) { // or NextVisible, also, the bool argument specifies whether to enter on children or not
					//Debug.Log (it.name);
					if (it.name == "eventPages")
						break;
				}

				//Draw page stuff here

				for (int p = 0; p < pages.Count; p++) {

					if (myTarget.conversationEvents [i].eventPages.Count < pages.Count) {
						ChatEventObject.EventPages eventPage = new ChatEventObject.EventPages ();
						eventPage.name = "Page Events " + p;
						//myTarget.conversationEvents [i].eventPages.Add (eventPage);
					}

					myTarget.conversationEvents [i].eventPages [p].name = "Event Page " + p.ToString ();

					/*
					GUILayout.Box ("Inspector Info");
					if (it.arraySize - 1 >= p)
						EditorGUILayout.PropertyField (it.GetArrayElementAtIndex (p), true);
					*/

				}

				if (myTarget.eventPages.Count > pages.Count)
					myTarget.eventPages.Clear ();

				#endregion


			}
			#endregion

			//EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("conversationEvents"), true);

			//Type type = obj.GetType ();
			//PropertyInfo[] properties = type.GetProperties ();

			EditorGUILayout.PropertyField (this.serializedObject.FindProperty ("conversationEvents"), true);

			myTarget.onPageProp = this.serializedObject.FindProperty ("conversationEvents");
			myTarget.onStartProp = this.serializedObject.FindProperty ("onStartEvents");
			myTarget.onEndProp = this.serializedObject.FindProperty ("onEndEvents");

			myTarget.onStartObject = this.serializedObject;
			this.serializedObject.ApplyModifiedProperties ();

		} catch {

		}
	}

	public void ReDraw ()
	{
		Draw ();
	}

	public SerializedObject GetThisSerializedObject ()
	{
		return this.serializedObject;
	}

}
