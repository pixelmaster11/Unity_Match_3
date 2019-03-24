using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Manager : MonoBehaviour
{

        //[SerializeField]
        //protected State ownerState;

        public abstract void ManagedUpdate();
        //public abstract void ForceReset();

        //[SerializeField]
        //protected List<ManagedObjects> managedObjects = new List<ManagedObjects>();
  


     
}


