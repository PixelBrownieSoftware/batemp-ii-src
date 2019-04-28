using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct bl
{
    public bl(string name, char letter)
    {
        this.name = name;
        this.letter = letter;
    }
    public string name;
    public char letter;
}
public class s_leveloader : MonoBehaviour
{
    public int currentlevel = 0;
    enum GAMESTATE { GAME, LOOSE, WIN };
    GAMESTATE GameState = GAMESTATE.GAME;
    Vector2Int v = new Vector2Int(0,0);
    TextAsset[] mapfiles;
    bool InEditor = false;
    int lev = 0;
    List<string> keywords = new List<string>();
    public List<o_block> PrefabBlocks = new List<o_block>();
    public List<TextAsset> levels = new List<TextAsset>();
    static List<o_ball> ballsInLevel = new List<o_ball>();

    public Sprite[] bgSprites;

    public int LevelTemperature = 0;
    int lev_width = 25;
    int lev_height = 25;
    public int lev_size = 25;
    public int tilesize = 20;
    public o_block[] tiles;
    string level = "";
    int breakpt = 0;

    Vector2 batrealpos;
    public GameObject BatObj;
    public GameObject CursorObj;

    enum LOAD_SATES
    {
        DATA,
        LEVEL,
        NONE
    }
    LOAD_SATES CURRENT_LOAD_STATE = LOAD_SATES.NONE;
    o_block.BLOCK_TEMPERATURE temperature = o_block.BLOCK_TEMPERATURE.NEUTRAL;

    enum DATA_LOAD_MODE {
        LENGTH,
        TIMER,
        PROTIP
    }
    public GameObject blockPrefab;
    public GameObject ball;
    Queue<o_block> blocks = new Queue<o_block>();
    public static Queue<o_ball> balls = new Queue<o_ball>();
    string currentKeyword = "";
    public int parseIndex = 0;
    s_game gam;

    public SpriteRenderer BGREND;

    public float maxTemperature;
    
    public int ballTemperature = 0;
    Vector2Int blockGenPoint = new Vector2Int(0, 0);

    public static void SpawnBall(int x, int y)
    {
        o_ball bal = balls.Dequeue();
        bal.transform.position = new Vector2(x, y);
        bal.active = true;
        balls.Enqueue(bal);
        ballsInLevel.Add(bal);
    }
    public static void SpawnBall(Vector2 p, float angle)
    {
        o_ball bal = balls.Dequeue();
        bal.angle = angle + 90;
        bal.transform.position = p;
        bal.active = true;
        balls.Enqueue(bal);
        ballsInLevel.Add(bal);
    }

    /*
    void ParseData(char s, int x, int y, int i)
    {
        switch (CURRENT_LOAD_STATE)
        {
            case LOAD_SATES.NONE:
                switch (currentKeyword)
                {
                    case ".level":
                        print("Switched level");
                        SwitchState(LOAD_SATES.LEVEL);
                        break;

                    case ".data":
                        print("Switched data");
                        SwitchState(LOAD_SATES.DATA);
                        break;
                }

                break;

            case LOAD_SATES.DATA:

                switch (currentKeyword)
                {
                    case ".time:":
                        print("Switched time");
                        string timerStr = "";
                        for (int o = i; o < 3; i++)
                        {
                            timerStr += s;
                        }
                        timer = int.Parse(timerStr);
                        break;

                    case ".end":
                        print("Switched back");
                        SwitchState(LOAD_SATES.NONE);
                        break;
                }
                break;

            case LOAD_SATES.LEVEL:
                if (x == lev_width - 1)
                {
                    y++;
                    x = 0;
                }
                x++;
                PutBlock(s, x, y);
                if (y == lev_height - 1)
                {
                }
                switch (currentKeyword)
                {
                    case ".end":
                        print("Switched out");
                        SwitchState(LOAD_SATES.NONE);
                        break;
                }
                break;
        }
    }
    */

    void ParseData2(string str, int i)
    {
        switch (CURRENT_LOAD_STATE)
        {
            case LOAD_SATES.NONE:
                switch (str)
                {
                    case ".level":
                        print("Switched level");
                        SwitchState(LOAD_SATES.LEVEL);
                        break;

                    case "data":
                        print("Switched data");
                        SwitchState(LOAD_SATES.DATA);
                        break;
                }
                break;

            case LOAD_SATES.DATA:
                switch (str)
                {
                    case "t:":
                        string ti = "";
                        for (int o = 0; o < 3; o++)
                        {
                            i++;
                            ti += level[i];
                        }
                        gam.timer = int.Parse(ti);
                        print("Time: " + gam.timer);
                        break;
                        
                }

                break;
                /*
            case LOAD_SATES.LEVEL:
                if (x == lev_width - 1)
                {
                    y++;
                    x = 0;
                }
                x++;
                PutBlock(s, blockGenPoint, blockGenPoint);
                if (y == lev_height - 1)
                {
                }
                switch (currentKeyword)
                {
                    case ".end":
                        print("Switched out");
                        SwitchState(LOAD_SATES.NONE);
                        break;
                }
                break;
                */
        }
    }

    void SwitchState(LOAD_SATES st) {

        currentKeyword = "";
        parseIndex = 0;
        CURRENT_LOAD_STATE = st;
    }

    void Start()
    {
        gam = GetComponent<s_game>();
        blockGenPoint = new Vector2Int(0, lev_size);
        BGREND.sprite = bgSprites[0];
        tiles = new o_block[(int)Mathf.Pow(lev_size,2) + 1];
        keywords.Add(".level");
        keywords.Add("data");
        keywords.Add(".width:");
        keywords.Add(".end;");
        keywords.Add("t:");

        //string level = mapfiles[lev].text;
        //TODO: Make statemachine

        switch (s_mainmenu.difficulty)
        {
            case s_mainmenu.DIFFICULTY_MODES.EASY:
                maxTemperature = 60;
                break;

            case s_mainmenu.DIFFICULTY_MODES.NORMAL:
                maxTemperature = 40;
                break;

            case s_mainmenu.DIFFICULTY_MODES.HARD:
            case s_mainmenu.DIFFICULTY_MODES.INSANE:
                maxTemperature = 30;
                break;
        }
        
        for (int x = 0; x < lev_size + 1; x++)
        {
            for (int y = 0; y < lev_size + 1; y++)
            {
                o_block bl = Instantiate(blockPrefab, new Vector2(x * tilesize, y * tilesize), Quaternion.identity).GetComponent<o_block>();
                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                tiles[BlockCalc(x, y)] = bl;
                //blocks.Enqueue(bl);
            }
        }
        for (int i = 0; i < 20; i++)
        {
            o_ball bl = Instantiate(ball, transform.position, Quaternion.identity).GetComponent<o_ball>();
            bl.active = false;
            balls.Enqueue(bl);
        }
        currentlevel = s_mainmenu.selectedlevel;
        CreateLevel();
        /*
        level += ".level";
        level += "00000000000000000000000000";
        level += "000--------------------000";
        level += "000----B---------------000";
        level += "000-----C---B-B--------000";
        level += "000----B-B---S---------000";
        level += "000---------B-B--------000";
        level += "000-----222----H-------000";
        level += "000--------3333-B------000";
        level += "000--------------------000";
        level += "000-1111111111-4444----000";
        level += "000----------1---------000";
        level += "000------B---1---------000";
        level += "000----------1---------000";
        level += "000---------------555--000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "000--------------------000";
        level += "00000000000000000000000000";
        level += "data";
        level += "t:020";
        level += ".end";
         */
    }


    public int BlockCalc(int x, int y)
    {
        return x + (y * (lev_size - 1));
    }

    public void ClearLevel()
    {
        foreach (o_ball b in ballsInLevel) {
            b.active = false;
        }
        for (int x = 0; x < lev_size; x++)
        {
            for (int y = 0; y < lev_size; y++)
            {
                tiles[BlockCalc(x, y)].CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                tiles[BlockCalc(x, y)].CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
            }
        }
    }

    /*
foreach (string str in keywords)
{
    if (level[i] != str[parseIndex])
    {
        continue;
    }
    if (currentKeyword == str)
    {
        switch (currentKeyword)
        {
            case ".level":
                SwitchState(LOAD_SATES.LEVEL);
                break;
        }
    }
}
*/
    bool CheckString(int i)
    {
        print(currentKeyword + " At: " + parseIndex);
        foreach (string str in keywords)
        {
            if (parseIndex == str.Length)
                continue;
            if (level[i] == str[parseIndex])
            {
                breakpt = i;
                return true;
            }
        }
        return false;
    }

    public o_block GetBlock(Vector2 pos)
    {
        //print(pos);
        int x = Mathf.RoundToInt(pos.x / tilesize);
        int y = Mathf.RoundToInt(pos.y / tilesize);
        x = Mathf.Clamp(x, 0, lev_width - 1);
        y = Mathf.Clamp(y, 0, lev_height -1);
        //print(" X"+ x + " Y:" + y);
        if (tiles[BlockCalc(x, y)] == null)
            return null;
        v = new Vector2Int(x, y);

        return tiles[BlockCalc(x, y)];
    }

    public bool CheckForBall(Vector2 pos) {

        //print(pos);
        int x = Mathf.RoundToInt(pos.x / tilesize);
        int y = Mathf.RoundToInt(pos.y / tilesize);
        x = Mathf.Clamp(x, 0, lev_width - 1);
        y = Mathf.Clamp(y, 0, lev_height - 1);
        //print(" X"+ x + " Y:" + y);
        foreach (o_ball b in ballsInLevel)
        {
            if (!b.active)
                continue;
            int bx = Mathf.RoundToInt(b.transform.position.x / tilesize);
            int by = Mathf.RoundToInt(b.transform.position.y / tilesize);
            x = Mathf.Clamp(x, 0, lev_width - 1);
            y = Mathf.Clamp(y, 0, lev_height - 1);
            if (bx == x && by == y) {
                return true;
            }
        }

        v = new Vector2Int(x, y);

        return false;
    }

    void PutBlock(char s, int x, int y) {
        o_block bl = tiles[BlockCalc(x, y)];
        print("X: " + blockGenPoint.x + " Y: " + blockGenPoint.y + " Char:" + s);

        switch (s)
        {
            case '-':
                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                break;
            case '0':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.INDESTRUCTABLE;
                break;

            case '1':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.BREAKABLE;
                break;

            case '2':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.COLD;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.BREAKABLE;
                break;

            case 'B':
                SpawnBall(x * tilesize, y * tilesize);
                break;

            case '3':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.HOT;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.BREAKABLE;
                break;
            case '4':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.HOT;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.INDESTRUCTABLE;
                break;
            case '5':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.COLD;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.INDESTRUCTABLE;
                break;

            case 'H':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.HASTEN;
                break;

            case 'S':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.SLOW;
                break;

            case 'C':

                bl.CURRENT_TEMPERATURE = o_block.BLOCK_TEMPERATURE.NEUTRAL;
                bl.CURRENT_TYPE = o_block.BLOCK_TYTPE.CLONE;
                break;
        }
    }


    public bool CheckTemp()
    {
        if (Mathf.Abs(LevelTemperature) >= maxTemperature)
        {
            return true;
        }
        return false;
    }

    public void CreateLevel()
    {
        if (currentlevel >= 8)
        {
            BGREND.sprite = bgSprites[1];
        }
        if (currentlevel >= levels.Count) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ending");
        }

        ballTemperature = 0;
        currentKeyword = "";
        
        parseIndex = 0;
        if (currentlevel > levels.Count)
            currentlevel = 0;

        level = levels[currentlevel].text;
        for (int i = 0; i < level.Length; i++)
        {
            bool ch = CheckString(i);
            currentKeyword += level[i];

            switch (CURRENT_LOAD_STATE)
            {
                case LOAD_SATES.LEVEL:
                    blockGenPoint.x += 1;
                    if (blockGenPoint.x == lev_size)
                    {
                        blockGenPoint.y -= 1;
                        blockGenPoint.x = 0;
                    }
                    if (blockGenPoint.y == 0)
                    {
                        blockGenPoint = new Vector2Int(0, lev_size);
                        SwitchState(LOAD_SATES.NONE);
                    }
                    PutBlock(level[i], blockGenPoint.x, blockGenPoint.y);
                    break;

                case LOAD_SATES.DATA:


                    break;
            }
            parseIndex++;
            if (!ch)
            {
                parseIndex = 0;
                currentKeyword = "";
            }
            else
            {
                if (currentKeyword == keywords.Find(s => s == currentKeyword))
                {
                    print("Found " + currentKeyword);
                    ParseData2(currentKeyword, i);
                    parseIndex = 0;
                    currentKeyword = "";
                }
            }
        }
        SwitchState(LOAD_SATES.NONE);
        print("Done");
    }

    public bl[] CreateLevel2(string lev)
    {
        gam = GetComponent<s_game>();
        keywords.Clear();
        keywords.Add(".level");
        keywords.Add("data");
        keywords.Add(".width:");
        keywords.Add(".text:");
        keywords.Add(".end;");
        keywords.Add("t:");

        SwitchState(LOAD_SATES.NONE);
        bl[] blocks = new bl[(int)Mathf.Pow(lev_size, 2)];
        currentKeyword = "";
        int blint = 0;
        print(blocks.Length);

        blockGenPoint = new Vector2Int(0, lev_size);

        parseIndex = 0;
        if (currentlevel > levels.Count)
            currentlevel = 0;

        level = lev;
        print(level);
        for (int i = 0; i < level.Length; i++)
        {
            bool ch = CheckString(i);
            currentKeyword += level[i];

            print(level[i]);

            switch (CURRENT_LOAD_STATE)
            {
                case LOAD_SATES.LEVEL:
                    /*
                    
                    blockGenPoint.x += 1;
                    if (blockGenPoint.x == lev_size + 1)
                    {
                        blockGenPoint.y += 1;
                        blockGenPoint.x = 0;
                    }
                    if (blockGenPoint.y == lev_size)
                    {
                        blockGenPoint = new Vector2Int(0, 0);
                        SwitchState(LOAD_SATES.NONE);
                    }
                     */
                    if (i == breakpt + Mathf.Pow(lev_size,2))
                    {
                        SwitchState(LOAD_SATES.NONE);
                    }
                    string st = "";
                    
                    switch (level[i])
                    {
                        case '1':
                            st = "Block";
                            break;

                        case '2':
                            st = "Ice";
                            break;

                        case '3':
                            st = "Hot";
                            break;

                        case '4':
                            st = "Hrd ht";
                            break;

                        case '5':
                            st = "Hrd Ice";
                            break;
                        case 'B':
                            st = "Ball";
                            break;

                        case '0':
                            st = "Ht Blk";
                            break;

                        case 'H':
                            st = "Hasten";
                            break;

                        case 'S':
                            st = "Slow";
                            break;

                        case 'C':
                            st = "Clone";
                            break;
                    }
                    print(level[i]);
                    blocks[blint] = new bl(st, level[i]);
                    blint++;
                    break;

                case LOAD_SATES.DATA:


                    break;
            }
            parseIndex++;
            if (!ch)
            {
                parseIndex = 0;
                currentKeyword = "";
            }
            else
            {
                if (currentKeyword == keywords.Find(s => s == currentKeyword))
                {
                    print("Found " + currentKeyword);
                    ParseData2(currentKeyword, i);
                    parseIndex = 0;
                    currentKeyword = "";
                }
            }
        }
        SwitchState(LOAD_SATES.NONE);
        print("Done");
        return blocks;
    }
    


}
