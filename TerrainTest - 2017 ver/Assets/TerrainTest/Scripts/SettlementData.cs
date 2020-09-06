//A set of classes for storing terrain and settlement data within a grid

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for WriteToCSV
using System.Text;
using System.IO;
using System;

//default terrain coordinate data, all coords start as this
public class CoordData 
{

	public Vector2Int pos;

	public float steepness; //steepness at this terrain coord
	public float height; //height at this terrain coord

	public virtual bool IsRoad()
	{
		return false;
	}
	public virtual bool IsBuilding()
	{
		return false;
	}
	public virtual bool IsBuildingCentre()
	{
		return false;
	}

}
//terrain coord data for a road
public class RoadCoord : CoordData
{

	//store both in world units

	public float fullWidth; //distance from road at which the road texture is 100%
	public float maxWidth; //distance at which the rod texture stops, blends between full and none until this point

	public override bool IsRoad()
	{
		return true;
	}
}
//terrain coord data for a building, note that single buildings take up multiple coordinates
public class BuildingCoord : CoordData
{

	public int buildingID;
	public float buildingHeight; //height at which the building starts, not including foundations

	public bool centre; //is this coordinate the centre of the building
	public float yRotation;

	public override bool IsBuilding()
	{
		return true;
	}
	public override bool IsBuildingCentre()
	{
		return centre;
	}
}

public class SettlementData 
{

	public int width;
	public int height;
	public List<List<CoordData>> terrainCoordData;



	public void Init (Terrain _terrain)
	{
		//populate the settlementData with the height and angle of the terrain

		TerrainData terData = _terrain.terrainData;

		//height is y, width is x
		width = terData.heightmapWidth;
		height = terData.heightmapHeight;
		terrainCoordData = new List<List<CoordData>> ();

		for (int y = 0; y < height; y++)
		{
			List<CoordData> tempList = new List<CoordData>();
			for (int x = 0; x < width; x++)
			{
				CoordData tempCoordData = new CoordData ();

				tempCoordData.pos = new Vector2Int(x,y);

				tempCoordData.height = terData.GetHeight (x, y);
				tempCoordData.steepness = terData.GetSteepness((float)x / (width - 1), (float)y / (height - 1));

				tempList.Add (tempCoordData);
			}
			terrainCoordData.Add (tempList);
		}

		Debug.Log ("Finished Creating Settlement Data");
	}

	public CoordData GetDataAt (int _x, int _y)
	{
		return terrainCoordData [_y][_x];
	}
	public void SetDataAt (int _x, int _y, CoordData _data)
	{
		terrainCoordData [_y] [_x] = _data;
	}

	//return all coordinates with a steepness less than _angle
	public List<CoordData> GetCoordsShallowerThan (float _angle)
	{
		List<CoordData> validcoords = new List<CoordData> ();

		foreach (List<CoordData> dataList in terrainCoordData)
		{
			foreach (CoordData data in dataList)
			{
				if (data.steepness <= _angle)
				{
					validcoords.Add (data);
				}
			}	 
		}

		return validcoords;
	}
	//return all coordinates with a height less than _height
	public List<CoordData> GetCoordsLowerThan (float _height)
	{
		List<CoordData> validcoords = new List<CoordData> ();

		foreach (List<CoordData> dataList in terrainCoordData)
		{
			foreach (CoordData data in dataList)
			{
				if (data.height <= _height)
				{
					validcoords.Add (data);
				}
			}	 
		}

		return validcoords;
	}
	//return all coordinates with a height less than _height and a steepness less than _angle
	public List<CoordData> GetCoordsLowerThanAndShallowerThan (float _height, float _angle)
	{
		List<CoordData> validcoords = new List<CoordData> ();

		foreach (List<CoordData> dataList in terrainCoordData)
		{
			foreach (CoordData data in dataList)
			{
				if (data.height <= _height)
				{
					if (data.steepness <= _angle)
					{
						validcoords.Add (data);
					}
				}
			}	 
		}

		return validcoords;
	}


	//set a coordinate to be a road coord
	public void SetRoadCoord(int _x, int _y, float _fullWidth, float _maxWidth)
	{
		CoordData currentData = GetDataAt(_x, _y);
		RoadCoord newData = new RoadCoord ();

		newData.pos = currentData.pos;
		newData.height = currentData.height;
		newData.steepness = currentData.steepness;

		newData.fullWidth = _fullWidth;
		newData.maxWidth = _maxWidth;

		SetDataAt (_x, _y, newData);

		//Debug.Log ("Setting Road Coord at (" + _x + "," + _y + ")");
	}

	//return all road coords
	public List<RoadCoord> GetRoadCoords()
	{
		List<RoadCoord> returnCoordDatas = new List<RoadCoord> ();

		foreach (List<CoordData> dataList in terrainCoordData)
		{
			foreach (CoordData data in dataList)
			{
				if (data.GetType () == typeof(RoadCoord))
				{
					returnCoordDatas.Add ((RoadCoord)data);
				}
			}	 
		}

		Debug.Log("Returning Road Coord Datas");

		return returnCoordDatas;
	}

	//set a coordinate to be a building coord
	public void SetBuildingCoord(int _x, int _y, int _buildingID, float _buildingHeight, bool _centre, float _yRotation)
	{
		CoordData currentData = GetDataAt(_x, _y);
		BuildingCoord newData = new BuildingCoord ();

		newData.pos = currentData.pos;
		newData.height = currentData.height;
		newData.steepness = currentData.steepness;

		newData.buildingID = _buildingID;
		newData.buildingHeight = _buildingHeight;

		newData.centre = _centre;

		newData.yRotation = _yRotation;

		SetDataAt (_x, _y, newData);

		//Debug.Log ("Setting Building Coord at (" + _x + "," + _y + ")");
	}

	//return all centre building coords
	public List<BuildingCoord> GetBuildingCoords()
	{
		List<BuildingCoord> returnCoordDatas = new List<BuildingCoord> ();

		foreach (List<CoordData> dataList in terrainCoordData)
		{
			foreach (CoordData data in dataList)
			{
				if (data.GetType () == typeof(BuildingCoord))
				{
					if (data.IsBuildingCentre () == true)
					{
						returnCoordDatas.Add ((BuildingCoord)data);
					}
				}
			}	 
		}

		Debug.Log("Returning Building Coord Datas");

		return returnCoordDatas;
	}



	public void SetStraightRoadCoords(int _xStart, int _yStart, int _xEnd, int _yEnd, float _fullWidth, float _maxWidth)
	{
		//https://www.redblobgames.com/grids/line-drawing.html
		//linear interpolation

		Vector2Int start = new Vector2Int (_xStart, _yStart);
		Vector2Int end = new Vector2Int (_xEnd, _yEnd);

		List<Vector2Int> roadCoords = new List<Vector2Int> ();

		int N = diagonalDistance (start, end);

		for (int i = 0; i <= N; i++)
		{
			float t = (N == 0 ? 0.0f : (float)i / N);
			roadCoords.Add (roundPoint (Vector2.Lerp(start, end, t)));
		}

		foreach (Vector2Int coord in roadCoords)
		{
			SetRoadCoord (coord.x, coord.y, _fullWidth, _maxWidth);
		}
	}

	int diagonalDistance(Vector2Int _p0, Vector2Int _p1)
	{
		int dx = _p1.x - _p0.x;
		int dy = _p1.y - _p0.y;

		return (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)));
	}

	Vector2Int roundPoint(Vector2 _p)
	{
		return new Vector2Int (Mathf.RoundToInt(_p.x), Mathf.RoundToInt(_p.y));
	}

	#region Debug

	//debug functions used to write coordinate data to a CSV format
	//allows the data to be visualised in Excel

	public void WriteHeightmapToCSV ()
	{
		// https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

		List<string[]> rowData = new List<string[]> ();
		string[] rowDataTemp;

		for (int y = height - 1; y >= 0; y--)
		{
			rowDataTemp = new string[width];

			for (int x = 0; x < width; x++)
			{
				rowDataTemp [x] = GetDataAt(x, y).height.ToString();
			}

			rowData.Add (rowDataTemp);
		}

		string[][] output = new string[rowData.Count][];

		for (int i = 0; i < output.Length; i++)
		{
			output [i] = rowData [i];
		}

		int length = output.GetLength (0);
		string delimiter = ",";

		//heightmap
		StringBuilder heightMap = new StringBuilder ();

		for (int i = 0; i < length; i++)
		{
			heightMap.AppendLine (string.Join (delimiter, output [i]));
		}

		string filePath = Application.dataPath + "/CSV/" + "terrain_height_data.csv";

		if (File.Exists(filePath) == true)
		{
			File.Delete(filePath);
		}

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(heightMap);
		outStream.Close ();


	}

	public void WriteSteepnessmapToCSV ()
	{
		// https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

		List<string[]> rowData = new List<string[]> ();
		string[] rowDataTemp;

		for (int y = height - 1; y >= 0; y--)
		{
			rowDataTemp = new string[width];

			for (int x = 0; x < width; x++)
			{
				rowDataTemp [x] = GetDataAt(x, y).steepness.ToString();
			}

			rowData.Add (rowDataTemp);
		}

		string[][] output = new string[rowData.Count][];

		for (int i = 0; i < output.Length; i++)
		{
			output [i] = rowData [i];
		}

		int length = output.GetLength (0);
		string delimiter = ",";

		//heightmap
		StringBuilder heightMap = new StringBuilder ();

		for (int i = 0; i < length; i++)
		{
			heightMap.AppendLine (string.Join (delimiter, output [i]));
		}

		string filePath = Application.dataPath + "/CSV/" + "terrain_steepness_data.csv";

		if (File.Exists(filePath) == true)
		{
			File.Delete(filePath);
		}

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(heightMap);
		outStream.Close ();


	}

	public void WriteLandmarksToCSV()
	{
		// https://sushanta1991.blogspot.com/2015/02/how-to-write-data-to-csv-file-in-unity.html

		List<string[]> rowData = new List<string[]> ();
		string[] rowDataTemp;

		for (int y = height - 1; y >= 0; y--)
		{
			rowDataTemp = new string[width];

			for (int x = 0; x < width; x++)
			{
				if (GetDataAt(x, y).GetType () == typeof(RoadCoord))
				{
					rowDataTemp[x] = "R";
				} 
				else if (GetDataAt(x, y).GetType () == typeof(BuildingCoord))
				{
					if (GetDataAt(x, y).IsBuildingCentre () == true)
					{
						rowDataTemp [x] = "CB";
					}
					else
					{
						rowDataTemp[x] = "B";
					}
				} 
				else
				{
					rowDataTemp[x] = "N";
				}
			}

			rowData.Add (rowDataTemp);
		}

		string[][] output = new string[rowData.Count][];

		for (int i = 0; i < output.Length; i++)
		{
			output [i] = rowData [i];
		}

		int length = output.GetLength (0);
		string delimiter = ",";

		//heightmap
		StringBuilder heightMap = new StringBuilder ();

		for (int i = 0; i < length; i++)
		{
			heightMap.AppendLine (string.Join (delimiter, output [i]));
		}

		string filePath = Application.dataPath + "/CSV/" + "terrain_lankmarks_data.csv";

		if (File.Exists(filePath) == true)
		{
			File.Delete(filePath);
		}

		StreamWriter outStream = System.IO.File.CreateText(filePath);
		outStream.WriteLine(heightMap);
		outStream.Close ();
	}

	#endregion
}


