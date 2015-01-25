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
  public GameObject winCollider;
  public GameObject failCollider;

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
    MapGenerationParams args = new MapGenerationParams(16, 8, MapGeneratorEngine.generatePredefinedMetaTiles(), 42);
    GeneratedTile[][] tiles = MapGeneratorEngine.generateMap(args);

    // Debug.Log(MapGeneratorEngine.getStringRepresentation(tiles));
    // lol TODO: make it work
    TileMapManager.Instance.GenerateTileMap(tiles);
    // winCollider.transform.position = new Vector3(0, TileMapManager.Instance.tiles[0][0].transform.position.y - 1, winCollider.transform.position.z);

    SoundManager.Instance.PlayMusic(AudioType.RobotDemo);
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
