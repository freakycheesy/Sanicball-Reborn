using System;
using System.Collections.Generic;
using Sanicball.Data;
using UnityEngine;
using UnityEngine.UI;

public class ModsPanel : MonoBehaviour
{
    public Text modsText;
    public List<Text> listedMods;
    public Transform modsParent;
    public const string Template = "mod ModTitle():\n";
    void OnEnable()
    {
        modsText.gameObject.SetActive(false);
        Refresh();
    }

    private void Refresh()
    {
        foreach(var mod in listedMods) Destroy(mod);
        foreach (var pallet in ActiveData.CustomStagesPallets)
        {
            Text spawnedMod = Instantiate(modsText.gameObject, modsParent).GetComponent<Text>();
            string modinfo = Template;
            modinfo = modinfo.Replace("ModTitle", $"{pallet.Author}.{pallet.name}");
            modinfo += "    Level:\n";
            foreach (var level in pallet.Stages)
            {
                modinfo += $"       ({level.name})\n";
            }
            modinfo += "    Playlist:\n";
            foreach (var song in pallet.Playlist)
            {
                modinfo += $"       ({song.name})\n";
            }
            modinfo += "    Avatars:\n";
            spawnedMod.supportRichText = true;
            spawnedMod.text = modinfo;
            spawnedMod.gameObject.SetActive(true);
            listedMods.Add(spawnedMod);
        }
    }

}
