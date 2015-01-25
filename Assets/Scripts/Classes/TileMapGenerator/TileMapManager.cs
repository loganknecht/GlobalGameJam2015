using FullInspector;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMapManager : BaseBehavior {

    //Probably doesn't need to be a singleton
    //BEGINNING OF SINGLETON CODE CONFIGURATION
    private static volatile TileMapManager _instance;
    private static object _lock = new object();
    public Tile[][] tiles; 

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

    public Tile[][] GenerateTileMap(GeneratedTile[][] newTileMap) {
        GameObject tileMap = new GameObject();
        tileMap.AddComponent<TileMap>();

        Tile[][] newTiles = new Tile[newTileMap.Length][];
        for(int col = 0; col < newTileMap.Length; col++) {
            newTiles[col] = new Tile[newTileMap[0].Length];
            for (int row = 0; row < newTileMap[0].Length; row++) {
                GeneratedTile genTile = newTileMap[col][row];
                GameObject newTileGameObject = null;
                if (genTile.type == GeneratedTileType.NONE) {
                    newTileGameObject = PatternFactory.CreateEmptyTile();
                } else if (genTile.type == GeneratedTileType.PATTERN) {
                    newTileGameObject = PatternFactory.CreateRunnerTile();
                }

                newTileGameObject.transform.position = new Vector3(col, row, newTileGameObject.transform.position.z);
                newTileGameObject.transform.parent = this.gameObject.transform;
            }
        }

        return newTiles;
    }
}
