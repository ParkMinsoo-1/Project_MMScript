using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using UnityEngine;

public static class ServerTime
{
    public static async Task<DateTime> GetServerTime()
    {
        var dbtime = FirebaseDatabase.DefaultInstance.GetReference("serverTime");
        await dbtime.SetValueAsync(ServerValue.Timestamp);

        var snapshot = await dbtime.GetValueAsync();

        if (snapshot.Exists)
        {
            long time = (long)snapshot.Value;
            return DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime;
        }
        else
        {
            throw new Exception("서버 시간 불러오기 실패");
        }
    }

    public static DateTime ConvertUtcToKst(DateTime utcTime)
    {
        return utcTime.AddHours(9);
    }

    public static DateTime GetLastMidnightKst(DateTime kstTime)
    {
        return new DateTime(kstTime.Year, kstTime.Month, kstTime.Day, 0, 0, 0);
    }
}
