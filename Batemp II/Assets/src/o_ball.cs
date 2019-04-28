using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o_ball : MonoBehaviour
{
    s_leveloader ll;
    public bool active = true;
    public SpriteRenderer rend;

    public Sprite[] sprites;

    public BoxCollider2D bx;
    public float speed = 60;
    public Vector2 Direction = new Vector2(0,0);
    public float angle = Mathf.PI;
    s_game gam;
    public LayerMask layer;
    public enum TEMPERATURE
    {
        HOT = 1,
        NEUTRAL = 0,
        COLD = -1
    }
    public TEMPERATURE Temperature;

    void Start()
    {
        Direction = new Vector2(1, 1);
        rend = GetComponent<SpriteRenderer>();
        ll = GameObject.Find("General").GetComponent<s_leveloader>();
        gam = GameObject.Find("General").GetComponent<s_game>();
        bx = GetComponent<BoxCollider2D>();
    }

    void Bounce(o_block bl)
    {
        Vector2 v= bl.transform.position - transform.position;
        print(v);
        float testAng = 45;
        Vector2 d = new Vector2();
        float ra = Mathf.Deg2Rad;

        //Mathf.Asin(v.normalized.x);
        float an = angle * Mathf.Deg2Rad;

        angle += 90;

        //Vector2 ve = new Vector2(Mathf.Sin(an), Mathf.Cos(an)).normalized;
        Direction = -v.normalized;
        
        for (int i = 0; i < 4; i++)
        {
            continue;
            //testAng += 90;
            if (testAng == -angle)

            print("Checking...");
            float a = testAng * ra;
            d = new Vector2(Mathf.Sin(a), Mathf.Cos(a)).normalized;
            //Collider2D c = Physics2D.Raycast(transform.position, (d * 2), bx.size.x).transform.GetComponent<Collider2D>();

            Collider2D c = Physics2D.OverlapBox((Vector2)transform.position + (d * 2), bx.size, 0, layer);
            print(c);
            if (c)
            {
                print(c.name);
                o_block offender = c.GetComponent<o_block>();
                if (offender)
                {
                    print(offender.CURRENT_TYPE);
                    if (offender.CURRENT_TYPE == o_block.BLOCK_TYTPE.NONE)
                    {
                        angle = testAng;
                        break;
                    }
                    else
                    {
                        testAng += 90;
                        continue;
                    }
                }
            }

        }
    }

    void Update()
    {
        if (active)
        {
            switch (Temperature)
            {
                case TEMPERATURE.NEUTRAL:
                    rend.sprite = sprites[0];
                    break;

                case TEMPERATURE.HOT:
                    rend.sprite = sprites[1];
                    break;

                case TEMPERATURE.COLD:
                    rend.sprite = sprites[2];
                    break;
            }

            float a = angle * Mathf.Deg2Rad;
            //Direction = new Vector2(Mathf.Sin(a), Mathf.Cos(a)).normalized;
            transform.position += (Vector3)Direction * speed * Time.deltaTime;
            //angle += 1;

            Collider2D c = Physics2D.OverlapBox(transform.position, bx.size, 0, layer);
            if (c)
            {
                o_block offender = c.GetComponent<o_block>();
                if (offender)
                {
                    switch (offender.CURRENT_TYPE)
                    {
                        case o_block.BLOCK_TYTPE.HASTEN:
                            speed += 25;
                            print("SPEED!!");
                            Rebound(offender);
                            offender.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                            break;

                        case o_block.BLOCK_TYTPE.INDESTRUCTABLE:

                            if (offender.CURRENT_TEMPERATURE == o_block.BLOCK_TEMPERATURE.NEUTRAL)
                            {
                                if (Temperature == TEMPERATURE.COLD)
                                {
                                    ll.ballTemperature--;
                                }
                                if (Temperature == TEMPERATURE.HOT)
                                {
                                    ll.ballTemperature++;
                                }
                            }
                            Temperature += (int)(TEMPERATURE)offender.CURRENT_TEMPERATURE;
                            Rebound(offender);

                            break;

                        case o_block.BLOCK_TYTPE.SLOW:

                            speed -= 25;
                            Rebound(offender);
                            offender.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                            break;

                        case o_block.BLOCK_TYTPE.BREAKABLE:

                            if (offender.CURRENT_TEMPERATURE == o_block.BLOCK_TEMPERATURE.NEUTRAL)
                            {
                                if (Temperature == TEMPERATURE.NEUTRAL)
                                {
                                    Rebound(offender);
                                    gam.blockCount++;
                                    offender.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                                }
                                else
                                {
                                    Rebound(offender);
                                }
                                //offender.CURRENT_TEMPERATURE == o_block.BLOCK_TEMPERATURE.NEUTRAL;
                            }

                            //if (offender.CURRENT_TEMPERATURE != o_block.BLOCK_TEMPERATURE.NEUTRAL)

                            if (offender.CURRENT_TEMPERATURE != o_block.BLOCK_TEMPERATURE.NEUTRAL)
                            {
                                if (offender.CURRENT_TEMPERATURE == (o_block.BLOCK_TEMPERATURE)Temperature)
                                    return;
                                else
                                    Rebound(offender);
                            }
                            TEMPERATURE oldtemp = Temperature;
                            Temperature += (int)(TEMPERATURE)offender.CURRENT_TEMPERATURE;
                            offender.CURRENT_TEMPERATURE += (int)(o_block.BLOCK_TEMPERATURE)oldtemp;

                            /*
                            if (offender.CURRENT_TEMPERATURE != o_block.BLOCK_TEMPERATURE.NEUTRAL)
                            {
                                TEMPERATURE oldtemp = Temperature;
                                Temperature = TEMPERATURE.NEUTRAL;
                                Rebound(offender);
                                offender.CURRENT_TEMPERATURE += (int)(o_block.BLOCK_TEMPERATURE)oldtemp;
                            }
                            else
                            {
                                offender.CURRENT_TEMPERATURE += (int)(o_block.BLOCK_TEMPERATURE)Temperature;
                                Rebound(offender);
                            }
                            */
                            break;

                        case o_block.BLOCK_TYTPE.CLONE:

                            Vector2 p = offender.transform.position;
                            s_leveloader.SpawnBall((int)p.x, (int)p.y);
                            print("Clone");
                            offender.CURRENT_TYPE = o_block.BLOCK_TYTPE.NONE;
                            Rebound(offender);
                            break;
                    }

                    /*
                    if (offender.CURRENT_TYPE != o_block.BLOCK_TYTPE.NONE)
                    {
                        s_soundmanager.sound.PlaySound("ball_hit");
                        Bounce(offender);
                    }
                    */

                }
            }

            if (angle > 2 * Mathf.PI * Mathf.Rad2Deg)
            {
                angle = 0;
            }
            Temperature = (TEMPERATURE)Mathf.Clamp((float)Temperature, -1, 1);
        }
        else {
            rend.sprite = null;
        }
        speed = Mathf.Clamp(speed, 20, 200);
    }

    void Rebound(o_block offender)
    {
        s_soundmanager.sound.PlaySound("ball_hit");
        Bounce(offender);
    }
}
