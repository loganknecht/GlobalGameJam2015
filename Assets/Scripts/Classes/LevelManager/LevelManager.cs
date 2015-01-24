using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

  //Probably doesn't need to be a singleton
  //BEGINNING OF SINGLETON CODE CONFIGURATION
  private static volatile LevelManager _instance;
  private static object _lock = new object();

  //Stops the lock being created ahead of time if it's not necessary
  static LevelManager() {
  }

  public static LevelManager Instance {
    get {
      if(_instance == null) {
        lock(_lock) {
          if (_instance == null) {
            GameObject LevelManagerGameObject = new GameObject("LevelManagerGameObject");
            _instance = (LevelManagerGameObject.AddComponent<LevelManager>()).GetComponent<LevelManager>();
          }
        }
      }
      return _instance;
    }
  }

  private LevelManager() {
  }

  public void Awake() {
    _instance = this;
  }
  //END OF SINGLETON CODE CONFIGURATION

  public void Start() {
  }

  public void Update() {
  }
}
