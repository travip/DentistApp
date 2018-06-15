using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForAck : CustomYieldInstruction
{
    public override bool keepWaiting
    {
        get
        {
            if(LpmsManager.instance.gotAck == true)
            {
                return false;
            }
            else if(LpmsManager.instance.gotNack == true)
            {
                return false;
            }
            else
                return true;
        }
    }

}
