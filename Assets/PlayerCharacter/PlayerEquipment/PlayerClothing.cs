using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClothing : MonoBehaviour
{
    private PlayerStatusManager playerStatusManager;
    private SkinnedMeshRenderer skinnedMesh;

    public SkinnedMeshRenderer helmetMesh;
    public SkinnedMeshRenderer upperBodySkinnedMesh;
    public SkinnedMeshRenderer unerBodySkinnedMesh;
    public SkinnedMeshRenderer shoesMesh;
    public SkinnedMeshRenderer glovesMesh;

    private void Awake()
    {
        playerStatusManager = GetComponent<PlayerStatusManager>();
    }


    public void EquipHelmet(HelmetData newHelmetData)
    {
        helmetMesh.sharedMesh = newHelmetData.helmetMesh;
        helmetMesh.materials = newHelmetData.helmetMaterials;
        playerStatusManager.SetUpHelmetData(newHelmetData);       
    }


    public void ChangeUppderBodyClothes(UpperBodyClothingSet newClothingSet)
    {
        upperBodySkinnedMesh.sharedMesh = newClothingSet.upperBodyMesh;
        upperBodySkinnedMesh.materials = newClothingSet.upperbodyMaterals;
        playerStatusManager.SetUpUpperClotheData(newClothingSet);        
    }

    public void EquipUnderBodyClothe(UnderClotheData newUnderBodyClotheData)
    {
        unerBodySkinnedMesh.sharedMesh = newUnderBodyClotheData.underClotheMesh;
        unerBodySkinnedMesh.materials = newUnderBodyClotheData.underClotheMaterals;
        playerStatusManager.SetUpUnderClotheData(newUnderBodyClotheData);        
    }

    public void EquipShoes(ShoesData newShoesData)
    {
        shoesMesh.sharedMesh = newShoesData.shoesMesh;
        shoesMesh.materials = newShoesData.shoesMaterials;
        playerStatusManager.SetUpShoesData(newShoesData);
    }

    public void EquipGloves(GlovesData newGlovesData)
    {
        glovesMesh.sharedMesh = newGlovesData.glovesMesh;
        glovesMesh.materials = newGlovesData.glovesMaterials;
        playerStatusManager.SetUpGlovesData(newGlovesData);
    }
}
