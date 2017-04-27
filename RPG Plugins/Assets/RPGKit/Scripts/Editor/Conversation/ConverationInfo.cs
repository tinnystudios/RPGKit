using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConverationInfo : MonoBehaviour
{

	//Pages
	//	Lines
	public List<Pages> pages = new List<Pages> ();

	public class Pages
	{
		public List<Lines> pages = new List<Lines> ();
	}

	public class Lines
	{

	}

}
