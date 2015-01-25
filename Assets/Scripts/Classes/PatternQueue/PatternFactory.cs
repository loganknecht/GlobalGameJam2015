using UnityEngine;

// This needs to be renamed to just Factory
public static class PatternFactory {
    public static GameObject CreatePattern() {
        GameObject gameObjectToCreate = null;
        gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PatternQueueObjects/PatternQueueObject") as GameObject);
        return gameObjectToCreate;
    }
}