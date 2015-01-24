using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueManager : MonoBehaviour {

	//Probably doesn't need to be a singleton
	//BEGINNING OF SINGLETON CODE CONFIGURATION
	private static volatile PatternQueueManager _instance;
	private static object _lock = new object();
	public PatternQueue patternQueue;

	//Stops the lock being created ahead of time if it's not necessary
	static PatternQueueManager() {
	}

	public static PatternQueueManager Instance {
		get {
			if(_instance == null) {
				lock(_lock) {
					if (_instance == null) {
						GameObject PatternQueueManagerGameObject = new GameObject("PatternQueueManagerGameObject");
						_instance = (PatternQueueManagerGameObject.AddComponent<PatternQueueManager>()).GetComponent<PatternQueueManager>();
					}
				}
			}
			return _instance;
		}
	}

	private PatternQueueManager() {
	}

	public void Awake() {
		_instance = this;
	}
	//END OF SINGLETON CODE CONFIGURATION

	public void Start() {
	}

	public void Update() {
		PerformDebugKeys();
	}

	public void PerformDebugKeys() {
		if(Input.GetKeyDown(KeyCode.G)) {
			Debug.Log("Generating Pattern In Queue");
			GetPatternQueue().AddAtFront(Pattern.Block);
		}
	}

	public PatternQueue GetPatternQueue() {
		return patternQueue;
	}
}
