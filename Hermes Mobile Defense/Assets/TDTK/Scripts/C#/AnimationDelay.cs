using UnityEngine;
using System.Collections;

public class AnimationDelay : MonoBehaviour {
	
	public float seconds;

	IEnumerator Delay() 
	{
		yield return new WaitForSeconds(seconds);
		animation.Play();
	}
}