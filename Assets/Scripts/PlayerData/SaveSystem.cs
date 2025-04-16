using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem {
    private const string SAVE_KEY = "PlayerData";

    public static void SaveProgress(PlayerData data) {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static PlayerData LoadProgress() {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return new PlayerData();
        string json = PlayerPrefs.GetString(SAVE_KEY);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}

[Serializable]
public class PlayerData {
    public int balance = 500;
    public int highScore = 0;
    public BlackjackProgress blackjackProgress = new BlackjackProgress();
}


[Serializable]
public class BlackjackProgress {
    public int totalGambled = 0;
    public int totalWinnings = 0;

    public void AddGamble(int amount) {
        totalGambled += amount;
    }

    public void RemoveGamble(int amount) {
        totalGambled -= amount;
    }

    public void AddWinnings(int amount) {
        totalWinnings += amount;
    }
}

