using UnityEngine;

// This needs to be renamed to just Factory
public static class PatternFactory {
    public static GameObject CreatePattern() {
        GameObject gameObjectToCreate = null;
        gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PatternQueueObjects/PatternQueueObject") as GameObject);
        return gameObjectToCreate;
    }

    public static GameObject CreateEmptyTile() {
        GameObject gameObjectToCreate = null;
        gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tiles/EmptyTile") as GameObject);
        return gameObjectToCreate;
    }

    public static GameObject CreateRunnerTile() {
        GameObject gameObjectToCreate = null;
        gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tiles/RunnerTile") as GameObject);
        return gameObjectToCreate;
    }

    public static GameObject CreateJumpTrigger() {
        GameObject gameObjectToCreate = null;
        gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Tiles/JumpTrigger") as GameObject);
        return gameObjectToCreate;
    }
}