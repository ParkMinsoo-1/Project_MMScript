using System.Collections.Generic;
using UnityEngine;

public static class StageDataLoader
{
    public static StageData LoadByID(TextAsset csv, int stageID)
    {
        var lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var values = line.Split(',');
            if (values.Length < 23)
            {
                Debug.LogWarning($"필드 수 부족: {line}");
                continue;
            }

            if (!int.TryParse(values[0], out int parsedID)) continue;
            if (parsedID != stageID) continue;

            var data = new StageData
            {
                ID = parsedID,
                Chapter = TryParseInt(values[1]),
                BaseDistance = TryParseFloat(values[2]),
                EnemyBaseHP = TryParseInt(values[3]),
                EnemyUnit1 = TryParseInt(values[4]),
                EnemyUnit2 = TryParseInt(values[5]),
                EnemyUnit3 = TryParseInt(values[6]),
                firstRewardItemIDs = ParseIntList(values[7]),
                repeatRewardItemIDs = new List<int> { TryParseInt(values[8]) },
                firstRewardAmounts = ParseIntList(values[9]),
                repeatRewardAmounts = new List<int> { TryParseInt(values[10]) },
                StageName = values[11],
                TeaTime = TryParseFloat(values[12]),
                ResetTime = TryParseFloat(values[13]),
                EnemyHeroID = TryParseInt(values[14]),
                CastleSprite = values[15],
                ActionPoint = TryParseInt(values[16]),
                BGList = new List<string>
                {
                    values[17], values[18], values[19],
                    values[20], values[21], values[22]
                },
                Type = TryParseInt(values[23]),
                GimicID = ParseIntList(values[24]),
                RaceID = TryParseInt(values[25]),
                BGMName = int.Parse(values[26]),
            };

            return data;
        }

        Debug.LogError($"Stage ID {stageID} not found in CSV.");
        return null;
    }

    private static int TryParseInt(string s)
    {
        return int.TryParse(s, out int result) ? result : 0;
    }

    private static float TryParseFloat(string s)
    {
        return float.TryParse(s, out float result) ? result : 0f;
    }

    private static List<int> ParseIntList(string input)
    {
        List<int> result = new();
        if (string.IsNullOrWhiteSpace(input)) return result;

        var split = input.Split(';');
        foreach (var s in split)
        {
            if (int.TryParse(s, out int value))
                result.Add(value);
        }
        return result;
    }
}
