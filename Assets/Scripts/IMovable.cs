using System.Collections;
using UnityEngine;

public interface IMovable
{
    public void Move();
    public IEnumerator TurnAround(float directionY);
}
