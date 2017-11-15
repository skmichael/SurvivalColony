using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Worker
{
	public class WorkerController : MonoBehaviour
	{
		private static int _curId;

		[SerializeField]private Transform[] _allObjects;
		private Animator _anim;
		private float _cold;
		private GameObject _grass;

		private int _myId;
		private NavMeshAgent _navAgent;
		private ResourceManager _resource;
		private GameObject _rock;
		private GameObject _tree;
		public Text CurrentJob;
		public Slider HealthBar;
		public Slider HungerBar;
		public Text InsulationText;
		public Slider StaminaBar;
		public Text TemperatureText;
		public Slider ThirstBar;
		public Worker Workers;

		private void Awake()
		{
			//worker Starting Settings
			Workers = new Worker
			{
				WorkerName = "",
				WorkerNumber = _myId = _curId++,
				WorkerAutoHarvest = false,
				WorkerIsSelected = false,
				WorkerRole = "Miner",
				WorkerHarvesting = false,
				WorkerRaidus = 10,
				WorkerHealth = 100,
				WorkerHunger = 100,
				WorkerInsulation = 0,
				WorkerStamina = 100,
				WorkerTemperature = 25,
				WorkerThirst = 100
			};
		}


		private void Start()
		{
			if (_allObjects.Length == 0)
				_allObjects = new Transform[_curId];
		
			_allObjects[_myId] = transform;

			_resource = GameObject.FindWithTag("MainCamera").GetComponentInChildren<ResourceManager>();
			_navAgent = GetComponentInChildren<NavMeshAgent>();
			_anim = GetComponentInChildren<Animator>();
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
				AutoWork(Workers.WorkerNumber, Workers.WorkerAutoHarvest);


			//Workers.WorkerAllowMovement = !Workers.WorkerHarvesting;

			if (Workers.WorkerIsSelected)
				IsSelected();

			//Roll Selection
			switch (Workers.WorkerRole)
			{
				case "Lumberjack":
					Lumberjack();
					break;
				case "Miner":
					Miner();
					break;
				case "Farmer":
					Farmer();
					break;
				default:
					Farmer();
					break;
			}


			//Hunger Controller
			if (Workers.WorkerHunger <= 100)
				Workers.WorkerHunger -= (.1f + _cold) * Time.deltaTime;

			if (Workers.WorkerHunger <= 0)
				Starvation();

			HungerBar.minValue = 0;
			HungerBar.maxValue = 100;
			//HungerBar.value = Workers.WorkerHunger;

			//Thirst Controller
			if (Workers.WorkerThirst <= 0)
				Dehydration();

			ThirstBar.minValue = 0;
			ThirstBar.maxValue = 100;
			ThirstBar.value = Workers.WorkerThirst;

			//Stamina Controller
			if (Workers.WorkerStamina <= 0)
				Fatigue();

			StaminaBar.minValue = 0;
			StaminaBar.maxValue = 100;
			StaminaBar.value = Workers.WorkerStamina;

			//Health Controller
			if (Workers.WorkerHealth <= 0)
				Dead();

			HealthBar.minValue = 0;
			HealthBar.maxValue = 100;
			HealthBar.value = Workers.WorkerHealth;

			//Temperauture Controller
			if (Workers.WorkerTemperature <= 20 - Workers.WorkerInsulation)
				Hypothermia();

			if (Workers.WorkerTemperature > 20 - Workers.WorkerInsulation &&
			    Workers.WorkerTemperature <= 40 - Workers.WorkerInsulation)
				Cold();

			if (Workers.WorkerTemperature > 60 + Workers.WorkerInsulation &&
			    Workers.WorkerTemperature <= 80 + Workers.WorkerInsulation)
				Hot();

			if (Workers.WorkerTemperature >= 100 + Workers.WorkerInsulation)
				Hyperthermia();

			TemperatureText.text = "Temperature " + Workers.WorkerTemperature + " F";
			InsulationText.text = "Insulation " + Workers.WorkerInsulation;
		}

		private void Hypothermia()
		{
			//Debug.Log("Hypothermia");
			Workers.WorkerHealth -= 2 * Time.deltaTime;
			_cold = .5f;
		}

		private void Cold()
		{
			//Debug.Log("Cold");
			_cold = .2f;
		}

		private void Hot()
		{
			//Debug.Log("Hot");
		}

		private void Hyperthermia()
		{
			//Debug.Log("Hyperthermia");
			Workers.WorkerHealth -= 2 * Time.deltaTime;
		}

		private void Dead()
		{
			if (Workers.WorkerHealth <= 0)
				print("Dead");
		}

		private void Fatigue()
		{
			if (Workers.WorkerStamina <= 0)
				Workers.WorkerStamina = 0;
		}

		private void Dehydration()
		{
			Workers.WorkerHealth -= 2 * Time.deltaTime;

			if (Workers.WorkerThirst <= 0)
				Workers.WorkerThirst = 0;
		}

		private void Starvation()
		{
			Workers.WorkerHealth -= 2 * Time.deltaTime;

			if (Workers.WorkerHunger <= 0)
				Workers.WorkerHunger = 0;
		}

		private void Farmer()
		{
			if (!Workers.WorkerAutoHarvest || !Workers.WorkerAllowMovement) return;
			var minDistance = float.MaxValue;
			var hitColliders = Physics.OverlapSphere(transform.position, Workers.WorkerRaidus);
			var i = 0;
			while (i < hitColliders.Length)
			{
				if (hitColliders[i].tag.Equals("Grass"))
				{
					var possiblePosition = hitColliders[i].transform.position;

					var currDistance = Vector3.Distance(transform.position, possiblePosition);

					if (currDistance < minDistance)
					{
						_grass = hitColliders[i].gameObject;

						minDistance = currDistance;
						Debug.DrawLine(transform.position, hitColliders[i].transform.position, Color.red);
						_navAgent.destination = hitColliders[i].transform.position;
						if (currDistance <= 2 && !Workers.WorkerHarvesting)
							HarvestGrass();
					}
				}
				i++;
			}
		}

		private void Miner()
		{
			if (!Workers.WorkerAutoHarvest || !Workers.WorkerAllowMovement) return;
			var minDistance = float.MaxValue;
			var hitColliders = Physics.OverlapSphere(transform.position, Workers.WorkerRaidus);
			var i = 0;
			while (i < hitColliders.Length)
			{
				if (hitColliders[i].tag.Equals("Rock"))
				{
					var possiblePosition = hitColliders[i].transform.position;

					var currDistance = Vector3.Distance(transform.position, possiblePosition);

					if (currDistance < minDistance)
					{
						_rock = hitColliders[i].gameObject;

						minDistance = currDistance;
						Debug.DrawLine(transform.position, hitColliders[i].transform.position, Color.red);
						_navAgent.destination = hitColliders[i].transform.position;
						if (currDistance <= 2 && !Workers.WorkerHarvesting)
							HarvestRocks();
					}
				}
				i++;
			}
		}

		private void Lumberjack()
		{
			if (!Workers.WorkerAutoHarvest || !Workers.WorkerAllowMovement) return;
			var minDistance = float.MaxValue;
			var hitColliders = Physics.OverlapSphere(transform.position, Workers.WorkerRaidus);
			var i = 0;
			while (i < hitColliders.Length)
			{
				if (hitColliders[i].tag.Equals("Tree"))
				{
					var possiblePosition = hitColliders[i].transform.position;

					var currDistance = Vector3.Distance(transform.position, possiblePosition);

					if (currDistance < minDistance)
					{
						_tree = hitColliders[i].gameObject;

						minDistance = currDistance;
						Debug.DrawLine(transform.position, hitColliders[i].transform.position, Color.blue);
						_navAgent.destination = hitColliders[i].transform.position;
						if (currDistance <= 2.5f && !Workers.WorkerHarvesting)
						{
							Debug.Log("HArt");
							StartCoroutine(IsHarvestingWood());
						}
					}
				}
				i++;
			}
		}

		private void HarvestGrass()
		{
			Workers.WorkerHarvesting = true;
			StartCoroutine(IsHarvestingGrass());
		}

		private void HarvestRocks()
		{
			Workers.WorkerHarvesting = true;
			StartCoroutine(IsHarvestingRock());
		}

		private void IsSelected()
		{
			CurrentJob.text = Workers.WorkerRole;
			if (Input.GetMouseButtonDown(0) && Workers.WorkerAllowMovement)
			{
				RaycastHit hit;

				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
					_navAgent.destination = hit.point;
			}
			_anim.SetBool("Walking", _navAgent.hasPath);
		}

		private void OnMouseDown()
		{
			Workers.WorkerIsSelected = !Workers.WorkerIsSelected;
		}

		public void WorkButton()
		{
			var id = GetComponent<WorkerController>()._myId;
			AutoWork(id, Workers.WorkerAutoHarvest);
		}

		public void AutoWork(int id, bool autoHarvest)
		{
			if (_myId == id && !autoHarvest && Workers.WorkerIsSelected)
			{
				Workers.WorkerAutoHarvest = true;
				Workers.WorkerAllowMovement = false;
			}
			else if (_myId == id && autoHarvest && Workers.WorkerIsSelected)
			{
				Workers.WorkerAutoHarvest = false;
			}
		}

		public void RoleSelect(string roll)
		{
			foreach (var workers in GameObject.FindGameObjectsWithTag("Player"))
				if (workers.GetComponent<WorkerController>().Workers.WorkerIsSelected)
					workers.GetComponent<WorkerController>().Workers.WorkerRole = roll;
		}


		private IEnumerator IsHarvestingWood()
		{
			Workers.WorkerHarvesting = true;
			yield return new WaitForSeconds(10f);
			Workers.WorkerHarvesting = false;
			_resource.WoodAmount += 10;
			Destroy(_tree);
		}

		private IEnumerator IsHarvestingRock()
		{
			yield return new WaitForSeconds(10f);
			Workers.WorkerHarvesting = false;
			_resource.StoneAmount += 10;
			Destroy(_rock);
		}

		private IEnumerator IsHarvestingGrass()
		{
			yield return new WaitForSeconds(10f);
			Workers.WorkerHarvesting = false;
			_resource.FiberAmount += 10;
			Destroy(_grass);
		}
	}
}