using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_game : MonoBehaviour
{
    enum GAMESTATE { GAME, LOOSE, WIN };
    o_block.BLOCK_TEMPERATURE temperature = o_block.BLOCK_TEMPERATURE.NEUTRAL;
    GAMESTATE GameState = GAMESTATE.GAME;

    public float timer;
    public Text text;
    int LevelTemperature = 0;
    Vector2 batrealpos;
    public GameObject BatObj;
    public int blockCount = 0;
    public int blockSpace = 15;
    public GameObject CursorObj;
    s_leveloader ll;

    public GameObject tempCursor;
    public float maxheight;
    public float minheight;

    public void GetTemperature()
    {
        int tempr = 0;
        for (int i = 0; i < ll.tiles.Length; i++)
        {
            switch (ll.tiles[i].CURRENT_TEMPERATURE)
            {
                case o_block.BLOCK_TEMPERATURE.COLD:
                    tempr--;
                    break;

                case o_block.BLOCK_TEMPERATURE.HOT:
                    tempr++;
                    break;
            }
        }
        LevelTemperature = tempr + ll.ballTemperature;
    }
    private void Start()
    {
        ll = GetComponent<s_leveloader>();
    }

    bool CheckTemp()
    {
        if (Mathf.Sign(LevelTemperature) >= 25)
        {
            return true;
        }
        return false;
    }

    void Restart()
    {
        ll.ClearLevel();
        timer = 1;
        GameState = GAMESTATE.LOOSE;
    }
    private void Update()
    {
        Vector2 mouse = Camera.main.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        float div = LevelTemperature / ll.maxTemperature;
        tempCursor.transform.position = new Vector3(481, (div * (maxheight - minheight))+ 195);
        
        batrealpos = Vector2.Lerp(batrealpos, mouse, 0.1f);
        BatObj.transform.position = batrealpos + new Vector2(0, 35);

        switch (GameState)
        {
            case GAMESTATE.GAME:

                string temp = "";
                switch (temperature)
                {
                    case o_block.BLOCK_TEMPERATURE.NEUTRAL:
                        temp = "Neutral";
                        break;

                    case o_block.BLOCK_TEMPERATURE.COLD:
                        temp = "Cold";
                        break;

                    case o_block.BLOCK_TEMPERATURE.HOT:
                        temp = "Hot";
                        break;
                }

                GetTemperature();
                text.text = "Up and down arrows to change temperature, Right click to remove and Left click to place" +
                    "\n" + "Blocks: " + blockCount + "/ " + blockSpace +
                    "\n" + "Temperature: " + LevelTemperature +
                    "\n" + "Time: " + Mathf.RoundToInt(timer) +
                    "\n" + "Bat temperature: " + temp + "\n" + 
                    //Camera.main.transform.GetChild(0).GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition) + "\n" + v +
                    "\n" + "Current Level: " + (ll.currentlevel + 1);
                if (LevelTemperature >= 15)
                {
                    text.text += "\n" + "Too hot...";
                }
                else if (LevelTemperature <= -15)
                {
                    text.text += "\n" + "Too cold...";
                }


                temperature = (o_block.BLOCK_TEMPERATURE)Input.GetAxisRaw("Vertical");
                temperature = (o_block.BLOCK_TEMPERATURE)Mathf.Clamp((int)temperature, -1, 1);


                o_block bl = ll.GetBlock(mouse);

                int mx = Mathf.RoundToInt(mouse.x / ll.tilesize);
                int my = Mathf.RoundToInt(mouse.y / ll.tilesize);

                my = Mathf.Clamp(my, 0, ll.lev_size);
                mx = Mathf.Clamp(mx, 0, ll.lev_size);

                if (!ll.CheckForBall(mouse))
                {
                    if (bl != null)
                    {
                        CursorObj.transform.position = new Vector3(mx * ll.tilesize, my * ll.tilesize);
                        if (Input.GetMouseButton(0))
                        {
                            if (bl.CURRENT_TYPE == o_block.BLOCK_TYTPE.NONE)
                            {
                                if (blockCount > 0)
                                {
                                    switch (temperature)
                                    {
                                        case o_block.BLOCK_TEMPERATURE.COLD:
                                            s_soundmanager.sound.PlaySound("place_ice");
                                            break;

                                        case o_block.BLOCK_TEMPERATURE.HOT:
                                            s_soundmanager.sound.PlaySound("place_fire");
                                            break;
                                    }
                                    bl.CURRENT_TEMPERATURE = temperature;
                                    bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.BREAKABLE;
                                    blockCount--;
                                }
                            }
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            if (bl.CURRENT_TYPE == o_block.BLOCK_TYTPE.BREAKABLE)
                            {
                                if (blockCount >= blockSpace)
                                {
                                    s_soundmanager.sound.PlaySound("remove_unable");
                                }
                            }
                        }
                        if (Input.GetMouseButton(1))
                        {
                            if (bl.CURRENT_TYPE == o_block.BLOCK_TYTPE.BREAKABLE)
                            {
                                if (blockCount < blockSpace)
                                {
                                    s_soundmanager.sound.PlaySound("remove");
                                    bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                                    bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                                    blockCount++;
                                }
                            }
                        }
                    }
                }
                if (CheckTemp())
                {
                    Restart();
                }
                if (timer <= 0)
                {
                    if (Mathf.Abs(LevelTemperature) >= 15)
                    {
                        Restart();
                        text.text = "Gameover!";
                    }
                    else
                    {
                        GameState = GAMESTATE.WIN;
                    }
                    print("Done");
                }

                timer -= Time.deltaTime;
                break;

            case GAMESTATE.LOOSE:

                blockCount = 0;
                timer -= Time.deltaTime;
                text.text = "Gameover!";
                if (timer <= 0)
                {
                    if (s_mainmenu.difficulty == s_mainmenu.DIFFICULTY_MODES.INSANE)
                        UnityEngine.SceneManagement.SceneManager.LoadScene(1);

                    ll.CreateLevel();
                    GameState = GAMESTATE.GAME;
                }
                break;

            case GAMESTATE.WIN:

                blockCount = 0;
                ll.ClearLevel();
                ll.currentlevel++;
                if (PlayerPrefs.GetInt("CurrentLevel") < ll.currentlevel)
                {
                    PlayerPrefs.SetInt("CurrentLevel", ll.currentlevel);
                }
                ll.CreateLevel();
                GameState = GAMESTATE.GAME;
                break;
        }
    }

   
}
