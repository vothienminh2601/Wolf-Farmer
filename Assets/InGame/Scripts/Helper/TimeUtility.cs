using System;
using System.Diagnostics;
using UnityEngine;

public static class TimeUtility
{
    private const string DATE_KEY = "last_login_date";
    
    public static long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
    public static float GetTimePassedSince(long lastSave)
    {
        if (lastSave <= 0) return 0f;
        long now = GetCurrentTimestamp();
        long diff = now - lastSave;
        return Mathf.Max(0, diff);
    }
        
    public static string FormatTime(int totalSeconds)
    {
        if (totalSeconds < 0) totalSeconds = 0;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string minStr = minutes.ToString("00");
        string secStr = seconds.ToString("00");

        if (minutes > 0)
            return $"{minStr}:{secStr} ";
        else
            return $"00:{secStr}";
    }

    /// <summary>
    /// Trả về chuỗi định dạng HH:MM:SS từ số giây.
    /// </summary>
    public static string FormatHHMMSS(int totalSeconds)
    {
        if (totalSeconds < 0) totalSeconds = 0;

        TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }

    /// <summary>
    /// Trả về chuỗi "X giờ Y phút" nếu lớn hơn 1 giờ.
    /// </summary>
    public static string FormatHoursMinutes(int totalSeconds)
    {
        if (totalSeconds < 0) totalSeconds = 0;

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;

        if (hours > 0)
            return $"{hours} giờ {minutes} phút";
        else
            return $"{minutes} phút";
    }

    // public static long GetTimePassed()
    // {
    //     long lastSavedTime = UserData.Instance.GetParam<long>("lastSavedTime");
    //     if (lastSavedTime == 0) return 0;
    //     long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    //     long timePassed = currentTime - lastSavedTime;
    //     return timePassed;
    // }

    // public static bool IsNewDay()
    // {
    //     string today = DateTime.Now.ToString("yyyyMMdd");
    //     string last = UserData.Instance.GetParam<string>(DATE_KEY);

    //     bool isNewDay = today != last;
    //     if (isNewDay)
    //         UserData.Instance.SetParam(DATE_KEY, today);

    //     return isNewDay;
    // }
    

    // public static void SetToday()
    // {
    //     string today = DateTime.Now.ToString("yyyyMMdd");
    //     string lastLoginStr = UserData.Instance.GetParam<string>(DATE_KEY);
    //     DateTime lastLogin = string.IsNullOrEmpty(lastLoginStr) ? DateTime.MinValue : DateTime.Parse(lastLoginStr);
    //     UserData.Instance.SetParam(DATE_KEY, lastLogin);
    // }
}