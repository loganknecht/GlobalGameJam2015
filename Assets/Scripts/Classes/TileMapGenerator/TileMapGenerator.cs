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

  public GeneratedTile() {
    type = GeneratedTileType.NONE;
  }
  public GeneratedTile(GeneratedTileType type) {
    this.type = type;
  }

  public override string ToString() {
    switch (type) {
      case GeneratedTileType.NONE:
        return "0";
      case GeneratedTileType.PATTERN:
        return "P";
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

  public virtual void setTile(int row, int column, GeneratedTile tile) {
    tiles[column, row] = tile;
  }

  public virtual GeneratedTile getTile(int row, int column) {
    return tiles[column, row];
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
    string[] lines = new string[tiles.GetLength(1) + 4];
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
    str = "";
    for (int col = 0; col < tiles.GetLength(0) + 2; col++) {
      str += "-";
    }
    lines[2] = str;
    for (int row = 0; row < tiles.GetLength(1); row++) {
      str = "|";
      for (int col = 0; col < tiles.GetLength(0); col++) {
        str += tiles[col, row];
      }
      str += "|";
      lines[row + 3] = str;
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
    for (int i = 0; i < META_TILE_WIDTH; i++) {
      metaTileA.createTile(i, halfHeight, GeneratedTileType.PATTERN);

      int heightB;
      int heightC;
      if (i < halfHeight) {
        heightB = qtrHeight;
        heightC = threeQtrHeight;
      } else {
        heightB = threeQtrHeight;
        heightC = qtrHeight;
      }
      metaTileB.createTile(i, heightB, GeneratedTileType.PATTERN);
      metaTileC.createTile(i, heightC, GeneratedTileType.PATTERN);

      if (i < qtrWidth + 1 || i > threeQtrWidth - 1) {
        metaTileD.createTile(i, halfHeight, GeneratedTileType.PATTERN);
      }
    }
    metaTiles.Add(metaTileA);
    metaTiles.Add(metaTileB);
    metaTiles.Add(metaTileC);
    metaTiles.Add(metaTileD);

    return metaTiles.ToArray();
  }

  // public static Dictionary<int, MetaTile> getMetaTileDictionary(MetaTile[] tiles) {
  //   Dictionary<int, MetaTile> dict = new Dictionary<int, MetaTile>();

  //   foreach (MetaTile tile in tiles) {
  //     dict.Add(tile.id, tile);
  //   }

  //   return dict;
  // }

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

  public static MetaTile[,] generateMap (int metaTilesWide, int metaTilesHigh, MetaTile[] availableTiles, int seed) {
    System.Random rando = new System.Random(seed);
    // Dictionary<int, MetaTile> dict = getMetaTileDictionary(availableTiles);

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

        int index = rando.Next(tilePool.Length);
        tiles[col, row] = tilePool[index];
      }
    }

    return tiles;
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
}
