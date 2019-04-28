using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class level_edit : EditorWindow
{
    bl[] listofBlocks;
    bl[] blocktable;
    float timer = 0;
    string areaname = "";
    bl selected;
    const int length = 25;

    int temperature = 0;

    s_leveloader ll = null;
    s_game gam = null;

    [MenuItem("L/PUPPET")]
    static void Init()
    {
        GetWindow<level_edit>("PUPPET");

    }

    /*
    public void LoadBlockTables()
    {

        for (int x = 0; x < length + 1; x++)
        {
            for (int y = 0; y < length + 1; y++)
            {
                if (blocktable[BlockCalc(x, y)].name == "")
                {
                    blocktable[BlockCalc(x, y)] = new bl();
                }
                if (GUI.Button(new Rect(20 * x, 20 * y, 20, 20), "COL"))
                {
                    blocktable[BlockCalc(x, y)] = selected;
                }
            }
        }
    }
    */

    int BlockCalc(int x, int y)
    {
        return x + (y * length);
    }

    void TemperatureFind() {
        int te = 0;
        for (int i = 0; i < blocktable.Length; i++)
        {
            if (blocktable[i].letter == '5' ||
                blocktable[i].letter == '2')
            {
                te--;
            }
            if (blocktable[i].letter == '4' ||
                blocktable[i].letter == '3')
            {
                te++;
            }
        }
        temperature = te;
    }

    float TimeFind()
    {
        return gam.timer;
    }

    private void OnGUI()
    {
        if (ll == null) {
            ll = GameObject.Find("General").GetComponent<s_leveloader>();
            gam = GameObject.Find("General").GetComponent<s_game>();
        }
        if (listofBlocks == null)
        {
            List<bl> blocks = new List<bl>();
            blocks.Add(new bl("Block",'1'));
            blocks.Add(new bl("Ice", '2'));
            blocks.Add(new bl("Hot", '3'));
            blocks.Add(new bl("Hrd Ice", '5'));
            blocks.Add(new bl("Hrd ht", '4'));
            blocks.Add(new bl("Hrd Blk", '0'));
            blocks.Add(new bl("Ball", 'B'));
            blocks.Add(new bl("Hasten", 'H'));
            blocks.Add(new bl("Slow", 'S'));
            blocks.Add(new bl("Clone", 'C'));
            blocks.Add(new bl("Null", '-'));

            blocktable = new bl[(length ) * (length)];
            for (int i = 0; i < blocktable.Length; i++)
            {
                blocktable[i] = new bl("Null", '-');
            }
            listofBlocks = blocks.ToArray();
        }
        else
        {
            TemperatureFind();
            for (int i = 0; i < listofBlocks.Length; i++)
            {
                if (GUI.Button(new Rect((20 * (length + 1)) + 10, 20 * i, 120, 20), listofBlocks[i].name + " Char: " + listofBlocks[i].letter))
                {
                    selected = listofBlocks[i];
                }
            }
            GUI.Label(new Rect((20 * (length + 2)), 20 * listofBlocks.Length + 1, 580, 60), selected.name + " Sum temperature: " + temperature + "\n Name: " + areaname + "\n Timer: " + timer);

            int ix = (length), iy = 0;
            for (int i = 0; i < blocktable.Length; i++)
            {
                ix--;
                //length + 1
                char st = '-';
                st = blocktable[i].letter;

                if (GUI.Button(new Rect(20 * ix, 20 * iy, 20, 20), st + ""))
                {
                    blocktable[i] = selected;
                }
                if (ix == 0) {
                    iy += 1;
                    ix = (length);
                }
            }

            if (GUI.Button(new Rect((20 * (length + 1)) + 10, 20 * (listofBlocks.Length + 2), 60, 20), "Save"))
            {
                string save = EditorUtility.SaveFilePanel("Save level", "", "New Level", ".txt");
                if (save != null)
                {
                    
                    int ind = 0;
                    string level = "";
                    level += ".level";
                    for(int i = 0; i < blocktable.Length; i++)
                            level += blocktable[i].letter;
                    level += "data";

                    if (timer < 10)
                        level += "t:00" + timer;
                    else if (timer < 100)
                        level += "t:0" + timer;
                    else
                        level += "t:" + timer;

                    level += ".end";
                    File.WriteAllText(save, level);
                    Debug.Log("Saved Level!");
                    try
                    {
                    } catch
                    {
                        Debug.Log("Couldn't save level!"); 
                    }
                }
            }
            if (GUI.Button(new Rect((20 * (length + 1)) + 10, 20 * (listofBlocks.Length + 3), 60, 20), "Load"))
            {
                string load = EditorUtility.OpenFilePanel("Save level","", ".txt");
                if (load != null)
                {
                    string jso = File.ReadAllText(load);
                    areaname = load;
                    blocktable = ll.CreateLevel2(jso);
                    timer = TimeFind();
                }
            }
            if (GUI.Button(new Rect((20 * (length + 1)) + 10, 20 * (listofBlocks.Length + 4), 60, 20), "Clear"))
            {
                for (int i = 0; i < blocktable.Length; i++)
                {
                    blocktable[i] = new bl("Null", '-');
                }
            }
            timer = EditorGUI.FloatField(new Rect((20 * (length + 1)) + 10, 20 * (listofBlocks.Length + 5), 60, 20), timer);
        }
    }
}
