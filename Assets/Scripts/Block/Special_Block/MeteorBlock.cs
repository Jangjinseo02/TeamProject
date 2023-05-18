using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBlock : Block
{
    //운석 블록은 그룹에 혼자만 존재
    public void OnlyOne()
    {
        group.AddGroup(this);
    }
}
