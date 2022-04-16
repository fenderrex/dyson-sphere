using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// map projections that aim to be traversable these projection an approch to alot for fast compute times would use rtos dma for table lookups  
public class DysonSphere : MonoBehaviour
{
    float _x = 0;
    float _y = 1;
    public float timeR = 0;
    public float raidous = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeR = Time.time;


    }
    void FixedUpdate()
    {

        var t = Ring(.05f, 5f, 0, true, 0);//set half to true so we dont overlaping rinds in the loop
        this.raidous = 5f;
        foreach (Tuple<float, float> ladatide in t)
        {
            Ring(0.01f, ladatide.Item1, ladatide.Item2, false, timeR);


        }
    }
    Tuple<float, float>[] Ring(float res, float raidous, float h, bool half, float dir)
    {

        List<Tuple<float, float>> j = new List<Tuple<float, float>>();
        //states:\
        //0 sphere
        //1 MakeCilender modifid Sinusoidal projection 
        //2 MakeCilenderProjection 
        var yy = half ? new int[] { 1, 0, 2 } : new int[] { 1, 0, 2 };//what shapes do we want to draw with this sweep? first shape is top to bottom.
        foreach (int astate in yy)
        {

            j = new List<Tuple<float, float>>();
            bool setup = false;
            Tuple<float, float> last = new Tuple<float, float>(0, 0);
            for (float i = 0; i <= (half ? 1 : 2 + res); i += res)
            {

                _x = Mathf.Sin(i * Mathf.PI) * raidous;
                _y = Mathf.Cos(i * Mathf.PI) * raidous;
                var k = new Tuple<float, float>(_x, _y);
                j.Add(k);
                if (setup == false)
                {
                    last = k;
                    setup = true;
                }
                else
                {
                    var lon = 150;

                    if (half)
                    {
                        Color color = new Color(i, _y / raidous > 0 ? i : 0, 1 - i);
                        Debug.DrawLine(TupleToVector(last, h, half, dir, astate), TupleToVector(k, h, half, dir, astate), color);
                        last = k;
                    }
                    else
                    {
                        Color color = new Color(i, 1 - i, _y / raidous > 0 ? i : 0);
                        Debug.DrawLine(TupleToVector(last, h, half, dir, astate), TupleToVector(k, h, half, dir, astate), color);
                        last = k;
                    }
                }
            }
        }
        return j.ToArray();
    }
    double TwoPIToLongitude(float x)//(-1)-1in 0-180out
    {
        return Mathf.Sin(x) * 180;

    }
    double LongitudeToTwoPI(float x)//0=180in (-1)-1out
    {
        return Mathf.Cos((Mathf.PI / 2) - (x / 180));

    }
    Vector3 MakeCilenderProjection(Tuple<float, float> pos, float h, bool flip, float dir)
    {

        float of45 = 0.70710678118f;
        var t = pos.Item1 >= 0 ? 1 : -1;
        var r = t * Mathf.Sqrt((pos.Item1 * pos.Item1) + (pos.Item2 * pos.Item2));//find radious
        var l = new Vector3(pos.Item1 / r, pos.Item2 / r, h);//make a cilender

        if (flip)
        {
            //l = new Vector3(pos.Item1 / r, h, pos.Item2 / r);//make a cilender
            l = new Vector3((pos.Item1 / r), h, (pos.Item2 / r) + 5);//make a cilender


            var x = Mathf.Sin(-timeR - 1.5708f);
            var y = Mathf.Cos(-timeR - 1.5708f);
            l = new Vector3((y * r * pos.Item1), (pos.Item2), 15 + (((x > 0) ? 1 : -1) * r));
            //l = new Vector3(pos.Item1+Mathf.Sin(dir) , pos.Item2 + Mathf.Sin(dir), h);
        }
        else
        {
            //print(pos.Item2);
            float a = pos.Item1;
            float b = pos.Item2 % (float)Math.PI;
            var x = pos.Item1;
            var y = pos.Item2;
            float dela = 0.000001f;
            if (x > 0 && y > 0)
            {//i
                l = new Vector3(((x / r) - 1) * r * r, h, this.raidous + 5);
            }
            else if (x > 0 && y < 0)//
            {
                l = new Vector3((-y) * r, h, this.raidous + 5);
            }
            else if (x < dela && y > 0)//iiii
            {
                l = new Vector3(((x / r) - 1) * r * r, h, this.raidous + 15);
            }
            else if (x < 0 && y < 0)//
            {
                l = new Vector3((y) * r, h, this.raidous + 15);
            }
            else if (x == 0 || y == 0)//
            {
                l = new Vector3(-pos.Item2, h, raidous + 20);//so if this is fliped we know dir is set

            }


        }
        return l;
    }
    Vector3 MakeCilenderProjectionFalty(Tuple<float, float> pos, float h, bool flip, float dir)
    {

        float of45 = 0.70710678118f;
        var t = pos.Item1 >= 0 ? 1 : -1;
        var r = t * Mathf.Sqrt((pos.Item1 * pos.Item1) + (pos.Item2 * pos.Item2));//find radious
        var l = new Vector3(pos.Item1 / r, pos.Item2 / r, h);//make a cilender

        if (flip)
        {
        }
        else
        {
            //print(pos.Item2);
            float a = pos.Item1;
            float b = pos.Item2 % (float)Math.PI;
            var x = Mathf.Sin(pos.Item1);
            var y = Mathf.Cos(pos.Item2);
            float dela = 0.000001f;
            l = new Vector3(pos.Item2, h, (x <= 0) ? 3 + raidous : raidous + 1);//so if this is fliped we know dir is set

        }
        return l;
    }
    Vector3 MakeCilender(Tuple<float, float> pos, float h, bool flip, float dir)
    {




        float of45 = 0.70710678118f;

        var r = Mathf.Sqrt((pos.Item1 * pos.Item1) + (pos.Item2 * pos.Item2));//find radious
        var l = new Vector3((pos.Item1 / r) + (2 * raidous), h, pos.Item2 / r);//make a cilender

        if (!flip)
        {

            l = l;
        }
        else
        {

            var x = Mathf.Sin(timeR);
            var y = Mathf.Cos(timeR);
            l = new Vector3(y + (2 * raidous), pos.Item2, x);//so if this is fliped we know dir is set

        }
        return l;
    }
    Vector3 TupleToVector(Tuple<float, float> pos, float h, bool flip, float dir, int state)
    {
        if (state == 1)
        {
            return MakeCilender(pos, h, flip, dir);

        }
        if (state == 2)
        {

            return MakeCilenderProjection(pos, h, flip, dir);
        }

        return !flip ? new Vector3(pos.Item1, h, pos.Item2) : VerticalArch(pos, h, flip, timeR);

    }
    Vector3 VerticalArch(Tuple<float, float> pos, float h, bool flip, float dir)
    {

        float of45 = 0.70710678118f;

        var r = Mathf.Sqrt((pos.Item1 * pos.Item1) + (h * h));//find radious
        var l = new Vector3(pos.Item1 / r, h, pos.Item2 / r);//make a cilender

        if (!flip)
        {

            l = l;
        }
        else
        {

            var x = Mathf.Sin(dir);
            var y = Mathf.Cos(dir);
            l = new Vector3(y * r, pos.Item2, x * r);//so if this is fliped we know dir is set

        }
        return l;
        ///



    }

    // Update is called once per frame
    void Update()
    {
        timeR = Time.time;
    }
}
