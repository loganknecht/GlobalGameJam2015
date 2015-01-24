using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DuplicateHelper {
    // This duplicates a game object and increments any number that is a part of its name
    [MenuItem("Tools/Duplicate/Duplicate And Increment Name %#d")]
    public static void DuplicateAndIncrementName() {
        List<GameObject> newGameObjects = new List<GameObject>();
        foreach(GameObject gameObj in Selection.gameObjects) {
            GameObject newGameObject = DuplicateAndIncrementGameObject(gameObj, GetIncrementedString(gameObj.name));
            newGameObjects.Add(newGameObject);
        }
        Selection.objects = newGameObjects.ToArray();
    }

    [MenuItem("Tools/Duplicate/Duplicate And Increment Name And Move Right %#RIGHT")]
    public static void DuplicateAndIncrementNameAndMoveRight() {
        List<GameObject> newGameObjects = new List<GameObject>();
        foreach(GameObject gameObj in Selection.gameObjects) {
            GameObject newGameObject = DuplicateAndIncrementGameObject(gameObj, GetIncrementedString(gameObj.name));
            newGameObject.transform.position = new Vector3(newGameObject.transform.position.x + 1,
                                                           newGameObject.transform.position.y,
                                                           newGameObject.transform.position.z);
            newGameObjects.Add(newGameObject);

            Tile[] tiles = newGameObject.GetComponentsInChildren<Tile>(true);
            foreach(Tile tile in tiles) {
                tile.tileX++;
            }
        }
        Selection.objects = newGameObjects.ToArray();
    }

    [MenuItem("Tools/Duplicate/Duplicate And Increment Name And Move Left %#LEFT")]
    public static void DuplicateAndIncrementNameAndMoveLeft() {
        List<GameObject> newGameObjects = new List<GameObject>();
        foreach(GameObject gameObj in Selection.gameObjects) {
            GameObject newGameObject = DuplicateAndIncrementGameObject(gameObj, GetIncrementedString(gameObj.name));
            newGameObject.transform.position = new Vector3(newGameObject.transform.position.x - 1,
                                                           newGameObject.transform.position.y,
                                                           newGameObject.transform.position.z);
            newGameObjects.Add(newGameObject);

            Tile[] tiles = newGameObject.GetComponentsInChildren<Tile>(true);
            foreach(Tile tile in tiles) {
                tile.tileX--;
            }
        }
        Selection.objects = newGameObjects.ToArray();
    }

    [MenuItem("Tools/Duplicate/Duplicate And Increment Name And Move Up %#UP")]
    public static void DuplicateAndIncrementNameAndMoveUp() {
        List<GameObject> newGameObjects = new List<GameObject>();
        foreach(GameObject gameObj in Selection.gameObjects) {
            GameObject newGameObject = DuplicateAndIncrementGameObject(gameObj, GetIncrementedString(gameObj.name));
            newGameObject.transform.position = new Vector3(newGameObject.transform.position.x,
                                                           newGameObject.transform.position.y + 1,
                                                           newGameObject.transform.position.z);
            newGameObjects.Add(newGameObject);

            Tile[] tiles = newGameObject.GetComponentsInChildren<Tile>(true);
            foreach(Tile tile in tiles) {
                tile.tileY++;
            }
        }
        Selection.objects = newGameObjects.ToArray();
    }

    [MenuItem("Tools/Duplicate/Duplicate And Increment Name And Move Down %#DOWN")]
    public static void DuplicateAndIncrementNameAndMoveDown() {
        List<GameObject> newGameObjects = new List<GameObject>();
        foreach(GameObject gameObj in Selection.gameObjects) {
            GameObject newGameObject = DuplicateAndIncrementGameObject(gameObj, GetIncrementedString(gameObj.name));
            newGameObject.transform.position = new Vector3(newGameObject.transform.position.x,
                                                           newGameObject.transform.position.y - 1,
                                                           newGameObject.transform.position.z);
            newGameObjects.Add(newGameObject);

            Tile[] tiles = newGameObject.GetComponentsInChildren<Tile>(true);
            foreach(Tile tile in tiles) {
                tile.tileY--;
            }
        }
        Selection.objects = newGameObjects.ToArray();
    }

    // This is a brilliant solution using lamda functions and regex, but it's not mine, it was taken from here:
    // http://stackoverflow.com/questions/15268931/increment-a-string-with-both-letters-and-numbers
    public static string GetIncrementedString(string stringToIncrement) {
        string newString = Regex.Replace(stringToIncrement, "\\d+", m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
        return newString;
    }

    private static GameObject DuplicateAndIncrementGameObject(GameObject gameObjectToDuplicate, string newGameObjectName) {
        // Object prefabRoot = PrefabUtility.GetPrefabParent(gameObjectToDuplicate);
        // GameObject newGameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
        GameObject newGameObject = (GameObject)Object.Instantiate(gameObjectToDuplicate);

        newGameObject.name = newGameObjectName;
        newGameObject.transform.parent = gameObjectToDuplicate.transform.parent;
        newGameObject.transform.position = gameObjectToDuplicate.transform.position;

        return newGameObject;
    }
}
