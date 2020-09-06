//The main script used for settlement generation

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent (typeof(NavMeshSurface))]
public class TerrainChecker : MonoBehaviour
{
    
	#region Terrain Manipulation Functions

	//NOTE - Terrains require at least 2 textures, 1 for a ground texture, 1 for a road texture

	[HideInInspector] public Camera raycastCamera;

	[HideInInspector] public float acceptableSteepness = 30.0f;
	public List<GameObject> buildingList;

	[HideInInspector] public int chosenBuilding;

	[HideInInspector] public int mode;
	//place building [1], draw road [2]

	public Terrain specifiedTerrain;
	[HideInInspector] public Terrain lastInteractedTerrain;

	//scale of buildings to terrain
	public float terrainToWorldScale;

	[HideInInspector] public int roadTextureIndex;

	//should be hidden
	[HideInInspector] public int terrainTexCount;

	//these 2 are only public so they can be seen in inspector (cus ReadOnlyInspector is a Unity 2018 thing)
	[System.Serializable] public class textureSteepnessBounds
	{
		public float minAngle;
		public float maxAngle;
	}

	[HideInInspector] public List<textureSteepnessBounds> textureSteepnessRules;

	//in Unity units
	[HideInInspector] public float fullRoadWidth = 1.0f;
	[HideInInspector] public float maxRoadWidth = 2.0f;

	[HideInInspector] public float maxRoadSteepness = 90.0f;

	//debug
	Vector2Int highestHeightTerrainCoord = Vector2Int.zero;
	List<Vector2Int> checkCoords = new List<Vector2Int> ();

	//debug function used for spawning a building on the terrain
	public void TryPlaceBuilding (Vector2Int _centre, Terrain _terrain)
	{

		float height = _terrain.terrainData.GetHeight (_centre.x, _centre.y);
		float relativeHeight = height / _terrain.terrainData.heightmapScale.y;
		float radiusSize = (Mathf.CeilToInt (buildingList [chosenBuilding].GetComponent<Building> ().getBaseSize () / 2.0f) / terrainToWorldScale);
		int checkSize = Mathf.CeilToInt (radiusSize);

		float heightTemp;
		float highestHeight = 0.0f;

		//debug
		checkCoords.Clear ();

		//checks a square around the position, so not too accurate but fast
		bool steepnessCheckPass = true;
		for (int x = _centre.x - checkSize; x <= _centre.x + checkSize; x++)
		{
			for (int y = _centre.y - checkSize; y <= _centre.y + checkSize; y++)
			{

				Vector2Int checkCoord = new Vector2Int (x, y);
				Vector2 relativeTerrainPos = TerrainCoordToRelativeTerrainPos (checkCoord, _terrain);

				if (x >= 0 && x < _terrain.terrainData.heightmapWidth && y >= 0 && y < _terrain.terrainData.heightmapHeight)
				{
					if (Mathf.Pow (Mathf.Pow (x - _centre.x, 2) + Mathf.Pow (y - _centre.y, 2), 0.5f) <= radiusSize)
					{

						checkCoords.Add (new Vector2Int (x, y));

						if (_terrain.terrainData.GetSteepness (relativeTerrainPos.x, relativeTerrainPos.y) > buildingList [chosenBuilding].GetComponent<Building> ().maxSteepness)
						{
							steepnessCheckPass = false;
							break;
						} 
						heightTemp = _terrain.terrainData.GetHeight (x, y);
						if (heightTemp > highestHeight)
						{
							highestHeight = heightTemp;

							highestHeightTerrainCoord = new Vector2Int (x, y);
						}
					}
				}

			}
		}



		if (steepnessCheckPass == false)
		{
			Debug.Log ("Position " + _centre + ", at height " + height + "is too steep to build on");
			return;
		}

		Debug.Log ("Placing " + buildingList [chosenBuilding].name + " at position " + _centre + ", at height " + height); 

		#region Flatten Terrain

//		List<Vector2Int> potentialCoords = new List<Vector2Int> (); 
//
//		for (int x = centre.x - radiusSize; x <= centre.x + radiusSize; x++) {
//			for (int y = centre.y - radiusSize; y <= centre.y + radiusSize; y++) {
//				
//				if (Mathf.Pow (Mathf.Pow (x - centre.x, 2) + Mathf.Pow (y - centre.y, 2), 0.5f) <= radiusSize) {
//					if (x >= 0 && x < terrain.terrainData.heightmapWidth && y >= 0 && y < terrain.terrainData.heightmapHeight) {
//						potentialCoords.Add (new Vector2Int (x, y));
//					}
//				}
//
//			}
//		}
//
//		//potentialCoords is now a square of width radiusSize * 2, with centre at the centre
//
//		/*
//		for (int i = potentialCoords.Count - 1; i >= 0; i--) {
//
//			Vector2 coord = potentialCoords [i];
//
//			//based on distance between 2 points being d = ( (x2 - x1)^2 + (y2 - y1)^2 )^1/2
//			if ( Mathf.Pow( Mathf.Pow(coord.x - centre.x, 2) + Mathf.Pow(coord.y - centre.y, 2) , 0.5f ) > radiusSize ) {
//				//then the coord lies outside the circle radius radiusSize about the centre
//
//				potentialCoords.RemoveAt (i);
//			}
//
//		}
//		*/
//
//		//potentialCoords now contains all coords which lie within a circle, radius radiusSize, about centre
//
//		float[,] heights = terrain.terrainData.GetHeights (0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
//
//		foreach (Vector2Int coord in potentialCoords) {
//
//			heights [coord.y, coord.x] = highestHeight / terrain.terrainData.heightmapScale.y;
//
//		}
//
//     	terrain.terrainData.SetHeights (0, 0, heights);

		#endregion

		#region Create Foundation

//		GameObject foundation = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
//		foundation.GetComponent<Collider> ().enabled = false;
//		foundation.transform.localScale = new Vector3 (radiusSize * 2.0f, highestHeight / 2.0f, radiusSize * 2.0f);
//
//		//find how much to displace the foundation
//		float d = _terrain.terrainData.GetHeight (_centre.x, _centre.y) - (0.5f * highestHeight);
//
//		foundation.transform.position = TerrainCoordToWorldPos (_centre, _terrain) - new Vector3 (0, d, 0);

		#endregion

		Vector3 spawnPos = TerrainCoordToWorldPos (_centre, _terrain);
		spawnPos.y = highestHeight + buildingList [chosenBuilding].GetComponent<Building> ().getHeightOffset ();

		Instantiate<GameObject> (buildingList [chosenBuilding], spawnPos, Quaternion.identity, _terrain.transform);

	}
	//debug function used for setting a road coordinate on the terrain
	public void TrySetPathTex (Vector2Int _terrainCoordCentre, Terrain _terrain, float _fullWidth, float _maxWidth)
	{
		//work out what terrain coords are to be designated as a road

		int checkSize = Mathf.CeilToInt (_maxWidth / 2);

		float[,,] newAlphamap = new float[1, 1, _terrain.terrainData.alphamapLayers];

		List<Vector2Int> terrainCheckCoords = new List<Vector2Int> ();

		for (int x = _terrainCoordCentre.x - checkSize; x <= _terrainCoordCentre.x + checkSize; x++)
		{
			for (int y = _terrainCoordCentre.y - checkSize; y <= _terrainCoordCentre.y + checkSize; y++)
			{

				terrainCheckCoords.Add (new Vector2Int (x, y));

			}
		}

		List<Vector2Int> textureCoords = TerrainCoordsToTextureCoords (terrainCheckCoords, _terrain);
		Vector2 nonGridTexCoordCentre = TerrainCoordToNonIntTextureCoord (_terrainCoordCentre);

		foreach (Vector2Int texCoord in textureCoords)
		{
			if (texCoord.x < 0 || texCoord.x >= _terrain.terrainData.alphamapWidth || texCoord.y < 0 || texCoord.y >= _terrain.terrainData.alphamapHeight)
			{
				continue;
			}

			float currentTerrainCoordRadius = Mathf.Pow (Mathf.Pow (texCoord.x - nonGridTexCoordCentre.x, 2) + Mathf.Pow (texCoord.y - nonGridTexCoordCentre.y, 2), 0.5f);

			if (currentTerrainCoordRadius <= _fullWidth / 2)
			{

				float[,,] currentAlphamapcoordValues = _terrain.terrainData.GetAlphamaps (texCoord.x, texCoord.y, 1, 1);

				for (int i = 0; i < terrainTexCount; i++)
				{
					if (i == roadTextureIndex)
					{
						newAlphamap [0, 0, i] = 1.0f;
					}
					else
					{
						newAlphamap [0, 0, i] = 0.0f;
					}
				}

				_terrain.terrainData.SetAlphamaps (texCoord.x, texCoord.y, newAlphamap);
	
			}
			else if (currentTerrainCoordRadius <= _maxWidth / 2)
			{
				//currentTerrainCoordRadius
				float percentageRoadFill = (currentTerrainCoordRadius - (_fullWidth / 2.0f)) / ((_maxWidth / 2.0f) - (_fullWidth / 2.0f));

				float[,,] currentAlphamapcoordValues = _terrain.terrainData.GetAlphamaps (texCoord.x, texCoord.y, 1, 1);

				float[] textureValues = new float[terrainTexCount];

				for (int i = 0; i < terrainTexCount; i++)
				{
					textureValues [i] = currentAlphamapcoordValues [0, 0, i];
				}

				textureValues = SetTextureValueAtIndex (textureValues, roadTextureIndex, 1.0f - percentageRoadFill);

				for (int i = 0; i < terrainTexCount; i++)
				{
					currentAlphamapcoordValues [0, 0, i] = textureValues [i];
				}

				_terrain.terrainData.SetAlphamaps (texCoord.x, texCoord.y, currentAlphamapcoordValues);
			}
		}


	}

	public void DrawPath (List<RoadCoord> _roadCoordDatas, Terrain _terrain)
	{

		List<RoadCoord> roadCoordDatasCopy = _roadCoordDatas;

		//go through every texture coord (expensive?) and check if it is close enough to the road, then decide on it's final texture like the TrySetPathTex function

		float[,,] newAlphamap = new float[1, 1, _terrain.terrainData.alphamapLayers];

		//to check which point is closest, compare each texture coord until the closest road coord is found
		//must be within maxWidth of the road coord, is in fullWidth, stop early (full width is always the same texture)
		float distanceToRoad = 0;
		float distanceToSelectedRoad = _terrain.terrainData.alphamapWidth + 1;
		int selectedRoadNodeIndex;
		Vector2Int selectedRoadCoord = new Vector2Int (_terrain.terrainData.heightmapWidth, _terrain.terrainData.heightmapHeight); //in terrain coords
		bool isValid = false;
		bool isFull = false;

		float fullDistance = 0;
		float maxDistance = 0;

		//for each alphamap value
		for (int y = 0; y < _terrain.terrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < _terrain.terrainData.alphamapWidth; x++)
			{


				if (x < 0 || x >= _terrain.terrainData.alphamapWidth || y < 0 || y >= _terrain.terrainData.alphamapHeight)
				{
					continue;
				}



				distanceToRoad = 0;
				distanceToSelectedRoad = _terrain.terrainData.alphamapWidth + 1;
				selectedRoadCoord = new Vector2Int (_terrain.terrainData.heightmapWidth, _terrain.terrainData.heightmapHeight);
				isValid = false;
				isFull = false;

				fullDistance = 0;
				maxDistance = 0;

				if (_roadCoordDatas.Count <= 0)
				{
					Debug.LogError ("No Road Points Added");
					return;
				}

				//for each road coordinate in settlement data, check if it is within the width of a road coord.
				//If it's within the full distance of a road coord, then the road texture value will be set to 1
				//else, the road texture value will be based on the distance from the selected road coord
				for (int i = 0; i < roadCoordDatasCopy.Count; i++)
				{
					distanceToRoad = Vector2.Distance (new Vector2 (x, y), (roadCoordDatasCopy [i].pos - new Vector2 (0.5f, 0.5f)));

					if (distanceToRoad < distanceToSelectedRoad)
					{
						distanceToSelectedRoad = distanceToRoad;
						selectedRoadNodeIndex = i;
						selectedRoadCoord = _roadCoordDatas [i].pos;

						fullDistance = (_roadCoordDatas [i].fullWidth / terrainToWorldScale) / 2.0f;
						maxDistance = (_roadCoordDatas [i].maxWidth / terrainToWorldScale) / 2.0f;

						if (distanceToRoad <= fullDistance)
						{
							//if less than full width, 
							isValid = true;
							isFull = true;
						}
						else if (distanceToRoad <= maxDistance)
						{
							isValid = true;

						}

					}
				}

				if (isValid == false)
				{
					continue;
				}


				if (isFull == true)
				{
					//texture coord is within fullWidth of a road coord, set the road texture value to 1
					float[,,] currentAlphamapcoordValues = _terrain.terrainData.GetAlphamaps (x, y, 1, 1);

					for (int i = 0; i < terrainTexCount; i++)
					{
						if (i == roadTextureIndex)
						{
							newAlphamap [0, 0, i] = 1.0f;
						}
						else
						{
							newAlphamap [0, 0, i] = 0.0f;
						}
					}

					_terrain.terrainData.SetAlphamaps (x, y, newAlphamap);

				}
				else if (isFull == false)
				{
					//texture coord is within maxWidth of a road coord, set the road texture value to be based on the percentage distance away from the road coord
					float percentageRoadFill = (distanceToSelectedRoad - (fullDistance)) / ((maxDistance) - (fullDistance));

					float[,,] currentAlphamapcoordValues = _terrain.terrainData.GetAlphamaps (x, y, 1, 1);

					float[] textureValues = new float[terrainTexCount];

					for (int i = 0; i < terrainTexCount; i++)
					{
						textureValues [i] = currentAlphamapcoordValues [0, 0, i];
					}

					//debugging
					try
					{
						textureValues = SetTextureValueAtIndex (textureValues, roadTextureIndex, 1.0f - percentageRoadFill);
					}
					catch
					{
						Debug.LogError ("SetTextureValueAtIndex threw exception");
					}

					for (int i = 0; i < terrainTexCount; i++)
					{
						currentAlphamapcoordValues [0, 0, i] = textureValues [i];
					}

					_terrain.terrainData.SetAlphamaps (x, y, currentAlphamapcoordValues);
				}
				


			}
		}
			
	}

	//sorting function, added as an attempt to reduce the computation cost of drawing the road texture
	Vector2Int currentTextureCoord;
	private int CompareRoadCoordsByDistanceToTarget (RoadCoord _A, RoadCoord _B)
	{
		// return < 0 if _A is less than _B
		// return 0 if _A equal _B
		// return > 0 if _A is greater than _B

		float distanceToA = Mathf.Pow (Mathf.Pow (currentTextureCoord.x - (_A.pos.x - 0.5f), 2) + Mathf.Pow (currentTextureCoord.y - (_A.pos.y - 0.5f), 2), 0.5f);
		float distanceToB = Mathf.Pow (Mathf.Pow (currentTextureCoord.x - (_B.pos.x - 0.5f), 2) + Mathf.Pow (currentTextureCoord.y - (_B.pos.y - 0.5f), 2), 0.5f);

		float AFullWidth = _A.fullWidth / terrainToWorldScale;
		float AMaxWidth = _A.maxWidth / terrainToWorldScale;
		float BFullWidth = _B.fullWidth / terrainToWorldScale;
		float BMaxWidth = _A.maxWidth / terrainToWorldScale;

		if (distanceToA < AFullWidth / 2.0f)
		{
			if (distanceToB < BFullWidth / 2.0f)
			{
				//tex coord is in full range of both A and B
				return 0;
			}

			//tex coord is in full range of A
			return -1;
		}
		if (distanceToB < BFullWidth / 2.0f)
		{
			//tex coord is in full range of B
			return 1;
		}

		if (distanceToA < AMaxWidth / 2.0f)
		{
			if (distanceToB < BMaxWidth / 2.0f)
			{
				float percentageFillA = (distanceToA - (AFullWidth / 2.0f)) / ((AMaxWidth / 2.0f) - (AFullWidth / 2.0f));
				float percentageFillB = (distanceToB - (BFullWidth / 2.0f)) / ((BMaxWidth / 2.0f) - (BFullWidth / 2.0f));

				if (percentageFillA > percentageFillB)
				{
					//tex coord gets higher fill from A
					return -1;
				}
				if (percentageFillA <= percentageFillB)
				{
					//tex coord gets higher (or equal) fill from B
					return 1;
				}
			}

			//tex coord is in max range of A
			return -1;
		}

		if (distanceToB < BMaxWidth / 2.0f)
		{
			//tex coord is in max range of B
			return 1;
		}

		//neither is valid
		return 0;

	}

	public void ResetWorldScale ()
	{
		//reset terrain to world scale (note, made only for square terrains, as terrains don't work too good if not square, cus the textures it uses at square)
		terrainToWorldScale = specifiedTerrain.terrainData.size.x / (float)specifiedTerrain.terrainData.heightmapResolution;
	}

	public void ResetWorldScale (Terrain _terrain)
	{
		//reset terrain to world scale (note, made only for square terrains, as terrains don't work too good if not square, cus the textures it uses at square)
		terrainToWorldScale = _terrain.terrainData.size.x / (float)_terrain.terrainData.heightmapResolution;
	}

	public void ResetTerrainSpecificValues ()
	{
		//Reset texture steepness rules, as well as the the terrainToWorldScale
		textureSteepnessRules = new List<textureSteepnessBounds> ();

		terrainTexCount = specifiedTerrain.terrainData.splatPrototypes.Length;

		for (int i = 0; i < terrainTexCount - 1; i++)
		{
			textureSteepnessRules.Add (new textureSteepnessBounds ());
			textureSteepnessRules [i].minAngle = (90.0f / (terrainTexCount - 1)) * i;
			textureSteepnessRules [i].maxAngle = (90.0f / (terrainTexCount - 1)) * (i + 1);
		}

		ResetWorldScale ();
	}

	public void UpdateTerrainSteepnessTexture (Terrain _terrain)
	{

		Debug.Log ("Generating Terrain Texture");

		//Automatically texture the terrain, based on the steepness bounds

		int alphamapLayerCount = _terrain.terrainData.alphamapLayers;

		float[,,] newAlphamap = new float[_terrain.terrainData.alphamapWidth, _terrain.terrainData.alphamapHeight, _terrain.terrainData.alphamapLayers];

		//stores flags for which textures will be used
		bool[] texFillFlags = new bool[alphamapLayerCount];
		int texFilledCount;
		float texFillValue;

		//stores splatmap values for each texture
		float[] texFillValues = new float[alphamapLayerCount];
		float largestMinAngle = -1.0f;
		float smallestMaxAngle = 91.0f;
		int blendingTex1;
		int blendingTex2;

		//For each texture coordinate
		for (int y = 0; y < _terrain.terrainData.alphamapHeight; y++)
		{
			for (int x = 0; x < _terrain.terrainData.alphamapWidth; x++)
			{

				//get the angle of the terrain at the texture coordinate
				float normX = (float)x / (_terrain.terrainData.alphamapWidth - 1);
				float normY = (float)y / (_terrain.terrainData.alphamapHeight - 1);
				float angle = _terrain.terrainData.GetSteepness (normX, normY);

				#region Ver. 2 - Crossover Blendings
				//Set the terrain texture, based on the steepness bounds set in the editor
				//If the bounds cross over, the texture values are set based on where the current angle falls between the crossed bounds
				//Otherwise the entire texture coordinate is set to a single texture

				bool hitRoadTex = false;

				//for each terrain texture, flag the textures which need values
				for (int i = 0; i < terrainTexCount; i++)
				{
					

					//if the texture is the road texture, set it's flag to false and mark that the roadTex has been past
					if (i == roadTextureIndex)
					{
						texFillFlags [i] = false;
						hitRoadTex = true;
						continue;
					}

					//if the is outside the texture bounds, flag it as having no value, else flag it as requiring a splat value
					if (angle < textureSteepnessRules [i + (hitRoadTex ? -1 : 0)].minAngle || angle >= textureSteepnessRules [i + (hitRoadTex ? -1 : 0)].maxAngle)
					{
						texFillFlags [i] = false;
					}
					else
					{
						texFillFlags [i] = true;
					}

				}

				//get the number of textures which have splat values
				texFilledCount = 0;
				for (int j = 0; j < terrainTexCount; j++)
				{

					if (j == roadTextureIndex)
					{
						continue;
					}

					if (texFillFlags [j] == true)
					{
						texFilledCount++;
					}
				}

				//if no textures are flagged, abort the texturing, which means that that some angles are not within the texture bounds
				if (texFilledCount <= 0)
				{
					//no textures, abort texture generation
					Debug.Log ("No texture to apply to coord (" + x + "," + y + ")! Aborting!");
					return;
				}
				else if (texFilledCount == 1) //if 1 texture is flagged, set that textures splat value to 1, and set the rest to 0
				{
					//1 texture, set that 1 texture to 1.0f
					for (int k = 0; k < terrainTexCount; k++)
					{

						if (k == roadTextureIndex)
						{
							newAlphamap [y, x, k] = 0.0f;
							continue;
						}

						if (texFillFlags [k] == true)
						{
							newAlphamap [y, x, k] = 1.0f;
						}
						else
						{
							newAlphamap [y, x, k] = 0.0f;
						}
					}
				}
				else //if more than 1 texture is flagged, calculate the splat values for those textures
				{
					//multiple textures, find range between which the 2 textures are drawn
					largestMinAngle = -1.0f;
					smallestMaxAngle = 91.0f;

					hitRoadTex = false;

					for (int l = 0; l < terrainTexCount; l++)
					{

						if (l == roadTextureIndex)
						{
							newAlphamap [y, x, l] = 0.0f;
							hitRoadTex = true;
							continue;
						}

						if (texFillFlags [l] == true)
						{
							if (textureSteepnessRules [l + (hitRoadTex ? -1 : 0)].minAngle > largestMinAngle)
							{
								largestMinAngle = textureSteepnessRules [l + (hitRoadTex ? -1 : 0)].minAngle;
							}
							if (textureSteepnessRules [l + (hitRoadTex ? -1 : 0)].maxAngle < smallestMaxAngle)
							{
								smallestMaxAngle = textureSteepnessRules [l + (hitRoadTex ? -1 : 0)].maxAngle;
							}
						}
						else
						{
							newAlphamap [y, x, l] = 0.0f;
						}
					}

					//stores the 2 textures to blend
					blendingTex1 = -1;
					blendingTex2 = -1;

					//gets the 2 textures which need splat values
					for (int m = 0; m < terrainTexCount; m++)
					{

						if (m == roadTextureIndex)
						{
							newAlphamap [y, x, m] = 0.0f;
							continue;
						}

						if (blendingTex1 == -1 && texFillFlags [m] == true)
						{
							blendingTex1 = m;
						}
						else if (blendingTex2 == -1 && texFillFlags [m] == true)
						{
							blendingTex2 = m;
						}
						else
						{
							newAlphamap [y, x, m] = 0.0f;
						}
					}

					//set the splat values for each of the 2 textures, equal to the fraction of the angle between the 2 bounds
					float tex1Value = (smallestMaxAngle - angle) / (smallestMaxAngle - largestMinAngle);
					float tex2Value = (angle - largestMinAngle) / (smallestMaxAngle - largestMinAngle);

					newAlphamap [y, x, blendingTex1] = tex1Value;
					newAlphamap [y, x, blendingTex2] = tex2Value;
				}
					
				#endregion
			}
		}

		//updates the alphamap with the new texture values
		_terrain.terrainData.SetAlphamaps (0, 0, newAlphamap);
	}

	#endregion


	#region Runtime Functions

	bool didRaycastHit;
    
	Vector3 raycastHitPoint;

	int terrainHeightmapWidth;
	int terrainHeightmapHeight;

	Vector2 terrainRelativePosition;

	Vector2Int heightmapHitPoint;
	Vector2Int alphamapHitPoint;

	float heightmapHitPointHeight;
	float heightmapScale;
	float heightmapHitPointReletiveHeight;

	float heightmapHitPointSteepness;

	void Start ()
	{
		
		if (raycastCamera == null)
		{
			raycastCamera = Camera.main;
		}

		lastInteractedTerrain = specifiedTerrain;

		ResetWorldScale ();
	}

	Vector2Int highlightedTerrainCoord = Vector2Int.zero;

	Vector2Int highlightedTextureCoord = Vector2Int.zero;

	List<float> listTextureValues;

	//debug
	bool hasStartedStraightRoadDrawing;
	Vector2Int straightRoadStartCoord;

	//the update function is used entirely for debugging
	void Update ()
	{
		

		//code for running the A* pathfinding on the settlement data (now obsolete)
		if (isPathfinding == true)
		{
			bool done = pathfindingObject.NextNodeCheck ();
			currentNodePosition = pathfindingObject.currentNode.pos;

			if (done == true)
			{
				isPathfinding = false;

				if (pathfindingObject.goalFound == true)
				{
					TryApplyPath ();
				}
			}

		}
		else
		{
			//cast a ray from the mouse position in game view onto a terrain, and set a number of variables, which are displayed on the screen
			//depending on whether the mode is set to building or road, a building or road will attempt to be added to the terrain
			RaycastHit hit;
			Ray ray = raycastCamera.ScreenPointToRay (Input.mousePosition);

			if (Physics.Raycast (ray, out hit))
			{

				didRaycastHit = true;

				Transform objectHit = hit.transform;

				if (objectHit.GetComponent<Terrain> () == null)
				{
					return;
				}

				Terrain terrain = objectHit.GetComponent<Terrain> ();
				lastInteractedTerrain = terrain;

				highlightedTerrainCoord = WorldPosToTerrainCoord (hit.point, terrain);

				highlightedTextureCoord = WorldPosToTexCoord (hit.point, terrain);

				raycastHitPoint = hit.point;

				heightmapHitPointHeight = terrain.terrainData.GetHeight (highlightedTerrainCoord.x, highlightedTerrainCoord.y);
				heightmapScale = terrain.terrainData.heightmapScale.y;
				heightmapHitPointReletiveHeight = heightmapHitPointHeight / heightmapScale;

				terrainHeightmapWidth = terrain.terrainData.heightmapWidth;
				terrainHeightmapHeight = terrain.terrainData.heightmapHeight;

				heightmapHitPoint = highlightedTerrainCoord;
				alphamapHitPoint = highlightedTextureCoord;

				terrainRelativePosition = WorldPosToRelativeTerrainPos (hit.point, terrain);
				heightmapHitPointSteepness = terrain.terrainData.GetSteepness (terrainRelativePosition.x, terrainRelativePosition.y);

				listTextureValues = new List<float> ();
				float[,,] textureValues = terrain.terrainData.GetAlphamaps (highlightedTextureCoord.x, highlightedTextureCoord.y, 1, 1);

				for (int i = 0; i < terrainTexCount; i++)
				{
					listTextureValues.Add (textureValues [0, 0, i]);
				}


				if (mode == 0) //building
				{
					if (Input.GetMouseButtonDown (0) == true)
					{
						//TryPlaceBuilding(highlightedTerrainCoord, terrain);

						if (HasSettlementData () == false)
						{
							GenerateNewSettlementData ();
						}
						//SetBuildingCoord (highlightedTerrainCoord.x, highlightedTerrainCoord.y);
						TrySetBuildingCoord (highlightedTerrainCoord, chosenBuilding); 
						ApplyBuildingsFromData ();
					}

				}
				else if (mode == 1) //road
				{
					//if you hold down mouse button 0, then a straight road will be drawn on the terrain between the start and end points
					if (Input.GetMouseButtonDown (0) == true)
					{
						//TrySetPathTex(highlightedTerrainCoord, terrain);
						if (HasSettlementData () == false)
						{
							GenerateNewSettlementData ();
						}
						//SetRoadCoord (highlightedTerrainCoord.x, highlightedTerrainCoord.y);
						hasStartedStraightRoadDrawing = true;
						straightRoadStartCoord = highlightedTerrainCoord;
					}

					if (hasStartedStraightRoadDrawing == true)
					{
						if (Input.GetMouseButtonUp (0) == true)
						{
							SetStraightRoad (straightRoadStartCoord.x, straightRoadStartCoord.y, highlightedTerrainCoord.x, highlightedTerrainCoord.y);

							hasStartedStraightRoadDrawing = false;
						}
					}

				}

			}
			else
			{

				didRaycastHit = false;

			}
		}
	}

	//to onGUI function is used to display debug data
	void OnGUI ()
	{

		GUI.contentColor = Color.black;
		GUILayout.BeginArea (new Rect (20, 20, 500, 500));

		if (didRaycastHit == true)
		{
            //displays the debug data found during the update function

			GUILayout.Label ("Position: " + raycastHitPoint);

			//get terrain position from world position

			GUILayout.Label ("Terrain Data Size: " + new Vector2 (terrainHeightmapWidth, terrainHeightmapHeight));

			GUILayout.Label ("Relative Pos on Terrain: " + terrainRelativePosition.ToString ("F5"));

			GUILayout.Label ("Terrain Coord: " + heightmapHitPoint);
			GUILayout.Label ("Texture Coord: " + alphamapHitPoint);

			GUILayout.Label ("XZ Pos Height: " + heightmapHitPointHeight.ToString ("F5"));
			GUILayout.Label ("Terrain Map Height Scale: " + heightmapScale.ToString ("F5"));
			GUILayout.Label ("XZ Pos Reletive Height: " + heightmapHitPointReletiveHeight.ToString ("F5"));

			GUILayout.Label ("XZ Pos Steepness: " + heightmapHitPointSteepness.ToString ("F5"));

			string stringTextureValues = "";

			for (int i = 0; i < terrainTexCount; i++)
			{
				stringTextureValues = stringTextureValues + listTextureValues [i].ToString ("F5") + ", ";
			}

			GUILayout.Label ("Texture Values: (" + stringTextureValues + ")");

		}
		else
		{

			GUILayout.Label ("Object Hit: " + "None");

		}
			
		GUILayout.EndArea ();
	}

	//used to display working points on the terrain, as gizmo rays
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay (raycastHitPoint, Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);

		Gizmos.color = Color.green;
		Gizmos.DrawRay (TerrainCoordToWorldPos (highlightedTerrainCoord, lastInteractedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);

		if (hasStartedStraightRoadDrawing == true)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawRay (TerrainCoordToWorldPos (straightRoadStartCoord, lastInteractedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);
			Gizmos.DrawLine (TerrainCoordToWorldPos (straightRoadStartCoord, lastInteractedTerrain) + (Vector3.up * 30), TerrainCoordToWorldPos (highlightedTerrainCoord, lastInteractedTerrain) + (Vector3.up * 30));

		}


		Gizmos.color = Color.white;
		//Gizmos.DrawRay (TerrainCoordToWorldPos (currentNodePosition, specifiedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay (TerrainCoordToWorldPos (lastTriedBuildingSpawn, specifiedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);

		Gizmos.color = Color.cyan;

		if (currentRoadSections != null)
		{
			for (int i = 0; i < currentRoadSections.Count; i++)
			{
				Gizmos.DrawRay (currentRoadSections [i], Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);
			}
		}

		Gizmos.color = Color.blue;

		if (debugLastPath != null)
		{
			for (int i = 0; i < debugLastPath.corners.Length; i++)
			{
				Gizmos.DrawRay (debugLastPath.corners [i], Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);
			}
		}

		Gizmos.color = Color.red;
		Gizmos.DrawRay (TerrainCoordToWorldPos (debugStartCoord, specifiedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);

		Gizmos.color = Color.green;
		Gizmos.DrawRay (TerrainCoordToWorldPos (debugGoalCoord, specifiedTerrain), Vector3.Normalize (new Vector3 (0, 1, 0)) * 30.0f);





	}

	#endregion

	#region Position Conversion Functions

	//Terrain Coordinate is the coordinate on the HeightMap
	//Texture Coordinate is the coordinate on the AlphaMap

	public Vector2Int WorldPosToTerrainCoord (Vector3 _worldPos, Terrain _terrain)
	{
		// WorldPos -> RelativeTerrainPos -> TerrainCoord
		//Find the relative world space position of the point, then divide it by the size of the terrain, to get a relative terrain fraction, from 0 to 1
		float relativeTerPosX = (_worldPos.x - _terrain.GetPosition ().x) / _terrain.terrainData.size.x;
		float relativeTerPosZ = (_worldPos.z - _terrain.GetPosition ().z) / _terrain.terrainData.size.z;
		//Multiply the relative terrain position by the terrain heightmap resolution, to get the relative terrain coordinate, from 0 to terrain heightmap resolution.
		//This is float and can lie between the exact terrain (heightmap) coordinate
		float relativeTerCoordX = _terrain.terrainData.heightmapWidth * relativeTerPosX;
		float relativeTerCoordZ = _terrain.terrainData.heightmapHeight * relativeTerPosZ;
		//Round each coordinate value to an integer, to get the terrain coordiante
		return new Vector2Int (Mathf.FloorToInt (relativeTerCoordX), Mathf.FloorToInt (relativeTerCoordZ));
	}

	public Vector2Int WorldPosToTexCoord (Vector3 _worldPos, Terrain _terrain)
	{

		// WorldPos -> RelativeTerrainPos -> TextureCoord

		//As with the WorldPosToTerrainCoord, but using the alphamap
		float relativeTerPosX = (_worldPos.x - _terrain.GetPosition ().x) / _terrain.terrainData.size.x;
		float relativeTerPosZ = (_worldPos.z - _terrain.GetPosition ().z) / _terrain.terrainData.size.z;

		float relativeTexCoordX = _terrain.terrainData.alphamapWidth * relativeTerPosX;
		float relativeTexCoordZ = _terrain.terrainData.alphamapHeight * relativeTerPosZ;

		return new Vector2Int (Mathf.FloorToInt (relativeTexCoordX), Mathf.FloorToInt (relativeTexCoordZ));
	}

	public Vector2 WorldPosToRelativeTerrainPos (Vector3 _worldPos, Terrain _terrain)
	{
		// WorldPos -> RelativeTerrainPos

		//As with the WorldPosToTerrainCoord, but returns the relative terrain fraction, from 0 to 1
		float relativeTerPosX = (_worldPos.x - _terrain.GetPosition ().x) / _terrain.terrainData.size.x;
		float relativeTerPosZ = (_worldPos.z - _terrain.GetPosition ().z) / _terrain.terrainData.size.z;

		return new Vector2 (relativeTerPosX, relativeTerPosZ);
	}

	public Vector3 TerrainCoordToWorldPos (Vector2Int _terrainCoord, Terrain _terrain)
	{
		// TerrainCoord -> RelativeTerrainPos -> WorldPos <- TerrainHeight

		//The reverse of the WorldPosToTerrainCoord, but the height at the terrain coord position is returned also
		float worldPosX = _terrain.GetPosition ().x + ((_terrain.terrainData.size.x / (float)(_terrain.terrainData.heightmapWidth - 1)) * (float)_terrainCoord.x);
		float worldPosZ = _terrain.GetPosition ().z + ((_terrain.terrainData.size.z / (float)(_terrain.terrainData.heightmapHeight - 1)) * (float)_terrainCoord.y);

		float worldPosY = _terrain.terrainData.GetHeight (_terrainCoord.x, _terrainCoord.y);

		return new Vector3 (worldPosX, worldPosY, worldPosZ);
	}

	public Vector2 TerrainCoordToRelativeTerrainPos (Vector2Int terrainCoord, Terrain terrain)
	{
		// TerrainCoord -> RelativeTerrainPos -> TextureCoord

		//The relative terrain fraction of a terrain coordinate
		float relativeTerPosX = (float)terrainCoord.x / terrain.terrainData.heightmapWidth;
		float relativeTerPosZ = (float)terrainCoord.y / terrain.terrainData.heightmapHeight;

		return new Vector2 (relativeTerPosX, relativeTerPosZ);
	}

	//currently relys on the terrain and texture resolution to be the 'same' size (ie. 513 and 512)
	public List<Vector2Int> TerrainCoordToTextureCoords (Vector2Int _terrainCoord, Terrain _terrain)
	{

		//adds each surrounding valid texture coordinate to returnList

		List<Vector2Int> returnList = new List<Vector2Int> ();

		for (int xmod = -1; xmod <= 0; xmod++)
		{
			for (int ymod = -1; ymod <= 0; ymod++)
			{
				
				Vector2Int tempCoord = new Vector2Int (_terrainCoord.x + xmod, _terrainCoord.y + ymod);

				if (tempCoord.x >= 0 && tempCoord.x < _terrain.terrainData.heightmapWidth && tempCoord.y >= 0 && tempCoord.y < _terrain.terrainData.heightmapHeight)
				{
					returnList.Add (tempCoord);
				}

			}
		}

		return returnList;
	}

	public List<Vector2Int> TerrainCoordsToTextureCoords (List<Vector2Int> _terrainCoords, Terrain _terrain)
	{

		//for each terrain coordinate, adds each surrounding valid texture coordinate to returnList

		List<Vector2Int> returnList = new List<Vector2Int> ();

		foreach (Vector2Int terrainCoord in _terrainCoords)
		{
			for (int xmod = -1; xmod <= 0; xmod++)
			{
				for (int ymod = -1; ymod <= 0; ymod++)
				{

					Vector2Int tempCoord = new Vector2Int (terrainCoord.x + xmod, terrainCoord.y + ymod);

					if (tempCoord.x >= 0 && tempCoord.x < _terrain.terrainData.heightmapWidth && tempCoord.y >= 0 && tempCoord.y < _terrain.terrainData.heightmapHeight)
					{
						//if the list already contains a coordinate, do not add it again
						if (returnList.Contains (tempCoord) == false)
						{
							returnList.Add (tempCoord);
						}
					}

				}
			}
		}

		return returnList;
	}

	public List<Vector2Int> TextureCoordToTerrainCoords (Vector2Int _textureCoord, Terrain _terrain)
	{

		//adds each surrounding valid texture coordinate to returnList

		List<Vector2Int> returnList = new List<Vector2Int> ();

		for (int xmod = 0; xmod <= 1; xmod++)
		{
			for (int ymod = 0; ymod <= 1; ymod++)
			{
				Vector2Int tempCoord = new Vector2Int (_textureCoord.x + xmod, _textureCoord.y + ymod);

				if (tempCoord.x >= 0 && tempCoord.x < _terrain.terrainData.alphamapWidth && tempCoord.y >= 0 && tempCoord.y < _terrain.terrainData.alphamapHeight)
				{
					returnList.Add (tempCoord);
				}
			}
		}

		return returnList;
	}

	public List<Vector2Int> TextureCoordsToTerrainCoords (List<Vector2Int> _textureCoords, Terrain _terrain)
	{

		//for each terrain coordinate, adds each surrounding valid texture coordinate to returnList

		List<Vector2Int> returnList = new List<Vector2Int> ();

		foreach (Vector2Int textureCoord in _textureCoords)
		{
			for (int xmod = 0; xmod <= 1; xmod++)
			{
				for (int ymod = 0; ymod <= 1; ymod++)
				{
					
					Vector2Int tempCoord = new Vector2Int (textureCoord.x + xmod, textureCoord.y + ymod);

					if (tempCoord.x >= 0 && tempCoord.x < _terrain.terrainData.alphamapWidth && tempCoord.y >= 0 && tempCoord.y < _terrain.terrainData.alphamapHeight)
					{
						//if the list already contains a coordinate, do not add it again
						if (returnList.Contains (tempCoord) == false)
						{
							returnList.Add (tempCoord);
						}
					}

				}
			}
		}

		return returnList;
	}

	//only works if terrain and texture have "same" size (513 and 512)
	public Vector2 TerrainCoordToNonIntTextureCoord (Vector2Int _terrainCoord)
	{

		//returns the terrain coordinate in texture coordinate space, by displacing it 0.5 to the left

		return new Vector2 (_terrainCoord.x - 0.5f, _terrainCoord.y - 0.5f);
	}

	#endregion

	#region Alphamap Functions

	float[] RemoveTextureFromTextureCoordAtIndex (float[] _textureValues, int _index)
	{
		//removes the indexed texture value from a set of texture values, maintaining the texture value ratio of the other texture values

		if (_index >= _textureValues.Length)
		{
			Debug.LogError ("Index Out Of Range");
			throw new UnityException ();
		}
		if (_textureValues [_index] >= 1.0f)
		{
			Debug.LogError ("Index is 100% of texture values");
			throw new UnityException ();
		}

		float[] returnValues = _textureValues;

		float removeTexValue = _textureValues [_index];

		for (int i = 0; i < _textureValues.Length; i++)
		{
			if (i == _index)
			{
				returnValues [i] = 0.0f;
			}
			else
			{
				returnValues [i] = _textureValues [i] / (1.0f - removeTexValue);
			}
		}

		return returnValues;

	}

	float[] SetTextureValueAtIndex (float[] _textureValues, int _index, float _value)
	{
		//sets a specific texture value in a set of texture values, and update all other texture values to maintain the texture value ratio of the other texture values
		if (_index >= _textureValues.Length)
		{
			Debug.LogError ("Index Out Of Range");
			throw new UnityException ();
		}
		if (_value > 1.0f || _value < 0.0f)
		{
			Debug.LogError ("Value Out Of Range (0 -> 1)");
			throw new UnityException ();
		}

		float[] returnValues = _textureValues;

		float setTextureOldValue = _textureValues [_index];

		for (int i = 0; i < _textureValues.Length; i++)
		{
			if (i == _index)
			{
				returnValues [i] = _value;
			}
			else
			{
				returnValues [i] = (1.0f - _value) * (_textureValues [i] + (setTextureOldValue / (_textureValues.Length - 1)));
			}
		}

		return returnValues;
	}

	float[] FillTextureValueAtIndex (float[] _textureValues, int _index)
	{
		//sets a texture value in a set of texture value to 1, and the other texture values to 0
		float[] returnValues = _textureValues;

		for (int i = 0; i < _textureValues.Length; i++)
		{
			if (i == _index)
			{
				returnValues [i] = 1.0f;
			}
			else
			{
				returnValues [i] = 0.0f;
			}
		}

		return returnValues;
	}

	#endregion

	#region Settlement Data

	SettlementData currentSettlementData;

	public bool HasSettlementData ()
	{
		return (currentSettlementData != null);
	}

	public void GenerateNewSettlementData ()
	{
		currentSettlementData = new SettlementData ();

		currentSettlementData.Init (specifiedTerrain);
	}

	public void ClearSettlementData ()
	{
		currentSettlementData = null;
	}

	public void WriteDataToCSV ()
	{
		currentSettlementData.WriteHeightmapToCSV ();
		currentSettlementData.WriteSteepnessmapToCSV ();
		currentSettlementData.WriteLandmarksToCSV ();
	}

	public void ApplyRoadsFromData ()
	{
		List<RoadCoord> roadCoords = new List<RoadCoord> ();

		roadCoords = currentSettlementData.GetRoadCoords ();

		DrawPath (roadCoords, specifiedTerrain);
	}

	public bool TrySetBuildingCoord (Vector2Int _centre, int _buildingID)
	{
		//attempts to add a building to the settlement data, if it's viable

		float radiusSize = (Mathf.CeilToInt (buildingList [_buildingID].GetComponent<Building> ().getBaseSize () / 2.0f) / terrainToWorldScale);
		int checkSize = Mathf.CeilToInt (radiusSize);

		float heightTemp;
		float highestHeight = 0.0f;

		//debug
		checkCoords.Clear ();

		//checks a square around the position, so not too accurate but fast
		int errorID = 0; //1 = too steep, 2 = road collision, 3 = building collision

		//checks a square around the centre position, slightly larger than the radius size
		for (int x = _centre.x - checkSize; x <= _centre.x + checkSize; x++)
		{
			for (int y = _centre.y - checkSize; y <= _centre.y + checkSize; y++)
			{

				Vector2Int checkCoord = new Vector2Int (x, y);
				Vector2 relativeTerrainPos = TerrainCoordToRelativeTerrainPos (checkCoord, specifiedTerrain);



				if (x >= 0 && x < currentSettlementData.width && y >= 0 && y < currentSettlementData.height)
				{
					//if the point is within the radius of the building
					if (Mathf.Pow (Mathf.Pow (x - _centre.x, 2) + Mathf.Pow (y - _centre.y, 2), 0.5f) <= radiusSize)
					{
						CoordData thisCoord = currentSettlementData.GetDataAt (x, y);

						//check the validity of the coord (is the angle less than the building's max steepness, and no road or building is already on that coord)
						if (thisCoord.steepness >= buildingList [_buildingID].GetComponent<Building> ().maxSteepness)
						{
							errorID = 1;
							break;
						}

						if (thisCoord.IsRoad () == true)
						{
							errorID = 2;
							break;
						}

						if (thisCoord.IsBuilding () == true)
						{
							errorID = 3;
							break;
						}

						checkCoords.Add (new Vector2Int (x, y));

						//records the highest height of the check coords
						heightTemp = thisCoord.height;
						if (heightTemp > highestHeight)
						{
							highestHeight = heightTemp;

							highestHeightTerrainCoord = new Vector2Int (x, y);
						}
					}
				}

			}
		}



		if (errorID == 1)
		{
			Debug.Log ("Too steep for building");
			return false;
		}
		else if (errorID == 2)
		{
			Debug.Log ("Intersects with road");
			return false;
		}
		else if (errorID == 3)
		{
			Debug.Log ("Intersects with building");
			return false;
		}
		else
		{
			Debug.Log ("Setting building at : " + _centre);
			float rotation = Random.Range (0, 360f);

			//set the check coords in settlement data to be building coords, with the centre coord being set as the centre of the building.
			foreach (Vector2Int coord in checkCoords)
			{
				if (coord.x == _centre.x && coord.y == _centre.y)
				{
					currentSettlementData.SetBuildingCoord (coord.x, coord.y, _buildingID, highestHeight, true, rotation);
				}
				else
				{
					currentSettlementData.SetBuildingCoord (coord.x, coord.y, _buildingID, highestHeight, false, rotation);
				}
			}
		}

		return true;

	}

	public void RemoveChildObjects ()
	{
		foreach (Transform child in specifiedTerrain.transform)
		{
			Destroy (child.gameObject);
		}
	}

	public void ApplyBuildingsFromData ()
	{
		RemoveChildObjects ();

		//for each building in settlement data, instantiate it's prefab, using the data stored in it's road coord

		List<BuildingCoord> buildingCoords = currentSettlementData.GetBuildingCoords ();

		foreach (BuildingCoord b in buildingCoords)
		{
			int buildingID = b.buildingID;
			Vector2Int centre = b.pos;

			float radiusSize = (Mathf.CeilToInt (buildingList [buildingID].GetComponent<Building> ().getBaseSize () / 2.0f) / terrainToWorldScale);
			float height = b.buildingHeight;

			Vector3 spawnPos = TerrainCoordToWorldPos (centre, specifiedTerrain);
			spawnPos.y = height + buildingList [buildingID].GetComponent<Building> ().getHeightOffset ();

			unconnectedBuildings.Add (Instantiate<GameObject> (buildingList [buildingID], spawnPos, Quaternion.Euler (0.0f, b.yRotation, 0.0f), specifiedTerrain.transform));
		}

		//create them

	}

	//keeps track of instantiated buildings
	public List<GameObject> connectedBuildings;
	public List<GameObject> unconnectedBuildings;

	public void InitSpawnedBuildings ()
	{
		if (connectedBuildings != null)
		{
			foreach (GameObject building in connectedBuildings)
			{
				DestroyImmediate (building);
			}
			connectedBuildings.Clear ();
		}
		if (unconnectedBuildings != null)
		{
			foreach (GameObject building in unconnectedBuildings)
			{
				DestroyImmediate (building);
			}
			unconnectedBuildings.Clear ();
		}

		unconnectedBuildings = new List<GameObject> ();
	}

	public void TryConnectBuildings ()
	{
		//rebuild navmesh before connecting
		navmeshPFObject.BakeNavmesh ();

		//for each unconnected building, attempt to connect it to the existing roads

		foreach (GameObject building in unconnectedBuildings)
		{
			TryConnectPointToRoads (WorldPosToTerrainCoord (building.transform.position, specifiedTerrain));

			connectedBuildings.Add (building);
		}

		//move these now connected building to connectedBuildings
		connectedBuildings.AddRange (unconnectedBuildings);
		unconnectedBuildings.Clear ();

	}

	#region debug

	public void SetRoadCoord (int _x, int _y)
	{
		currentSettlementData.SetRoadCoord (_x, _y, fullRoadWidth, maxRoadWidth);
	}

	public void SetStraightRoad (int _xStart, int _yStart, int _xEnd, int _yEnd)
	{
		currentSettlementData.SetStraightRoadCoords (_xStart, _yStart, _xEnd, _yEnd, fullRoadWidth, maxRoadWidth);
	}

	public void SetBuildingCoord (int _x, int _y)
	{
		//current 0.0f, will update later
		currentSettlementData.SetBuildingCoord (_x, _y, chosenBuilding, 0.0f, true, 0.0f);
	}

	public void GetBuildingCoods ()
	{
		currentSettlementData.GetBuildingCoords ();
	}


	#endregion

	#endregion

	#region Pathfinding

	//region for A* pathfinding on the settlement data
	//no longer used, was too slow

	Pathfinding pathfindingObject;
	bool isPathfinding;

	[HideInInspector] public Vector2Int currentNodePosition;

	//default node cost for debugging
	public void SetupPathfinding (Vector2Int _start, Vector2Int _goal, float _nodeCost)
	{
		//setup the pathfinding object to begin pathfinding

		if (pathfindingObject == null)
		{
			pathfindingObject = new Pathfinding ();
		}

		pathfindingObject.Init ();
		pathfindingObject.Setup (_start, _goal, currentSettlementData, _nodeCost, maxRoadSteepness);
		pathfindingObject.SetFirstNode ();

		isPathfinding = true;

	}

	[HideInInspector] public float defaultNodeCost;
	[HideInInspector] public Vector2Int testStart;
	[HideInInspector] public Vector2Int testGoal;


	public void StartPathfinding ()
	{
		if (testStart == testGoal)
		{
			Debug.LogError ("Start == Goal");

			return;
		}

		SetupPathfinding (testStart, testGoal, defaultNodeCost);
	}

	public void TryApplyPath ()
	{
		//tries to draw the generated path onto the terrain

		List<Vector2Int> path;

		path = pathfindingObject.GetPath ();

		if (path == null)
		{
			Debug.Log ("NavmeshPathfindingObject found no path");
			return;
		}

		//sets each settlement coord on the path to be a road coord
		foreach (Vector2Int coord in path)
		{
			currentSettlementData.SetRoadCoord (coord.x, coord.y, fullRoadWidth, maxRoadWidth);
		}

		//draw all the road coordinates in settlement data
		DrawPath (currentSettlementData.GetRoadCoords (), specifiedTerrain);
	}

	#endregion

	#region Navmesh Pathfinding

	//the navmesh pathfinding object which will be used for pathfinding
	NavmeshPathfinding navmeshPFObject;

	//the list of all created paths
	RoadPaths createdRoadPaths;

	//stores the last generated path, for displaying gizmos on the terrain
	NavMeshPath debugLastPath;
	//stores all the road sections, for displaying gizmos on the terrain and for connecting buildings to the roadmap
	List<Vector3> currentRoadSections;

	public void InitNavmeshPF ()
	{
		if (navmeshPFObject == null)
		{
			navmeshPFObject = new NavmeshPathfinding ();
		}

		navmeshPFObject.BakeNavmesh (gameObject.GetComponent<NavMeshSurface> ());
		createdRoadPaths = new RoadPaths ();

		debugLastPath = new NavMeshPath ();
		currentRoadSections = new List<Vector3> ();
	}

	public void AddNavmeshPathToSettlementData (NavMeshPath _path)
	{
		//converts a path to terrain coords and adds it to the settlement data

		if (currentSettlementData == null)
		{
			return;
		}

		List<Vector2Int> pathInTerrainCoordSpace = new List<Vector2Int> ();

		for (int i = 0; i < _path.corners.Length; i++)
		{
			pathInTerrainCoordSpace.Add (WorldPosToTerrainCoord (_path.corners [i], specifiedTerrain));
		}

		for (int i = 1; i < pathInTerrainCoordSpace.Count; i++)
		{
			currentSettlementData.SetStraightRoadCoords (pathInTerrainCoordSpace [i - 1].x, pathInTerrainCoordSpace [i - 1].y, pathInTerrainCoordSpace [i].x, pathInTerrainCoordSpace [i].y, fullRoadWidth, maxRoadWidth);
		}
	}

	public void DrawNavmeshPath (NavMeshPath _path)
	{
		//converts a path to terrain coords and adds it to the settlement data
		//then draws the road coords to the terrain

		if (currentSettlementData == null)
		{
			return;
		}

		List<Vector2Int> pathInTerrainCoordSpace = new List<Vector2Int> ();

		for (int i = 0; i < _path.corners.Length; i++)
		{
			pathInTerrainCoordSpace.Add (WorldPosToTerrainCoord (_path.corners [i], specifiedTerrain));
		}

		for (int i = 1; i < pathInTerrainCoordSpace.Count; i++)
		{
			currentSettlementData.SetStraightRoadCoords (pathInTerrainCoordSpace [i - 1].x, pathInTerrainCoordSpace [i - 1].y, pathInTerrainCoordSpace [i].x, pathInTerrainCoordSpace [i].y, fullRoadWidth, maxRoadWidth);
		}

		DrawPath (currentSettlementData.GetRoadCoords (), specifiedTerrain);
	}

	public void AddPathToRoadPaths (NavMeshPath _path)
	{
		createdRoadPaths.AddPath (_path);
	}

	public bool TryGetPathToGoalFromRoads (Vector2Int _goalCoord, ref NavMeshPath _path)
	{
		//attempts to find the shortest path from one of the road sections, then returns it

		List<NavMeshPath> potentialPaths = new List<NavMeshPath> ();

		//for each road section, attempt to find a path to the goal
		foreach (Vector3 startPos in currentRoadSections)
		{
			NavMeshPath tempPath = new NavMeshPath ();
			if (navmeshPFObject.GetPath (startPos, TerrainCoordToWorldPos (_goalCoord, specifiedTerrain), tempPath) == true)
			{
				potentialPaths.Add (tempPath);
			}
		}

		if (potentialPaths.Count == 0)
		{
			//no path found, return false
			return false;
		}

		//take either the shortest complete path, or the longest partial path
		NavMeshPath shortestCompletePath = new NavMeshPath ();
		float shortestCompletePathLength = specifiedTerrain.terrainData.heightmapHeight * 2.0f;
		NavMeshPath shortestPartialPath = new NavMeshPath ();
		float shortestPartialPathLength = specifiedTerrain.terrainData.heightmapHeight * 2.0f;
		bool completePath = false;

		//for each path found, check if the path is full, or partial
		//find the shortest complete path and the shortest partial path
		foreach (NavMeshPath path in potentialPaths)
		{
			float pathLength = GetPathLength (path);

			if (WorldPosToTerrainCoord (path.corners [path.corners.Length - 1], specifiedTerrain) == _goalCoord)
			{
				//if the path reaches the goal, then it is a complete path
				if (pathLength < shortestCompletePathLength)
				{
					shortestCompletePathLength = pathLength;
					shortestCompletePath = path;
					completePath = true;
				}
			}
			else //is a partial path
			{
				if (pathLength < shortestPartialPathLength)
				{
					//otherwise, the path is partial
					shortestPartialPathLength = pathLength;
					shortestPartialPath = path;
				}
			}
		}

		NavMeshPath usePath = new NavMeshPath ();

		//if a complete path was found, set the _path to the shortest complete path
		if (completePath == true)
		{
			usePath = shortestCompletePath;
		}
		else //else, set the _path to the shortest partial path
		{
			usePath = shortestPartialPath;
		}

		_path = usePath;

		//as a path was found, return true
		return true;
	}

	//find path across the Terrain, if _yAxis is false, try across the x axis, if _yAxis is true, try across the z axis (y of the heightmap)
	public bool TryFindCrossRoad (bool _crossYAxis, ref NavMeshPath _path)
	{

		Vector2Int startCoord = new Vector2Int ();
		Vector2Int goalCoord = new Vector2Int ();

		bool foundValidEndCoords = false;

		//randomly select 2 points on either side of the terrain, offsetting them so they will be on the navmesh
		if (_crossYAxis == false)
		{
			startCoord.y = Mathf.CeilToInt (maxRoadWidth / 2.0f);
			goalCoord.y = specifiedTerrain.terrainData.heightmapHeight - 1 - startCoord.y;
			if (TryGetValidEdgePosition (false, ref startCoord) == true && TryGetValidEdgePosition (false, ref goalCoord) == true)
			{
				foundValidEndCoords = true;
			}
		}
		else
		{
			startCoord.x = Mathf.CeilToInt (maxRoadWidth / 2.0f);
			goalCoord.x = specifiedTerrain.terrainData.heightmapWidth - 1 - startCoord.x;
			if (TryGetValidEdgePosition (true, ref startCoord) == true && TryGetValidEdgePosition (true, ref goalCoord) == true)
			{
				foundValidEndCoords = true;
			}
		}

		if (foundValidEndCoords == false)
		{
			Debug.Log ("Failed To Find Valid Start and Goal coords");
			return false;
		}
		debugStartCoord = startCoord;
		debugGoalCoord = goalCoord;

		bool foundPath = false;

		if (navmeshPFObject.GetPath (TerrainCoordToWorldPos (startCoord, specifiedTerrain), TerrainCoordToWorldPos (goalCoord, specifiedTerrain), _path) == true)
		{
			foundPath = true;

			if (WorldPosToTerrainCoord (_path.corners [0], specifiedTerrain) == startCoord && WorldPosToTerrainCoord (_path.corners [_path.corners.Length - 1], specifiedTerrain) == goalCoord)
			{
				Debug.Log ("Complete Path Found");
			}
			else
			{
				Debug.Log ("Partial Path Found");
				NavMeshPath altPath = new NavMeshPath ();
				navmeshPFObject.GetPath (TerrainCoordToWorldPos (goalCoord, specifiedTerrain), TerrainCoordToWorldPos (startCoord, specifiedTerrain), altPath);

				//whichever is longer, use that

				if (GetPathLength (altPath) > GetPathLength (_path))
				{
					_path = altPath;
				}
			}
		}

		if (foundPath == false)
		{
			Debug.Log ("Failed to Find Path");
			return false;
		}
		else
		{
			return true;
		}
	}

	public float GetPathLength (NavMeshPath _path)
	{
		float totalLength = 0;

		for (int i = 1; i < _path.corners.Length; i++)
		{
			totalLength += Vector3.Distance (_path.corners [i - 1], _path.corners [i]);
		}

		return totalLength;
	}

	[HideInInspector] public Vector2Int debugStartCoord;
	[HideInInspector] public Vector2Int debugGoalCoord;

	[HideInInspector] public float maxSectionDistance = 30.0f;

	public void TryDrawCrossRoad (bool _crossYAxis)
	{
		NavMeshPath path = new NavMeshPath ();

		if (TryFindCrossRoad (_crossYAxis, ref path) == true)
		{
			debugLastPath = path;
			AddPathToRoadPaths (path);
			AddNavmeshPathToSettlementData (path);
			currentRoadSections = createdRoadPaths.GetSectionedPathPositions (maxSectionDistance);
		}

	}

	//debug
	[HideInInspector] public Vector2Int debugCoord;

	public void TryConnectPointToRoads (Vector2Int _goalcoord)
	{
		NavMeshPath path = new NavMeshPath ();

		if (TryGetPathToGoalFromRoads (_goalcoord, ref path) == true)
		{
			debugLastPath = path;
			AddPathToRoadPaths (path);
			AddNavmeshPathToSettlementData (path);
			currentRoadSections = createdRoadPaths.GetSectionedPathPositions (maxSectionDistance);
		}
	}



	[HideInInspector] public bool debugYAxisBool = false;

	public bool TryGetValidEdgePosition (bool _yAxis, ref Vector2Int _vec)
	{
		//sets the x or y coordinate to a random position on the terrain

		if (HasSettlementData () == false)
		{
			return false;
		}

		bool success = false;
		int counter = 0;

		//tries to find a position which has a steepness less than the max road steepness
		while (counter < 10 && success == false)
		{

			if (_yAxis == true)
			{
				_vec.y = Random.Range (0, specifiedTerrain.terrainData.heightmapHeight);
			}
			else
			{
				_vec.x = Random.Range (0, specifiedTerrain.terrainData.heightmapWidth);
			}

			float steepnessAtPoint = currentSettlementData.GetDataAt (_vec.x, _vec.y).steepness;

			if (steepnessAtPoint < maxRoadSteepness)
			{
				success = true;
			}
			else
			{
				counter++;
			}

		}

		Debug.Log ("Try Get Valid Edge Position: " + success + " in: " + (counter + 1) + " tries");

		return success;
	}

	#region debug

	//debug functions to test navmesh pathfinding

	NavMeshPath testPath;

	public void ClearTestPath ()
	{
		if (testPath != null)
		{
			testPath.ClearCorners ();
		}
	}

	public void TestNavmeshPathfinding ()
	{

		testPath = new NavMeshPath ();

		if (navmeshPFObject.GetPath (TerrainCoordToWorldPos (testStart, specifiedTerrain), TerrainCoordToWorldPos (testGoal, specifiedTerrain), testPath) == false)
		{
			return;
		}

		DrawNavmeshPath (testPath);

		Debug.Log ("New Path: ");
		for (int i = 0; i < testPath.corners.Length; i++)
		{
			Debug.Log ("\t" + testPath.corners [i]);
		}

	}



	#endregion

	#endregion

	#region BuildingSpawning

	int maxBuildingSpawnTries = 10;

	//try and spawn a building at a random position
	public void RandomSpawnBuilding ()
	{
		bool success = false;
		int counter = 0;

		while (success == false && counter < maxBuildingSpawnTries)
		{
			Debug.Log ("Spawn Building Try: " + (++counter));

			success = TryRandomSpawnBuilding ();
		}

		Debug.Log ("Building Spawned");
	}

	Vector2Int lastTriedBuildingSpawn;

	public bool TryRandomSpawnBuilding ()
	{
		if (connectedBuildings == null)
		{
			connectedBuildings = new List<GameObject> ();
		}

		float highestHeight = 0;
		float highestSteepness = 0.0f;

		//get the maximum spawn height and steepness of all the buildings
		for (int i = 0; i < buildingList.Count; i++)
		{
			if (highestHeight <= buildingList [i].GetComponent<Building> ().maxHeight)
			{
				highestHeight = buildingList [i].GetComponent<Building> ().maxHeight;
			}
			if (highestSteepness <= buildingList [i].GetComponent<Building> ().maxSteepness)
			{
				highestSteepness = buildingList [i].GetComponent<Building> ().maxSteepness;
			}
		}

		//get all the terrain coordinate from settlement data that are lower and shallower than the max height and steepness
		List<CoordData> coordUseList = new List<CoordData> ();
		coordUseList = currentSettlementData.GetCoordsLowerThanAndShallowerThan (highestHeight, highestSteepness);

		Vector2Int prospectivePosition = coordUseList [Random.Range (0, coordUseList.Count)].pos;

		lastTriedBuildingSpawn = prospectivePosition;


		Debug.Log ("Try Add at: " + prospectivePosition);


		List<int> validBuildings = new List<int> ();

		//for each building type
		for (int i = 0; i < buildingList.Count; i++)
		{
			float buildingMaxSteepness = buildingList [i].GetComponent<Building> ().maxSteepness;
			float buildingMaxHeight = buildingList [i].GetComponent<Building> ().maxHeight;
			float buildingRadius = buildingList [i].GetComponent<Building> ().radius / terrainToWorldScale;

			//if the randomly selected position is too high or too steep, then it is not a valid building
			if (specifiedTerrain.terrainData.GetHeight (prospectivePosition.x, prospectivePosition.y) <= buildingMaxSteepness)
			{
				if (specifiedTerrain.terrainData.GetSteepness (prospectivePosition.x, prospectivePosition.y) <= buildingMaxHeight)
				{
					//if building would be slightly off map, then it is not a valid building
					if (prospectivePosition.x - buildingRadius > 0.0f &&
					    prospectivePosition.x + buildingRadius < specifiedTerrain.terrainData.heightmapWidth &&
					    prospectivePosition.y - buildingRadius > 0.0f &&
					    prospectivePosition.y + buildingRadius < specifiedTerrain.terrainData.heightmapHeight)
					{
						if (buildingList [i].GetComponent<Building> ().useProximityCondition == true)
						{
							//DEBUG
							if (Vector2.Distance (prospectivePosition, buildingList [i].GetComponent<Building> ().debugPoint) < buildingList [i].GetComponent<Building> ().maxDistanceFromPoint)
							{
								validBuildings.Add (i);
							}
						}
						else
						{
							validBuildings.Add (i);
						}
					}
				}
			}
		}

		if (validBuildings.Count == 0)
		{
			Debug.Log ("\tNo Valid Buildings");
			return false;
		}

		//randomly select one of the valid buildings
		int selectedBuilding = validBuildings [Random.Range (0, validBuildings.Count)];

		//try and set the building at that coord
		bool worked = TrySetBuildingCoord (prospectivePosition, selectedBuilding);

		if (worked == true)
		{
			Debug.Log ("\tSuccessfully Added - Buidling ID: " + selectedBuilding);
		}
		else
		{
			Debug.Log ("\tBuilding Not Added");
		}



		return worked;
	}



	//debug


	#endregion
}