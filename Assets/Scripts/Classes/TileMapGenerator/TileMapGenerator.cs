using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
* GeneratedTileType enum
*/
public enum GeneratedTileType {
  NONE, PATTERN
}

/*
* GeneratedTile
*
* Represents a wold tile
*/
public class GeneratedTile {
  public GeneratedTileType type;
  public Vector2 worldCoord;
  public bool hasJumpTrigger;

  public GeneratedTile() {
    type = GeneratedTileType.NONE;
  }
  public GeneratedTile(GeneratedTileType type) {
    this.type = type;
  }

  public void setWorldCoordinate(Vector2 worldCoord) {
    this.worldCoord = worldCoord;
  }

  public Vector2 getWorldCoordinate() {
    return worldCoord;
  }

  public override string ToString() {
    switch (type) {
      case GeneratedTileType.NONE:
        return "0";
      case GeneratedTileType.PATTERN:
        if (hasJumpTrigger) {
          return "J";
        } else {
          return "P";
        }
      default:
        return "X";
    }
  }
}

/*
* MetaTilePlacementRule
*
* A rule that decides whether or not a meta tile
* can be placed next to another meta tile
*/
public class MetaTilePlacementRule {
  public bool allValid;
  public List<int> validNeighbors;

  public MetaTilePlacementRule() {
    allValid = true;
    validNeighbors = new List<int>();
  }

  public bool isValidNeighbor(int metaTileId) {
    // if (!allValid && metaTileId == 0) {
      // Debug.Log(" : " + validNeighbors.IndexOf(metaTileId));
    // }
    return allValid || validNeighbors.IndexOf(metaTileId) != -1;
  }

  public void addValidNeighbor(int metaTileId, bool allValid) {
    validNeighbors.Add(metaTileId);
    this.allValid = allValid;
  }
}

/*
* MetaTile
*/
public class MetaTile {

  private static int ID_COUNTER = 0;
  private static int GetId() {
    return ID_COUNTER++;
  }

  public readonly int id;
  public GeneratedTile[,] tiles;

  public MetaTilePlacementRule leftRule;
  public MetaTilePlacementRule topLeftRule;
  public MetaTilePlacementRule topRule;
  public MetaTilePlacementRule topRightRule;
  public MetaTilePlacementRule rightRule;
  public MetaTilePlacementRule bottomRightRule;
  public MetaTilePlacementRule bottomRule;
  public MetaTilePlacementRule bottomLeftRule;

  public MetaTile(int width, int height) {
    id = GetId();
    tiles = new GeneratedTile[width, height];

    leftRule = new MetaTilePlacementRule();
    topLeftRule = new MetaTilePlacementRule();
    topRule = new MetaTilePlacementRule();
    topRightRule = new MetaTilePlacementRule();
    rightRule = new MetaTilePlacementRule();
    bottomRightRule = new MetaTilePlacementRule();
    bottomRule = new MetaTilePlacementRule();
    bottomLeftRule = new MetaTilePlacementRule();

    for (int col = 0; col < tiles.GetLength(0); col++) {
      for (int row = 0; row < tiles.GetLength(1); row++) {
        tiles[col, row] = new GeneratedTile();
      }
    }
  }

  public MetaTile(GeneratedTile[,] tiles) {
    this.tiles = tiles;
  }

  public virtual void createTile(int column, int row, GeneratedTileType type) {
    GeneratedTile tile = new GeneratedTile(type);
    tiles[column, row] = tile;
  }

  public virtual void setTile(int column, int row, GeneratedTile tile) {
    tiles[column, row] = tile;
  }

  public virtual GeneratedTile getTile(int column, int row) {
    return tiles[column, row];
  }

  public int getColumnCount() {
    return tiles.GetLength(0);
  }

  public int getRowCount() {
    return tiles.GetLength(1);
  }

  public void AddValidNeighborLeft(int metaTileId, bool allValid){
    leftRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborLeft(int metaTileId){
    return leftRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborTopLeft(int metaTileId, bool allValid){
    topLeftRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborTopLeft(int metaTileId){
    return topLeftRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborTop(int metaTileId, bool allValid){
    topRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborTop(int metaTileId){
    return topRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborTopRight(int metaTileId, bool allValid){
    topRightRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborTopRight(int metaTileId){
    return topRightRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborRight(int metaTileId, bool allValid){
    rightRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborRight(int metaTileId){
    return rightRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborBottomRight(int metaTileId, bool allValid){
    bottomRightRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborBottomRight(int metaTileId){
    return bottomRightRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborBottom(int metaTileId, bool allValid){
    bottomRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborBottom(int metaTileId){
    return bottomRule.isValidNeighbor(metaTileId);
  }

  public void AddValidNeighborBottomLeft(int metaTileId, bool allValid){
    bottomLeftRule.addValidNeighbor(metaTileId, allValid);
  }
  public bool isValidNeighborBottomLeft(int metaTileId){
    return bottomLeftRule.isValidNeighbor(metaTileId);
  }

  public override string ToString() {
    string str = "";
    string[] lines = ToStringArray();
    foreach(string line in lines) {
      str += line + "\n";
    }
    return str;
  }

  public string[] ToStringArray() {
    string[] lines = new string[tiles.GetLength(1) + 6];
    string str = "";
    for (int col = 0; col < tiles.GetLength(0) + 2; col++) {
      str += "=";
    }
    lines[0] = str;

    str = "| MT: " + id;
    while (str.Length < tiles.GetLength(0) + 1) {
      str += " ";
    }
    str += "|";
    lines[1] = str;

    str = "| " + tiles[0, 0].getWorldCoordinate().x + " " + tiles[0,0].getWorldCoordinate().y;
    while (str.Length < tiles.GetLength(0) + 1) {
      str += " ";
    }
    str += "|";
    lines[2] = str;

    Vector2 lastTileCoord = tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1].getWorldCoordinate();
    str = lastTileCoord.x + " " + lastTileCoord.y + " |";
    while (str.Length < tiles.GetLength(0) + 1) {
      str = " " + str;
    }
    str = "|" + str;
    lines[3] = str;

    str = "";
    for (int col = 0; col < tiles.GetLength(0) + 2; col++) {
      str += "-";
    }
    lines[4] = str;
    for (int row = 0; row < tiles.GetLength(1); row++) {
      str = "|";
      for (int col = 0; col < tiles.GetLength(0); col++) {
        str += tiles[col, row];
      }
      str += "|";
      lines[row + 5] = str;
    }
    str = "";
    for (int col = 0; col < tiles.GetLength(0) + 2; col++) {
      str += "=";
    }
    lines[lines.Length - 1] = str;
    return lines;
  }
}

/*
* MapGenerationParams
*/
public class MapGenerationParams {
  public MetaTile[] availableTiles;
  public int seed;
  public int metaTilesWide;
  public int metaTilesHigh;
  public int tileWidth = 1;
  public int tileHeight = 1;
  public Vector2 worldCoordinateOffset = new Vector2(0, 0);

  public MapGenerationParams(int metaTilesWide, int metaTilesHigh, MetaTile[] availableTiles, int seed) {
    this.seed = seed;
    this.metaTilesWide = metaTilesWide;
    this.metaTilesHigh = metaTilesHigh;
    this.availableTiles = availableTiles;
  }
}

/*
* MapGeneratorEngine
*
* This static class contains a method for generaing
* a map out of meta tiles and associated helper methods.
*/
public static class MapGeneratorEngine {
  public const int META_TILE_WIDTH = 10;
  public const int META_TILE_HEIGHT = 10;

  // TODO need a better way of creating the predifined meta tiles
  public static MetaTile[] generatePredefinedMetaTiles() {
    List<MetaTile> metaTiles = new List<MetaTile>();

    int halfWidth = META_TILE_WIDTH / 2;
    int qtrWidth = halfWidth / 2;
    int threeQtrWidth = META_TILE_WIDTH - qtrWidth;

    int halfHeight = META_TILE_HEIGHT / 2;
    int qtrHeight = halfHeight / 2;
    int threeQtrHeight = META_TILE_HEIGHT - qtrHeight;

    MetaTile metaTileA = new MetaTile(META_TILE_WIDTH, META_TILE_HEIGHT);
    MetaTile metaTileB = new MetaTile(META_TILE_WIDTH, META_TILE_HEIGHT);
    MetaTile metaTileC = new MetaTile(META_TILE_WIDTH, META_TILE_HEIGHT);
    MetaTile metaTileD = new MetaTile(META_TILE_WIDTH, META_TILE_HEIGHT);

    MetaTile metaTileE = new MetaTile(META_TILE_WIDTH, META_TILE_HEIGHT);
    for (int i = 0; i < META_TILE_WIDTH; i++) {
      if (i > 1 && i < META_TILE_WIDTH - 2) {
        metaTileA.createTile(i, halfHeight, GeneratedTileType.PATTERN);
        if (i == META_TILE_WIDTH - 3) {
          metaTileA.getTile(i, halfHeight).hasJumpTrigger = true;
        }
      }

      int heightB;
      int heightC;
      if (i < halfHeight) {
        heightB = qtrHeight + 1;
        heightC = threeQtrHeight - 1;
      } else {
        heightB = threeQtrHeight - 1;
        heightC = qtrHeight + 1;
      }
      metaTileB.createTile(i, heightB, GeneratedTileType.PATTERN);
      metaTileC.createTile(i, heightC, GeneratedTileType.PATTERN);

      if (i == halfHeight - 1 || i == META_TILE_WIDTH - 1) {
        metaTileB.getTile(i, heightB).hasJumpTrigger = true;
        metaTileC.getTile(i, heightC).hasJumpTrigger = true;
      }

      if (i < qtrWidth + 1 || i > threeQtrWidth - 1) {
        metaTileD.createTile(i, halfHeight, GeneratedTileType.PATTERN);
      }
      if (i == qtrWidth || i == META_TILE_WIDTH - 1) {
        metaTileD.getTile(i, halfHeight).hasJumpTrigger = true;
      }

      if (i > 0 && i < 3) {
        metaTileE.createTile(i, 1, GeneratedTileType.PATTERN);
        if (i == 2) {
          metaTileE.getTile(i, 1).hasJumpTrigger = true;
        }
      } else if (i >= 4 && i < 6)  {
        metaTileE.createTile(i, 2, GeneratedTileType.PATTERN);
        if (i == 5) {
          metaTileE.getTile(i, 2).hasJumpTrigger = true;
        }
      } else if (i >= 7 && i < 9)  {
        metaTileE.createTile(i, 3, GeneratedTileType.PATTERN);
        if (i == 8) {
          metaTileE.getTile(i, 3).hasJumpTrigger = true;
        }
      }
    }

    // metaTileD.AddValidNeighborLeft(1, false);
    // metaTileD.AddValidNeighborLeft(2, false);
    // metaTileD.AddValidNeighborLeft(3, false);

    metaTiles.Add(metaTileA);
    metaTiles.Add(metaTileB);
    metaTiles.Add(metaTileC);
    metaTiles.Add(metaTileD);

    metaTiles.Add(metaTileE);

    return metaTiles.ToArray();
  }

  public static MetaTile[] getValidMetaTilePool(
      MetaTile[] availableTiles,
      MetaTile left,
      MetaTile topLeft,
      MetaTile top,
      MetaTile topRight,
      MetaTile right,
      MetaTile bottomRight,
      MetaTile bottom,
      MetaTile bottomLeft) {

    List<MetaTile> validTiles = new List<MetaTile>();
    foreach(MetaTile tile in availableTiles) {
      int tileId = tile.id;
      if (
          (left == null || left.isValidNeighborLeft(tileId)) ||
          (topLeft == null || topLeft.isValidNeighborTopLeft(tileId)) ||
          (top == null || top.isValidNeighborTop(tileId)) ||
          (topRight == null || topRight.isValidNeighborTopRight(tileId)) ||
          (right == null || right.isValidNeighborRight(tileId)) ||
          (bottomRight == null || bottomRight.isValidNeighborBottomRight(tileId)) ||
          (bottom == null || bottom.isValidNeighborBottom(tileId)) ||
          (bottomLeft == null || bottomLeft.isValidNeighborBottomLeft(tileId))) {

        validTiles.Add(tile);
      }
    }

    return validTiles.ToArray();
  }

  public static void setWorldCoordinates(MetaTile[,] tiles, float tileWidth, float tileHeight, Vector2 offset) {
    for (int col = 0; col < tiles.GetLength(0); col++) {
      for (int row = 0; row < tiles.GetLength(1); row++) {
        MetaTile mt = tiles[col, row];
        float xSoFar = col * mt.getColumnCount() * tileWidth;
        float ySoFar = row * mt.getRowCount() * tileHeight;
        for (int tileCol = 0; tileCol < mt.getColumnCount(); tileCol++) {
          for (int tileRow = 0; tileRow < mt.getRowCount(); tileRow++) {
            float worldX = offset.x + (tileCol * tileWidth) + xSoFar;
            float worldY = offset.y + (tileRow * tileHeight) + ySoFar;
            mt.getTile(tileCol, tileRow).setWorldCoordinate(new Vector2(worldX, worldY));
          }
        }
      }
    }
  }

  public static GeneratedTile[][] flattenMetaTiles(MetaTile[,] metaTiles) {
    int tilesWide = metaTiles.GetLength(0) * metaTiles[0,0].getColumnCount();
    int tilesHigh = metaTiles.GetLength(1) * metaTiles[0,0].getRowCount();
    int metaTilesWide = metaTiles[0,0].getColumnCount();
    int metaTilesHigh = metaTiles[0,0].getRowCount();
    GeneratedTile[][] flattened = new GeneratedTile[tilesWide][];

    for (int tCol = 0; tCol < tilesWide; tCol++) {
      flattened[tCol] = new GeneratedTile[tilesHigh];
      int mCol = (int)(tCol / metaTilesWide);
      int mtCol = tCol % metaTilesWide;
      for (int tRow = 0; tRow < tilesHigh; tRow++) {
        int mRow = (int)(tRow / metaTilesHigh);
        int mtRow = tRow % metaTilesHigh;
        flattened[tCol][tRow] = metaTiles[mCol, mRow].getTile(mtCol, mtRow);
      }
    }

    return flattened;
  }

  public static MetaTile[,] generateMetaTileMap (MapGenerationParams args) {
    int seed = args.seed;
    int metaTilesWide = args.metaTilesWide;
    int metaTilesHigh = args.metaTilesHigh;
    MetaTile[] availableTiles = args.availableTiles;

    System.Random rando = new System.Random(seed);

    MetaTile[,] tiles = new MetaTile[metaTilesWide, metaTilesHigh];

    MetaTile[] tilePool;
    for (int col = 0; col < metaTilesWide; col++) {
      for (int row = 0; row < metaTilesHigh; row++) {
        MetaTile left = null, topLeft = null, top = null;
        if (col > 0) {
          left = tiles[col - 1, row];
          if (row > 0) {
            topLeft = tiles[col - 1, row - 1];
          }
        }
        if (row > 0) {
          top = tiles[col, row - 1];
        }
        tilePool = getValidMetaTilePool(availableTiles, left, topLeft, top, null, null, null, null, null);
        // Debug.Log("- " + tilePool.Length);

        int index = rando.Next(tilePool.Length);
        tiles[col, row] = tilePool[index];
      }
    }

    setWorldCoordinates(tiles, args.tileWidth, args.tileHeight, args.worldCoordinateOffset);

    return tiles;
  }

  public static GeneratedTile[][] generateMap(MapGenerationParams args) {
    MetaTile[,] metaTiles = generateMetaTileMap(args);
    return flattenMetaTiles(metaTiles);
  }

  public static string getStringRepresentation(MetaTile[,] tiles) {
    string str = "";
    for (int row = 0; row < tiles.GetLength(1); row++) {
      string[][] mtReps = new string[tiles.GetLength(0)][];
      for (int col = 0; col < tiles.GetLength(0); col++) {
        mtReps[col] = tiles[col, row].ToStringArray();
      }
      for (int tileY = 0; tileY < mtReps[0].Length; tileY++) {
        for (int tileX = 0; tileX < mtReps.Length; tileX++) {
          str += mtReps[tileX][tileY];
        }
        str += "\n";
      }
    }
    return str;
  }

  public static string getStringRepresentation(GeneratedTile[][] tiles) {
    string str = "";
    for (int row = 0; row < tiles[0].Length; row++) {
      for (int col = 0; col < tiles.Length; col++) {
        GeneratedTile tile = tiles[col][row];
        str += tile;
      }
      str += "\n";
    }
    return str;
  }
}
