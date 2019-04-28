using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o_block : MonoBehaviour
{
    public Sprite[] block_sprites;
    BoxCollider2D bx;
    public SpriteRenderer render;
    public enum BLOCK_TEMPERATURE {
        HOT = 1,
        COLD = -1,
        NEUTRAL = 0
    }
    public BLOCK_TEMPERATURE CURRENT_TEMPERATURE { get; set; }
    public enum BLOCK_TYTPE
    {
        BREAKABLE,
        INDESTRUCTABLE,
        HASTEN,
        SLOW,
        CLONE,
        BOMB,
        NONE
    }
    public BLOCK_TYTPE CURRENT_TYPE { get; set; }

    void Start()
    {
        bx = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>(); 
    }

    void Update()
    {
        CURRENT_TEMPERATURE = (o_block.BLOCK_TEMPERATURE)Mathf.Clamp((float)CURRENT_TEMPERATURE, -1, 1);

        if (CURRENT_TYPE == BLOCK_TYTPE.NONE)
            bx.enabled = false;
        else
            bx.enabled = true;

        switch (CURRENT_TEMPERATURE) {
            case BLOCK_TEMPERATURE.NEUTRAL:

                switch (CURRENT_TYPE)
                {
                    case BLOCK_TYTPE.BREAKABLE:
                        render.sprite = block_sprites[4];
                        break;

                    case BLOCK_TYTPE.INDESTRUCTABLE:

                        render.sprite = block_sprites[5];
                        break;

                    case BLOCK_TYTPE.NONE:
                        render.sprite = null;
                        
                        break;

                    case BLOCK_TYTPE.HASTEN:

                        render.sprite = block_sprites[6];
                        break;

                    case BLOCK_TYTPE.SLOW:

                        render.sprite = block_sprites[7];
                        break;

                    case BLOCK_TYTPE.CLONE:

                        render.sprite = block_sprites[8];
                        break;
                }
                break;

            case BLOCK_TEMPERATURE.COLD:

                switch (CURRENT_TYPE)
                {
                    case BLOCK_TYTPE.BREAKABLE:

                        render.sprite = block_sprites[2];
                        break;

                    case BLOCK_TYTPE.INDESTRUCTABLE:

                        render.sprite = block_sprites[3];
                        break;
                        
                }
                break;

            case BLOCK_TEMPERATURE.HOT:

                switch (CURRENT_TYPE)
                {
                    case BLOCK_TYTPE.BREAKABLE:

                        render.sprite = block_sprites[0];
                        break;

                    case BLOCK_TYTPE.INDESTRUCTABLE:

                        render.sprite = block_sprites[1];
                        break;

                }
                break;

        }
    }
}
