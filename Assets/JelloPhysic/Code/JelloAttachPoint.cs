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
using System;

/// <summary>
/// The JelloAttachPoint class may be used to peg a transform to a point on a JelloBody using one to three JelloPointMass objects of the JelloBody.
/// This is useful for adding things like eyes and mouths to the JelloBody. 
/// </summary>
[Serializable]
public class JelloAttachPoint
{
	#region PUBLIC VARIABLES

	/// <summary>
	/// The JelloPointMass indices that this JelloAttachPoint are pegged to.
	/// up to three.
	/// </summary>
	public int[] affectedIndices;
	
	/// <summary>
	/// The angle the JelloAttachPoint.AttachedTransform should be kept offset from the JelloBody.Angle.
	/// </summary>
	public float transformAngle = 0f;

	/// <summary>
	/// The source JelloBody used to derive the JelloAttachPoint.point.
	/// </summary>
	public JelloBody body;
	
	/// <summary>
	/// The source JelloBody transform.
	/// </summary>
	public Transform transform;
	
	/// <summary>
	/// The position that the JelloAttachPoint.AttachedTransform will be moved to.
	/// This value is stored local to the JelloAttachPoint.body.
	/// </summary>
	public Vector2 point;
	
	/// <summary>
	/// Wheter to rotate the JelloAttachPoint.AttachPoint with JelloAttachPoint.body.
	/// </summary>
	public bool rotate = false;

	/// <summary>
	/// The weight of each JelloPointMass.Position in deriving the JelloAttachPoint.point.
	/// This array should be the same lenght as the JelloAttachPoint.affectedIndices array. 
	/// </summary>
	public float[] scalars;

	#endregion

	#region PRIVATE VARIABLES

	
	/// <summary>
	/// The transform to be moved by this JelloAttachPoint.
	/// </summary>
	[SerializeField]
	private Transform mAttachedTransform;
	
	#endregion

	#region PUBLIC PROPERTIES

	/// <summary>
	/// Get or set the transform that will be moved by this JelloAttachPoint .
	/// </summary>
	/// <value>The attached transform.</value>
	public Transform AttachedTransform
	{
		get{ return mAttachedTransform; }
		set
		{
			mAttachedTransform = value;
			if(value != null)
				transformAngle = mAttachedTransform.localEulerAngles.z - body.Angle;
		}
	}

	#endregion

	#region CONSTRUCTORS

	/// <summary>
	/// Default Constructor
	/// </summary>
	public JelloAttachPoint(){}

	/// <summary>
	/// Constructor.
	/// Will default to using 2 JelloPointMass objects as "legs".
	/// </summary>
	/// <param name="attachPoint">The point (local to the JelloAttachPoint.body) at which to attach the JelloAttachPoint.AttachedTransform.</param>
	/// <param name="jelloBody">The JelloBody to to be attached to.</param>
	/// <param name="useBaseShape">Whether to use the  JelloBody.Shape positions (instead of JelloPointMass.Position) when building the JelloAttachPoint.</param>
	public JelloAttachPoint(Vector2 attachPoint, JelloBody jelloBody, bool useBaseShape = true)
	{
		Rebuild(attachPoint, jelloBody, useBaseShape);
	}

	/// <summary>
	/// Constructor.
	/// Will default to 2 using 2 JelloPointMAss objects as "Legs" if the indices array is erroneous or null.
	/// </summary>
	/// <param name="attachPoint">The point (local to the JelloAttachPoint.body) at which to attach the JelloAttachPoint.AttachedTransform.</param>
	/// <param name="jelloBody">The JelloBody to to be attached to.</param>
	/// <param name="indeces">The JelloPointMass Indices. Should have a length of 1, 2, or 3.</param>
	/// <param name="useBaseShape">Whether to use the  JelloBody.Shape positions (instead of JelloPointMass.Position) when building the JelloAttachPoint.</param>
	public JelloAttachPoint(Vector2 attachPoint, JelloBody jelloBody, int[] indeces, bool useBaseShape = true)
	{
		Rebuild (attachPoint, jelloBody, indeces, useBaseShape);
	}

	#endregion

	#region REBUILD

	/// <summary>
	/// Rebuild this JelloAttachPoint.
	/// </summary>
	/// <param name="attachPoint">The point (local to the JelloAttachPoint.body) at which to attach the JelloAttachPoint.AttachedTransform.</param>
	/// <param name="jelloBody">The JelloBody to to be attached to.</param>
	/// <param name="useBaseShape">Whether to use the  JelloBody.Shape positions (instead of JelloPointMass.Position) when building the JelloAttachPoint.</param>
	/// <param name="numLegs">Number of JelloPointMass objects to use as "legs" (use 1, 2, or 3).</param>
	public void Rebuild(Vector2 attachPoint, JelloBody jelloBody, bool useBaseShape = true, int numLegs = 0)
	{
		body = jelloBody;
		transform = body.transform;
		
		if(numLegs != 0)
		{
			if(numLegs < 0)
				numLegs = 1;
			if(numLegs > 3)
				numLegs = 3;
			
			affectedIndices = new int[numLegs];
		}
		else if(affectedIndices == null || affectedIndices.Length == 0 || affectedIndices.Length > 3)
			affectedIndices = new int[2];//default to 3?
		
		Vector2[] shape = new Vector2[body.Shape.VertexCount];
		if(useBaseShape)
		{
			for(int i = 0; i < shape.Length; i++)
				shape[i] = body.Shape.getVertex(i);
		}
		else
		{
			attachPoint = transform.TransformPoint(attachPoint);
			
			for(int i = 0; i < shape.Length; i++)
				shape[i] = body.getPointMass(i).Position;
		}
		
		if(affectedIndices.Length == 1)
		{
			affectedIndices = JelloShapeTools.GetClosestIndices(attachPoint, shape, 1);
			scalars = new float[1];
			scalars[0] = 1f;
		}
		else if(affectedIndices.Length == 2)
		{
			Vector2 hit;
			affectedIndices = JelloShapeTools.FindClosestEdgeOnShape(attachPoint, shape);
			scalars = new float[2];
			JelloVectorTools.getClosestPointOnSegmentSquared (attachPoint, shape[affectedIndices[0]], shape[affectedIndices[1]], out hit, out scalars[1]);
			scalars[0] = 1 - scalars[1];
		}
		else if(affectedIndices.Length == 3)
		{
			Vector2[] shapePerimeter = new Vector2[body.EdgePointMassCount];
			if(useBaseShape)
			{
				shapePerimeter = body.Shape.EdgeVertices;
			}
			else
			{
				for(int i = 0; i < shapePerimeter.Length; i++)
					shapePerimeter[i] = body.getEdgePointMass(i).Position;
			}
			
			affectedIndices = JelloShapeTools.FindContainingTriangle(attachPoint, shape, shapePerimeter, body.Shape.Triangles, out scalars);
		}
		
		point = Vector2.zero;
		for(int i = 0; i < affectedIndices.Length; i++)
			point += shape[affectedIndices[i]] * scalars[i];
		
		
		if(!useBaseShape)
			point = transform.InverseTransformPoint(point);
		
		if(mAttachedTransform != null)
		{
			Vector3 newPos = transform.TransformPoint(point);
			newPos.z = mAttachedTransform.position.z;
			mAttachedTransform.position = newPos;
		}
		
	}
	
	/// <summary>
	/// Rebuild this JelloAttachPoint.
	/// </summary>
	/// <param name="attachPoint">The point (local to the JelloAttachPoint.body) at which to attach the JelloAttachPoint.AttachedTransform.</param>
	/// <param name="jelloBody">The JelloBody to to be attached to.</param>
	/// <param name="indices">The JelloPointMass Indices. Should have a length of 1, 2, or 3.</param>
	/// <param name="useBaseShape">Whether to use the  JelloBody.Shape positions (instead of JelloPointMass.Position) when building the JelloAttachPoint.</param>
	public void Rebuild(Vector2 attachPoint, JelloBody jelloBody, int[] indices, bool useBaseShape = true)
	{
		body = jelloBody;
		transform = body.transform;
		
		if(indices == null)
		{
			if(affectedIndices == null)
			{
				Rebuild(attachPoint, jelloBody, useBaseShape);
				return;
			}
			else
			{
				indices = affectedIndices;
			}
		}
		else if(indices.Length > 4)
		{
			affectedIndices = new int[3];
			for(int i = 0; i < 3; i++)
				affectedIndices[i] = indices[i];
		}
		else
		{
			affectedIndices = indices;
		}
		
		Vector2[] verts = new Vector2[3];
		
		if(useBaseShape)
		{
			for(int i = 0; i < affectedIndices.Length; i++)
				verts[i] = body.Shape.getVertex(affectedIndices[i]); 
		}
		else
		{
			attachPoint = transform.TransformPoint(attachPoint);
			for(int i = 0; i < affectedIndices.Length; i++)
				verts[i] = body.getPointMass(affectedIndices[i]).Position;
		}
		
		if(affectedIndices.Length == 1)
		{
			scalars = new float[1];
			scalars[0] = 1f;
		}
		else if(affectedIndices.Length == 2)
		{
			Vector2 hit;
			
			scalars = new float[2];
			JelloVectorTools.getClosestPointOnSegmentSquared (attachPoint, verts[0], verts[1], out hit, out scalars[1]);
			scalars[0] = 1 - scalars[1];
		}
		else if (affectedIndices.Length == 3)
		{
			scalars = JelloShapeTools.GetBarycentricCoords(attachPoint, verts);
		}
		
		//throw into for loop...
		point = Vector2.zero;
		for(int i = 0; i < affectedIndices.Length; i++)
			point += scalars[i] * verts[i];
		
		if(!useBaseShape)
			point = transform.InverseTransformPoint(point);
		
		if(mAttachedTransform != null)
		{
			Vector3 newPos = transform.TransformPoint(point);
			newPos.z = mAttachedTransform.position.z;
			mAttachedTransform.position = newPos;
		}
	}

	#endregion


	#region UPDATE

	/// <summary>
	/// Update this JelloAttachPoint .
	/// Will derive the new position for the JelloAttachPoint.point, AttachTransform.Position and AttachedTrasnform.Angle.
	/// This will be called by the simulation and should not need to be called manualy.
	/// </summary>
	/// <param name="useBaseShape">Whether to use the JelloBody.Shape positions to determine the new position.</param>
	public Vector2 Update(bool useBaseShape)
	{
		point = Vector2.zero;
		if(useBaseShape)
		{
			for(int i = 0; i < affectedIndices.Length; i++)
				point += body.Shape.getVertex(affectedIndices[i]) * scalars[i];

			if(point.x == float.NaN)
			{
				for(int i = 0; i < affectedIndices.Length; i++)
					Debug.Log (body.Shape.getVertex(affectedIndices[i]) + "     " + scalars[i]);
			}
		}
		else
		{
			for(int i = 0; i < affectedIndices.Length; i++)
				point += body.getPointMass(affectedIndices[i]).Position * scalars[i];

			point = transform.InverseTransformPoint(point);

			if(point.x == float.NaN)
			{
				for(int i = 0; i < affectedIndices.Length; i++)
					Debug.Log (body.getPointMass(affectedIndices[i]).Position + "     " + scalars[i]);
			}
		}

	
		if(mAttachedTransform != null)
		{
			Vector3 newPos = transform.TransformPoint(point);
			newPos.z = mAttachedTransform.position.z;
			mAttachedTransform.position = newPos;
			if(rotate)
			{
				float angle = 0;
				int originalSign = 1;
				float originalAngle = 0;
				float thisAngle;
				Vector2 center = Vector2.zero;
				Vector2 centerGlobal = Vector2.zero;

				if(affectedIndices.Length == 1)
				{
					centerGlobal = body.Position;
				}
				else
				{
					for(int i = 0; i < affectedIndices.Length; i++)
						center += body.Shape.getVertex(affectedIndices[i]);
					center /= affectedIndices.Length;
					
					centerGlobal = Vector2.zero;
					for(int i = 0; i < affectedIndices.Length; i++)
						centerGlobal += body.getPointMass(affectedIndices[i]).Position;
					centerGlobal /= affectedIndices.Length;

				}

				for (int i = 0; i < affectedIndices.Length; i++)
				{
					thisAngle = Vector2.Angle(body.Shape.getVertex(affectedIndices[i]) - center, body.getPointMass(affectedIndices[i]).Position - centerGlobal);//(Vector2)body.transform.TransformPoint(center));
					if(Vector3.Cross(body.Shape.getVertex(affectedIndices[i]) - center, body.getPointMass(affectedIndices[i]).Position - centerGlobal).z < 0f)//(Vector2)body.transform.TransformPoint(center)).z < 0f)
						thisAngle*= -1f;
					
					if (i == 0)
					{
						originalSign = (thisAngle >= 0f) ? 1 : -1;
						originalAngle = thisAngle;
					}
					else
					{
						if ((Mathf.Abs(thisAngle - originalAngle) > 180f) && ( (thisAngle >= 0f ? 1 : -1) != originalSign))
						{
							thisAngle =  ((thisAngle >= 0f ? 1 : -1) == -1) ? 360f + thisAngle : - thisAngle;
						}
					}
					
					angle += thisAngle;
				}
				
				angle /= affectedIndices.Length;

				mAttachedTransform.eulerAngles = new Vector3(mAttachedTransform.eulerAngles.x, mAttachedTransform.eulerAngles.y, angle + transformAngle);
			}
		}

		return point;
	}

	/// <summary>
	/// Update this JelloAttachPoint in editor mode.
	/// This is used by the custom editory.
	/// </summary>
	/// <returns>The upated position.</returns>
	public Vector2 UpdateEditorMode()
	{
		Vector2 returnPoint = Vector2.zero;
		for(int i = 0; i < affectedIndices.Length; i++)
			returnPoint += body.Shape.getVertex(affectedIndices[i]) * scalars[i];

		transform = body.transform;

		if(mAttachedTransform != null)
		{
			Vector3 globalPos = transform.TransformPoint(returnPoint);
			globalPos.z = mAttachedTransform.position.z;
			mAttachedTransform.position = globalPos;

			if(rotate)
				mAttachedTransform.eulerAngles = new Vector3(mAttachedTransform.eulerAngles.x, mAttachedTransform.eulerAngles.y, transformAngle + body.Angle);

		}
		
		return returnPoint;
	}

	#endregion
}