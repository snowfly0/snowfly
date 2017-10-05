using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//根据成长值获取鱼的大小
public static class FishShortExtenstions
{
    public static float GetScale(this ActorEntity actor)
    {
        if (actor.GrowValue < 30)
        {
            return 0.7f;
        }
        else if (actor.GrowValue >= 50)
        {
            return 1.2f;
        }
        else if (actor.GrowValue >= 80)
        {
            return 1.5f;
        }
        else return 1.0f;
    }
}

