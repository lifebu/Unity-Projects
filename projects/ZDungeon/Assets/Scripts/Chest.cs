using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour 
{
    public enum Loot
    {
        SmallKey,
        BossKey,
        Rupee
    }
    public Loot loot;
    public int amount = int.MaxValue;

    public void playerInteracted(Player player)
	{
        if (amount == int.MaxValue && loot == Loot.SmallKey)
        {
            SoundManager.instance.Play("Chest",SoundManager.SoundType.SFX, false);
            SoundManager.instance.Play("Get_Key",SoundManager.SoundType.SFX, false);
            // TODO: Need some Graphic for getting a Key
            transform.gameObject.SetActive(false);
        }
        else if ((amount == 1 || amount == 5) && loot == Loot.Rupee)
        {
            SoundManager.instance.Play("Chest",SoundManager.SoundType.SFX, false);
            SoundManager.instance.Play("Rupee1",SoundManager.SoundType.SFX, false);
            player.addRupee(amount);
            // TODO: Need some Graphic for getting a Key
            transform.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Chest.cs: player Interacted but the Chest was not setup correctly!");
            Debug.LogError("Loot: " + loot.ToString() + "Amount: " + amount);
        }

	}
}
