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

/// <summary>
/// The JelloJoint class allows you to connect a JelloBody to another JelloBody, a Rigidbody2D, a Collider2D, or a fixed point in global space.
/// You may use from one to three JelloPointMass objects on each JelloBody to affect and be affected by the JelloJoint.
/// This class is meant to be used without at least one JelloBody.
/// </summary>
[System.Serializable]
public class JelloJoint
{

	//TODO implement distance.
	#region PUBLIC VARIABLES

	/// <summary>
	/// The first Rigidbody2D connected by this JelloJoint.
	/// </summary>
	public Rigidbody2D rigidbodyA;

	/// <summary>
	/// The second Rigidbody2D connected by this JelloJoint.
	/// </summary>
	public Rigidbody2D rigidbodyB;

	/// <summary>
	/// The first JelloBody connected by this JelloJoint.
	/// </summary>
	public JelloBody bodyA;

	/// <summary>
	/// The second JelloBody connected by this JelloJoint.
	/// </summary>
	public JelloBody bodyB;

	/// <summary>
	/// The indices of the Affected / affecting JelloPointMass objects for JelloJoint.bodyA.
	/// May be 1, 2, or 3.
	/// </summary>
	public int[] affectedIndicesA;
	
	/// <summary>
	/// The indices of the Affected / affecting JelloPointMass objects for JelloJoint.bodyB.
	/// May be 1, 2, or 3.
	/// </summary>
	public int[] affectedIndicesB;

	/// <summary>
	/// The weight of each JelloPointMass.Position / JelloPointMass.velocity for JelloJoint.bodyA.
	/// This array should be the same lenght as JelloJoint.affectedIndicesA.
	/// </summary>
	public float[] scalarsA;

	/// <summary>
	/// The weight of each JelloPointMass.Position / JelloPointMass.velocity for JelloJoint.bodyB.
	/// This array should be the same lenght as JelloJoint.affectedIndicesB.
	/// </summary>
	public float[] scalarsB;

	/// <summary>
	/// The first anchor, local to JelloJoint.TransformA.
	/// </summary>
	public Vector2 localAnchorA;

	/// <summary>
	/// The second anchor, local to JelloJoint.TransformB.
	/// </summary>
	public Vector2 localAnchorB;

	/// <summary>
	/// The first anchor in global coordinates.
	/// </summary>
	public Vector2 globalAnchorA;

	/// <summary>
	/// The second anchor in global coordinates
	/// </summary>
	public Vector2 globalAnchorB;

	/// <summary>
	/// Whether this JelloJoint is breakable.
	/// </summary>
	public bool breakable;

	/// <summary>
	/// The relative velocity at which this JelloJoint breaks.
	/// </summary>
	public float breakVelocity = 5f;

	/// <summary>
	/// The number of similar JelloJoint objects. 
	/// Used when multiple JelloJoints connect a JelloBody to a Rigidbody2D.
	/// </summary>
	public int numSimilar = 1;

	/// <summary>
	/// The JelloJoint has been destroyed (exceeded JelloJoint.breakVelocity).
	/// </summary>
	public bool destroyed;

	#endregion


	#region PRIVATE/PROTECTED VARIABLES

	/// <summary>
	/// The first transform connected by this JelloJoint.
	/// </summary>
	[SerializeField]
	protected Transform mTransformA;
	
	/// <summary>
	/// The second transform connected by this JelloJoint.
	/// </summary>
	[SerializeField]
	protected Transform mTransformB;

	#endregion

	#region PUBLIC PROPERTIES

	/// <summary>
	/// Get or set the first transform this JelloJoint connects to.
	/// Rigidbody2D and JelloBody references are set as well.
	/// </summary>
	/// <value>The first transform.</value>
	public Transform TransformA
	{
		get
		{
			return mTransformA;
		}
		set
		{
			if(value == mTransformB)
				mTransformA = null;
			else
				mTransformA = value;
			
			if(mTransformA == null)
			{
				rigidbodyA = null;
				bodyA = null;
				scalarsA = null;
				localAnchorA = Vector2.zero;
			}
			else
			{
				rigidbodyA = mTransformA.GetComponent<Rigidbody2D>();
				bodyA = mTransformA.GetComponent<JelloBody>();
			}
			//todo consider setting up some more defaults?
		}
	}

	/// <summary>
	/// Get or set the second transform this JelloJoint connects to.
	/// Rigidbody2D and JelloBody references are set as well.
	/// </summary>
	/// <value>The second transform.</value>
	public Transform TransformB
	{
		get
		{
			return mTransformB;
		}
		set
		{
			if(value == mTransformA)
				mTransformB = null;
			else
				mTransformB = value;
			
			
			if(mTransformB == null)
			{
				rigidbodyB = null;
				bodyB = null;
				scalarsB = null;
				localAnchorB = Vector2.zero;
			}
			else
			{
				rigidbodyB = mTransformB.GetComponent<Rigidbody2D>();
				bodyB = mTransformB.GetComponent<JelloBody>();
			}
			//todo consider setting up some more defaults?
		}
	}


	#endregion

	#region CONSTRUCTORS

	/// <summary>
	/// The default constructor.
	/// </summary>
	public JelloJoint(){}

	/// <summary>
	/// The Constructor.
	/// Automatically uses the center of each Transform as its local anchor point.
	/// Will default to using 2 affected indices for JelloBody objects.
	/// </summary>
	/// <param name="xformA">The first Transform.</param>
	/// <param name="xformB">The second Transform.</param>
	public JelloJoint(Transform xformA, Transform xformB)
	{
		TransformA = xformA;
		localAnchorA = Vector2.zero;
		SetupAnchor(xformA, localAnchorA, true, true);
		
		TransformB = xformB;
		localAnchorB = Vector2.zero;
		SetupAnchor(xformB, localAnchorB, false, true);
	}

	/// <summary>
	/// Highly configurable Constructor.
	/// </summary>
	/// <param name="xformA">The first Transform.</param>
	/// <param name="xformB">The second Transform.</param>
	/// <param name="anchorA">The first anchor (Local to the first Transform).</param>
	/// <param name="anchorB">The second Anchor (Local to the second Transform).</param>
	/// <param name="useBaseShapeA">Whether to use JelloBody.Shape (instead of its JelloPointMass objects) of the first JelloBody. Has no effect if first Transform has no JelloBody attached.</param>
	/// <param name="useBaseShapeB">Whether to use JelloBody.Shape (instead of its JelloPointMass objects) of second JelloBody. Has no effect if second Transform has no JelloBody attached.</param>
	/// <param name="numAffectedIndicesA">The number affected indices for first JelloBody. Has no effect if first Transform has no JelloBody attached.</param>
	/// <param name="numAffectedIndicesB">Number affected indices for second body. Has no effect if second Transform has no JelloBody attached.</param>
	public JelloJoint(Transform xformA, Transform xformB, Vector2 anchorA, Vector2 anchorB, bool useBaseShapeA, bool useBaseShapeB, int numAffectedIndicesA = 2, int numAffectedIndicesB = 2)
	{
		TransformA = xformA;
		SetupAnchor(xformA, anchorA, true, useBaseShapeA, numAffectedIndicesA);
		TransformB = xformB;
		SetupAnchor(xformB, anchorB, false, useBaseShapeB, numAffectedIndicesB);
	}

	#endregion

	#region SETUP/REBUILD

	//anchor is local //vertices are local //have vector2 return?
	/// <summary>
	/// Rebuilds the anchor.
	/// </summary>
	/// <param name="anchor">The anchor (In local space).</param>
	/// <param name="isAnchorA">Whether to rebuild the first anchor as opposed to rebuilding the second anchor.</param>
	/// <param name="useBaseShape">Whether use JelloBody.Shape instead of JelloPointMass.Position. Has no effect if Transform has no JelloBody attached.</param>
	/// <param name="affectedIndices">The indices of the affected / affecting JelloPointMass objects. Has no effect if Transform has no JelloBody attached.</param>
	/// <param name="affectedVertices">The positions (in local space) of the affected / affecting JelloPointMass objects. Has no effect if Transform has no JelloBody attached.</param>
	public void RebuildAnchor(Vector2 anchor, bool isAnchorA, bool useBaseShape, int[] affectedIndices = null, Vector2[] affectedVertices = null)
	{
		//Vector2 point;

		if(isAnchorA)
		{
			localAnchorA = anchor;

			if(mTransformA == null)
				return;

			if(bodyA != null)
			{
				if(affectedIndices == null)
					affectedIndices = affectedIndicesA;
				else
					affectedIndicesA = affectedIndices;

				if(affectedVertices == null)//grab from point mass positions?
				{
					if(useBaseShape)
					{
						affectedVertices = new Vector2[affectedIndicesA.Length];
						for(int i = 0; i < affectedIndicesA.Length; i++)
							affectedVertices[i] = bodyA.Shape.getVertex(affectedIndicesA[i]);
					}
					else
					{
						affectedVertices = new Vector2[affectedIndicesA.Length];
						for(int i = 0; i < affectedIndicesA.Length; i++)
							affectedVertices[i] = bodyA.getPointMass(affectedIndicesA[i]).LocalPosition;
					}
				}

				if(affectedIndices != null)
				{
					if(affectedIndices.Length == 1)
					{
						scalarsA = new float[1];
						scalarsA[0] = 1f;
					}
					else if(affectedIndices.Length == 2)
					{
						Vector2 hit;

						scalarsA = new float[2];
						JelloVectorTools.getClosestPointOnSegmentSquared (localAnchorA, affectedVertices[0], affectedVertices[1], out hit, out scalarsA[1]);
						scalarsA[0] = 1 - scalarsA[1];
					}
					else if(affectedIndices.Length == 3)
					{
						scalarsA = JelloShapeTools.GetBarycentricCoords(localAnchorA, affectedVertices);
					}
				}
			}
		}
		else
		{
			localAnchorB = anchor;
			
			if(mTransformB == null)
				return;
			
			if(bodyB != null)
			{
				if(affectedIndices == null)
					affectedIndices = affectedIndicesB;
				else
					affectedIndicesB = affectedIndices;
				
				if(affectedVertices == null)//grab from point mass positions?
				{
					if(useBaseShape)
					{
						affectedVertices = new Vector2[affectedIndicesB.Length];
						for(int i = 0; i < affectedIndicesB.Length; i++)
							affectedVertices[i] = bodyB.Shape.getVertex(affectedIndicesB[i]);
					}
					else
					{
						affectedVertices = new Vector2[affectedIndicesB.Length];
						for(int i = 0; i < affectedIndicesB.Length; i++)
							affectedVertices[i] = bodyB.getPointMass(affectedIndicesB[i]).LocalPosition;
					}
				}
				
				if(affectedIndices != null)
				{
					if(affectedIndices.Length == 1)
					{
						scalarsB = new float[1];
						scalarsB[0] = 1f;
					}
					else if(affectedIndices.Length == 2)
					{
						Vector2 hit;
						
						scalarsB = new float[2];
						JelloVectorTools.getClosestPointOnSegmentSquared (localAnchorB, affectedVertices[0], affectedVertices[1], out hit, out scalarsB[1]);
						scalarsB[0] = 1 - scalarsB[1];
					}
					else if(affectedIndices.Length == 3)
					{
						scalarsB = JelloShapeTools.GetBarycentricCoords(localAnchorB, affectedVertices);
					}
				}
			}
		}
	}

	/// <summary>
	/// Set up the anchor.
	/// </summary>
	/// <returns>The anchor's position.</returns>
	/// <param name="xform">The Transform of the anchor.</param>
	/// <param name="anchor">The anchor point, local to the given Transform.</param>
	/// <param name="isAnchorA">Whether to set up the first anchor instead of the second.</param>
	/// <param name="useBaseShape">Whether to use JelloBody.Shape instead of its JelloPointMass objects. Has no effect if no JelloBody is attached to the Transform.</param>
	/// <param name="numPointsAffected">The number of PointMasses affected / affecting this anchor. Has no effect if no JelloBody is attached to the Transform.</param>
	public Vector2 SetupAnchor(Transform xform, Vector2 anchor, bool isAnchorA, bool useBaseShape, int numPointsAffected = 0)
	{
		if(isAnchorA)
			TransformA = xform;
		else
			TransformB = xform;

		if(xform == null)
		{
			if(isAnchorA)
			{
				affectedIndicesA = null;
				scalarsA = null;

				if(TransformB != null)
				{
					globalAnchorA = TransformB.TransformPoint(GetAnchorPointB(useBaseShape));
					return globalAnchorA;
				}
				else
				{
					return Vector2.zero;
				}
			}
			else
			{
				affectedIndicesB = null;
				scalarsB = null;

				if(TransformA != null)
				{
					globalAnchorB = TransformA.TransformPoint(GetAnchorPointA(useBaseShape));
					return globalAnchorB;
				}
				else
				{
					return Vector2.zero;
				}
			}


			//return Vector2.zero;
		}

		Vector2 returnPosition = anchor;

		if(numPointsAffected < 1)
			numPointsAffected = 2;
		else if(numPointsAffected > 3)
			numPointsAffected = 3;

		if(isAnchorA)
		{
			localAnchorA = anchor;

			if(bodyA != null)
			{
				Vector2[] shape = new Vector2[bodyA.Shape.VertexCount];
				if(useBaseShape) //TODO tighten this up a bit
				{
					for(int i = 0; i < shape.Length; i++)
						shape[i] = bodyA.Shape.getVertex(i);
				}
				else
				{
					for(int i = 0; i < bodyA.PointMassCount; i++)
						shape[i] = bodyA.getPointMass(i).Position;
				}

				Vector2 point = localAnchorA;
				if(!useBaseShape)
					point = xform.TransformPoint(point);

				if(numPointsAffected == 1)
				{
					affectedIndicesA = JelloShapeTools.GetClosestIndices(point, shape, 1);
					scalarsA = new float[1];
					scalarsA[0] = 1f;
					returnPosition = shape[affectedIndicesA[0]];
				}
				else if(numPointsAffected == 2)
				{
					Vector2 hit;
					affectedIndicesA = JelloShapeTools.FindClosestEdgeOnShape(point, shape);
					scalarsA = new float[2];
					JelloVectorTools.getClosestPointOnSegmentSquared (point, shape[affectedIndicesA[0]], shape[affectedIndicesA[1]], out hit, out scalarsA[1]);
					scalarsA[0] = 1 - scalarsA[1];
					returnPosition = shape[affectedIndicesA[0]] * scalarsA[0] + shape[affectedIndicesA[1]] * scalarsA[1]; 
				}
				else if(numPointsAffected == 3)
				{
					Vector2[] shapePerimeter = new Vector2[bodyA.EdgePointMassCount];
					if(useBaseShape)
					{
						shapePerimeter = bodyA.Shape.EdgeVertices;
					}
					else
					{
						for(int i = 0; i < shapePerimeter.Length; i++)
							shapePerimeter[i] = bodyA.getEdgePointMass(i).Position;
					}
					
					affectedIndicesA = JelloShapeTools.FindContainingTriangle(point, shape, shapePerimeter, bodyA.Shape.Triangles, out scalarsA);

					returnPosition = shape[affectedIndicesA[0]] * scalarsA[0] + shape[affectedIndicesA[1]] * scalarsA[1] + shape[affectedIndicesA[2]] * scalarsA[2];
				}

				if(!useBaseShape)
					returnPosition = mTransformA.InverseTransformPoint(returnPosition);
			}
		}
		else
		{
			localAnchorB = anchor;
			
			if(bodyB != null)
			{
				Vector2[] shape = new Vector2[bodyB.Shape.VertexCount];
				if(useBaseShape)
				{
					for(int i = 0; i < shape.Length; i++)
						shape[i] = bodyB.Shape.getVertex(i);
				}
				else
				{
					for(int i = 0; i < bodyB.PointMassCount; i++)
						shape[i] = bodyB.getPointMass(i).Position;
				}
			
				Vector2 point = localAnchorB;
				if(!useBaseShape)
					point = xform.TransformPoint(point);
				
				if(numPointsAffected == 1)
				{
					affectedIndicesB = JelloShapeTools.GetClosestIndices(point, shape, 1);
					scalarsB = new float[1];
					scalarsB[0] = 1f;
					returnPosition = shape[affectedIndicesB[0]] * scalarsB[0];
				}
				else if(numPointsAffected == 2)
				{
					Vector2 hit;
//					affectedIndicesB = JelloShapeTools.GetClosestIndices(point, shape, 2);
					affectedIndicesB = JelloShapeTools.FindClosestEdgeOnShape(point, shape);
					scalarsB = new float[2];
					JelloVectorTools.getClosestPointOnSegmentSquared (point, shape[affectedIndicesB[0]], shape[affectedIndicesB[1]], out hit, out scalarsB[1]);
					scalarsB[0] = 1 - scalarsB[1];
					returnPosition = shape[affectedIndicesB[0]] * scalarsB[0] + shape[affectedIndicesB[1]] * scalarsB[1];
				}
				else if(numPointsAffected == 3)
				{
					Vector2[] shapePerimeter = new Vector2[bodyB.EdgePointMassCount];
					if(useBaseShape)
					{
						shapePerimeter = bodyB.Shape.EdgeVertices;
					}
					else
					{
						for(int i = 0; i < shapePerimeter.Length; i++)
							shapePerimeter[i] = bodyB.getEdgePointMass(i).Position;
					}
					
					affectedIndicesB = JelloShapeTools.FindContainingTriangle(point, shape, shapePerimeter, bodyB.Shape.Triangles, out scalarsB);

					returnPosition = shape[affectedIndicesB[0]] * scalarsB[0] + shape[affectedIndicesB[1]] * scalarsB[1] + shape[affectedIndicesB[2]] * scalarsB[2];
				}

				if(!useBaseShape)
					returnPosition = mTransformB.InverseTransformPoint(returnPosition);}
		}

		return returnPosition;
	}

	#endregion

	#region SOLVE

	/// <summary>
	/// Solves the JelloJoint and applies velocities to each Rigidbody2D / JelloBody involved.
	/// This is called regularly by the simulation and should not need to be called by the user.
	/// </summary>
	/// <param name="deltaTime">The amount of time elapsed.</param>
	public void Solve(float deltaTime)
	{
		if(mTransformA == null && mTransformB == null)
		{
			destroyed = true;
			return;
		}

		Vector2 vap = Vector2.zero;
		Vector2 vbp = Vector2.zero;
		float aMassSum = Mathf.Infinity;
		float bMassSum = Mathf.Infinity;
		float invma = 0f;
		float invmb = 0f;
		Vector2 posA = Vector2.zero;
		Vector2 posB = Vector2.zero;
		Vector2 directionA = Vector2.zero;
		Vector2 directionB = Vector2.zero;

		if(TransformA != null)
		{
			if(bodyA != null)
			{
				for(int i = 0; i < affectedIndicesA.Length; i++)
				{
					vap += bodyA.getPointMass(affectedIndicesA[i]).velocity * scalarsA[i];
					invma += bodyA.getPointMass(affectedIndicesA[i]).InverseMass * scalarsA[i];
					aMassSum += bodyA.getPointMass(affectedIndicesA[i]).Mass * scalarsA[i];
					posA += bodyA.getPointMass(affectedIndicesA[i]).Position * scalarsA[i];
				}
			}
			else if (rigidbodyA != null)
			{
				posA = mTransformA.TransformPoint(localAnchorA);
				directionA = posA - (Vector2)mTransformA.position;
				vap = rigidbodyA.velocity + rigidbodyA.angularVelocity * Mathf.Deg2Rad * new Vector2(-directionA.y, directionA.x);
				
				if(rigidbodyA.mass != 0f && !rigidbodyA.isKinematic)
				{
					invma = 1f / rigidbodyA.mass;
					aMassSum = rigidbodyA.mass;
				}
			}
			else
			{
				posA = mTransformA.TransformPoint(localAnchorA);
				directionA = posA - (Vector2)mTransformA.position;
			}
		}
		else if(TransformB != null)
		{
			posA = globalAnchorA;
		}
		else
		{
			return;
		}

		if(mTransformB != null)
		{
			if(bodyB != null)
			{
				for(int i = 0; i < affectedIndicesB.Length; i++)
				{
					vbp += bodyB.getPointMass(affectedIndicesB[i]).velocity * scalarsB[i];
					invmb += bodyB.getPointMass(affectedIndicesB[i]).InverseMass * scalarsB[i];
					bMassSum += bodyB.getPointMass(affectedIndicesB[i]).Mass * scalarsB[i];
					posB += bodyB.getPointMass(affectedIndicesB[i]).Position * scalarsB[i];
				}
			}
			else if (rigidbodyB != null)
			{
				posB = TransformB.TransformPoint(localAnchorB);
				directionB = posB - (Vector2)mTransformB.position;
				vbp = rigidbodyB.velocity + rigidbodyB.angularVelocity * Mathf.Deg2Rad * new Vector2(-directionB.y, directionB.x);
				
				if(rigidbodyB.mass != 0f && !rigidbodyB.isKinematic)
				{
					invmb = 1f / rigidbodyB.mass;
					bMassSum = rigidbodyB.mass;
				}
			}
			else
			{
				posB = mTransformB.TransformPoint(localAnchorB);
				directionB = posB - (Vector2)mTransformB.position;
			}
		}
		else if(mTransformA != null)
		{
			posB = globalAnchorB;
		}
		else
		{
			return;
		}


		////////////////////////////////////////////////////////
		//Debug.DrawLine(posA, posB, Color.magenta);
		Vector2 normal = posA - posB; //this isnt normalized...

		float distance = normal.magnitude;

		if(distance != 0f)
			normal /= distance;

		Vector2 vab = vap - vbp;

		float invMomentInertiaA = aMassSum == Mathf.Infinity ? 0f : 1f;
		float invMomentInertiaB = bMassSum == Mathf.Infinity ? 0f : 1f;
		
		float denomA = 0f;
		float denomB = 0f;
		if(bodyA == null && aMassSum != Mathf.Infinity)
		{
			denomA = JelloVectorTools.CrossProduct(directionA, normal);
			denomA *= denomA * invMomentInertiaA;
		}
		if(bodyB == null && bMassSum != Mathf.Infinity)
		{
			denomB = JelloVectorTools.CrossProduct(directionB, normal);
			denomB *= denomB * invMomentInertiaB;
		}
		//constraint impulse scalar
		float j = -Vector2.Dot (vab, normal);

		j -= distance / deltaTime;//TODO assign all distance to bodies based on mass.......? 

		bool destroy = false;
		if(breakable && Mathf.Abs(j) > breakVelocity)
			destroy = true;

		float denom = invma + invmb + denomA + denomB; 
		if(denom == 0f)
			denom = 1f;
		j /= denom;

		////////////////////////////////////////////////////////

		if(bodyA != null)
		{
			for(int i = 0; i < affectedIndicesA.Length; i++)
				bodyA.getPointMass(affectedIndicesA[i]).velocity += j * normal * invma * scalarsA[i];
		}
		else if (rigidbodyA != null && !rigidbodyA.isKinematic)
		{
			rigidbodyA.velocity += j * normal * invma / (numSimilar > 0 ? numSimilar : 1f);
			rigidbodyA.angularVelocity += JelloVectorTools.CrossProduct(directionA, j * normal) * Mathf.Rad2Deg / (numSimilar > 0 ? numSimilar : 1f);
		}
		
		if(bodyB != null)
		{
			for(int i = 0; i < affectedIndicesB.Length; i++)
				bodyB.getPointMass(affectedIndicesB[i]).velocity -= j * normal * invmb * scalarsB[i];
		}
		else if (rigidbodyB != null && !rigidbodyB.isKinematic)
		{
			rigidbodyB.velocity -= j * normal * invmb / (numSimilar > 0 ? numSimilar : 1f);
			rigidbodyB.angularVelocity -= JelloVectorTools.CrossProduct(directionB, j * normal) * Mathf.Rad2Deg / (numSimilar > 0 ? numSimilar : 1f);
		}

		if(destroy)
			Destroy();
	}

	#endregion

	#region HELPER METHODS

	/// <summary>
	/// Get the first anchor point.
	/// Point will be in local space if a JelloJoint.TransformA is not null, otherwise it will be in global space.
	/// </summary>
	/// <returns>The first anchor point.</returns>
	/// <param name="useBaseShape">Whether to use JelloBody.Shape instead of its JelloPointMass objects. Has no effect if JelloJoint.bodyA is null</param>
	public Vector2 GetAnchorPointA(bool useBaseShape)
	{
		Vector2 point = globalAnchorA;
		
		if(mTransformA != null)
		{
			if(bodyA != null)
			{
				point = Vector2.zero;

				if(affectedIndicesA != null && affectedIndicesA.Length > 0)
				{
					if(useBaseShape)
					{
						for(int i = 0; i < affectedIndicesA.Length; i++)
							point += bodyA.Shape.getVertex(affectedIndicesA[i]) * scalarsA[i];
					}
					else
					{
						for(int i = 0; i < affectedIndicesA.Length; i++)
							point += bodyA.getPointMass(affectedIndicesA[i]).Position * scalarsA[i];
						point = TransformA.InverseTransformPoint(point);
					}
				}
			}
			else
			{
				point = localAnchorA;
			}
		}
		
		return point;
	}

	// <summary>
	/// Get the second anchor point.
	/// Point will be in local space if a JelloJoint.TransformB is not null, otherwise it will be in global space.
	/// </summary>
	/// <returns>The second anchor point.</returns>
	/// <param name="useBaseShape">Whether to use JelloBody.Shape instead of its JelloPointMass objects. Has no effect if JelloJoint.bodyB is null</param>
	public Vector2 GetAnchorPointB(bool useBaseShape)
	{
		Vector2 point = globalAnchorB;

		if(mTransformB != null)
		{
			if(bodyB != null)
			{
				point = Vector2.zero;

				if(affectedIndicesB != null && affectedIndicesB.Length > 0)
				{
					if(useBaseShape)
					{
						for(int i = 0; i < affectedIndicesB.Length; i++)
							point += bodyB.Shape.getVertex(affectedIndicesB[i]) * scalarsB[i];
					}
					else
					{
						for(int i = 0; i < affectedIndicesB.Length; i++)
							point += bodyB.getPointMass(affectedIndicesB[i]).Position * scalarsB[i];
						point = TransformB.InverseTransformPoint(point);
					}
				}
			}
			else
			{
				point = localAnchorB;
			}
		}

		return point;
	}

	/// <summary>
	/// Destroy this JelloJoint.
	/// </summary>
	public void Destroy()
	{
		if(bodyA != null)
			bodyA.RemoveJoint(this);
		if(bodyB != null)
			bodyB.RemoveJoint(this);

		destroyed = true;
	}
	
	#endregion
}
