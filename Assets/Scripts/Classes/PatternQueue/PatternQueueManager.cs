using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternQueueManager : MonoBehaviour {

    //Probably doesn't need to be a singleton
    //BEGINNING OF SINGLETON CODE CONFIGURATION
    private static volatile PatternQueueManager _instance;
    private static object _lock = new object();
    public PatternQueue patternQueue;
    public float generationDelay = 0f;
    public float generationDelayMax = 3f;
    public float minGenerationDelay = 1f;
    public float maxGenerationDelay = 3f;

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
        PerformGenerationDelayLogic();
    }

    public void PerformDebugKeys() {
        if(Input.GetKeyDown(KeyCode.G)) {
            Debug.Log("Generating Pattern In Queue");
            GetPatternQueue().AddPattern();
        }
    }

    public void PerformGenerationDelayLogic() {
        generationDelay += Time.deltaTime;
        CheckToGenerateNewPattern();
        CheckForExpiredPatternQueuObjects();
    }

    public void ResetGenerationDelay() {
        generationDelay = 0f;
        generationDelayMax = Random.Range(minGenerationDelay, maxGenerationDelay);
    }

    public PatternQueue GetPatternQueue() {
        return patternQueue;
    }

    public void PerformPatternAdd() {
        patternQueue.AddPattern();
    }

    public void CheckToGenerateNewPattern() {
        if(generationDelay > generationDelayMax) {
            PerformPatternAdd();
            ResetGenerationDelay();
        }
    }

    public void CheckForExpiredPatternQueuObjects() {
        List<PatternQueueObject> patternQueueObjectToRemove = new List<PatternQueueObject>();
        foreach(PatternQueueObject patternQueueObject in GetPatternQueue().patternQueueObjects) {
            if(patternQueueObject.HasExpired()) {
                patternQueueObject.GetTargetPathingReference().SetTargetPosition(new Vector3(patternQueueObject.transform.position.x, 
                                                                                             -50,
                                                                                             patternQueueObject.transform.position.z));
                patternQueueObjectToRemove.Add(patternQueueObject);
            }
        }
        foreach(PatternQueueObject patternQueueObject in patternQueueObjectToRemove) {
            GetPatternQueue().patternQueueObjects.Remove(patternQueueObject);
            patternQueueObject.GetTargetPathingReference().SetOnArrivalLogic(patternQueueObject.DestroyOnArrival);
        }
    }
}
