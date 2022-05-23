using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimObjSpawner : StateMachineBehaviour 
{
	public GameObject gO;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		GameObject instance = Instantiate(gO);
		instance.transform.position = animator.transform.position;
	}
}
