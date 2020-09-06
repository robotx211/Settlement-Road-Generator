//classes for storing the paths of roads in world spawn, generated using a navmesh

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//need to store paths, in order to connect buildings to the main roads
public class RoadPath
{
	List<Vector3> corners;

	public void SetPath (NavMeshPath _path)
	{
		if (corners == null)
		{
			corners = new List<Vector3> ();
		} else
		{
			corners.Clear ();
		}

		foreach (Vector3 corner in _path.corners)
		{
			corners.Add (corner);
		}
	}
	public List<Vector3> GetPath()
	{
		return corners;
	}

	//returns a path which has been sectioned, so no distance between 2 subsequent points is further away than the max distance
	public List<Vector3> GetSectionedPath(float _maxDistance)
	{
		List<Vector3> returnList = new List<Vector3> ();

		returnList.Add (corners [0]);

		for (int i = 1; i < corners.Count; i++)
		{
			float cornerDistance = Vector3.Distance (corners [i - 1], corners [i]);

			//if the distance between the 2 points is more than the max distance, split it into sections based on maxDistance
			if (cornerDistance > _maxDistance)
			{
				//calculate number of sections
				int sectionCount = Mathf.CeilToInt (cornerDistance / _maxDistance);

				//for each section
				for (int j = 1; j < sectionCount; j++)
				{
					//add a lerped between the 2 corners, stepping through fractions based on the number of sections, e.g. 4 sections, lerp at 1/4, 2/4 and 3/4
					returnList.Add(Vector3.Lerp(corners [i - 1], corners [i], (float)j/(float)sectionCount));
				}

			}
			returnList.Add (corners [i]);
		}

		return returnList;
	}
}

//class for holing all road paths
public class RoadPaths 
{
	List<RoadPath> listOfPaths;

	public void AddPath (NavMeshPath _path)
	{
		if (listOfPaths == null)
		{
			listOfPaths = new List<RoadPath> ();
		}

		RoadPath newPath = new RoadPath ();
		newPath.SetPath (_path);
		listOfPaths.Add (newPath);
	}
	public void ClearPaths()
	{
		listOfPaths.Clear ();
	}

	public List<Vector3> GetSectionedPathPositions(float _maxDistance)
	{
		List<Vector3> returnList = new List<Vector3> ();

		foreach (RoadPath path in listOfPaths)
		{
			returnList.AddRange (path.GetSectionedPath (_maxDistance));
		}

		return returnList;
	}

}
