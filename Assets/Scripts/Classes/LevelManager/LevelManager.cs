using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

  //Probably doesn't need to be a singleton
  //BEGINNING OF SINGLETON CODE CONFIGURATION
  private static volatile LevelManager _instance;
  private static object _lock = new object();

  public GameObject levelFailedPanel;
  public GameObject levelWonPanel;

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

    // TEST CODE
<<<<<<< HEAD
    MetaTile[,] tiles = MapGeneratorEngine.generateMap(8, 4, MapGeneratorEngine.generatePredefinedMetaTiles(), 42);
    // TileMapManager.Instance.SetTileMap();
=======
    MapGenerationParams args = new MapGenerationParams(8, 4, MapGeneratorEngine.generatePredefinedMetaTiles(), 42);
    MetaTile[,] tiles = MapGeneratorEngine.generateMap(args);
>>>>>>> cc1d0599f0d6af1ac885db38bdbde9dcdd282e93
    Debug.Log(MapGeneratorEngine.getStringRepresentation(tiles));

    // lol TODO: make it work
    TileMapManager.Instance.GenerateTileMap(null);
  }

  public void Update() {
  }

  public void ChangeSceneToMainMenu() {
    Application.LoadLevel("MainMenu");
  }

  public void ChangeSceneToLevelOne() {
    Application.LoadLevel("LevelOne");
  }

  public void TriggerLevelFailedPanel() {
    levelFailedPanel.GetComponent<UIPanel>().alpha = 1;
  }

  public void TriggerLevelWonPanel() {
    levelWonPanel.GetComponent<UIPanel>().alpha = 1;
  }
}
