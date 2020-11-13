using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void onHit(int damage);
    void checkAlive();
}
