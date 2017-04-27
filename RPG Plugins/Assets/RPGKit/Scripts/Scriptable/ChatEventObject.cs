using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

public class ChatEventObject : MonoBehaviour
{
	[SerializeField]
	public SerializedProperty onStartProp;

	[SerializeField]
	public SerializedProperty onEndProp;


	[SerializeField]
	public SerializedProperty onPageProp;

	[SerializeField]
	public SerializedObject onStartObject;


	[SerializeField]
	public SerializedProperty onConvoProp;
	public int lastCharIndex = 0;

	[Serializable]
	public class CustomUnityEvent : UnityEvent
	{

	}

	[SerializeField]
	public CustomUnityEvent onStartEvents;
	[SerializeField]
	public CustomUnityEvent onEndEvents;
	public int characterIndex = 0;


	//Page has a list of events
	//public CustomUnityEvent[] eventPages;
	public List<EventPages> eventPages;
	[SerializeField]
	public List<ConversationEvents> conversationEvents = new List<ConversationEvents> ();
	public AssetType assetType;

	[Serializable]
	public class EventPages
	{
		[HideInInspector]
		public string name = "Page";
		public CustomUnityEvent onStartEvents;
		public CustomUnityEvent onEndEvents;
	}

	[Serializable]
	public class ConversationEvents
	{

		public ConversationEventPage conversationEventPage = new ConversationEventPage ();

		[HideInInspector]
		public string name = "Conversation";
		public List<EventPages> eventPages = new List<EventPages> ();


	}

	[Serializable]
	public class ConversationEventPage
	{
		[HideInInspector]
		public string name = "Conversation Event Page";
		public CustomUnityEvent onStartEvents;
		public CustomUnityEvent onEndEvents;
	}

}