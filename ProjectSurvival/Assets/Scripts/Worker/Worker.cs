namespace Worker
{
	public class Worker
	{
		//Worker Vitals
		public float WorkerHealth { get; set; }
		public float WorkerHunger { get; set; }
		public float WorkerInsulation { get; set; }
		public float WorkerStamina { get; set; }
		public float WorkerTemperature { get; set; }
		public float WorkerThirst { get; set; }

		//Worker Job
		public int WorkerNumber { get; set; }
		public float WorkerRaidus { get; set; }
		public string WorkerName { get; set; }
		public string WorkerRole { get; set; }
		public bool WorkerAutoHarvest { get; set; }
		public bool WorkerIsSelected { get; set; }
		public bool WorkerAllowMovement { get; set; }
		public bool WorkerHarvesting { get; set; }
	}
}

