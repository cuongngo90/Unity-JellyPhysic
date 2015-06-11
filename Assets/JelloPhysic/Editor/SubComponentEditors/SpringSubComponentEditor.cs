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

public class SpringSubComponentEditor : SubComponentEditor 
{
	public GUIContent shapeMatchingContent = new GUIContent ("Enabled");//, "Uses spring forces to attempt to keep its original shape");
	public GUIContent buildSpringsContent = new GUIContent("Rebuild");//, "Quickly create a set of springs inside the body.");
	public GUIContent stiffnessContent = new GUIContent ("Stiffness");//, "How strong the spring is");
	public GUIContent dampingContent = new GUIContent ("Damping");//, "How much the spring resists change in length");
	public GUIContent lengthMultiplierContent = new GUIContent("Mulitplier");//, "Increase/Decrease at-rest length of this spring");
	public GUIContent deleteSpringContent = new GUIContent ("X");//, "Delete this spring");
	public GUIContent edgeDefaultsContent = new GUIContent ("Defaults");
	public GUIContent internalDefaultsContent = new GUIContent ("Defaults");
	public GUIContent customDefaultsContent = new GUIContent ("Defaults");
	public GUIContent setDefaultEdgeContent = new GUIContent ("Apply");//, "Set all Edge-Springs to the default settings above");
	public GUIContent setDefaultCustomContent = new GUIContent ("Apply");//, "Set all Custom-Springs to the default settings above");
	public GUIContent setDefaultInternalContent = new GUIContent ("Apply");//, "Set all Internal-Springs to the default settings above");
	public GUIContent edgeSpringsContent = new GUIContent ("Edge Springs");//, "New Edge-Springs will be created with these settings" +
	//"\nChanging these settings will change the settings of any Edge-Springs already at default");
	public GUIContent shapeMatchingFoldoutContent = new GUIContent ("ShapeMatching");
	public GUIContent customSpringsContent = new GUIContent("Custom Spring");//, "New Custom-Springs will be created with these settings" + 
	//"\nChanging these settings will change the settings of any Custom-Springs already at default");
	public GUIContent internalSpringsContent = new GUIContent("Internal Springs");//, "New Internal-Springs will be created with these settings" + 
	//"\nChanging these settings will change the settings of any Internal-Springs already at default");
	public GUIContent addSpringContent = new GUIContent ("Add Custom Spring");//, "Initiate Custom-Spring creation" +
	//"\nAfter clicking this button, move cursor to the desired point on body and click" +
	//"\nThen move cursor to next desired point on body and click to complete the spring");

	public static bool showShapeMatchingFoldout;
	public static bool showEdgeDefaultsFoldout;
	public static bool showInternalDefaultsFoldout;
	public static bool showCustomDefaultsFoldout;

	SerializedProperty eShapeMatching;
	SerializedProperty eShapeSpringK;
	SerializedProperty eShapeSpringDamp;
	SerializedProperty eEdgeSpringK;
	SerializedProperty eEdgeSpringDamp;
	SerializedProperty eCustomSpringK;
	SerializedProperty eCustomSpringDamp;
	SerializedProperty eInternalSpringK;
	SerializedProperty eInternalSpringDamp;
	SerializedProperty eEdgeSprings;
	SerializedProperty eInternalSprings;
	SerializedProperty eCustomSprings;

	public JelloSpringBody springBody;
	public JelloSpring newSpring;
	public JelloSpringBodyEditor springBodyEditor;

	public SpringSubComponentEditor(Editor editor) : base(editor)
	{
		name = "Springs";
		springBodyEditor = (JelloSpringBodyEditor)editor;
		springBody = (JelloSpringBody)springBodyEditor.serializedObject.targetObject;

		handlePositions = new Vector3[2];
		handleSizes = new float[2];
		
		eShapeMatching = springBodyEditor.serializedObject.FindProperty("mShapeMatchingOn");
		eShapeSpringK = springBodyEditor.serializedObject.FindProperty("mShapeSpringK");
		eShapeSpringDamp = springBodyEditor.serializedObject.FindProperty("mShapeSpringDamp");
		eEdgeSpringK = springBodyEditor.serializedObject.FindProperty("mDefaultEdgeSpringK");
		eEdgeSpringDamp = springBodyEditor.serializedObject.FindProperty("mDefaultEdgeSpringDamp");
		eInternalSpringK = springBodyEditor.serializedObject.FindProperty("mDefaultInternalSpringK");
		eInternalSpringDamp = springBodyEditor.serializedObject.FindProperty("mDefaultInternalSpringDamp");
		eCustomSpringK = springBodyEditor.serializedObject.FindProperty("mDefaultCustomSpringK");
		eCustomSpringDamp = springBodyEditor.serializedObject.FindProperty("mDefaultCustomSpringDamp");
		eEdgeSprings = springBodyEditor.serializedObject.FindProperty("mEdgeSprings");
		eInternalSprings = springBodyEditor.serializedObject.FindProperty("mInternalSprings");
		eCustomSprings = springBodyEditor.serializedObject.FindProperty("mCustomSprings");
		
		drawIndex = editIndex = -1; 
		newSpring = null;
		newSubComponentState = AddSubComponentState.inactive;
	}

	public override void DrawEditorGUI ()
	{
		//springBodyEditor.serializedObject.Update();

		multiEditing = springBodyEditor.serializedObject.isEditingMultipleObjects;

		if(!multiEditing)
		{	
			if(GUILayout.Button(addSpringContent, EditorStyles.miniButton)) //TODO move this...
			{
				editIndex = -1;
				drawIndex = -1;
				newSpring = null;
				newSubComponentState = AddSubComponentState.initiated;
			}

			if(newSubComponentState != AddSubComponentState.inactive)
			{
				EditorGUILayout.HelpBox("Click near any Point Mass on the Jello Body" +
				                        "\nEsc to cancel", MessageType.Info);
			}


		}

		//TODO this might work better in a popup menu...
		if(springBody.SpringCount != 0) 
		{
			EditorGUI.indentLevel++;
			DrawShapeMatchingGUI();
			DrawEdgeSpringsGUI();
			DrawInternalSpringsGUI();
			DrawCustomSpringsGUI();
			EditorGUI.indentLevel--;
		}

		if(newSubComponentState != AddSubComponentState.inactive )
		{
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			{
				newSubComponentState = AddSubComponentState.inactive;
				mainEditor.Repaint();
			}
		}

		//springBodyEditor.serializedObject.ApplyModifiedProperties();
	}


	public void DrawEditSpringGUI(SerializedProperty springArray, int index)
	{
		SerializedProperty eSpringStiffness = springArray.GetArrayElementAtIndex(index).FindPropertyRelative("stiffness");
		SerializedProperty eSpringDamping = springArray.GetArrayElementAtIndex(index).FindPropertyRelative("damping");
		SerializedProperty eSpringMultiplier = springArray.GetArrayElementAtIndex(index).FindPropertyRelative("lengthMultiplier");

		EditorGUILayout.PropertyField(eSpringStiffness, stiffnessContent);
		EditorGUILayout.PropertyField(eSpringDamping, dampingContent);
		
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eSpringMultiplier, lengthMultiplierContent);
		if(EditorGUI.EndChangeCheck()) 
			SceneView.RepaintAll();
	}

	public void DrawShapeMatchingGUI()
	{
		GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
		if(eShapeSpringK.prefabOverride || eShapeSpringDamp.prefabOverride || eShapeMatching.prefabOverride)
			foldoutStyle.fontStyle = FontStyle.Bold;

		showShapeMatchingFoldout = EditorGUILayout.Foldout(showShapeMatchingFoldout, shapeMatchingFoldoutContent, foldoutStyle);

		if(showShapeMatchingFoldout)
		{	
			EditorGUI.showMixedValue = eShapeMatching.hasMultipleDifferentValues;
			EditorGUILayout.PropertyField(eShapeMatching, shapeMatchingContent);
			EditorGUI.showMixedValue = false;
			
			if(eShapeMatching.boolValue)
			{
				EditorGUI.indentLevel++;
				
				EditorGUI.showMixedValue = eShapeSpringK.hasMultipleDifferentValues;
				EditorGUILayout.PropertyField(eShapeSpringK, stiffnessContent);
				EditorGUI.showMixedValue = false;
				
				EditorGUI.showMixedValue = eShapeSpringDamp.hasMultipleDifferentValues;
				EditorGUILayout.PropertyField(eShapeSpringDamp, dampingContent);
				EditorGUI.showMixedValue = false;
				
				EditorGUI.indentLevel--;
			}
		}
	}

	public void DrawEdgeSpringsGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eEdgeSprings, edgeSpringsContent);
		if(EditorGUI.EndChangeCheck())
		{
			if(eEdgeSprings.isExpanded)
			{
				eInternalSprings.isExpanded = false;
				eCustomSprings.isExpanded = false;
			}
			drawIndex = -1;
			editIndex = -1;
			SceneView.RepaintAll();
		}

		if(eEdgeSprings.isExpanded)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();

			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
			
			if(eEdgeSpringK.prefabOverride || eEdgeSpringDamp.prefabOverride)
				foldoutStyle.fontStyle = FontStyle.Bold;

			showEdgeDefaultsFoldout = EditorGUILayout.Foldout(showEdgeDefaultsFoldout, edgeDefaultsContent, foldoutStyle);

			if(GUILayout.Button(setDefaultEdgeContent, EditorStyles.miniButton))
			{
				for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
				{
					JelloSpringBody t = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
					
					t.setEdgeSpringConstants(t.DefaultEdgeSpringStiffness, t.DefaultEdgeSpringDamping);
				}

				EditorUtility.SetDirty(springBody);
			}
			EditorGUILayout.EndHorizontal ();

			if(showEdgeDefaultsFoldout)
			{	
				EditorGUI.showMixedValue = eEdgeSpringK.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eEdgeSpringK, stiffnessContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultEdgeSpringStiffness = eEdgeSpringK.floatValue;
					}

					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;
				
				EditorGUI.showMixedValue = eEdgeSpringDamp.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eEdgeSpringDamp, dampingContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultEdgeSpringDamping = eEdgeSpringDamp.floatValue;
					}

					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;	
			}

			EditorGUILayout.Separator();

			if(multiEditing)
			{
				EditorGUILayout.HelpBox("Springs may not be edited when multiple GameObjects are selected", MessageType.Info);
				EditorGUI.indentLevel--;
				return;
			}
		
			int offset = 0;// zero because nothing comes before edgesprings.

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));

			for(int i = 0; i < springBody.EdgeSpringCount; i++)
			{
				EditorGUILayout.BeginHorizontal();
				
				string text = "Spring # " + (i + offset) ;
				
				string tooltip = "PMA: " + springBody.getEdgeSpring(i).pointMassA +
					" \nPMB: " + springBody.getEdgeSpring(i).pointMassB +
						"\nRest Length: " + springBody.getEdgeSpring(i).length +
						"\nSpring K: " + springBody.getEdgeSpring(i).stiffness +
						"\nDamping: " + springBody.getEdgeSpring(i).damping;

				SerializedProperty eSpring = eEdgeSprings.GetArrayElementAtIndex(i);
				eSpring.isExpanded = editIndex == i + offset;
			
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eSpring, new GUIContent(text, tooltip));
				if(EditorGUI.EndChangeCheck())
				{
					if(!eSpring.isExpanded)
						editIndex = -1;
					else if(eSpring.isExpanded)
					{
						editIndex = i + offset;
						handlePositions[0] = springBody.Shape.getVertex(springBody.getEdgeSpring(i).pointMassA);
						handlePositions[1] = springBody.Shape.getVertex(springBody.getEdgeSpring(i).pointMassB);
					}
					SceneView.RepaintAll();
				}
				
				if(Event.current.type == EventType.Repaint && GUI.tooltip != prevTooltip)
				{
					//mouse out
					if(prevTooltip != "")
						drawIndex = -1;
					
					//mouse over
					if(GUI.tooltip != "")
						drawIndex = i + offset;//offset this in internal springs and in custom springs...
					
					prevTooltip = GUI.tooltip;
					
					SceneView.RepaintAll();
				}
				
				if(GUILayout.Button(deleteSpringContent, EditorStyles.miniButton, GUILayout.Width(20f)))
				{
					springBody.RemoveSpring(springBody.getEdgeSpring(i));

					if(editIndex > i + offset)
						editIndex--;
					else if(editIndex == i + offset)
						editIndex = -1;
					if(drawIndex > i + offset)
						drawIndex--;
					else if (editIndex == i + offset)
						drawIndex = -1;

					EditorUtility.SetDirty(springBody);

					break;
				}
				
				EditorGUILayout.EndHorizontal();	
				
				if(editIndex == i + offset) //offset this in internal and custom
				{
					DrawEditSpringGUI(eEdgeSprings, i);
				}
			}
			EditorGUILayout.EndScrollView();
			EditorGUI.indentLevel--;
		}
	}



	public void DrawInternalSpringsGUI()
	{
		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eInternalSprings, internalSpringsContent);
		if(EditorGUI.EndChangeCheck())
		{
			if(eInternalSprings.isExpanded)
			{
				eEdgeSprings.isExpanded = false;
				eCustomSprings.isExpanded = false;
			}
			drawIndex = -1;
			editIndex = -1;
			SceneView.RepaintAll();
		}

		if(GUILayout.Button(buildSpringsContent, EditorStyles.miniButton))//TODO make sure this plays well with serialized object...
		{
			springBody.clearInternalSprings();
			springBody.BuildInternalSprings();

			EditorUtility.SetDirty(springBody);
		}
		
		EditorGUILayout.EndHorizontal();


		if(eInternalSprings.isExpanded)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();

			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
			if(eInternalSpringK.prefabOverride || eInternalSpringDamp.prefabOverride)
				foldoutStyle.fontStyle = FontStyle.Bold;

			showInternalDefaultsFoldout = EditorGUILayout.Foldout(showInternalDefaultsFoldout, internalDefaultsContent, foldoutStyle);

			if(GUILayout.Button(setDefaultInternalContent, EditorStyles.miniButton))
			{
				for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
				{
					JelloSpringBody t = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
					
					t.setInternalSpringConstants(t.DefaultInternalSpringStiffness, t.DefaultInternalSpringDamping);
				}

				EditorUtility.SetDirty(springBody);
			}
			EditorGUILayout.EndHorizontal();

			if(showInternalDefaultsFoldout)
			{	
				EditorGUI.showMixedValue = eInternalSpringK.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eInternalSpringK, stiffnessContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultInternalSpringStiffness = eInternalSpringK.floatValue;
					}

					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;
				
				EditorGUI.showMixedValue = eInternalSpringDamp.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eInternalSpringDamp, dampingContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultInternalSpringDamping = eInternalSpringDamp.floatValue;
					}
					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;
			}

			EditorGUILayout.Separator();

			if(multiEditing)
			{
				EditorGUILayout.HelpBox("Springs may not be edited when multiple GameObjects are selected", MessageType.Info);
				EditorGUI.indentLevel--;
				return;
			}

			int offset = springBody.EdgeSpringCount;// internal springs come after edge springs.

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));

			for(int i = 0; i < springBody.InternalSpringCount; i++)
			{
				EditorGUILayout.BeginHorizontal();
				
				string text = "Spring # " + (i + offset);
				
				string tooltip = "PMA: " + springBody.getInternalSpring(i).pointMassA +
					" \nPMB: " + springBody.getInternalSpring(i).pointMassB +
						"\nRest Length: " + springBody.getInternalSpring(i).length +
						"\nSpring K: " + springBody.getInternalSpring(i).stiffness +
						"\nDamping: " + springBody.getInternalSpring(i).damping;

				SerializedProperty eSpring = eInternalSprings.GetArrayElementAtIndex(i);
				eSpring.isExpanded = editIndex == i + offset;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eSpring, new GUIContent(text, tooltip));
				if(EditorGUI.EndChangeCheck())
				{
					if(!eSpring.isExpanded)
						editIndex = -1;
					else if(eSpring.isExpanded)
					{
						editIndex = i + offset;
						handlePositions[0] = springBody.Shape.getVertex(springBody.getInternalSpring(i).pointMassA);
						handlePositions[1] = springBody.Shape.getVertex(springBody.getInternalSpring(i).pointMassB);
					}
					SceneView.RepaintAll();
				}
				
				if(Event.current.type == EventType.Repaint && GUI.tooltip != prevTooltip)
				{
					//mouse out
					if(prevTooltip != "")
						drawIndex = -1;
					
					//mouse over
					if(GUI.tooltip != "")
						drawIndex = i + offset;//offset this in internal springs and in custom springs...
					
					prevTooltip = GUI.tooltip;
					
					SceneView.RepaintAll();
				}
				
				if(GUILayout.Button(deleteSpringContent, EditorStyles.miniButton, GUILayout.Width(20f)))
				{
					springBody.RemoveSpring(springBody.getInternalSpring(i));

					if(editIndex > i + offset)
						editIndex--;
					else if(editIndex == i + offset)
						editIndex = -1;
					if(drawIndex > i + offset)
						drawIndex--;
					else if (editIndex == i + offset)
						drawIndex = -1;

					EditorUtility.SetDirty(springBody);

					break;
				}
				
				EditorGUILayout.EndHorizontal();	
				
				if(editIndex == i + offset) //offset this in internal and custom
				{
					DrawEditSpringGUI(eInternalSprings, i);
				}
			}
			EditorGUILayout.EndScrollView();
			EditorGUI.indentLevel--;
		}
	}

	public void DrawCustomSpringsGUI()
	{
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(eCustomSprings, customSpringsContent);
		if(EditorGUI.EndChangeCheck())
		{
			if(eCustomSprings.isExpanded)
			{
				eEdgeSprings.isExpanded = false;
				eInternalSprings.isExpanded = false;
			}
			drawIndex = -1;
			editIndex = -1;
			SceneView.RepaintAll();
		}

		if(eCustomSprings.isExpanded)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal ();

			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
			if(eCustomSpringK.prefabOverride || eCustomSpringDamp.prefabOverride)
				foldoutStyle.fontStyle = FontStyle.Bold;

			showCustomDefaultsFoldout = EditorGUILayout.Foldout(showCustomDefaultsFoldout, customDefaultsContent, foldoutStyle);

			if(GUILayout.Button (setDefaultCustomContent, EditorStyles.miniButton))
			{
				for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
				{
					JelloSpringBody t = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
					
					t.setCustomSpringConstants(t.DefaultCustomSpringStiffness, t.DefaultCustomSpringDamping);
				}

				EditorUtility.SetDirty(springBody);
			}
			EditorGUILayout.EndHorizontal ();


			if(showCustomDefaultsFoldout)
			{
				EditorGUI.showMixedValue = eCustomSpringK.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eCustomSpringK, stiffnessContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultCustomSpringStiffness = eCustomSpringK.floatValue;
					}

					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;
				
				EditorGUI.showMixedValue = eCustomSpringDamp.hasMultipleDifferentValues;
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eCustomSpringDamp, dampingContent);
				if(EditorGUI.EndChangeCheck())
				{
					for(int i = 0; i < springBodyEditor.serializedObject.targetObjects.Length; i++)
					{
						JelloSpringBody sb = (JelloSpringBody)springBodyEditor.serializedObject.targetObjects[i];
						sb.DefaultCustomSpringDamping = eCustomSpringDamp.floatValue;
					}

					EditorUtility.SetDirty(springBody);
				}
				EditorGUI.showMixedValue = false;
			}

			EditorGUILayout.Separator();

			if(multiEditing)
			{
				EditorGUILayout.HelpBox("Springs may not be edited when multiple GameObjects are selected", MessageType.Info);
				EditorGUI.indentLevel--;
				return;
			}

			int offset = springBody.EdgeSpringCount + springBody.InternalSpringCount;// custom springs come after edge springs and internal springs.

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false, GUILayout.MinHeight(minScrollViewHeight));

			for(int i = 0; i < springBody.CustomSpringCount; i++)
			{
				EditorGUILayout.BeginHorizontal();
				
				string text = "Spring # " + (i + offset);
				
				string tooltip = "PMA: " + springBody.getCustomSpring(i).pointMassA +
					" \nPMB: " + springBody.getCustomSpring(i).pointMassB +
						"\nRest Length: " + springBody.getCustomSpring(i).length +
						"\nSpring K: " + springBody.getCustomSpring(i).stiffness +
						"\nDamping: " + springBody.getCustomSpring(i).damping;

				SerializedProperty eSpring = eCustomSprings.GetArrayElementAtIndex(i);
				eSpring.isExpanded = editIndex == i + offset;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(eSpring, new GUIContent(text, tooltip));
				if(EditorGUI.EndChangeCheck())
				{
					if(!eSpring.isExpanded)
						editIndex = -1;
					else if(eSpring.isExpanded)
					{
						editIndex = i + offset;
						handlePositions[0] = springBody.Shape.getVertex(springBody.getCustomSpring(i).pointMassA);
						handlePositions[1] = springBody.Shape.getVertex(springBody.getCustomSpring(i).pointMassB);
					}
					SceneView.RepaintAll();
				}
				
				if(Event.current.type == EventType.Repaint && GUI.tooltip != prevTooltip)
				{
					//mouse out
					if(prevTooltip != "")
						drawIndex = -1;
					
					//mouse over
					if(GUI.tooltip != "")
						drawIndex = i + offset;//offset this in internal springs and in custom springs...
					
					prevTooltip = GUI.tooltip;
					
					SceneView.RepaintAll();
				}
				
				if(GUILayout.Button(deleteSpringContent, EditorStyles.miniButton, GUILayout.Width(20f)))
				{
					springBody.RemoveSpring(springBody.getCustomSpring(i));

					if(editIndex > i + offset)
						editIndex--;
					else if(editIndex == i + offset)
						editIndex = -1;
					if(drawIndex > i + offset)
						drawIndex--;
					else if (editIndex == i + offset)
						drawIndex = -1;

					EditorUtility.SetDirty(springBody);

					break;
				}
				
				EditorGUILayout.EndHorizontal();	
				
				if(editIndex == i + offset) //offset this in internal and custom
				{
					DrawEditSpringGUI(eCustomSprings, i);
				}
			}
			EditorGUILayout.EndScrollView();
			EditorGUI.indentLevel--;
		}
	}


	public override void DrawSceneGUI ()
	{
		if(springBody == null || multiEditing)
			return;

		//draw the hovered over spring
		if(drawIndex != -1)
		{	
			JelloSpring spring = springBody.getSpring(drawIndex);
			Vector3 posA;
			Vector3 posB;
			
			posA = springBody.transform.TransformPoint(springBody.Shape.getVertex(spring.pointMassA));
			posB = springBody.transform.TransformPoint(springBody.Shape.getVertex(spring.pointMassB));
				
			Handles.color = Color.magenta;
				
			if(spring.lengthMultiplier != 1f)
			{
				float dist = Vector2.Distance(posA, posB) * spring.lengthMultiplier;
				Vector3 mid = (posA + posB) * 0.5f;
				posA = mid + (mid - posA).normalized * dist * 0.5f;
				posB = mid + (mid - posB).normalized * dist * 0.5f;
			}
				
			Handles.DrawLine(posA,posB);
		}
			
		//TODO make it remember the selected spring?
		//draw the currently selected spring
		if(editIndex != -1 && editIndex < springBody.SpringCount)
		{
			JelloSpring spring = springBody.getSpring(editIndex);
			Handles.color = Color.cyan;

			Vector3[] globalHandlePositions = new Vector3[2];
			for(int i = 0; i < handlePositions.Length; i++)
				globalHandlePositions[i] = springBody.transform.TransformPoint(handlePositions[i]);
			CalculateHandleSizes(handlePositions);

			bool mouseUp = false;
			
			if(Event.current.type == EventType.mouseUp)
				mouseUp = true;
				
			for(int i = 0; i < handlePositions.Length; i++)
				handlePositions[i] = springBody.transform.InverseTransformPoint( Handles.FreeMoveHandle(springBody.transform.TransformPoint(handlePositions[i]), Quaternion.identity, handleSizes[i], Vector3.zero, Handles.CircleCap));
			//handlePositions[1] = springBody.transform.InverseTransformPoint( Handles.FreeMoveHandle(springBody.transform.TransformPoint(handlePositions[1]), Quaternion.identity, HandleUtility.GetHandleSize(handlePositions[1]) * 0.15f, Vector3.zero, Handles.CircleCap));
				
			Handles.color = Color.magenta;
			Handles.DrawLine(springBody.transform.TransformPoint(handlePositions[0]), springBody.transform.TransformPoint(handlePositions[1]));
				
			if(mouseUp)
			{
				if((Vector2)handlePositions[0] != springBody.Shape.getVertex(spring.pointMassA))
				{
					Vector2[] points = new Vector2[springBody.Shape.VertexCount];
					for(int i = 0; i < springBody.Shape.VertexCount; i++)
						points[i] = springBody.Shape.getVertex(i);
					
					spring.pointMassA = JelloShapeTools.FindClosestVertexOnShape(handlePositions[0], points);
					
					handlePositions[0] = springBody.Shape.getVertex(spring.pointMassA);
					
					spring.length = Vector2.Distance(springBody.Shape.getVertex(spring.pointMassA), springBody.Shape.getVertex(spring.pointMassB));

					EditorUtility.SetDirty(springBody);
				}
				
				if((Vector2)handlePositions[1] != springBody.Shape.getVertex(spring.pointMassB))
				{
					Vector2[] points = new Vector2[springBody.Shape.VertexCount];
					for(int i = 0; i < springBody.Shape.VertexCount; i++)
						points[i] = springBody.Shape.getVertex(i);
					
					spring.pointMassB = JelloShapeTools.FindClosestVertexOnShape(handlePositions[1], points);
					
					handlePositions[1] = springBody.Shape.getVertex(spring.pointMassB);
					
					spring.length = Vector2.Distance(springBody.Shape.getVertex(spring.pointMassA), springBody.Shape.getVertex(spring.pointMassB));

					EditorUtility.SetDirty(springBody);
				}
			}
				
			Vector3 posA = springBody.transform.TransformPoint(springBody.Shape.getVertex(spring.pointMassA));
			Vector3 posB = springBody.transform.TransformPoint(springBody.Shape.getVertex(spring.pointMassB));
				
			if(spring.lengthMultiplier != 1f)
			{
				float dist = Vector2.Distance(posA, posB) * spring.lengthMultiplier;
				Vector3 mid = (posA + posB) * 0.5f;
				posA = mid + (mid - posA).normalized * dist * 0.5f;
				posB = mid + (mid - posB).normalized * dist * 0.5f;
			}
			
			Handles.color = Color.blue;
			
			Handles.DrawLine(posA,posB);
		}
			
		if(newSubComponentState != AddSubComponentState.inactive)
		{
			
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			{
				newSubComponentState = AddSubComponentState.inactive;
			}
			
			
			int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
			
			if(Event.current.type == EventType.Layout)
				HandleUtility.AddDefaultControl(controlID);
			
			Handles.color = Color.red;
			
			Vector3 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin; //need where this ray intersects the zplane
			Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, springBody.transform.position.z));
			Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			float dist = 0f;
			plane.Raycast(ray, out dist);
			pos = ray.GetPoint(dist);
			Vector3 mousePosWorld = new Vector3(pos.x, pos.y, springBody.transform.position.z);
			
			if(newSubComponentState == AddSubComponentState.assignedFirst)
			{
				pos = springBody.transform.TransformPoint(springBody.Shape.getVertex(newSpring.pointMassA));
				Handles.CircleCap(3, pos, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.15f);
				Handles.DrawLine( pos, mousePosWorld);
				
				Handles.color = Color.blue;
				Handles.CircleCap(3, mousePosWorld, Quaternion.identity, HandleUtility.GetHandleSize(mousePosWorld) * 0.15f);
				
				if(Event.current.type == EventType.MouseUp)
				{
					Vector2[] points = new Vector2[springBody.Shape.VertexCount];
					for(int i = 0; i < springBody.Shape.VertexCount; i++)
						points[i] = springBody.Shape.getVertex(i);
					
					newSpring.pointMassB = JelloShapeTools.FindClosestVertexOnShape(springBody.transform.InverseTransformPoint(mousePosWorld), points);
					newSpring.length = Vector2.Distance(springBody.Shape.getVertex(newSpring.pointMassA), springBody.Shape.getVertex(newSpring.pointMassB));
					newSpring.stiffness = springBody.DefaultCustomSpringStiffness;
					newSpring.damping = springBody.DefaultCustomSpringDamping;
					
					editIndex = springBody.SpringCount;
					springBody.addCustomSpring(newSpring);
					newSubComponentState = AddSubComponentState.inactive;

					handlePositions[0] = springBody.Shape.getVertex(newSpring.pointMassA);
					handlePositions[1] = springBody.Shape.getVertex(newSpring.pointMassB);

					eCustomSprings.isExpanded = true;
					eEdgeSprings.isExpanded = false;
					eInternalSprings.isExpanded = false;

					EditorUtility.SetDirty(springBody);
				}
			}
			if(newSubComponentState == AddSubComponentState.initiated)
			{
				Handles.CircleCap(3, mousePosWorld, Quaternion.identity, HandleUtility.GetHandleSize(mousePosWorld) * 0.15f);
				
				if(Event.current.type == EventType.MouseUp)
				{
					Vector2[] points = new Vector2[springBody.Shape.VertexCount];
					for(int i = 0; i < springBody.Shape.VertexCount; i++)
						points[i] = springBody.Shape.getVertex(i);
					
					newSpring = new JelloSpring();
					newSpring.pointMassA = JelloShapeTools.FindClosestVertexOnShape(springBody.transform.InverseTransformPoint(mousePosWorld), points);
					newSubComponentState = AddSubComponentState.assignedFirst;
				}
			}
			
			SceneView.RepaintAll();
		}
		
		if(newSubComponentState != AddSubComponentState.inactive || editIndex != -1)
		{
			Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			
			for(int i = springBody.EdgeSpringCount; i < springBody.SpringCount; i++)
			{
				if(editIndex == i)
					continue;
				Handles.DrawLine(springBody.transform.TransformPoint(springBody.Shape.getVertex(springBody.getSpring(i).pointMassA)), springBody.transform.TransformPoint(springBody.Shape.getVertex(springBody.getSpring(i).pointMassB)));
			}
			
			mainEditor.DrawPointMasses(springBody, false);
		}
	}




}
