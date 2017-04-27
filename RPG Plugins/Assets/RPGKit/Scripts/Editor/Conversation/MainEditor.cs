using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.SceneManagement;

public class MainEditor : EditorWindow
{
	public int currentConversation = 0;

	#region VARIABLES

	public static MainEditor _instance;
	public string pathToConversations = "Assets/RPGKit/Database/Conversations/";
	public const string pathCharacterList = "Assets/ConversationList.asset";
	public ConversationList conversationList;

	public AssetType assetType;
	public int characterIndex = 0;
	public ConversationList.AssetBase _character = new ConversationList.AssetBase ();

	float titleWidth = 150;
	float smallButtonSize = 50;
	float mediumButtonSize = 80;
	float texture2DSize = 80;
	public string inputName = "";
	public string inputConversationName = "";
	public string inputPageName = "";
	public Texture2D inputImage;

	Vector2 scrollPos;
	Vector2 scrollPosImages;
	Vector2 scrollPosConversations;
	public static EditorWindow window;
	GUISkin editorSkin;
	GUISkin defaultSkin;


	public string[] events = new string[]{ "event1", "event2" };

	#endregion

	[MenuItem ("RPGEditors/Meh")]
	static void  Init ()
	{
		window = EditorWindow.GetWindow (typeof(MainEditor));

	}

	#region OnGUI

	public bool isHide = false;

	void OnGUI ()
	{

		if (isHide) {
			if (GUILayout.Button ("Unhide")) {
				isHide = false;
			}
		}

		if (isHide)
			return;

		if (Application.isPlaying)
			return;

		_instance = this;
		if (defaultSkin == null)
			defaultSkin = GUI.skin;

		//if (Input.GetKey (KeyCode.KeypadEnter) || Input.GetKey ("enter"))
		//Deselect ();

		if (editorSkin == null)
			editorSkin = (GUISkin)(AssetDatabase.LoadAssetAtPath ("Assets/RPGKit/GUISkins/GUISkin.guiskin", typeof(GUISkin)));
		
		GUI.skin = editorSkin;

		smallButtonSize = 35;

		if (window != null) {
			window.minSize = new Vector2 (500, 500);
			window.maxSize = new Vector2 (500, 500);
		}
		if (conversationList == null) {
			conversationList = AssetDatabase.LoadAssetAtPath ("Assets/ConversationList.asset", typeof(ConversationList)) as ConversationList;
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Characters", GUILayout.Width (Screen.width * 0.5F), GUILayout.Height (50))) {
			assetType = AssetType.character;
		}

		if (GUILayout.Button ("Main Story", GUILayout.Width (Screen.width * 0.5F), GUILayout.Height (50))) {
			assetType = AssetType.story;
		}
		GUILayout.EndHorizontal ();


		//conversationList = EditorGUILayout.ObjectField (conversationList, typeof(ConversationList), true) as ConversationList;
	
		if (assetType == AssetType.character)
			_character = conversationList.characters [characterIndex];

		if (assetType == AssetType.story)
			_character = conversationList.stories [0].chapters [0];

		//#here
		FindTarget ();

		EditorSceneManager.MarkAllScenesDirty ();

		if (targetObject == null) {
			eventTargetType = EventTargetTypes.global;
		}

		if (targetObject != null)
			eventTargetType = EventTargetTypes.target;

		if (targetObject != null) {
			Selection.activeObject = targetObject.transform;
			Selection.activeTransform = targetObject.transform;
		}

		scrollPos =	EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (Screen.width), GUILayout.Height (Screen.height * 0.9F));

		if (assetType == AssetType.character) {
			DisplayCharacterCreation ();
		}

		if (assetType == AssetType.story) {
			if (GUILayout.Button ("New Story")) {
				AddStory ();
			}
		
			for (int i = 0; i < conversationList.stories.Count; i++) {

				ConversationList.Stories curStory = conversationList.stories [i];
				curStory.name = "Story";

				if (GUILayout.Button (i.ToString () + ":" + curStory.name))
					curStory.isActive = !curStory.isActive;

				//STORY IS ACTIVE
				if (curStory.isActive) {
					
					if (GUILayout.Button ("Add Chapter")) {
						AddChapter (curStory);
					}

					int chapterIndex = 0;

					foreach (ConversationList.Chapters curChapter in curStory.chapters) {



						curChapter.name = "Chapter";
						if (GUILayout.Button (chapterIndex.ToString () + ":" + curChapter.name))
							curChapter.isActive = !curChapter.isActive;
					
						if (curChapter.isActive) {

							characterIndex = chapterIndex;

							foreach (ConversationList.Chapters otherChaps in curStory.chapters) {
								if (otherChaps != curChapter)
									otherChaps.isActive = false;
							}


							DisplayConversationContent ();
							GUILayout.BeginVertical ();
						
							GUILayout.BeginHorizontal ();
							GUILayout.Space (40);

							GUILayout.BeginVertical ();
							GUILayout.Label ("Notes: ");
							curChapter.notes = GUILayout.TextArea (curChapter.notes, GUILayout.Height (150));
							GUILayout.EndVertical ();

							GUILayout.EndHorizontal ();
							GUILayout.EndVertical ();

							//Make everything else disable

						}

						chapterIndex++;
					}

				}

					
				//Get current story



			}


		}

		EditorGUILayout.EndScrollView ();

	}

	#endregion


	#region STORY



	public void AddStory ()
	{
		ConversationList.Stories aStory = new ConversationList.Stories ();
		conversationList.stories.Add (aStory);
	}

	public void AddChapter (ConversationList.Stories curStory)
	{
		ConversationList.Chapters aChapter = new ConversationList.Chapters ();
		curStory.chapters.Add (aChapter);
	}

	#endregion

	#region Display



	public void DisplayCharacterCreation ()
	{

		GUILayout.Space (20);
		GUILayout.Space (20);

		DisplayInformation ();

		DisplayAddCharacter ();

		GUILayout.Space (20);

		DisplayConversationContent ();

		GUILayout.Space (20);
		DisplayCharacterImages ();

		EditorUtility.SetDirty (conversationList);

	}

	public void DisplayCharacterImages ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (40);
		GUILayout.BeginVertical (EditorStyles.helpBox);
		GUILayout.Box ("Images", GUI.skin.GetStyle ("title"));
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical ();

		GUI.skin = defaultSkin;

		inputImage = (Texture2D)EditorGUILayout.ObjectField (inputImage, typeof(Texture2D), true, GUILayout.Width (texture2DSize), GUILayout.Height (texture2DSize));

		if (GUILayout.Button ("Add", GUILayout.Width (mediumButtonSize))) {
			ConversationList.CharacterImages image = new ConversationList.CharacterImages ();
			_character.characterImages.Add (image);
		}
		GUILayout.EndVertical ();

		scrollPosImages = EditorGUILayout.BeginScrollView (scrollPosImages, GUILayout.Width (Screen.width * 0.65F), GUILayout.Height (140));
	
		GUILayout.BeginHorizontal ();
		for (int i = 0; i < _character.characterImages.Count; i++) {
			GUILayout.BeginVertical ();

			if (GUILayout.Button ("Delete", GUILayout.Width (texture2DSize))) {
				_character.characterImages.RemoveAt (i);
				return;
			}

			_character.characterImages [i].image = (Texture2D)EditorGUILayout.ObjectField ("", _character.characterImages [i].image, typeof(Texture2D), true, GUILayout.Width (texture2DSize), GUILayout.Height (texture2DSize));
			_character.characterImages [i].mood = EditorGUILayout.TextField ("", _character.characterImages [i].mood, GUILayout.Width (texture2DSize));
			GUILayout.EndVertical ();
		}

		EditorGUILayout.EndScrollView ();
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}

	public void DisplayAddCharacter ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (40);
		GUILayout.BeginHorizontal (EditorStyles.helpBox);
	

		GUILayout.Label ("Name: ");
		inputName = GUILayout.TextField (inputName);

		if (GUILayout.Button ("Add", GUILayout.Width (mediumButtonSize))) {
			AddCharacter ();
		}
			
		GUILayout.EndHorizontal ();
		GUILayout.EndHorizontal ();
	}

	public void DisplayInformation ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (20);

		GUILayout.BeginVertical ();

		CharacterPicker ();

		GUILayout.EndVertical ();

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Space (40);
		GUILayout.BeginVertical (EditorStyles.helpBox);

		GUILayout.BeginHorizontal ();

		if (GUILayout.Button ("X", GUILayout.Width (smallButtonSize))) {
			DeleteCharacter ();
		}

		GUILayout.Box ("Character Info", GUI.skin.GetStyle ("title"));


		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Name: ");
		_character.name = EditorGUILayout.TextField (_character.name);

		GUILayout.EndHorizontal ();
		//_character.assetType = (AssetType)EditorGUILayout.EnumPopup ("Asset Type", _character.assetType);

		/*
		if (conversationList.characters.Count == 1) {
			//You cannt change it to story
			_character.assetType = AssetType.character;
		}

		for (int i = 0; i < conversationList.characters.Count; i++) {
			if (conversationList.characters [i].assetType == AssetType.story) {
				conversationList.chapters.Add (conversationList.characters [i]);
				conversationList.characters.RemoveAt (i);
			}
		
			if (i > conversationList.characters.Count - 1) {
				characterIndex = conversationList.characters.Count - 1;
				break;
			}


		}
		*/

		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
	}



	public void DisplayConversationContent ()
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Space (40);
		GUILayout.BeginVertical (EditorStyles.helpBox);

		GUILayout.Box ("Converastions", GUI.skin.GetStyle ("title"));

		DisplayConversationList ();

		GUILayout.FlexibleSpace ();

		GUILayout.BeginHorizontal ();

		GUILayout.Label ("Name: ");

		inputConversationName = GUILayout.TextField (inputConversationName);

		if (GUILayout.Button ("Add", GUILayout.Width (mediumButtonSize))) {
			AddConversation ();
		}

		GUILayout.EndHorizontal ();

		GUILayout.EndVertical ();

		GUILayout.EndHorizontal ();

	}

	public void DisplayConversationList ()
	{

		if (_character == null)
			return;

		if (_character.conversations == null)
			return;

		_character.playType = (PlayType)EditorGUILayout.EnumPopup ("Play Type", _character.playType);

		for (int i = 0; i < _character.conversations.Count; i++) {

			currentConversation = i;

			#region CONVERSATION TITLE LINE
			GUILayout.BeginHorizontal (); //ALL COVEDRED

			if (GUILayout.Button ("X", GUILayout.Width (smallButtonSize))) {
				_character.conversations.RemoveAt (i);
				Deselect ();
				GUILayout.EndHorizontal ();
				return;
			}

			if (GUILayout.Button (_character.conversations [i].name)) {
				_character.conversations [i].isActive = !_character.conversations [i].isActive;
			}

			if (GUILayout.Button ("v", GUILayout.Width (smallButtonSize))) {
				if (i == _character.conversations.Count - 1) {
					GUILayout.EndHorizontal ();
					return;
				}

				ConversationList.Conversations temp = _character.conversations [i + 1];
				_character.conversations [i + 1] = _character.conversations [i];
				_character.conversations [i] = temp;

				MoveConversationEvent (i, -1);

				Deselect ();
			}

			if (GUILayout.Button ("^", GUILayout.Width (smallButtonSize))) {
				
				if (i == 0) {
					GUILayout.EndHorizontal ();
					return;
				}
				ConversationList.Conversations temp = _character.conversations [i - 1];
				_character.conversations [i - 1] = _character.conversations [i];
				_character.conversations [i] = temp;

				MoveConversationEvent (i, 1);

				Deselect ();
			}

			GUILayout.EndHorizontal ();

			#endregion

			//LOGIC
			if (_character.conversations [i].isActive) {

				_character.conversations [i].name = EditorGUILayout.TextField (_character.conversations [i].name);



				if (_character.playType == PlayType.byEvent) {
					string[] characterStrings = GetCharacterStrings (conversationList.chapters, false, AssetType.story);
					_character.conversations [i].playOnIndex = EditorGUILayout.Popup ("Play On Chapter", _character.conversations [i].playOnIndex, characterStrings, GUILayout.Width (500));
				}


				GUILayout.Space (20);

				#region Add Button
				if (eventTargetType != EventTargetTypes.target) {
					if (targetObject == null) {
						GUILayout.Box ("Events");

						GUILayout.BeginHorizontal ();

						GUILayout.Label ("A scene character is required to use events");
						GUILayout.FlexibleSpace ();


						if (GUILayout.Button ("Add", GUILayout.Width (mediumButtonSize))) {

							GameObject newTarget = new GameObject ();
							newTarget.name = _character.name;
							newTarget.AddComponent<ChatEventObject> ();
							newTarget.GetComponent<ChatEventObject> ().characterIndex = characterIndex;



							ChatEventObject chatScript = newTarget.GetComponent<ChatEventObject> ();
							chatScript.assetType = assetType;



							targetObject = newTarget;
							eventTargetType = EventTargetTypes.target;

						}


						GUILayout.EndHorizontal ();

					}

				}
				#endregion
	
				DisplayEventTips ();

				#region Display Conversation Events
				if (eventTargetType == EventTargetTypes.target) {


					if (Selection.activeGameObject != targetObject)
						return;

					if (targetObject != null) {

						if (_character.conversations.Count > targetObject.GetComponent<ChatEventObject> ().conversationEvents.Count) {
							
							Debug.Log ("Recreating");
						} else {
							CheckEventsPage (_character.conversations [currentConversation]);
						}
					}

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Target Object");
					targetObject = EditorGUILayout.ObjectField (targetObject, typeof(GameObject), true) as GameObject;
					GUILayout.EndHorizontal ();

				
					if (targetObject == null)
						FindTarget ();
					
					Selection.activeObject = targetObject.transform;
					Selection.activeTransform = targetObject.transform;

					ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();

					if (chatScript == null) {
						GUILayout.Box ("Please add the'ChatEvent' component");
						return;
					}

					if (chatScript.onPageProp == null) {
						Redraw ();
						return;
					}

					if (chatScript.conversationEvents != null) {
						if (chatScript.conversationEvents.Count < _character.conversations.Count) {
							Debug.Log ("Had to redraw");
							Redraw ();
							return;
						}
					} else {
						Debug.Log ("No event page");
					}

					try {

						SerializedObject sObject = GetSereializedObject ();
						if (currentConversation > sObject.FindProperty ("conversationEvents").arraySize) {
							Debug.Log ("waiting");
							Redraw ();
							return;
						}
						SerializedProperty sProp = sObject.FindProperty ("conversationEvents").GetArrayElementAtIndex (currentConversation);

						sObject.Update ();
						//pageProp = chatScript.onPageProp.GetArrayElementAtIndex (currentConversation);

						it = sProp.Copy ();
						//Debug.Log ("begin searchign for conversation events page");
						while (it.Next (true)) {
							if (it.name == "conversationEventPage")
								break;
						}
						//Debug.Log ("finished searchign for conversation events page");
						chatScript.onStartObject = sObject;
						chatScript.onStartObject.Update ();
						chatScript.onPageProp = sProp;

						GUI.skin = defaultSkin;

						EditorGUILayout.PropertyField (it, true);

						GUI.skin = editorSkin;

						sObject.ApplyModifiedProperties ();
						chatScript.onStartObject.ApplyModifiedProperties ();
					} catch {
						//Basically happens when you are not highlighting the object
						Redraw ();
					}

				}
				#endregion

				if (targetObject != null) {
					//if (_character.conversations.Count > targetObject.GetComponent<ChatEventObject> ().conversationEvents.Count)
					//RecreateAllEvents ();
				}

				//Show pages
				//Very awkward here
				GUILayout.BeginHorizontal ();
				GUILayout.Space (20);

				DisplayPageList (_character.conversations [i]);
				GUILayout.EndHorizontal ();
			}



		}

	}


	public void DisplayEventTips ()
	{
		GUILayout.Box ("Event Actions", "title");
		GUILayout.Box ("Events can only be shared within the same scene \n To use the same function, target the prefab");
	}

	public void DisplayPageList (ConversationList.Conversations conversation)
	{
		GUILayout.BeginVertical ();

		GUILayout.Space (20);
		GUILayout.Box ("Pages", GUI.skin.GetStyle ("title"));

		for (int i = 0; i < conversation.pages.Count; i++) {



			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("X", GUILayout.Width (smallButtonSize))) {
				conversation.pages.RemoveAt (i);
				Deselect ();
				return;
			}

			if (GUILayout.Button ("" + i + ": " + conversation.pages [i].name)) {
				conversation.pages [i].isActive = !conversation.pages [i].isActive;
			}

			if (GUILayout.Button ("v", GUILayout.Width (smallButtonSize))) {
				if (i == conversation.pages.Count - 1) {
					return;
				}

				ConversationList.Pages temp = conversation.pages [i + 1];
				conversation.pages [i + 1] = conversation.pages [i];
				conversation.pages [i] = temp;

				MoveEventPage (i, -1);

				Deselect ();
			}

			if (GUILayout.Button ("^", GUILayout.Width (smallButtonSize))) {

				if (i == 0)
					return;
				ConversationList.Pages temp = conversation.pages [i - 1];
				conversation.pages [i - 1] = conversation.pages [i];
				conversation.pages [i] = temp;

				MoveEventPage (i, 1);


				Deselect ();
			}

			GUILayout.EndHorizontal ();

			//LOGIC
			if (conversation.pages [i].isActive) {

				GUILayout.BeginVertical (EditorStyles.helpBox);
				DisplayEvents (i);
				GUILayout.EndVertical ();

				//Show pages
				GUILayout.BeginHorizontal ();
				GUILayout.Space (20);
				GUI.skin = defaultSkin;

				//List of images

				GUILayout.BeginVertical ();
				string[] moodStrings = GetMoodStringsFromCharacter ();

				conversation.pages [i].moodIndex = EditorGUILayout.Popup ("", conversation.pages [i].moodIndex, moodStrings, GUILayout.Width (texture2DSize));

				if (conversation.pages [i].moodIndex == moodStrings.Length - 1) {

					conversation.pages [i].image = (Texture2D)EditorGUILayout.ObjectField ("", conversation.pages [i].customImage, typeof(Texture2D), false, GUILayout.Width (texture2DSize), GUILayout.Height (texture2DSize));
					conversation.pages [i].customImage = conversation.pages [i].image;
				} else {
					conversation.pages [i].image = (Texture2D)EditorGUILayout.ObjectField ("", _character.characterImages [conversation.pages [i].moodIndex].image, typeof(Texture2D), false, GUILayout.Width (texture2DSize), GUILayout.Height (texture2DSize));
				}


				//conversation.pages [i].image = (Texture2D)EditorGUILayout.ObjectField ("", conversation.pages [i].image, typeof(Texture2D), GUILayout.Width (texture2DSize), GUILayout.Height (texture2DSize));
				GUILayout.EndVertical ();



				GUI.skin = editorSkin;
				DisplayLineList (conversation.pages [i]);
				GUILayout.EndHorizontal ();
			}

		}

		//END


		GUILayout.BeginHorizontal (EditorStyles.helpBox);

		GUILayout.Label ("Name: ");
		inputPageName = GUILayout.TextField (inputPageName);

		if (GUILayout.Button ("Add Page", GUILayout.Width (mediumButtonSize))) {
			AddPage (conversation);
		}

		GUILayout.EndHorizontal ();
	
		GUILayout.Space (20);

		GUILayout.EndVertical ();
	}


	public void DisplayLineList (ConversationList.Pages pages)
	{
		GUILayout.BeginVertical ();

	

		GUILayout.BeginHorizontal (EditorStyles.helpBox);


		/*
		if (GUILayout.Button ("Add Line", GUILayout.Width (mediumButtonSize))) {
			AddLine (pages);
		}
		*/

		pages.outputText = GUILayout.TextArea (pages.outputText, GUILayout.Width (350), GUILayout.MinHeight (90));
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();

		GUILayout.Space (20);

		GUILayout.EndVertical ();
	}

	#endregion

	#region SORTING

	public void MoveEventPage (int i, int offset)
	{
	
		ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();
		List<ChatEventObject.EventPages> eventPages = chatScript.conversationEvents [currentConversation].eventPages;

		ChatEventObject.EventPages tempPage = eventPages [i - offset];
		eventPages [i - offset] = eventPages [i];
		eventPages [i] = tempPage;
	
	}

	public void MoveConversationEvent (int i, int offset)
	{

		ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();
		List<ChatEventObject.ConversationEvents> events = chatScript.conversationEvents;

		ChatEventObject.ConversationEvents tempPage = events [i - offset];
		events [i - offset] = events [i];
		events [i] = tempPage;

	}

	#endregion

	#region Add

	public void AddCharacter ()
	{

		ConversationList.Characters c = new ConversationList.Characters ();
		c.name = inputName;
		c.uniqueId = System.Guid.NewGuid ().ToString ();
		conversationList.characters.Add (c);

		//Jump index
		characterIndex = conversationList.characters.Count - 1;
		_character = conversationList.characters [characterIndex];

		Deselect ();

	}


	public void AddConversation ()
	{
		if (_character == null)
			return;

		ConversationList.Conversations convo = new ConversationList.Conversations ();
		_character.conversations.Add (convo);
		convo.name = _character.conversations.Count.ToString () + ": " + inputConversationName;

		//Make template page
		//AddPage (convo);


		Deselect ();
	}

	public void AddPage (ConversationList.Conversations conversation)
	{
		ConversationList.Pages newPage = new ConversationList.Pages ();
		newPage.name = inputPageName;
		conversation.pages.Add (newPage);

		//Get target

		CheckEventsPage (conversation);
	}

	public void CheckEventsPage (ConversationList.Conversations conversation)
	{

		if (targetObject == null)
			return;

		ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();

		//Set ID here too
		for (int i = 0; i < conversation.pages.Count; i++) {
			ChatEventObject.EventPages eventPage = new ChatEventObject.EventPages ();
			chatScript.conversationEvents [currentConversation].eventPages.Add (eventPage);
		}
	}

	public void RecreateAllEvents ()
	{
		
		ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();

		for (int i = 0; i < _character.conversations.Count; i++) {
			ChatEventObject.ConversationEvents conversationEvent = new ChatEventObject.ConversationEvents ();
			chatScript.conversationEvents.Add (conversationEvent);

			for (int p = 0; p < _character.conversations.Count; p++) {
				ChatEventObject.EventPages eventPage = new ChatEventObject.EventPages ();
				chatScript.conversationEvents [i].eventPages.Add (eventPage);
			}

		}

	}



	#endregion

	#region EVENT

	public EventTargetTypes eventTargetType;
	public GameObject targetObject;
	SerializedProperty pageProp;
	SerializedProperty it;



	public void DisplayEvents (int i)
	{

		if (Application.isPlaying)
			return;

		DisplayEventTips ();

		if (eventTargetType == EventTargetTypes.target) {


			if (targetObject != null) {
				CheckEventsPage (_character.conversations [currentConversation]);
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Target Object");
			targetObject = EditorGUILayout.ObjectField (targetObject, typeof(GameObject), true) as GameObject;
			GUILayout.EndHorizontal ();

			//Find it
			ChatEventObject[] allEventCharacters = GameObject.FindObjectsOfType<ChatEventObject> ();

			/*
			List<GameObject> targets = new List<GameObject> ();

			foreach (ChatEventObject g in allEventCharacters) {
				if (g.characterIndex == characterIndex)
					targets.Add (g.gameObject);
			}


			if (targets.Count != 0)
				targetObject = targets [0];
			else
				targetObject = null;
				*/
			FindTarget ();

			if (targetObject == null || !targetObject.activeInHierarchy) {
				return;
			}


			Selection.activeObject = targetObject.transform;
			Selection.activeTransform = targetObject.transform;

			ChatEventObject chatScript = targetObject.GetComponent<ChatEventObject> ();

			if (chatScript == null) {
				GUILayout.Box ("Please add the'ChatEvent' component");
				return;
			}

			chatScript.enabled = true;

			//Update the inspector to get it?

			if (chatScript.onPageProp == null) {
				Redraw ();
				return;
			}

			if (chatScript.conversationEvents [currentConversation].eventPages != null) {
				if (chatScript.conversationEvents [currentConversation].eventPages.Count < _character.conversations [currentConversation].pages.Count) {
					//Redraw
					Debug.Log ("Had to redraw");
					Redraw ();
					return;
				}
			} else {
				Debug.Log ("No even page");
			}

			try {
				chatScript.onStartObject.Update ();

				pageProp = chatScript.onPageProp;

				it = pageProp.Copy ();

				while (it.Next (true)) {
					if (it.name == "eventPages")
						break;
				}

				if (it.arraySize - 1 >= i)
					EditorGUILayout.PropertyField (it.GetArrayElementAtIndex (i), true);
				
				chatScript.onStartObject.ApplyModifiedProperties ();
			} catch {
				Redraw ();
			}
		}
	}


	//May be very inefficient
	public void Redraw ()
	{
		ChatInspectorDrawer[] editors = (ChatInspectorDrawer[])Resources.FindObjectsOfTypeAll (typeof(ChatInspectorDrawer));
		if (editors.Length > 0) {
			editors [0].ReDraw ();
		}
	}

	public SerializedObject GetSereializedObject ()
	{
		ChatInspectorDrawer[] editors = (ChatInspectorDrawer[])Resources.FindObjectsOfTypeAll (typeof(ChatInspectorDrawer));
		if (editors.Length > 0) {
			//chatScript.onStartObject = editors [0].serializedObject;
			return editors [0].serializedObject;
		}
		return new SerializedObject (new Object ());
	}

	public void FindTarget ()
	{

		if (targetObject != null) {

			if (targetObject.GetComponent<ChatEventObject> ().characterIndex != characterIndex || targetObject.GetComponent<ChatEventObject> ().assetType != assetType) {
				targetObject = null;
			}
		}


		ChatEventObject[] allEventCharacters = GameObject.FindObjectsOfType<ChatEventObject> ();

		for (int i = 0; i < allEventCharacters.Length; i++) {
			if (allEventCharacters [i].characterIndex == characterIndex) {
				if (allEventCharacters [i].GetComponent<ChatEventObject> ().assetType == assetType) {
					targetObject = allEventCharacters [i].gameObject;
					break;
				}
			}
				
		}

	}

	#endregion

	#region Unsorted

	public void CharacterPicker ()
	{

		characterIndex = EditorGUILayout.Popup ("", characterIndex, GetCharacterStrings (conversationList.characters, false, AssetType.character), "popUp", GUILayout.Width (titleWidth));
		_character = conversationList.characters [characterIndex];

	}

	public string[] GetMoodStringsFromCharacter ()
	{
		List<string> strings = new List<string> ();



		for (int i = 0; i < _character.characterImages.Count; i++)
			strings.Add (_character.characterImages [i].mood);

		strings.Add ("Custom");

		return strings.ToArray ();
	}

	public static string[] GetCharacterStrings (List<ConversationList.Characters> list, bool t, AssetType assetType)
	{
		List<string> strings = new List<string> ();

		if (t)
			strings.Add ("none");

		for (int i = 0; i < list.Count; i++) {
			if (assetType == list [i].assetType)
				strings.Add (list [i].name);
		}
		return strings.ToArray ();
	}

	public static string[] GetChaptersStrings (List<ConversationList.Chapters> list, bool t)
	{
		List<string> strings = new List<string> ();

		if (t)
			strings.Add ("none");

		for (int i = 0; i < list.Count; i++) {
			strings.Add (i.ToString () + list [i].name);
		}
		return strings.ToArray ();
	}



	public void DeleteCharacter ()
	{
		if (conversationList.characters.Count == 1) {
			Debug.Log ("You need to have at least 1 character in the game!");
			return;
		}

		conversationList.characters.RemoveAt (characterIndex);

		if (characterIndex != 0)
			characterIndex -= 1;
	}

	#endregion

	public ConversationList  Create ()
	{
		ConversationList asset = ScriptableObject.CreateInstance<ConversationList> ();
		AssetDatabase.CreateAsset (asset, "Assets/ConversationList.asset");
		AssetDatabase.SaveAssets ();
		return asset;
	}

	public void Deselect ()
	{
		GUIUtility.hotControl = 0;
		GUIUtility.keyboardControl = 0;
		Redraw ();
	}

	public static MainEditor singletonInstance {
		get {
			return _instance;
		}
	}

}


public enum EventTargetTypes
{
	none,
	//Called directly from the manager
	global,
	//Call from target
	target
}

