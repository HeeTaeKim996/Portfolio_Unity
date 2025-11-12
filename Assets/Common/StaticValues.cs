using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StaticValues
{
    public static float defaultDrag = 0;
    public static float highDrag = 1;


    public static PhysicMaterial defaultMaterial = new PhysicMaterial
    {
        dynamicFriction = 0.5f,
        staticFriction = 0.5f,
        bounciness = 0, 
        frictionCombine = PhysicMaterialCombine.Average, 
        bounceCombine = PhysicMaterialCombine.Minimum      
    };

    public static PhysicMaterial onIceMaterial = new PhysicMaterial
    {
        dynamicFriction = 0.1f,
        staticFriction = 0.1f,
        bounciness = 0,
        frictionCombine = PhysicMaterialCombine.Minimum,
        bounceCombine = PhysicMaterialCombine.Minimum
    };

    public static float addForce5f = 7f;
    public static float addForceTiny = 1.5f;



    // FadoeObject

    public static float fadeObjectFadeTime = (float)1f / 0.5f;
    public static float fadeObjectFadeCheckInterval = 0.3f;
}
