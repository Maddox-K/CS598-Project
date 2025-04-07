using UnityEngine;

[System.Serializable]
public class QuestReward
{
    public int coins;
    public string[] items;
    public bool clearsObstacle;

    public QuestReward(int coins, string[] items, bool clears)
    {
        this.coins = coins;
        this.items = items;
        clearsObstacle = clears;
    }
}
