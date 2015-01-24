using UnityEngine;

public static class PatternFactory {
    public static GameObject CreatePattern(Pattern patternToCreate) {
        GameObject gameObjectToCreate = null;
        switch(patternToCreate) {
            case(Pattern.Block):
                gameObjectToCreate = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PatternQueueObjects/Cube") as GameObject);
            break;
            default:
                Debug.Log("YOU SHOULD NOT BE HITTING THIS LOGIC!!!!");
            break;
        }
        return gameObjectToCreate;
    }
}