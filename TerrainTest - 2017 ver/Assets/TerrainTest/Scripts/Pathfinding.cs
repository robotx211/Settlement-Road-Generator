//classes for A* pathfinding on the settlement data
//NOTE - NOT USED DUE TO INEFFICIENCY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//nodes to represent the coordinates of the settlement data
class Node
{
	public Vector2Int pos;

	public Node parent;

	public float cost; //based on steepness and if already a road
	public float totalCost; //total cost of the path so far
	public float pathLength; //total length of the path so far

	public int hWeight; //manhattan distance to goal
	public float gWeight; //based on the total cost of the path, and the length of the path so far
	public float fWeight; //sum of hWeight and gWeight

	//terrain specific values
	public float height; //height of current node on terrain
	public float steepness; //steepness of current node on terrain

	public bool isRoad; //is current node already a road
	public bool isBuilding; //is current node a building

	public float defaultCost;

	public void SetParent (Node _parent)
	{
		parent = _parent;
	}

	public void SetDefaultCost(float _defaultCost)
	{
		defaultCost = _defaultCost;
	}
	public void CalcCost()
	{
		if (isBuilding == true)
		{
			cost = 0.0f;
		}

		if (isRoad == true) {
			cost = 0.0f;
		} else {
			cost = defaultCost  /*+ (steepness / 90.0f)*/ ;
		}
	}
	public void CalcTotalCost()
	{
		if (parent != null)
		{
			totalCost = parent.totalCost + cost;
		}
		else
		{
			totalCost = cost;
		}
	}
	public void CalcPathDistance()
	{
		if (parent != null)
		{
			if (Mathf.Abs (pos.x - parent.pos.x) + Mathf.Abs (pos.y - parent.pos.y) == 1)
			{
				pathLength = 1.0f + parent.pathLength;
			}
			if (Mathf.Abs (pos.x - parent.pos.x) + Mathf.Abs (pos.y - parent.pos.y) == 2)
			{
				pathLength = 1.4142f + parent.pathLength;
			}
		}
		else
		{
			pathLength = 0.0f;
		}
	}

	public void CalcHWeight (Vector2Int _goalPos)
	{
		hWeight = Mathf.Abs (pos.x - _goalPos.x) + Mathf.Abs (pos.y - _goalPos.y);
	}
	public void CalcGWeight ()
	{
		CalcPathDistance ();

		CalcCost ();
		CalcTotalCost ();

		gWeight = pathLength + totalCost;
	}
	public void CalcFWeight()
	{
		fWeight = hWeight + gWeight;
	}  //could include CalcHWeight and CalcGWeight into this, as they should all be calculated at the same time

	public Node()
	{
		pos = Vector2Int.zero;
		parent = null;

		cost = 0.0f;
		totalCost = 0.0f;
		pathLength = 0.0f;

		hWeight = 0;
		gWeight = 0.0f;
		fWeight = 0.0f;

		height = 0.0f;
		steepness = 0.0f;

		isRoad = false;
		isBuilding = false;
	}
	public Node(Vector2Int _pos, float _height, float _steepness, bool _isRoad, bool _isBuilding)
	{
		pos = _pos;
		parent = null;

		cost = 0.0f;
		totalCost = 0.0f;
		pathLength = 0.0f;

		hWeight = 0;
		gWeight = 0.0f;
		fWeight = 0.0f;

		height = _height;
		steepness = _steepness;

		isRoad = _isRoad;
		isBuilding = _isBuilding;
	}

	public void ResetNode()
	{
		parent = null;

		cost = 0.0f;
		totalCost = 0.0f;
		pathLength = 0.0f;

		hWeight = 0;
		gWeight = 0.0f;
		fWeight = 0.0f;
	}
}

class NodeManager
{
	public int height;
	public int width;
	public List<List<Node>> nodeMap; //eg. (3,7) = nodeMap[7][3]

	public void InitNodeMap (SettlementData _settlementData, float _nodeCost)
	{
		height = _settlementData.height;
		width = _settlementData.width;

		nodeMap = new List<List<Node>> ();

		for (int y = 0; y < height; y++)
		{

			List<Node> tempList = new List<Node> ();

			for (int x = 0; x < width; x++)
			{
				Node tempNode = new Node ();
				tempNode.pos = _settlementData.GetDataAt (x, y).pos;

				tempNode.height = _settlementData.GetDataAt(x,y).height;
				tempNode.steepness = _settlementData.GetDataAt(x,y).steepness;

				tempNode.isRoad = _settlementData.GetDataAt(x,y).IsRoad ();
				tempNode.isBuilding = _settlementData.GetDataAt(x,y).IsBuilding ();

				tempNode.SetDefaultCost (_nodeCost);

				tempList.Add (tempNode);
			}

			nodeMap.Add (tempList);
		}


		Debug.Log ("DEBUG");
	}

	public void ClearNodeMap ()
	{
		for (int y = 0; y < nodeMap.Count; y++)
		{
			for (int x = 0; x < nodeMap [y].Count; x++)
			{
				nodeMap [y] [x].ResetNode ();
			}
		}
	}

	public Node GetNode (int _x, int _y)
	{
		return nodeMap [_y] [_x];
	}

}

//class which performs the pathfinding
class Pathfinding 
{

	public Vector2Int startPos;
	public Vector2Int goalPos;

	public Node startNode;
	public Node goalNode;
	public Node currentNode;
	public Node successorNode;

	public NodeManager currentNodeManager;

	public List<Node> openNodes;
	public List<Node> closedNodes;

	public float maxSteepness = 90.0f;

	//single pathfinding stuff
	public bool goalFound;
	bool finished;

	int maxX;
	int maxY;

	public Node GetNodeExpansion (int _xShift, int _yShift)
	{
		return currentNodeManager.GetNode (currentNode.pos.x + _xShift, currentNode.pos.y + _yShift);
	}
	public float FindGWeightDiff (int _xShift, int _yShift)
	{
		if (_xShift + _yShift == 0 || _xShift + _yShift == 2 || _xShift + _yShift == -2)
		{
			return 1.4142f;
		}
		else
		{
			return 1.0f;
		}
	}

	public void Init ()
	{
		startPos = Vector2Int.zero;
		goalPos = Vector2Int.zero;

		currentNode = null;
		successorNode = null;

		currentNodeManager = null;

		openNodes = new List<Node> ();
		closedNodes = new List<Node> ();
	}
	public void Setup (Vector2Int _start, Vector2Int _goal, SettlementData _settlementData, float _nodeCost, float _maxRoadSteepness)
	{
		startPos = _start;
		goalPos = _goal;

		currentNodeManager = new NodeManager ();
		currentNodeManager.InitNodeMap (_settlementData, _nodeCost);

		maxSteepness = _maxRoadSteepness;

	}

	public void InitOpen ()
	{
		openNodes.Clear ();
	}
	public void InitClosed()
	{
		closedNodes.Clear ();
	}

	//node sorting function
	public int FWeightLessCompare (Node _node1, Node _node2)
	{
		//if node1 FWeight is less, return -1
		//if node2 FWeight is less, return 1
		//if they are the same, return 0

		if (_node1.fWeight < _node2.fWeight)
		{
			return -1;
		}
		if (_node1.fWeight > _node2.fWeight)
		{
			return 1;
		}
		if (_node1.fWeight == _node2.fWeight)
		{
			return 0;
		}
		return 0;
	}

	public void SetFirstNode()
	{
		goalFound = false;

		startNode = currentNodeManager.GetNode (startPos.x, startPos.y);
		goalNode = currentNodeManager.GetNode (goalPos.x, goalPos.y);

		maxX = currentNodeManager.width;
		maxY = currentNodeManager.height;

		currentNode = null;

		startNode.CalcHWeight (goalPos);
		startNode.CalcGWeight ();
		startNode.CalcFWeight ();

		openNodes.Add (startNode);


	}

	public bool NextNodeCheck()
	{

		//check each open node, and expand them using A* expansion
		if (openNodes.Count != 0 && goalFound == false)
		{
			currentNode = openNodes [0];

			//Debug.Log ("Current Node: " + currentNode.pos);

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (!((currentNode.pos.x + x) < 0 ||
					    (currentNode.pos.x + x) > maxX ||
					    (currentNode.pos.y + y) < 0 ||
					    (currentNode.pos.y + y) > maxY))
					{
						if (AStarNodeExpansion (x, y) == true)
						{
							Debug.Log ("Goal Found");
							goalFound = true;
							break;
						}
					}
				}

				if (goalFound == true)
				{
					break;
				}
			}

			closedNodes.Add (currentNode);
			openNodes.RemoveAt (0);

			openNodes.Sort (FWeightLessCompare);
		}
		else if (goalFound == true)
		{
			Debug.Log ("Path Found! :)");
			return true;
		}
		else if (openNodes.Count == 0)
		{
			Debug.Log ("No Path Found! :(");
			return true;
		}

		//is pathfinding done or not?
		return false;
	}

	//calculates whether a node should be expanded, based on it's distance from the goal and the total path distance so far
	bool AStarNodeExpansion(int _xShift, int _yShift)
	{

		successorNode = GetNodeExpansion (_xShift, _yShift);

		if (successorNode.isBuilding == true)
		{
			return false;
		}
		if (successorNode.steepness > maxSteepness) 
		{
			return false;
		}

		if (openNodes.Contains (successorNode) == false &&
		    closedNodes.Contains (successorNode) == false)
		{
			successorNode.SetParent (currentNode);

			if (successorNode == goalNode)
			{
				Debug.Log ("Goal Found");
				return true;
			}
			else
			{
				successorNode.CalcHWeight (goalPos);
				successorNode.CalcGWeight ();
				successorNode.CalcFWeight ();
				openNodes.Add (successorNode);
			}
		}
		else if (openNodes.Contains (successorNode) == true &&
		         closedNodes.Contains (successorNode) == false)
		{
			if (FindGWeightDiff (_xShift, _yShift) + successorNode.cost + currentNode.gWeight < successorNode.gWeight)
			{
				successorNode.SetParent (currentNode);
				successorNode.CalcGWeight ();
				successorNode.CalcFWeight ();
			}
		}
		else if (openNodes.Contains (successorNode) == false &&
		         closedNodes.Contains (successorNode) == true)
		{
			if (FindGWeightDiff (_xShift, _yShift) + successorNode.cost + currentNode.gWeight < successorNode.gWeight)
			{
				successorNode.SetParent (currentNode);
				successorNode.CalcGWeight ();
				successorNode.CalcFWeight ();

				openNodes.Add (successorNode);
				closedNodes.Remove (successorNode);
			}
		}
		else if (openNodes.Contains (successorNode) == true &&
		         closedNodes.Contains (successorNode) == true)
		{
			Debug.LogError ("NODE " + successorNode.pos + " IN BOTH OPEN AND CLOSED!");
		}

		return false;
	}

	//returns a path of terrain coordinates
	public List<Vector2Int> GetPath ()
	{

		if (goalFound != true)
		{
			Debug.Log ("No path to return");
			return null;
		}

		List<Vector2Int> rtnPath = new List<Vector2Int> ();

		currentNode = currentNodeManager.GetNode (goalPos.x, goalPos.y);

		//write path backwards, following parents from goal to start

		while (currentNode != startNode)
		{
			rtnPath.Add (currentNode.pos);
			currentNode = currentNode.parent;
		}

		//add start node
		rtnPath.Add (currentNode.pos);

		//reverse the path, so it goes from start to goal
		rtnPath.Reverse ();

		return rtnPath;
	}


}

