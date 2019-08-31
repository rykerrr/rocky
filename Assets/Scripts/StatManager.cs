using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatManager
{
    public static int currentCoins;
    public static int currentWeapon = 0;

    public static int ConvertToCoins(int resourcesAmn, int wavesSurvived, bool saveAfterConversion = true)
    {
        int buffer;

        buffer = Mathf.RoundToInt((((resourcesAmn / 50)) + wavesSurvived) * WaveSpawner.coinMultiplier);
        buffer += ((GameMaster.weaponIdOnStart / 100) * buffer);

        if (saveAfterConversion)
        {
            currentCoins += buffer;
            SaveChanges();
        }


        return buffer;
    }

    public static void SaveChanges()
    {
        PlayerPrefs.SetInt("Coins", currentCoins);
        PlayerPrefs.SetInt("WeaponID", currentWeapon);
    }

    public static void GetWeapon()
    {
        currentWeapon = PlayerPrefs.GetInt("WeaponID");
    }
}
