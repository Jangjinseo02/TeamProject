using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public void Set(GameObject target);
    public void Use(GameObject target);
}
