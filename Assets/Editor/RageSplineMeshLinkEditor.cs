/*
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

[CanEditMultipleObjects]
[CustomEditor(typeof(RageSplineMeshLink))]
public class RageSplineMeshLinkEditor : MeshLinkEditor 
{
	GUIContent updateColliderContent = new GUIContent("Update", "Update the mesh to include internal points");//TODO this should only be for updating the mesh to show the internal point.

	protected override void DrawInspectorGUI ()
	{//TODO serialize???
		base.DrawInspectorGUI();

		serializedObject.Update();

		if(GUILayout.Button(updateColliderContent, EditorStyles.miniButton))
		{
			Undo.RecordObjects(serializedObject.targetObjects, "RSML update");
			 
			for(int i = 0; i < serializedObject.targetObjects.Length; i++)
			{
				RageSplineMeshLink link = (RageSplineMeshLink)serializedObject.targetObjects[i];

				Undo.RecordObject(link, "RSML update");
				Undo.RecordObject(link.LinkedMeshFilter.sharedMesh, "RSML update");
				Undo.RecordObject(link.body, "RSML update");

				link.Initialize(true);
				
				EditorUtility.SetDirty(link);
				EditorUtility.SetDirty(link.LinkedMeshFilter);
				EditorUtility.SetDirty(link.body);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

}
