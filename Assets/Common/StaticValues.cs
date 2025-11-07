using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StaticValues
{
    public static float defaultDrag = 0;
    public static float highDrag = 1; // 예시용도


    public static PhysicMaterial defaultMaterial = new PhysicMaterial
    {
        dynamicFriction = 0.5f,  // 동적마찰. 물체가 움직이는 동안의 전체 마찰력
        staticFriction = 0.5f,      // 정적 마찰. 물체가 움직이지 않는 동안의 마찰력. 예시) 빙판길에서, 플레이어가 움직이다 멈출 때, 이 값이 낮으면, 천천히 미끄러지면서 멈춤
        bounciness = 0,     // 반발력. 물체가 충돌 후 튕겨나가는 정도
        frictionCombine = PhysicMaterialCombine.Average,        // 두 물체가 충도할 때, 두 물체의 마찰 계수를 결합하는 방식. Average를 사용하면, 두 물체의 마찰계수의 평균값을 사용
        bounceCombine = PhysicMaterialCombine.Minimum       // 두 물체가 충돌할 때, 두 물체의 반발력을 결합하는 방식. Minimum을 사용하면, 두 물체 중 더 낮은 반발력을 반발력으로 사용
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
