using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEntity : IEntity
{
    public Vector2 Pos { get; set; }
    public int Type { get; set; }
    public int Depth { get; set; }
}
