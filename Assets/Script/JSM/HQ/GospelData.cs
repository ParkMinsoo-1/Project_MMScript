using System.Collections.Generic;
[System.Serializable]
public class GospelData
{
    public int id;
    public int buildID;
    public int order;
    public int cost;
    public string description;
    public string name;
    public List<int> statIndex;
    public float effectValue;

    public GospelData(int id, int buildID, int order, int cost, string desc, string name, List<int> statIndex, float effectValue)
    {
        this.id = id;
        this.buildID = buildID;
        this.order = order;
        this.cost = cost;
        this.description = desc;
        this.name = name;
        this.statIndex = statIndex;
        this.effectValue = effectValue;
    }
}
