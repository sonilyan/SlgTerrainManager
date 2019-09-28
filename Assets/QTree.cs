using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTree
{
	public int Length;
	public QTree[] Child;
	public Vector2 position;
	public Bounds Bounds;

	private int x1, x2;
	private int y1, y2;

	public int depth;
	public List<IEntity> es = new List<IEntity>();
	public QTree(Vector2 pos,int len,int depth)
	{
		Length = len;
		position = pos;
		Bounds = new Bounds(new Vector3(pos.x, 0, pos.y), new Vector3(len, 0, len));

		x1 = (int)pos.x - len / 2;
		x2 = (int)pos.x + len / 2;
		
		y1 = (int)pos.y - len / 2;
		y2 = (int)pos.y + len / 2;

		this.depth = depth;
		
	}

	public bool Check(int x,int y)
	{
		if (x > x1 && x <= x2 && y > y1 && y <= y2)
			return true;
		return false;
	}

	public void BuildQTree(IEntity[] es)
	{
		foreach (var e in es)
		{
			UpdateQtree(this, e);
		}
	}

	void UpdateQtree(QTree qTree, IEntity e)
	{
		if (qTree.Check((int)e.Pos.x, (int)e.Pos.y))
		{
			if (e.Depth > qTree.depth)
			{
				var childLen = qTree.Length / 2;
				var subLen = qTree.Length / 4;

				if (qTree.Child == null)
				{
					qTree.Child = new QTree[4];
					qTree.Child[0] = new QTree(qTree.position + new Vector2(subLen, subLen), childLen, qTree.depth + 1);
					qTree.Child[1] = new QTree(qTree.position + new Vector2(-subLen, -subLen), childLen, qTree.depth + 1);
					qTree.Child[2] = new QTree(qTree.position + new Vector2(subLen, -subLen), childLen, qTree.depth + 1);
					qTree.Child[3] = new QTree(qTree.position + new Vector2(-subLen, subLen), childLen, qTree.depth + 1);
				}

				foreach (var q in qTree.Child)
				{
					UpdateQtree(q, e);
				}
			}
			else if(e.Depth == qTree.depth)
			{
				qTree.es.Add(e);
			}
		}
	}

	private static int minLen = 100;
}

public interface IEntity
{
	Vector2 Pos { get; }
	int Type { get; }
	int Depth { get; }
}
