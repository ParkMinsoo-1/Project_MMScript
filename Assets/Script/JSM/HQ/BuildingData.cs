using System.Collections.Generic;
using Unity.VisualScripting;

public class BuildingData
{
    public int id;
    public string displayName;
    public string imageName;
    public int raceId;
    public int gold;
    public int blueprint;
    public List<int> goldList;
    public List<int> costList;
    public List<int> orderByLevel;
    public List<int> raceIDList;
    public string description;

    public BuildingData(int id, string displayName, string imageName, int raceId, int gold, int blueprint, List<int> GoldList ,List<int> CostList, List<int> orderByLevel, List<int> raceIDList, string description)
    {
        this.id = id;
        this.displayName = displayName;
        this.imageName = imageName;
        this.raceId = raceId;
        this.gold = gold;
        this.blueprint = blueprint;
        this.goldList = GoldList;
        this.costList = CostList;
        this.orderByLevel = orderByLevel;
        this.raceIDList = raceIDList;
        this.description = description;
    }
}
