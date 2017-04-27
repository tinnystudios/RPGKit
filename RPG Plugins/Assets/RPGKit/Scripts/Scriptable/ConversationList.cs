using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class ConversationList : ScriptableObject
{
	public List<Characters> characters;
	public List<Characters> chapters;
	public List<Stories> stories;

	public int currentCharacterIndex = 0;
	public int currentChapter = 0;

	[System.Serializable]
	public class Conversations
	{
		public bool isActive;
		public string name = "Conversation";
		public List<Pages> pages = new List<Pages> ();
		public int playOnIndex = 0;

		//A list of existing events
		//During chapter 1, say these stuff

	}

	[System.Serializable]
	public class MessageEvent
	{
		public string message = "ExampleMessageStart";
		public GameObject receiver;
		public string parameter = "10";
		public bool isActive = false;
	}

	[System.Serializable]
	public class Pages
	{
		public bool isActive;
		public string name = "Page name";
		public Texture2D image;
		public Texture2D customImage;
		public int characterIndex;
		public int moodIndex;
		public string outputText = "This is the default page text.";

	}

	[System.Serializable]
	public class Characters : AssetBase
	{

	}

	[System.Serializable]
	public class Chapters : AssetBase
	{
		public bool isActive;
	}

	[System.Serializable]
	public class CharacterImages
	{
		public Texture2D image;
		public string mood;
	}

	[System.Serializable]
	public class Stories
	{
		public string name;
		public bool isActive;
		public List<Chapters> chapters = new List<Chapters> ();
	}

	[System.Serializable]
	public class AssetBase
	{
		public string name = "Default Character";
		public string uniqueId = "";
		public int index = 0;
		public List<Conversations> conversations = new List<Conversations> ();
		public List<CharacterImages> characterImages = new List<CharacterImages> ();
		public PlayType playType;
		public AssetType assetType;
		public string notes = "Empty note for you to fill out anything you want, this will not go on screen";
	}

	//PlayStyle
	//Order
	//Event
	//Random

}

public enum PlayType
{
	byOrder,
	byEvent,
	byRandom
}

public enum AssetType
{
	character,
	story,
	unsorted
}

	
