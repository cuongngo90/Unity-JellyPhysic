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


[CustomEditor(typeof(JelloWorld))]
public class JelloWorldEditor : Editor
{

	private JelloWorld tar;
	private static bool showCollisionSettings;
	private static bool showSleepSettings;
	private static bool showDebugSettings;

	private GUIContent iterationsContent = new GUIContent("Solver Iterations");//, "Forces will be solved this many times over Time.FixedDeltaTime" + 
	                                                        //"\nA larger number will increase stability of the simulation but will hurt performance");
	private GUIContent collisionFoldoutContent = new GUIContent("Collision Settings");//, "Collision forces are simulated via springs" + 
	                                                            //"\nFold out to adjust collision response values");
	private GUIContent sleepFoldoutContent = new GUIContent("Sleep Settings");//, "Fold out to adjust JelloBody sleep settings");
	private GUIContent debugFoldoutContent = new GUIContent("Debug Settings");//, "Fold out to select debug settings");
	private GUIContent sleepVelocityContent = new GUIContent("Velocity");//, "Bodies at or below this velocity and set angular velocity will start to fall asleep");
	private GUIContent sleepAngularVelocityContent = new GUIContent("Angular Velocity");//, "Bodies at or below this angular velocity and set velocity will start to fall asleep");
	private GUIContent sleepCountContent = new GUIContent("Count");//, "Bodies at or below set velocity and set angular velocity for this many iterations will be put to sleep");
	private GUIContent wakeVelocityContent = new GUIContent("Wake Velocity");
	private GUIContent wakeAngularVelocityContent = new GUIContent("Wake Angular Velocity");
	private GUIContent debugPositionsContent = new GUIContent("Outline");//, "Draw an outline of the body");
	private GUIContent debugVelocitiesContent = new GUIContent("Velocity");//, "Draw the velocity of each point mass");
	private GUIContent debugShapeContent = new GUIContent("Base Shape");//, "Draw an outline of the base shape of the body");
	private GUIContent debugTransformedShapeContent = new GUIContent("Shape(Xformed)");//, "Draw an outline of the base shape xformed by scale/rotation");
	private GUIContent debugSpringsContent = new GUIContent("Springs");//, "Draw all springs"); //TODO color separate between normal and custom
	private GUIContent debugPressureContent = new GUIContent("Pressure");//, "Draw edge normals (direction of pressure forces)");
	private GUIContent debugShapeMatchingContent = new GUIContent("Shape Matching");//, "Draw Xformed base shape displacement used to calculate shape matching forces");
	private GUIContent defaultMaterialContent = new GUIContent("Default Physics Material");//, "This physics material will be used by default for colliders withoug a physics material selected.");
	private GUIContent penetrationContent = new GUIContent("Penetration Threshold");
	private GUIContent penetrationTangentContent = new GUIContent("Tangent Penetration Threshold");
	private GUIContent mtvContent = new GUIContent("MTV Modifier");

	void OnEnable()
	{
		tar = (JelloWorld)serializedObject.targetObject;
	}

	public override void OnInspectorGUI()
	{
		tar.Iterations = EditorGUILayout.IntField(iterationsContent, tar.Iterations);

		tar.defaultPhysicsMaterial = (PhysicsMaterial2D)EditorGUILayout.ObjectField(defaultMaterialContent, tar.defaultPhysicsMaterial, typeof(PhysicsMaterial2D), true);

		showCollisionSettings = EditorGUILayout.Foldout(showCollisionSettings, collisionFoldoutContent);

		if(showCollisionSettings)
		{
			EditorGUI.indentLevel++;

			tar.PenetrationThreshold = EditorGUILayout.FloatField(penetrationContent, tar.PenetrationThreshold);
			tar.TangentPenetrationThreshold = EditorGUILayout.FloatField(penetrationTangentContent, tar.TangentPenetrationThreshold);
			tar.mtvModifierGlobal = EditorGUILayout.FloatField(mtvContent, tar.mtvModifierGlobal);
		
			EditorGUI.indentLevel--;
		}

		showSleepSettings = EditorGUILayout.Foldout(showSleepSettings, sleepFoldoutContent);

		if(showSleepSettings)
		{
			EditorGUI.indentLevel++;

			tar.sleepVelocity = EditorGUILayout.FloatField(sleepVelocityContent, tar.sleepVelocity);
			tar.sleepAngularVelocity = EditorGUILayout.FloatField(sleepAngularVelocityContent, tar.sleepAngularVelocity);
			tar.sleepCntRequired = EditorGUILayout.IntField(sleepCountContent, tar.sleepCntRequired);
			tar.wakeVelocity = EditorGUILayout.FloatField(wakeVelocityContent, tar.wakeVelocity);
			tar.wakeAngularVelocity = EditorGUILayout.FloatField(wakeAngularVelocityContent, tar.wakeAngularVelocity);

			EditorGUI.indentLevel--;
		}

		showDebugSettings = EditorGUILayout.Foldout(showDebugSettings, debugFoldoutContent);

		if(showDebugSettings)
		{
			EditorGUI.indentLevel++;

			tar.showPointMassPositions = EditorGUILayout.Toggle(debugPositionsContent, tar.showPointMassPositions);
			tar.showVelocities = EditorGUILayout.Toggle(debugVelocitiesContent, tar.showVelocities);
			tar.showBaseShape = EditorGUILayout.Toggle(debugShapeContent, tar.showBaseShape);
			tar.showXformedBaseShape = EditorGUILayout.Toggle(debugTransformedShapeContent, tar.showXformedBaseShape);
			tar.showSprings = EditorGUILayout.Toggle(debugSpringsContent, tar.showSprings);
			tar.showPressureNormals = EditorGUILayout.Toggle(debugPressureContent, tar.showPressureNormals);
			tar.showShapeMatching = EditorGUILayout.Toggle(debugShapeMatchingContent, tar.showShapeMatching);

			EditorGUI.indentLevel--;
		}

		//DrawDefaultInspector();
	}
}
