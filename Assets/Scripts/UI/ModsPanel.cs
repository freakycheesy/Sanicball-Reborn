using System;
using System.Collections.Generic;
using System.Linq;
using Sanicball;
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
            modinfo = CreateCategory(pallet, Category.Stages, modinfo);
            modinfo = CreateCategory(pallet, Category.Playlist, modinfo);
            modinfo = CreateCategory(pallet, Category.Avatars, modinfo);
            modinfo = CreateCategory(pallet, Category.Powerups, modinfo);
            spawnedMod.supportRichText = true;
            spawnedMod.text = modinfo;
            spawnedMod.gameObject.SetActive(true);
            listedMods.Add(spawnedMod);
        }
    }
    private enum Category : byte {
        Stages,
        Playlist,
        Avatars,
        Powerups,
    }
    private static string CreateCategory(SanicPallet pallet, Category key, string modinfo)
    {
        string category = key.ToString();
        modinfo += $"    crate {category}():\n";
        switch (key)
        {
            case Category.Stages:
                foreach (var obj in pallet.Stages)
                {
                    modinfo += $"       {obj.name};\n";
                }
                break;
            case Category.Playlist:
                foreach (var obj in pallet.Playlist)
                {
                    modinfo += $"       {obj.name};\n";
                }
                break;
            case Category.Avatars:
                foreach (var obj in pallet.Avatars)
                {
                    modinfo += $"       {obj.name};\n";
                }
                break;
            case Category.Powerups:
                foreach (var obj in pallet.Powerups)
                {
                    modinfo += $"       {obj.name};\n";
                }
                break;
        }
        

        return modinfo;
    }
}
