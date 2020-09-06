//component used for storing and visualising the settings on premade buildings

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshModifier))]
public class Building : MonoBehaviour {

//	public float width;
//	public float length;

	public float radius;

	public float height;

	public float maxHeight;
	public float maxSteepness;

	//debug
	[HideInInspector] public bool useProximityCondition;
	[HideInInspector] public Vector2Int debugPoint;
	[HideInInspector] public float maxDistanceFromPoint;

	[HideInInspector] public float debugRoadWidth = 8.0f;

	NavMeshModifier navmeshMod;

	void OnDrawGizmos() {

		Gizmos.color = Color.green;
		Gizmos.matrix = transform.localToWorldMatrix;

		Gizmos.DrawWireCube ( new Vector3 (0,0,0), new Vector3( radius * 2.0f, height, radius * 2.0f) );

		Gizmos.matrix = Matrix4x4.identity;

	}

    public float getBaseSize () {

//        if (width > length) {
//            return width;
//        } else {
//            return length;
//        }

		return radius * 2.0f;
    }

    public float getHeightOffset() {

        return height / 2;

    }

	void OnValidate()
	{
		if (this.gameObject == null)
		{
			return;
		}

		if (navmeshMod == null)
		{
			navmeshMod = GetComponent<NavMeshModifier> ();
		}

		navmeshMod.ignoreFromBuild = true;
		navmeshMod.overrideArea = false;
		navmeshMod.area = 0; //walkable, the list is 0 indexed

	}

}
