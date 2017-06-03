using UnityEngine;
using UnityEngine.UI;

public class ClearTimeText : MonoBehaviour
{
	private void Start ()
    {
	    var totalTime = GameManager.Instance.GameTime;
	    var mins = Mathf.FloorToInt(totalTime / 60);
	    var seconds = Mathf.RoundToInt(totalTime % 60);
	    GetComponent<Text>().text = "Clear time: " + mins + " minutes and " + seconds + " seconds";
	}
}
