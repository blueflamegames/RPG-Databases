using Godot;
using System;

[Tool]
public class Weapon : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    string iconPath = "";
    int weaponSelected = 0;
    int statEdit = -1;
    // Called when the node enters the scene tree for the first time.
    public void Start()
    {
        Godot.Collections.Dictionary jsonDictionary = this.GetParent().GetParent().Call("ReadData", "Weapon") as Godot.Collections.Dictionary;
        
        for (int i = 0; i < jsonDictionary.Count; i++)
        {
            Godot.Collections.Dictionary weaponData = jsonDictionary["weapon" + i] as Godot.Collections.Dictionary;
            if (i > GetNode<OptionButton>("WeaponButton").GetItemCount() - 1)
            {
                GetNode<OptionButton>("WeaponButton").AddItem(weaponData["name"] as string);
            }
            else
            {
                GetNode<OptionButton>("WeaponButton").SetItemText(i, weaponData["name"] as string);
            }
        }

        jsonDictionary = this.GetParent().GetParent().Call("ReadData", "System") as Godot.Collections.Dictionary;
        
        Godot.Collections.Dictionary systemData = jsonDictionary["elements"] as Godot.Collections.Dictionary;
        for (int i = 0; i < systemData.Count; i++)
        {
            if (i > GetNode<OptionButton>("ElementLabel/ElementButton").GetItemCount() - 1)
            {
                GetNode<OptionButton>("ElementLabel/ElementButton").AddItem(systemData[i.ToString()] as string);
            }
            else
            {
                GetNode<OptionButton>("ElementLabel/ElementButton").SetItemText(i, systemData[i.ToString()] as string);
            }
        }

        systemData = jsonDictionary["weapons"] as Godot.Collections.Dictionary;
        for (int i = 0; i < systemData.Count; i++)
        {
            if (i > GetNode<OptionButton>("WTypeLabel/WTypeButton").GetItemCount() - 1)
            {
                GetNode<OptionButton>("WTypeLabel/WTypeButton").AddItem(systemData[i.ToString()] as string);
            }
            else
            {
                GetNode<OptionButton>("WTypeLabel/WTypeButton").SetItemText(i, systemData[i.ToString()] as string);
            }
        }

        systemData = jsonDictionary["slots"] as Godot.Collections.Dictionary;
        int final_id = 0;
        foreach (String str in systemData.Keys)
        {
            if (str[0] == 'w')
            {
                int id = Convert.ToInt32(str.Remove(0, 1)) - final_id;
                if (id > GetNode<OptionButton>("SlotLabel/SlotButton").GetItemCount() - 1)
                {
                    GetNode<OptionButton>("SlotLabel/SlotButton").AddItem(systemData[str] as string);
                }
                else
                {
                    GetNode<OptionButton>("SlotLabel/SlotButton").SetItemText(id, systemData[str] as string);
                }
            }
            else
            {
                final_id += 1;
            }
        }
        RefreshData(0);
    }

    private void RefreshData(int id)
    {
        Godot.Collections.Dictionary jsonDictionary = this.GetParent().GetParent().Call("ReadData", "Weapon") as Godot.Collections.Dictionary;
        Godot.Collections.Dictionary weaponData = jsonDictionary["weapon" + id] as Godot.Collections.Dictionary;

        jsonDictionary = this.GetParent().GetParent().Call("ReadData", "System") as Godot.Collections.Dictionary;
        Godot.Collections.Dictionary systemData = jsonDictionary["stats"] as Godot.Collections.Dictionary;

        GetNode<LineEdit>("NameLabel/NameText").Text = weaponData["name"] as string;
        string icon = weaponData["icon"] as string;
        if (icon != "")
        {
            GetNode<Sprite>("IconLabel/IconSprite").Texture = GD.Load(weaponData["icon"] as string) as Godot.Texture;
        }
        GetNode<TextEdit>("DescLabel/DescText").Text = weaponData["description"] as string;
        GetNode<OptionButton>("WTypeLabel/WTypeButton").Selected = Convert.ToInt32(weaponData["weapon_type"]);
        GetNode<OptionButton>("SlotLabel/SlotButton").Selected = Convert.ToInt32(weaponData["slot_type"]);
        GetNode<SpinBox>("PriceLabel/PriceSpin").Value = Convert.ToInt32(weaponData["price"]);
        GetNode<OptionButton>("ElementLabel/ElementButton").Selected = Convert.ToInt32(weaponData["element"]);

        GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatNameCont/StatNameList").Clear();
        GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatValueCont/StatValueList").Clear();
        for (int i = 0; i < systemData.Count; i++)
        {
            string statName = systemData[i.ToString()] as string;
            Godot.Collections.Dictionary weaponStatFormula = weaponData["stat_list"] as Godot.Collections.Dictionary;
            string statFormula = "";
            if (weaponStatFormula.Contains(statName))
            {
                statFormula = weaponStatFormula[statName as string] as string;
            }
            else
            {
                statFormula = "0";
            }
            GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatNameCont/StatNameList").AddItem(statName);
            GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatValueCont/StatValueList").AddItem(statFormula);
        }

    }

    private void _on_AddWeaponButton_pressed()
    {
        GetNode<OptionButton>("WeaponButton").AddItem("NewWeapon");
        int id = GetNode<OptionButton>("WeaponButton").GetItemCount() - 1;
        Godot.Collections.Dictionary jsonDictionary = this.GetParent().GetParent().Call("ReadData", "Weapon") as Godot.Collections.Dictionary;
        Godot.Collections.Dictionary weaponData = new Godot.Collections.Dictionary();
        Godot.Collections.Dictionary weaponStats = new Godot.Collections.Dictionary();
        weaponData.Add("name", "NewWeapon");
        weaponData.Add("icon", "");
        weaponData.Add("description", "New created weapon");
        weaponData.Add("weapon_type", 0);
        weaponData.Add("slot_type", 0);
        weaponData.Add("price", 50);
        weaponData.Add("element", 0);
        weaponStats.Add("hp", "0");
        weaponStats.Add("mp", "0");
        weaponStats.Add("atk", "10");
        weaponStats.Add("def", "2");
        weaponStats.Add("int", "2");
        weaponStats.Add("res", "1");
        weaponStats.Add("spd", "0");
        weaponStats.Add("luk", "0");
        weaponData.Add("stat_list", weaponStats);
        jsonDictionary.Add("weapon" + id, weaponData);
        this.GetParent().GetParent().Call("StoreData", "Weapon", jsonDictionary);
    }

    private void _on_WeaponSaveButton_pressed()
    {
        SaveWeaponData();
        RefreshData(weaponSelected);
    }

    private void _on_Search_pressed()
    {
        GetNode<FileDialog>("IconLabel/IconSearch").PopupCentered();
    }

    private void _on_IconSearch_file_selected(string path)
    {
        iconPath = path;
        GetNode<Sprite>("IconLabel/IconSprite").Texture = GD.Load(path) as Godot.Texture;
    }

    private void _on_WeaponButton_item_selected(int id)
    {
        weaponSelected = id;
        RefreshData(id);
    }

    private void SaveWeaponData()
    {
        Godot.Collections.Dictionary jsonDictionary = this.GetParent().GetParent().Call("ReadData", "Weapon") as Godot.Collections.Dictionary;
        Godot.Collections.Dictionary weaponData = jsonDictionary["weapon" + weaponSelected] as Godot.Collections.Dictionary;
        Godot.Collections.Dictionary weaponStatFormula = weaponData["stat_list"] as Godot.Collections.Dictionary;
        weaponData["name"] = GetNode<LineEdit>("NameLabel/NameText").Text;
        GetNode<OptionButton>("WeaponButton").SetItemText(weaponSelected, GetNode<LineEdit>("NameLabel/NameText").Text);
        weaponData["icon"] = iconPath;
        weaponData["description"] = GetNode<TextEdit>("DescLabel/DescText").Text;
        weaponData["weapon_type"] = GetNode<OptionButton>("WTypeLabel/WTypeButton").Selected;
        weaponData["slot_type"] = GetNode<OptionButton>("SlotLabel/SlotButton").Selected;
        weaponData["price"] = GetNode<SpinBox>("PriceLabel/PriceSpin").Value;
        weaponData["element"] = GetNode<OptionButton>("ElementLabel/ElementButton").Selected;
        int items = GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatNameCont/StatNameList").GetItemCount();
        for (int i = 0; i < items; i++)
        {
            string stat = GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatNameCont/StatNameList").GetItemText(i);
            string formula = GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatValueCont/StatValueList").GetItemText(i);
            weaponStatFormula[stat] = formula;
        }
        this.GetParent().GetParent().Call("StoreData", "Weapon", jsonDictionary);
    }

    private void _on_StatValueList_item_activated(int index)
    {
        string stat_name = GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatNameCont/StatNameList").GetItemText(index);
        string stat_formula = GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatValueCont/StatValueList").GetItemText(index);
        GetNode<Label>("StatEditor/StatLabel").Text = stat_name;
        GetNode<LineEdit>("StatEditor/StatEdit").Text = stat_formula;
        statEdit = index;
        GetNode<WindowDialog>("StatEditor").Show();
    }

    private void _on_OkButton_pressed()
    {
        string stat_formula = GetNode<LineEdit>("StatEditor/StatEdit").Text;
        GetNode<ItemList>("StatsLabel/StatsContainer/DataContainer/StatValueCont/StatValueList").SetItemText(statEdit, stat_formula);
        SaveWeaponData();
        statEdit = -1;
        GetNode<WindowDialog>("StatEditor").Hide();
    }

    private void _on_CancelButton_pressed()
    {
        statEdit = -1;
        GetNode<WindowDialog>("StatEditor").Hide();
    }
    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
