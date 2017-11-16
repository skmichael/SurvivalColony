using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private int _currentGrassAmount;
	private int _currentRockAmount;
	private int _currentTreeAmount;
	[SerializeField] private int _maxGrassAmount = 500;
	[SerializeField] private int _maxRockAmount = 500;
	[SerializeField] private int _maxTreeAmount = 500;

	public GameObject Grass;
	public GameObject Rocks;
	public GameObject Sun;
	public GameObject Trees;
	public Terrain Td;
	public LayerMask Ground;

	private void Start()
	{
	}


	private void Update()
	{
		

		//Spawn Harvestable Objects
		if (_currentRockAmount < _maxRockAmount)
			SpawnRock();
		if (_currentTreeAmount < _maxTreeAmount)
			SpawnTree();
		if (_currentGrassAmount < _maxGrassAmount)
			SpawnGrass();

		//Sun Rotation
		Sun.transform.Rotate(1f * Time.deltaTime, 0, 0);
	}

	private void SpawnTree()
	{
		for (var i = _currentTreeAmount; i < _maxTreeAmount; i++)
		{
			Vector3 randomTreeSpawn = new Vector3(Random.Range(-500, 500), 0, Random.Range(-500, 500));

			RaycastHit hit;
			Physics.Raycast(randomTreeSpawn, Vector3.down, out hit);
			if(Td.SampleHeight(randomTreeSpawn) >= 200)
			{
				var tree = Instantiate(Trees, randomTreeSpawn, Quaternion.Euler(0, Random.Range(0, 360), 0));
				tree.transform.parent = transform;
				_currentTreeAmount += 1;
			}
		}
	}

	private void SpawnRock()
	{
		for (var i = _currentRockAmount; i < _maxRockAmount; i++)
		{
			var randomRockSpawn = new Vector3(Random.Range(-500, 500), 0.2f, Random.Range(-500, 500));

			RaycastHit hit;
			Physics.Raycast(randomRockSpawn, Vector3.down, out hit);
			if (Td.SampleHeight(randomRockSpawn) >= 200)
			{
				var rock = Instantiate(Rocks, randomRockSpawn, Quaternion.Euler(0, Random.Range(0, 360), 0));
				rock.transform.parent = transform;
				_currentRockAmount += 1;
			}
		}
	}

	private void SpawnGrass()
	{
		for (var i = _currentGrassAmount; i < _maxGrassAmount; i++)
		{
			var randomGrassSpawn = new Vector3(Random.Range(-500, 500), 0f, Random.Range(-500, 500));

			RaycastHit hit;
			Physics.Raycast(randomGrassSpawn, Vector3.down, out hit);
			if (Td.SampleHeight(randomGrassSpawn) >= 200)
			{
				var grass = Instantiate(Grass, randomGrassSpawn, Quaternion.Euler(0, Random.Range(0, 360), 0));
				grass.transform.parent = transform;
				_currentGrassAmount += 1;
			}
		}
	}
	
	//private string BuildChunkFileName(Vector3 v)
	//{
	//	return Application.persistentDataPath + "/savedata/Chunk" +
	//	       (int) v.x + "" +
	//	       (int) v.y + "" +
	//	       (int) v.z +
	//		   "chunkSize" +
	//	       "_" + 
	//	       ".dat";
	//}
	//
	//private bool Load() //read data from file
	//{
	//	var chunkFile = BuildChunkFileName(Trees.transform.position);
	//	if (File.Exists(chunkFile))
	//	{
	//		var bf = new BinaryFormatter();
	//		var file = File.Open(chunkFile, FileMode.Open);
	//		bd = new BlockData();
	//		bd = (BlockData) bf.Deserialize(file);
	//		file.Close();
	//		//Debug.Log("Loading chunk from file: " + chunkFile);
	//		return true;
	//	}
	//	return false;
	//}
	//
	//public void Save() //write data to file
	//{
	//	var chunkFile = BuildChunkFileName(chunk.transform.position);
	//
	//	if (!File.Exists(chunkFile))
	//		Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));
	//	var bf = new BinaryFormatter();
	//	var file = File.Open(chunkFile, FileMode.OpenOrCreate);
	//	bd = new BlockData(chunkData);
	//	bf.Serialize(file, bd);
	//	file.Close();
	//}
}