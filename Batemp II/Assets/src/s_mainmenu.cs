using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_mainmenu : MonoBehaviour
{
    public enum DIFFICULTY_MODES
    {
        EASY,
        NORMAL,
        HARD,
        INSANE
    }
    public static DIFFICULTY_MODES difficulty;
    public static int selectedlevel;
    public List<TextAsset> levels = new List<TextAsset>();
    public GUIStyle gstyle;

    public bool debugmode = false;

    enum MENUMODE
    {
        START,
        LEVEL_SELECT,
        INSTRUCTIONS,
        OPTIONS
    }
    MENUMODE menu;

    bool DrawText(Rect rec, string n)
    {
        if (GUI.Button(rec, n, gstyle))
            return true;
        return false;
    }

    private void OnGUI()
    {
        switch (menu)
        {
            case MENUMODE.START:
                if(DrawText(new Rect(0, 20 * 1, 90, 20), "Play"))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                }
                if (DrawText(new Rect(0, 20 * 3, 90, 20), "Level select"))
                {
                    menu = MENUMODE.LEVEL_SELECT;
                }
                if (DrawText(new Rect(0, 20 * 5, 90, 20), "Instructions"))
                {
                    menu = MENUMODE.INSTRUCTIONS;
                }
                /*
                if (debugmode)
                {
                    if (DrawText(new Rect(0, 20 * 7, 90, 20), "DEBUG MODE: ON"))
                    {
                        debugmode = false;
                    }
                }
                else
                {
                    if (DrawText(new Rect(0, 20 * 7, 90, 20), "DEBUG MODE: OFF"))
                    {
                        debugmode = true;
                    }
                }
                */
                if (DrawText(new Rect(0, 20 * 9, 90, 20), "Options"))
                {
                    menu = MENUMODE.OPTIONS;
                }
                /*
                if (DrawText(new Rect(0, 20 * 11, 90, 20), "Reset progress"))
                {
                    PlayerPrefs.SetInt("CurrentLevel", 0);
                }
                */
                break;

            case MENUMODE.LEVEL_SELECT:
                int i;
                if (difficulty == DIFFICULTY_MODES.INSANE)
                {
                    i = 2;
                    GUI.Label(new Rect(20, 0, 540, 540),
                        "Unfortunately, you can't select levels in insane mode." + "\n" +
                        "You have to play everything in one go I'm afraid..." + "\n" );
                }
                else
                {
                    for (i = 0; i < levels.Count; i++)
                    {
                        if (PlayerPrefs.GetInt("CurrentLevel") < i && !debugmode)
                        {
                            DrawText(new Rect(0, 20 * i, 120, 20), "Level: " + (i+1) + " (LOCKED)");
                        }
                        else
                        {
                            if (DrawText(new Rect(0, 20 * i, 80, 20), "Level: " + (i + 1)))
                            {
                                selectedlevel = i;
                                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                            }
                        }
                    }
                }
                if (DrawText(new Rect(0, 20 * i+1, 90, 20), "Back"))
                {
                    menu = MENUMODE.START;
                }
                break;

            case MENUMODE.INSTRUCTIONS:
                GUI.Label(new Rect(20,0,540,540), 
                    "Right click to remove blocks" + "\n"+
                    "Left click to place blocks" + "\n" +
                    "You need to mainatan the temperature around 15 to -15 degrees when the timer runs out" + "\n" +
                    "If a heated or frozen ball touches a wall, it will change the temperature depending on its state." + "\n"+
                    "A heated or frozen ball passes through a heated or frozen block respectively." + "\n");
                if (DrawText(new Rect(0, 20 * 8, 90, 20), "Back"))
                {
                    menu = MENUMODE.START;
                }
                break;

            case MENUMODE.OPTIONS:

                switch (difficulty)
                {
                    case DIFFICULTY_MODES.EASY:

                        if (DrawText(new Rect(0, 20 * 2, 210, 20), "Easy - Can withstand up to 60 degrees."))
                        {
                            difficulty++;
                        }
                        break;
                    case DIFFICULTY_MODES.NORMAL:

                        if (DrawText(new Rect(0, 20 * 2, 210, 20), "Normal - Can withstand up to 40 degrees."))
                        {
                            difficulty++;
                        }
                        break;

                    case DIFFICULTY_MODES.HARD:

                        if (DrawText(new Rect(0, 20 * 2, 210, 20), "Hard - Can withstand up to 30 degrees."))
                        {
                            difficulty++;
                        }
                        break;
                    case DIFFICULTY_MODES.INSANE:

                        if (DrawText(new Rect(0, 20 * 2, 210, 20), "Insane - Same as hard but you start over again when you loose."))
                        {
                            difficulty = 0;
                        }
                        break;
                }
                if (DrawText(new Rect(0, 20 * 4, 90, 20), "Back"))
                {
                    menu = MENUMODE.START;
                }
                break;
        }
    }
}
