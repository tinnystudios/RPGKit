using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtensionsGUI
{

	public static void CopyGUIStyle (string a, string b)
	{
		GUIStyle buttonStyle = GUI.skin.GetStyle (a);
		GUIStyle titleStyle = GUI.skin.GetStyle (b);

		titleStyle.normal = buttonStyle.normal;
		titleStyle.hover = buttonStyle.hover;
		titleStyle.active = buttonStyle.active;
		titleStyle.focused = buttonStyle.focused;
		titleStyle.onNormal = buttonStyle.onNormal;
		titleStyle.onHover = buttonStyle.onHover;
		titleStyle.border = buttonStyle.border;
		titleStyle.margin = buttonStyle.margin;
		titleStyle.padding = buttonStyle.padding;
		titleStyle.overflow = buttonStyle.overflow;
		titleStyle.font = buttonStyle.font;
		titleStyle.fontSize = buttonStyle.fontSize;
		titleStyle.fontStyle = buttonStyle.fontStyle;
		titleStyle.alignment = buttonStyle.alignment;
		titleStyle.wordWrap = buttonStyle.wordWrap;
		titleStyle.richText = buttonStyle.richText;
		titleStyle.clipping = buttonStyle.clipping;
		titleStyle.imagePosition = buttonStyle.imagePosition;
		titleStyle.contentOffset = buttonStyle.contentOffset;
		titleStyle.fixedWidth = buttonStyle.fixedWidth;
		titleStyle.fixedHeight = buttonStyle.fixedHeight;
		titleStyle.stretchWidth = buttonStyle.stretchWidth;
		titleStyle.stretchHeight = buttonStyle.stretchHeight;
	}

}
