using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Developer : Jae Young Kwon
Version : 22.06.03
*/

public class NameTagCtr : MonoBehaviour
{
    
    public static void SetUIName(GameObject _obj)
    {
        _obj.name = "@ " + _obj.name.Replace("(Clone)", "");
    }

}