﻿/*
Copyright (c) 2014 David Stier

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


******Jello Physics was born out of Walabers JellyPhysics. As such, the JellyPhysics license has been include.
******The original JellyPhysics library may be downloaded at http://walaber.com/wordpress/?p=85.


Copyright (c) 2007 Walaber

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using UnityEditor;

public class SubComponentEditor 
{
	public string name = "none";//TODO consider using GUIContent
	public int drawIndex = -1;
	public int editIndex = -1;
	public Vector3[] handlePositions;
	public float[] handleSizes;
	public string prevTooltip;
	public float minHandleSize;
	public bool multiEditing;
	public Vector2 scrollPos = Vector2.zero;
	public float minScrollViewHeight = 110f;

	public JelloBody body;
	public JelloBodyEditor mainEditor;

	public AddSubComponentState newSubComponentState = AddSubComponentState.inactive;
	public enum AddSubComponentState { inactive, initiated, assignedFirst }

	public SubComponentEditor(Editor editor)
	{
		mainEditor = (JelloBodyEditor)editor;
		body = (JelloBody)editor.target;
	}
	
	public virtual void DrawEditorGUI()
	{
		EditorGUILayout.HelpBox("Please Select a sub component to edit", MessageType.Info);
	}

	public virtual void DrawSceneGUI(){}

	public virtual void SetEditIndex(int index)
	{
		editIndex = index;
	}

	public void CalculateHandleSizes(Vector3[] globalHandlePositions) //TODO implement in other sub-editors...
	{
		minHandleSize = HandleUtility.GetHandleSize(body.transform.position) * 0.075f;

		for(int i = 0; i < handleSizes.Length; i++)
			handleSizes[i] = minHandleSize * 2f;
		
		for(int i = globalHandlePositions.Length - 1; i > -1; i--)
		{
			for(int n = i - 1; n > -1; n--)
			{
				if((globalHandlePositions[i] - globalHandlePositions[n]).sqrMagnitude < (handleSizes[i] + handleSizes[n]) * (handleSizes[i] + handleSizes[n]))
				{
					
					bool sizeDown = true;
					for(int s = n + 1; s < globalHandlePositions.Length; s++)
					{
						if((globalHandlePositions[i] - globalHandlePositions[s]).sqrMagnitude < (handleSizes[i] + handleSizes[s]) * (handleSizes[i] + handleSizes[s]))
						{
							if(handleSizes[s] < minHandleSize * 2f)
								sizeDown = false;
						}
					}
					float ratio = (globalHandlePositions[i] - globalHandlePositions[n]).sqrMagnitude / ((handleSizes[i] + handleSizes[n]) * (handleSizes[i] + handleSizes[n]));
					
					if(sizeDown)//size down this
					{
						handleSizes[i] = minHandleSize + minHandleSize * ratio;
					}
					else//size up other
					{
						handleSizes[n] = handleSizes[i] + minHandleSize * (1 - ratio);
					}
				}
			}
		}
	}
}
