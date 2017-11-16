using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Worker
{
	public class WorkerController : MonoBehaviour
	{
		private static int _curId;
	
		[SerializeField]private List<Transform> _allObjects;
		private Animator _anim;
		private float _cold;
		private GameObject _resourse;

		private int _myId;
		private NavMeshAgent _navAgent;
		private ResourceManager _resource;
		//private GameObject _rock;
		//private GameObject _tree;
		public Text CurrentJob;
		public Slider HealthBar;
		public Slider HungerBar;
		public Text InsulationText;
		public Slider StaminaBar;
		public Text TemperatureText;
		public Slider ThirstBar;
		public Worker Workers;
		public Transform WorkerPrefab;

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
			if (_allObjects.Count == 0)
				_allObjects = new List<Transform>(_curId);
		
			//_allObjects[_myId] = transform;

			_resource = GameObject.FindWithTag("MainCamera").GetComponentInChildren<ResourceManager>();
			_navAgent = GetComponentInChildren<NavMeshAgent>();
			_anim = GetComponentInChildren<Animator>();


			//CurrentJob.text
			//HealthBar.maxValue = 100f;
			//HealthBar.minValue = 0f;
			//HungerBar.
			//Insulation
			//StaminaBar
			//Temperatur
			//ThirstBar.
		}


		private void Update()
		{

			if (Input.GetKeyDown(KeyCode.T))
			{
				var workerprefab = Instantiate(WorkerPrefab, Vector3.forward, Quaternion.Euler(0, 0, 0));
				_allObjects.Add(workerprefab);
				
			}

			if (Input.GetKeyDown(KeyCode.Q))
				AutoWork(Workers.WorkerNumber, Workers.WorkerAutoHarvest);


			Workers.WorkerAllowMovement = !Workers.WorkerHarvesting;

			if (Workers.WorkerIsSelected)
			{ 
			
				IsSelected();
			}

			//Update Vitals Based On Whose Selected
			foreach (var i in GameObject.FindGameObjectsWithTag("Player"))
			{
				var workers = i.GetComponent<WorkerController>();
				if (workers.Workers.WorkerIsSelected)
				{
					CurrentJob.text = workers.Workers.WorkerRole;
					HealthBar.value = workers.Workers.WorkerHealth;
					HungerBar.value = workers.Workers.WorkerHunger;
					InsulationText.text = "Insulation" + workers.Workers.WorkerInsulation;
					StaminaBar.value = workers.Workers.WorkerStamina;
					TemperatureText.text = "Temperature" + workers.Workers.WorkerTemperature;
					ThirstBar.value = workers.Workers.WorkerThirst;
				}
			}



			//Roll Selection
				switch (Workers.WorkerRole)
			{
				case "Lumberjack":
					WorkJob("Tree");
					break;
				case "Miner":
					WorkJob("Rock");
					break;
				case "Farmer":
					WorkJob("Grass");
					break;
				default:
					WorkJob("Farmer");
					break;
			}

			#region workerController
			//Hunger Controller
			if (Workers.WorkerHunger <= 100)
				Workers.WorkerHunger -= (.1f + _cold) * Time.deltaTime;

			if (Workers.WorkerHunger <= 0)
				Starvation();

			//HungerBar.minValue = 0;
			//HungerBar.maxValue = 100;
			//HungerBar.value = Workers.WorkerHunger;

			//Thirst Controller
			if (Workers.WorkerThirst <= 0)
				Dehydration();

			//ThirstBar.minValue = 0;
			//ThirstBar.maxValue = 100;
			//ThirstBar.value = Workers.WorkerThirst;

			//Stamina Controller
			if (Workers.WorkerStamina <= 0)
				Fatigue();

			//StaminaBar.minValue = 0;
			//StaminaBar.maxValue = 100;
			//StaminaBar.value = Workers.WorkerStamina;

			//Health Controller
			if (Workers.WorkerHealth <= 0)
				Dead();

			//HealthBar.minValue = 0;
			//HealthBar.maxValue = 100;
			//HealthBar.value = Workers.WorkerHealth;

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

			//TemperatureText.text = "Temperature " + Workers.WorkerTemperature + " F";
			//InsulationText.text = "Insulation " + Workers.WorkerInsulation;
#endregion
		}

		private void WorkJob(string job)
		{
			if (!Workers.WorkerAutoHarvest || !Workers.WorkerAllowMovement) return;
			var minDistance = float.MaxValue;
			var hitColliders = Physics.OverlapSphere(transform.position, Workers.WorkerRaidus);
			var i = 0;
			while (i < hitColliders.Length)
			{
				if (hitColliders[i].tag.Equals(job))
				{
					var possiblePosition = hitColliders[i].transform.position;

					var currDistance = Vector3.Distance(transform.position, possiblePosition);

					if (currDistance < minDistance)
					{
						_resourse = hitColliders[i].gameObject;

						minDistance = currDistance;
						Debug.DrawLine(transform.position, hitColliders[i].transform.position, Color.red);
						_navAgent.destination = hitColliders[i].transform.position;
						if (currDistance <= 2 && !Workers.WorkerHarvesting)
						{
							StartCoroutine(Harvesting(job));
						}
					}
				}
				i++;
			}
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
			print("button");
			foreach (var id in GameObject.FindGameObjectsWithTag("Player"))
			{
				var worker = id.GetComponent<WorkerController>().Workers;
				if (worker.WorkerIsSelected)
				{
					worker.WorkerAutoHarvest = !worker.WorkerAutoHarvest;
					worker.WorkerAllowMovement = !worker.WorkerAllowMovement;
				}
			}
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
			{
				var worker = workers.GetComponent<WorkerController>().Workers;
				if (worker.WorkerIsSelected)
					worker.WorkerRole = roll;
			}
		}

		private IEnumerator Harvesting(string job)
		{
			Workers.WorkerHarvesting = true;
			yield return new WaitForSeconds(10f);
			Workers.WorkerHarvesting = false;
			switch (job)
			{
				case "Tree":
					_resource.WoodAmount += 10;
					break;
				case "Rock":
					_resource.StoneAmount += 10;
					break;
				case "Grass":
					_resource.FiberAmount += 10;
					break;
				default:
					break;
			}
			Destroy(_resourse);
		}

		//////Weather Health Effects//////
		private void Hypothermia()
		{
			Workers.WorkerHealth -= 2 * Time.deltaTime;
			_cold = .5f;
		}

		private void Cold()
		{
			_cold = .2f;
		}

		private void Hot()
		{
		}

		private void Hyperthermia()
		{
			Workers.WorkerHealth -= 2 * Time.deltaTime;
		}

		private void Dead()
		{
			if (Workers.WorkerHealth <= 0)
				print("Dead");
		}

		//////Worker Health Effects//////
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

	}
}