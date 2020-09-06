//class used for pathfinding on a given navmesh

using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.AI;

//makes use of NavMeshComponents, by Unity Technologies, 2017.2 version

public class NavmeshPathfinding
{

	public NavMeshSurface currentNavmesh;

	public void BakeNavmesh ()
	{
		currentNavmesh.BuildNavMesh ();
	}
	public void BakeNavmesh (NavMeshSurface _navMesh)
	{
		currentNavmesh = _navMesh;

		currentNavmesh.BuildNavMesh ();
	}
		
	public bool GetPath (Vector3 _startPos, Vector3 _goalPos, NavMeshPath _path)
	{
		bool returnBool = NavMesh.CalculatePath (_startPos, _goalPos, NavMesh.AllAreas, _path);

		if (returnBool == false)
		{
			Debug.LogError ("No Path Found");
		}

		return returnBool;
	}

}
