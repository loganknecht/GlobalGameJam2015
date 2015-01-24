using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMapManager : MonoBehaviour {

  //Probably doesn't need to be a singleton
  //BEGINNING OF SINGLETON CODE CONFIGURATION
  private static volatile TileMapManager _instance;
  private static object _lock = new object();

  //Stops the lock being created ahead of time if it's not necessary
  static TileMapManager() {
  }

  public static TileMapManager Instance {
    get {
      if(_instance == null) {
        lock(_lock) {
          if (_instance == null) {
            GameObject TileMapManagerGameObject = new GameObject("TileMapManagerGameObject");
            _instance = (TileMapManagerGameObject.AddComponent<TileMapManager>()).GetComponent<TileMapManager>();
          }
        }
      }
      return _instance;
    }
  }

  private TileMapManager() {
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
