using UnityEngine;
using UnityEngine.UI;
public class ResourceManager : MonoBehaviour
{
	public int WoodAmount;
	public int StoneAmount;
	public int FiberAmount;

	public Text WoodText;
	public Text StoneText;
	public Text FiberText;

	private void Update ()
	{
		WoodText.text = "Wood: " + WoodAmount;
		StoneText.text = "Stone: " + StoneAmount;
		FiberText.text = "Fiber: " + FiberAmount;
	}
}
