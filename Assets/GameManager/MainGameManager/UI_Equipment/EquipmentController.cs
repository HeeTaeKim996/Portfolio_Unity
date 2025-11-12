using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EquipmentController : MonoBehaviour
{
    public static EquipmentController instance { get; private set; }

    public GameObject playerController;
    public GameObject forTestController;
    private PlayerHealth playerHealth;
    [HideInInspector]
    public WeaponManager weaponManager;
    [HideInInspector]
    public PlayerClothing playerClothing;
    private ItemInfoPanel itemInfoPanel;
    private StatusInfoPanel statusInfoPanel;

    private int activeTouchId = -1;


    private RectTransform rectTransform;

    public RectTransform startRect;
    public RectTransform backRect;
    public RectTransform exitRect;

    private WeaponEquipButton equipItemButton;

    public List<RectTransform> blankSpaces;


    // ITem UI, Object Prefabs
    public GameObject weaponUIPrefab;
    public GameObject weaponObjectPrefab;

    public GameObject secpondaryWeaponUIPrefab;
    public GameObject secondaryWeaponObjectPrefab;

    public GameObject upperClotheUIPrefab;
    public GameObject upperClotheObjectPrefab;

    public GameObject underClotheUIPrefab;
    public GameObject underClotheObjectPrefab;

    public GameObject helmetUIPrefab;
    public GameObject helmetObjectPrefab;

    public GameObject shoesUIPrefab;
    public GameObject shoesObjectPrefab;

    public GameObject glovesUIPrefab;
    public GameObject glovesObjectPrefab;


    // EquipmentSpaces
    public RectTransform equipWeaponSpace;
    public RectTransform equipSecondaryWeaponSpace;
    public RectTransform equipUpperClotheSpace;
    public RectTransform equipUnderClotheSpace;
    public RectTransform equipHelmetSpace;
    public RectTransform equipGlovesSpace;
    public RectTransform equipShoesSpace;

    // CurrentEquippedEquipments
    private GameObject currentEquippedWeapon = null;
    private GameObject currentEquippedSecondaryWeapon = null;
    private GameObject currentEquippedUpperClothe = null;
    private GameObject currentEquippedUnderClothe = null;
    private GameObject currentEquippedHelemet = null;
    private GameObject currentEquippedShoes = null;
    private GameObject currentequippedGloves = null;

    // Equipment_Null
    public WeaponData weaponData_Null;
    public UpperBodyClothingSet upperClothe_Null;
    public UnderClotheData underClothe_Null;
    public HelmetData helmet_Null;
    public ShoesData shoes_Null;
    public GlovesData gloves_Null;

    // currentShowedItem
    private GameObject currentShowedItem = null;
    private ItemCategory currentShowedItemCategory;

    // ForEquipmentScene
    private Camera equipmentCamera;
    public RenderTexture equipmentRenderTexture;
    public RawImage equipmentSceneImage;
    private ForEquipmentScene_PlayerRotator equipmentPlayerRoator;
    private ForEquipmentScene_PlayerEquipController equipmentPlayer;

    private WeaponData saveForEquipmentShow_WeaponData;
    private SecondaryWeaponData saveForEquipmentShow_SecondaryWeaponData;
    private UpperBodyClothingSet saveForEquipemntShow_UpperClotheData;
    private UnderClotheData saveForEquipmentShow_UnderClotheData;
    private HelmetData saveForEquipmentShow_Helmet;
    private ShoesData saveForEquipmentShow_Shoes;
    private GlovesData saveForEquipmentShow_Gloves;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerHealth = FindObjectOfType<PlayerHealth>();
        rectTransform = GetComponent<RectTransform>();
        weaponManager = FindObjectOfType<WeaponManager>();
        playerClothing = FindObjectOfType<PlayerClothing>();
        itemInfoPanel = GetComponentInChildren<ItemInfoPanel>();
        equipItemButton = GetComponentInChildren<WeaponEquipButton>();
        statusInfoPanel = GetComponentInChildren<StatusInfoPanel>();
        equipmentPlayerRoator = GetComponentInChildren<ForEquipmentScene_PlayerRotator>();

        CanvasGroup canvasGroup = backRect.GetComponent<CanvasGroup>();


        LoadEquipment();


        if(canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
    }

    private void Start()
    {
        backRect.gameObject.SetActive(false);
        itemInfoPanel.gameObject.SetActive(false);
        statusInfoPanel.gameObject.SetActive(false);
    }



    private void OnEnable()
    {
        StartCoroutine(DelayEquipItems());
    }

    private IEnumerator DelayEquipItems()
    {
        yield return null;

        // Weapon
        Transform startEquipWeapon = equipWeaponSpace.childCount > 0 ? equipWeaponSpace.GetChild(0) : null;
        WeaponItem weaponItem = startEquipWeapon?.GetComponent<WeaponItem>();
        if (weaponItem != null)
        {
            EquipWeapon( startEquipWeapon.gameObject, weaponItem.weaponData);
        }
        else
        {
            EquipWeapon(null, weaponData_Null);
        }

        // SecondaryWeapon
        Transform startEquipSecondaryWeapon = equipSecondaryWeaponSpace.childCount > 0 ? equipSecondaryWeaponSpace.GetChild(0) : null;
        SecondaryWeaponItem secondaryWeaponItem = startEquipSecondaryWeapon?.GetComponent<SecondaryWeaponItem>();
        if (secondaryWeaponItem != null)
        {
            EquipSecondaryWeapon(startEquipSecondaryWeapon.gameObject, secondaryWeaponItem.secondaryWeaponData);
        }
        else
        {
            EquipSecondaryWeapon(null, null);
        }

        // UpperClothe
        Transform startEquipUpperClothe = equipUpperClotheSpace.childCount > 0 ? equipUpperClotheSpace.GetChild(0) : null;
        UpperClotheItem upperClotheItem = startEquipUpperClothe?.GetComponent<UpperClotheItem>();
        if (upperClotheItem != null)
        {
            EquipUpperClothe(startEquipUpperClothe.gameObject, upperClotheItem.upperBodyClothingSet);
        }
        else
        {
            EquipUpperClothe(null, null);
        }

        // UnderClothe
        Transform startEquipUnderClothe = equipUnderClotheSpace.childCount > 0 ? equipUnderClotheSpace.GetChild(0) : null;
        UnderClotheItem underClotheItem = startEquipUnderClothe?.GetComponent<UnderClotheItem>();
        if (underClotheItem != null)
        {
            EquipUnderClothe(startEquipUnderClothe.gameObject, underClotheItem.underClotheData);
        }
        else
        {
            EquipUnderClothe(null, null);
        }

        // Helmet
        Transform startEquipHelmet = equipHelmetSpace.childCount > 0 ? equipHelmetSpace.GetChild(0) : null;
        HelmetItem helmetItem = startEquipHelmet?.GetComponent<HelmetItem>();
        if(helmetItem != null)
        {
            EquipHelmet(startEquipHelmet.gameObject, helmetItem.helmetData);
        }
        else
        {
            EquipHelmet(null, null);
        }

        // Shoes
        Transform startEquipShoes = equipShoesSpace.childCount > 0 ? equipShoesSpace.GetChild(0) : null;
        ShoesItem shoesItem = startEquipShoes?.GetComponent<ShoesItem>();
        if(shoesItem != null)
        {
            EquipShoes(startEquipShoes.gameObject, shoesItem.shoesData);
        }
        else
        {
            EquipShoes(null, null);
        }

        //Gloves
        Transform startEquipGloves = equipGlovesSpace.childCount > 0 ? equipGlovesSpace.GetChild(0) : null;
        GlovesItem gloveSItem = startEquipGloves?.GetComponent<GlovesItem>();
        if(gloveSItem != null)
        {
            EquipGloves(startEquipGloves.gameObject, gloveSItem.glovesData);
        }
        else
        {
            EquipGloves(null, null);
        }
    }


    private void Update()
    {

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if(touch.phase == TouchPhase.Began && activeTouchId == -1 && isTouchWithinRect(touch.position, startRect))
            {
                activeTouchId = touch.fingerId;
            }

            if(touch.phase == TouchPhase.Ended && activeTouchId == touch.fingerId && isTouchWithinRect(touch.position, startRect))
            {

                backRect.gameObject.SetActive(true);
                OpenUpEquipmentScene();


                startRect.gameObject.SetActive(false);
                playerController.SetActive(false);
                forTestController.SetActive(false);

                activeTouchId = -1;
            }


            if (touch.phase == TouchPhase.Began && activeTouchId == -1 && isTouchWithinRect(touch.position, exitRect))
            {
                activeTouchId = touch.fingerId;
            }
            if (touch.phase == TouchPhase.Ended && activeTouchId == touch.fingerId && isTouchWithinRect(touch.position, exitRect))
           {
                backRect.gameObject.SetActive(false);
                CloseEquipmentScene();

                startRect.gameObject.SetActive(true);
                playerController.SetActive(true);
                forTestController.SetActive(true);

                activeTouchId = -1;
            }
        }
    }


    
    
    // @@ PrimaryMethods

    private bool isTouchWithinRect(Vector2 touchPosition, RectTransform rectTransform)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touchPosition, null, out localPoint);

        return rectTransform.rect.Contains(localPoint);
    }

    public RectTransform FindEmptySlot()
    {
        foreach(RectTransform slot in blankSpaces)
        {
            if(slot.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }
    private void OpenUpEquipmentScene()
    {
        Debug.Log("EquipmentController : OpenUpEquipmentScence Active Check");
        SceneManager.LoadScene("EquipmentScene", LoadSceneMode.Additive);
        StartCoroutine(FindCameraInEquipmentScene());
    }

    private IEnumerator FindCameraInEquipmentScene()
    {
        while (!SceneManager.GetSceneByName("EquipmentScene").isLoaded)
        {
            yield return null;
        }

        Scene equipmentScene = SceneManager.GetSceneByName("EquipmentScene");

        foreach (GameObject rootObj in equipmentScene.GetRootGameObjects())
        {
            rootObj.transform.position += new Vector3(0, 100, 0);
        }



        foreach (GameObject rootobj in equipmentScene.GetRootGameObjects())
        {
            Camera cam = rootobj.GetComponent<Camera>();
            if (cam != null)
            {
                equipmentCamera = cam;
                break;
            }
        }

        foreach (GameObject rootobj in equipmentScene.GetRootGameObjects())
        {
            ForEquipmentScene_PlayerEquipController newEquipmentScene_Player = rootobj.GetComponent<ForEquipmentScene_PlayerEquipController>();
            if (newEquipmentScene_Player != null)
            {
                equipmentPlayerRoator.GetEquipmentPlayer(newEquipmentScene_Player);
                equipmentPlayer = newEquipmentScene_Player;

                if(saveForEquipmentShow_WeaponData != null)
                {
                    equipmentPlayer.EquipWeapon(saveForEquipmentShow_WeaponData);
                }
                if(saveForEquipmentShow_SecondaryWeaponData != null)
                {
                    equipmentPlayer.EquipSecondaryWeapon(saveForEquipmentShow_SecondaryWeaponData);
                }
                if(saveForEquipemntShow_UpperClotheData != null)
                {
                    equipmentPlayer.EquipUpperClothe(saveForEquipemntShow_UpperClotheData);
                }
                if(saveForEquipmentShow_UnderClotheData != null)
                {
                    equipmentPlayer.EquipUnderClothe(saveForEquipmentShow_UnderClotheData);
                }
                if(saveForEquipmentShow_Helmet != null)
                {
                    equipmentPlayer.EquipHelmet(saveForEquipmentShow_Helmet);
                }
                if(saveForEquipmentShow_Shoes != null)
                {
                    equipmentPlayer.EquipShoes(saveForEquipmentShow_Shoes);
                }
                if(saveForEquipmentShow_Gloves != null)
                {
                    equipmentPlayer.EquipGloves(saveForEquipmentShow_Gloves);
                }

                break;
            }
        }

        if (equipmentCamera != null)
        {
            equipmentCamera.targetTexture = equipmentRenderTexture;
            equipmentSceneImage.texture = equipmentRenderTexture;
        }

    }
    private void CloseEquipmentScene()
    {
        Scene equipmentScene = SceneManager.GetSceneByName("EquipmentScene");

        if (equipmentScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync("EquipmentScene");
            Debug.Log("EquipmentController : CloseEquipmentScene Active Check");
        }
        else
        {
        }
    }

    // @@ Item's Methods


    // EquipItem Methods
    public void EquipWeapon(GameObject weapon1, WeaponData newWeaponData1)
    {
        currentEquippedWeapon = weapon1;
        weaponManager.EquipWeapon(newWeaponData1);


        if(equipmentPlayer != null)
        {
            equipmentPlayer.EquipWeapon(newWeaponData1);
        }
        saveForEquipmentShow_WeaponData = newWeaponData1;        
    }

    public void EquipSecondaryWeapon(GameObject secondaryWeapon1, SecondaryWeaponData newSecondaryWeaponData)
    {
        currentEquippedSecondaryWeapon = secondaryWeapon1;
        weaponManager.EquipSecondaryWeapon(newSecondaryWeaponData);

        if(equipmentPlayer != null)
        {
            equipmentPlayer.EquipSecondaryWeapon(newSecondaryWeaponData);
        }
        
        saveForEquipmentShow_SecondaryWeaponData = newSecondaryWeaponData;       
    }
    public void EquipUpperClothe(GameObject upperClothe1, UpperBodyClothingSet newupperClotheData1)
    {
        currentEquippedUpperClothe = upperClothe1;
        UpperBodyClothingSet givingUpperClotheData;
        if(newupperClotheData1 != null)
        {
            givingUpperClotheData = newupperClotheData1;
        }
        else
        {
            givingUpperClotheData = upperClothe_Null;
        }


        playerClothing.ChangeUppderBodyClothes(givingUpperClotheData);

        if(equipmentPlayer != null)
        {

            equipmentPlayer.EquipUpperClothe(givingUpperClotheData);
            

        }
        saveForEquipemntShow_UpperClotheData = givingUpperClotheData;      
    }
    public void EquipUnderClothe(GameObject underClothe1, UnderClotheData newUnderClotheData)
    {
        currentEquippedUnderClothe = underClothe1;
        UnderClotheData givingUnderClotheData;
        if(newUnderClotheData != null)
        {
            givingUnderClotheData = newUnderClotheData;
        }
        else
        {
            givingUnderClotheData = underClothe_Null;
        }

        playerClothing.EquipUnderBodyClothe(givingUnderClotheData);

        if(equipmentPlayer != null)
        {
            equipmentPlayer.EquipUnderClothe(givingUnderClotheData);           
        }

        saveForEquipmentShow_UnderClotheData = givingUnderClotheData;        
    }

    public void EquipHelmet(GameObject newHelmet, HelmetData newHelmetData)
    {
        currentEquippedHelemet = newHelmet;
        HelmetData givingHelmelData;
        if (newHelmet != null)
        {
            givingHelmelData = newHelmetData;
        }
        else
        {
            givingHelmelData = helmet_Null;
        }


        playerClothing.EquipHelmet(givingHelmelData);


        if (equipmentPlayer != null)
        {
            equipmentPlayer.EquipHelmet(givingHelmelData);
        }
        saveForEquipmentShow_Helmet = givingHelmelData;
    }

    public void EquipShoes(GameObject newShoes, ShoesData newShoesData)
    {
        currentEquippedShoes = newShoes;
        ShoesData givingShoesData;
        if(newShoesData != null)
        {
            givingShoesData = newShoesData;
        }
        else
        {
            givingShoesData = shoes_Null;
        }

        playerClothing.EquipShoes(givingShoesData);

        if(equipmentPlayer != null)
        {
            equipmentPlayer.EquipShoes(givingShoesData);
        }
        saveForEquipmentShow_Shoes = givingShoesData;
    }

    public void EquipGloves(GameObject newGloves, GlovesData newGlovesData)
    {
        currentequippedGloves = newGloves;
        GlovesData givingGlovesdata;
        if(newGlovesData != null)
        {
            givingGlovesdata = newGlovesData;
        }
        else
        {
            givingGlovesdata = gloves_Null;
        }

        playerClothing.EquipGloves(givingGlovesdata);

        if(equipmentPlayer != null)
        {
            equipmentPlayer.EquipGloves(givingGlovesdata);
        }
        saveForEquipmentShow_Gloves = givingGlovesdata;
    }

    // ShowItem Methods
    public void ShowItemInfo(GameObject newGameObject, ItemCategory showedItemCategory1)
    {
        itemInfoPanel.gameObject.SetActive(true);
        equipItemButton.gameObject.SetActive(true);

        currentShowedItem = newGameObject;
        currentShowedItemCategory = showedItemCategory1;

        if (currentShowedItemCategory == ItemCategory.category_Weapon)
        {
            WeaponItem weaponItem1 = newGameObject.GetComponent<WeaponItem>();
            itemInfoPanel.SetInfoWeapon(weaponItem1.weaponData);

            if (currentShowedItem == currentEquippedWeapon)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if (currentShowedItemCategory == ItemCategory.category_SecondaryWeapon)
        {
            SecondaryWeaponItem secondaryWeaponItem1 = newGameObject.GetComponent<SecondaryWeaponItem>();
            itemInfoPanel.SetInfoSecondaryWeapon(secondaryWeaponItem1.secondaryWeaponData);

            if (currentShowedItem == currentEquippedSecondaryWeapon)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if (currentShowedItemCategory == ItemCategory.category_UpperClothe)
        {
            UpperClotheItem upperClotheItem1 = newGameObject.GetComponent<UpperClotheItem>();
            itemInfoPanel.SetInfoUpperClothe(upperClotheItem1.upperBodyClothingSet);

            if (currentShowedItem == currentEquippedUpperClothe)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if (currentShowedItemCategory == ItemCategory.category_UnderClothe)
        {
            UnderClotheItem underClotheItem1 = newGameObject.GetComponent<UnderClotheItem>();
            itemInfoPanel.SetInfoUnderClothe(underClotheItem1.underClotheData);

            if (currentShowedItem == currentEquippedUnderClothe)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if(currentShowedItemCategory == ItemCategory.category_Helmet)
        {
            HelmetItem helmetItem = newGameObject.GetComponent<HelmetItem>();
            itemInfoPanel.SetInfoHelmet(helmetItem.helmetData);

            if(currentShowedItem == currentEquippedHelemet)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if(currentShowedItemCategory == ItemCategory.category_Shoes)
        {
            ShoesItem shoesItem = newGameObject.GetComponent<ShoesItem>();
            itemInfoPanel.SetInfoSHoes(shoesItem.shoesData);

            if(currentShowedItem == currentEquippedShoes)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
        else if(currentShowedItemCategory == ItemCategory.category_Gloves)
        {
            GlovesItem gloveSItem = newGameObject.GetComponent<GlovesItem>();
            itemInfoPanel.SEtInfoGloves(gloveSItem.glovesData);

            if(currentShowedItem == currentequippedGloves)
            {
                equipItemButton.gameObject.SetActive(false);
            }
        }
    }
    public void ShutDownItemInfo()
    {
        itemInfoPanel.gameObject.SetActive(false);
    }

    public void EquipCurrentShowedItem()
    {
        GameObject equipItem;
        RectTransform equipSpace;

        if (currentShowedItemCategory == ItemCategory.category_Weapon)
        {
            equipItem = currentEquippedWeapon;
            equipSpace = equipWeaponSpace;

            WeaponItem weaponItem1 = currentShowedItem.GetComponent<WeaponItem>();
            EquipWeapon(currentShowedItem, weaponItem1.weaponData);
        }
        else if (currentShowedItemCategory == ItemCategory.category_SecondaryWeapon)
        {
            equipItem = currentEquippedSecondaryWeapon;
            equipSpace = equipSecondaryWeaponSpace;

            SecondaryWeaponItem secondaryWeaponItem1 = currentShowedItem.GetComponent<SecondaryWeaponItem>();
            EquipSecondaryWeapon(currentShowedItem, secondaryWeaponItem1.secondaryWeaponData);
        }
        else if (currentShowedItemCategory == ItemCategory.category_UpperClothe)
        {
            equipItem = currentEquippedUpperClothe;
            equipSpace = equipUpperClotheSpace;

            UpperClotheItem upperClotheItem1 = currentShowedItem.GetComponent<UpperClotheItem>();
            EquipUpperClothe(currentShowedItem, upperClotheItem1.upperBodyClothingSet);
        }
        else if (currentShowedItemCategory == ItemCategory.category_UnderClothe)
        {
            equipItem = currentEquippedUnderClothe;
            equipSpace = equipUnderClotheSpace;

            UnderClotheItem underClotheItem1 = currentShowedItem.GetComponent<UnderClotheItem>();
            EquipUnderClothe(currentShowedItem, underClotheItem1.underClotheData);
        }
        else if(currentShowedItemCategory == ItemCategory.category_Helmet)
        {
            equipItem = currentEquippedHelemet;
            equipSpace = equipHelmetSpace;

            HelmetItem helmetItem = currentShowedItem.GetComponent<HelmetItem>();
            EquipHelmet(currentShowedItem, helmetItem.helmetData);
        }
        else if(currentShowedItemCategory == ItemCategory.category_Shoes)
        {
            equipItem = currentEquippedShoes;
            equipSpace = equipShoesSpace;

            ShoesItem shoesItem = currentShowedItem.GetComponent<ShoesItem>();
            EquipShoes(currentShowedItem, shoesItem.shoesData);
        }
        else if(currentShowedItemCategory == ItemCategory.category_Gloves)
        {
            equipItem = currentequippedGloves;
            equipSpace = equipGlovesSpace;

            GlovesItem glovesItem = currentShowedItem.GetComponent<GlovesItem>();
            EquipGloves(currentShowedItem, glovesItem.glovesData);
        }
        else
        {
            equipItem = null;
            equipSpace = null;
        }

        if (equipItem != null && currentShowedItem != null)
        {
            RectTransform equipItemRect = equipItem.GetComponent<RectTransform>();
            RectTransform currentShowedRect = currentShowedItem.GetComponent<RectTransform>();
            Transform currentShowedParent = currentShowedRect.parent;


            equipItemRect.SetParent(currentShowedParent);
            equipItemRect.anchoredPosition = Vector2.zero;

            currentShowedRect.SetParent(equipSpace);
            currentShowedRect.anchoredPosition = Vector2.zero;

        }
        else if (equipItem == null && currentShowedItem != null)
        {
            RectTransform currentShowedRect = currentShowedItem.GetComponent<RectTransform>();

            currentShowedRect.SetParent(equipSpace);
            currentShowedRect.anchoredPosition = Vector2.zero;
        }

        equipItemButton.gameObject.SetActive(false);
    }


    // AddItem Method
    public void addItemToInventory(GameObject newItem)
    {
        RectTransform emptySlot = FindEmptySlot();
        if (emptySlot != null)
        {
            IItemCategory itemCategory1 = newItem.GetComponent<IItemCategory>();
            ItemCategory itemCategory2 = itemCategory1.Category;

            if (itemCategory2 == ItemCategory.category_Weapon)
            {
                WeaponItem weaponItem1 = newItem.GetComponent<WeaponItem>();

                GameObject newWeaponUI = Instantiate(weaponUIPrefab, emptySlot);
                WeaponItem weaponItem2 = newWeaponUI.GetComponent<WeaponItem>();

                if (weaponItem2 != null)
                {
                    weaponItem2.Initialize(weaponItem1.weaponData);
                }
            }

            else if (itemCategory2 == ItemCategory.category_SecondaryWeapon)
            {
                SecondaryWeaponItem secondaryWeaponItem1 = newItem.GetComponent<SecondaryWeaponItem>();

                GameObject secondaryWeaponUI = Instantiate(secpondaryWeaponUIPrefab, emptySlot);
                SecondaryWeaponItem secondaryWeaponItem2 = secondaryWeaponUI.GetComponent<SecondaryWeaponItem>();

                if (secondaryWeaponItem2 != null)
                {
                    secondaryWeaponItem2.Initialize(secondaryWeaponItem1.secondaryWeaponData);
                }
            }

            else if (itemCategory2 == ItemCategory.category_UpperClothe)
            {
                UpperClotheItem upperBodyClotheItem1 = newItem.GetComponent<UpperClotheItem>();

                GameObject newUpperClotheUI = Instantiate(upperClotheUIPrefab, emptySlot);
                UpperClotheItem upperBodyClotheItem2 = newUpperClotheUI.GetComponent<UpperClotheItem>();

                if (upperBodyClotheItem2 != null)
                {
                    upperBodyClotheItem2.Initialize(upperBodyClotheItem1.upperBodyClothingSet);
                }
            }
            else if (itemCategory2 == ItemCategory.category_UnderClothe)
            {
                UnderClotheItem underBodyClotheItem1 = newItem.GetComponent<UnderClotheItem>();

                GameObject newUnderClotheUI = Instantiate(underClotheUIPrefab, emptySlot);
                UnderClotheItem underBodyClotheItem2 = newUnderClotheUI.GetComponent<UnderClotheItem>();

                if (underBodyClotheItem2 != null)
                {
                    underBodyClotheItem2.Initialize(underBodyClotheItem1.underClotheData);
                }
            }
            else if(itemCategory2 == ItemCategory.category_Helmet)
            {
                HelmetItem helmetItem = newItem.GetComponent<HelmetItem>();

                GameObject helmetUI = Instantiate(helmetUIPrefab, emptySlot);
                HelmetItem newHelmetItem = helmetUI.GetComponent<HelmetItem>();

                if(newHelmetItem != null)
                {
                    newHelmetItem.Initialize(helmetItem.helmetData);
                }
            }
            else if(itemCategory2 == ItemCategory.category_Shoes)
            {
                ShoesItem shoesItem = newItem.GetComponent<ShoesItem>();

                GameObject shoesUI = Instantiate(shoesUIPrefab, emptySlot);
                ShoesItem newShoesITem = shoesUI.GetComponent<ShoesItem>();

                if(newShoesITem != null)
                {
                    newShoesITem.Initialize(shoesItem.shoesData);
                }
            }
            else if(itemCategory2 == ItemCategory.category_Gloves)
            {
                GlovesItem glovesItem = newItem.GetComponent<GlovesItem>();

                GameObject glovesUI = Instantiate(glovesUIPrefab, emptySlot);
                GlovesItem newGlovesItem = glovesUI.GetComponent<GlovesItem>();

                if(newGlovesItem != null)
                {
                    newGlovesItem.Initialize(glovesItem.glovesData);
                }
            }
            else
            {

            }
        }
        else
        {

        }
    }


    // DiscardItem Method
    public void DiscardCurrentShowedItem()
    {
        if(currentShowedItem != null)
        {
            Vector3 spawnPosition = playerHealth.transform.position;
            spawnPosition.y += 0.5f;

            if (currentShowedItemCategory == ItemCategory.category_Weapon)
            {
                WeaponItem currentShowedWeapon = currentShowedItem.GetComponent<WeaponItem>();
                WeaponData currentShowedWeaponData = currentShowedWeapon.weaponData;

                GameObject discardedWeapon = Instantiate(weaponObjectPrefab, spawnPosition, Quaternion.identity);

                WeaponItem discardedWeaponItem = discardedWeapon.GetComponent<WeaponItem>();

                if (discardedWeaponItem != null)
                {
                    discardedWeaponItem.Initialize(currentShowedWeaponData);
                }

                if (currentShowedItem == currentEquippedWeapon)
                {
                    EquipWeapon(null, weaponData_Null);
                }
            }

            else if (currentShowedItemCategory == ItemCategory.category_SecondaryWeapon)
            {
                SecondaryWeaponItem currentShowedSecondaryWeapon = currentShowedItem.GetComponent<SecondaryWeaponItem>();
                SecondaryWeaponData currentShowedSecondaryWeaponData = currentShowedSecondaryWeapon.secondaryWeaponData;

                GameObject discardedSecondaryWeapon = Instantiate(secondaryWeaponObjectPrefab, spawnPosition, Quaternion.identity);

                SecondaryWeaponItem discardedSecondaryWeaponItem = discardedSecondaryWeapon.GetComponent<SecondaryWeaponItem>();

                if (discardedSecondaryWeaponItem != null)
                {
                    discardedSecondaryWeaponItem.Initialize(currentShowedSecondaryWeaponData);
                }
                if (currentShowedItem == currentEquippedSecondaryWeapon)
                {
                    EquipSecondaryWeapon(null, null);
                }
            }

            else if (currentShowedItemCategory == ItemCategory.category_UpperClothe)
            {
                UpperClotheItem currentShowedUpperClothe = currentShowedItem.GetComponent<UpperClotheItem>();
                UpperBodyClothingSet currentShowedUpperClotheData = currentShowedUpperClothe.upperBodyClothingSet;

                GameObject discardedUpperClothe = Instantiate(upperClotheObjectPrefab, spawnPosition, Quaternion.identity);

                UpperClotheItem discardedUpperClotheItem = discardedUpperClothe.GetComponent<UpperClotheItem>();

                if (discardedUpperClotheItem != null)
                {
                    discardedUpperClotheItem.Initialize(currentShowedUpperClotheData);
                }
                if (currentShowedItem == currentEquippedUpperClothe)
                {
                    EquipUpperClothe(null, null);
                }
            }

            else if (currentShowedItemCategory == ItemCategory.category_UnderClothe)
            {
                UnderClotheItem currentShowedUnderClothe = currentShowedItem.GetComponent<UnderClotheItem>();
                UnderClotheData currentShowedUnderClotheData = currentShowedUnderClothe.underClotheData;

                GameObject discardedUnderClothe = Instantiate(underClotheObjectPrefab, spawnPosition, Quaternion.identity);

                UnderClotheItem discardedUnderClotheItem = discardedUnderClothe.GetComponent<UnderClotheItem>();

                if (discardedUnderClotheItem != null)
                {
                    discardedUnderClotheItem.Initialize(currentShowedUnderClotheData);
                }
                if (currentShowedItem == currentEquippedUnderClothe)
                {
                    EquipUnderClothe(null, null);
                }
            }
            
            else if(currentShowedItemCategory == ItemCategory.category_Helmet)
            {
                HelmetItem currentShowedHelmet = currentShowedItem.GetComponent<HelmetItem>();

                GameObject discardedHelmet = Instantiate(helmetObjectPrefab, spawnPosition, Quaternion.identity);
                HelmetItem discardedHelmetItem = discardedHelmet.GetComponent<HelmetItem>();
                discardedHelmetItem.Initialize(currentShowedHelmet.helmetData);

                if(currentShowedItem == currentEquippedHelemet)
                {
                    EquipHelmet(null, null);
                }             
            }
            
            else if(currentShowedItemCategory == ItemCategory.category_Shoes)
            {
                ShoesItem currentShowedShoes = currentShowedItem.GetComponent<ShoesItem>();

                GameObject discardedShoes = Instantiate(shoesObjectPrefab, spawnPosition, Quaternion.identity);
                ShoesItem discardedShoesItem = discardedShoes.GetComponent<ShoesItem>();
                discardedShoesItem.Initialize(currentShowedShoes.shoesData);

                if(currentShowedItem == currentEquippedShoes)
                {
                    EquipShoes(null, null);
                }
            }

            else if(currentShowedItemCategory == ItemCategory.category_Gloves)
            {
                GlovesItem currentShowedGloves = currentShowedItem.GetComponent<GlovesItem>();

                GameObject discardedGlove = Instantiate(glovesObjectPrefab, spawnPosition, Quaternion.identity);
                GlovesItem discardedGlovesItem = discardedGlove.GetComponent<GlovesItem>();
                discardedGlovesItem.Initialize(currentShowedGloves.glovesData);

                if(currentShowedItem == currentequippedGloves)
                {
                    EquipGloves(null, null);
                }
            }

            Destroy(currentShowedItem);
            currentShowedItem = null;
            ShutDownItemInfo();
        }        
    }

    // @@
    public void PostStatusInfoPanel()
    {
        statusInfoPanel.gameObject.SetActive(true);
    }


    // @@ SaveEquipmentData
    public void SaveEquipment()
    {
        EquipmentJsonSaveData saveData = new EquipmentJsonSaveData();

        saveData.equippedSlots.Add(GetSlotData(equipWeaponSpace));
        saveData.equippedSlots.Add(GetSlotData(equipSecondaryWeaponSpace));
        saveData.equippedSlots.Add(GetSlotData(equipUpperClotheSpace));
        saveData.equippedSlots.Add(GetSlotData(equipUnderClotheSpace));
        saveData.equippedSlots.Add(GetSlotData(equipHelmetSpace));
        saveData.equippedSlots.Add(GetSlotData(equipShoesSpace));
        saveData.equippedSlots.Add(GetSlotData(equipGlovesSpace));

        foreach(RectTransform slot in blankSpaces){
            SlotData slotData = GetSlotData(slot);

            saveData.inventorySlots.Add(slotData);
        }

        SaveSystem.SaveEquipmentJsonData(saveData);
    }
    private SlotData GetSlotData(RectTransform slot)
    {
        if(slot.childCount > 0)
        {
            Transform item = slot.GetChild(0);

            if(item.TryGetComponent(out WeaponItem weaponItem))
            {
                return new SlotData
                {
                    itemCategory = "Weapon",
                    itemScriptablePath = weaponItem.weaponData.scriptablePath
                };
            }
            else if(item.TryGetComponent(out SecondaryWeaponItem secondaryWeaonItem))
            {
                return new SlotData
                {
                    itemCategory = "SecondaryWeapon",
                    itemScriptablePath = secondaryWeaonItem.secondaryWeaponData.scriptablePath
                };
            }
            else if(item.TryGetComponent(out UpperClotheItem upperClotheItem))
            {
                return new SlotData
                {
                    itemCategory = "UpperClothe",
                    itemScriptablePath = upperClotheItem.upperBodyClothingSet.scriptablePath
                };
            }
            else if(item.TryGetComponent(out UnderClotheItem underClotheItem))
            {
                return new SlotData
                {
                    itemCategory = "UnderClothe",
                    itemScriptablePath = underClotheItem.underClotheData.scriptablePath
                };
            }
            else if(item.TryGetComponent(out HelmetItem helmetItem))
            {
                return new SlotData
                {
                    itemCategory = "Helmet",
                    itemScriptablePath = helmetItem.helmetData.scriptPath
                };
            }
            else if(item.TryGetComponent(out ShoesItem shoesItem))
            {
                return new SlotData
                {
                    itemCategory = "Shoes",
                    itemScriptablePath = shoesItem.shoesData.scriptPath
                };
            }
            else if(item.TryGetComponent(out GlovesItem glovesItem))
            {
                return new SlotData
                {
                    itemCategory = "Gloves",
                    itemScriptablePath = glovesItem.glovesData.scriptPath
                };
            }
        }
        return null;
    }

    public void LoadEquipment()
    {
        EquipmentJsonSaveData loadData = SaveSystem.LoadEquipmentJsonData();

        SetSlotData(equipWeaponSpace, loadData.equippedSlots.Count > 0 ? loadData.equippedSlots[0] : null);       
        SetSlotData(equipSecondaryWeaponSpace, loadData.equippedSlots.Count > 1 ? loadData.equippedSlots[1] : null);
        SetSlotData(equipUpperClotheSpace, loadData.equippedSlots.Count > 2 ? loadData.equippedSlots[2] : null);
        SetSlotData(equipUnderClotheSpace, loadData.equippedSlots.Count > 3 ? loadData.equippedSlots[3] : null);
        SetSlotData(equipHelmetSpace, loadData.equippedSlots.Count > 4 ? loadData.equippedSlots[4] : null);
        SetSlotData(equipShoesSpace, loadData.equippedSlots.Count > 5 ? loadData.equippedSlots[5] : null);
        SetSlotData(equipGlovesSpace, loadData.equippedSlots.Count > 6 ? loadData.equippedSlots[6] : null);

        for(int i = 0; i < blankSpaces.Count; i++)
        {
            if(i < loadData.inventorySlots.Count)
            {
                SetSlotData(blankSpaces[i], loadData.inventorySlots[i]);
            }
        }
    }
    private void SetSlotData(RectTransform slot, SlotData slotData)
    {
        if(slotData != null)
        {
            GameObject newItem = null;
            
            switch (slotData.itemCategory)
            {
                case "Weapon":
                    newItem = Instantiate(weaponUIPrefab, slot);

                    WeaponData newWeaponData = Resources.Load<WeaponData>(slotData.itemScriptablePath);

                    WeaponItem newWeaponItem = newItem.GetComponent<WeaponItem>();
                    if(newWeaponData != null && newWeaponItem != null)
                    {
                        newWeaponItem.Initialize(newWeaponData);
                    }
                    else
                    {

                    }
                    break;
                    
                case "SecondaryWeapon":
                    newItem = Instantiate(secpondaryWeaponUIPrefab, slot);

                    SecondaryWeaponData newSecondaryWeaponData = Resources.Load<SecondaryWeaponData>(slotData.itemScriptablePath);

                    SecondaryWeaponItem newSecondaryWeaponItem = newItem.GetComponent<SecondaryWeaponItem>();

                    if(newSecondaryWeaponData != null && newSecondaryWeaponItem != null)
                    {
                        newSecondaryWeaponItem.Initialize(newSecondaryWeaponData);
                    }
                    else
                    {

                    }
                    break;
                case "UpperClothe":
                    newItem = Instantiate(upperClotheUIPrefab, slot);

                    UpperBodyClothingSet newUpperClotheData = Resources.Load<UpperBodyClothingSet>(slotData.itemScriptablePath);

                    UpperClotheItem newUpperClotheItem = newItem.GetComponent<UpperClotheItem>();

                    if(newUpperClotheData != null && newUpperClotheItem != null)
                    {
                        newUpperClotheItem.Initialize(newUpperClotheData);
                    }
                    else
                    {

                    }
                    break;
                case "UnderClothe":
                    newItem = Instantiate(underClotheUIPrefab, slot);
                    UnderClotheData newUnderClotheData = Resources.Load<UnderClotheData>(slotData.itemScriptablePath);

                    UnderClotheItem newUnderClotheItem = newItem.GetComponent<UnderClotheItem>();

                    if(newUnderClotheData != null && newUnderClotheItem != null)
                    {
                        newUnderClotheItem.Initialize(newUnderClotheData);
                    }
                    else
                    {

                    }
                    break;
                case "Helmet":
                    newItem = Instantiate(helmetUIPrefab, slot);

                    HelmetData helmetData = Resources.Load<HelmetData>(slotData.itemScriptablePath);

                    HelmetItem helmetItem = newItem.GetComponent<HelmetItem>();

                    if(helmetData != null && helmetItem != null)
                    {
                        helmetItem.Initialize(helmetData);  
                    }
                    else
                    {

                    }
                    break;
                case "Shoes":
                    newItem = Instantiate(shoesUIPrefab, slot);

                    ShoesData shoesData = Resources.Load<ShoesData>(slotData.itemScriptablePath);

                    ShoesItem shoesItem = newItem.GetComponent<ShoesItem>();

                    if(shoesData != null && shoesItem != null)
                    {
                        shoesItem.Initialize(shoesData);
                    }
                    else
                    {

                    }
                    break;
                case "Gloves":
                    newItem = Instantiate(glovesUIPrefab, slot);

                    GlovesData glovesData = Resources.Load<GlovesData>(slotData.itemScriptablePath);

                    GlovesItem glovesItem = newItem.GetComponent<GlovesItem>();

                    if(glovesData != null && glovesItem != null)
                    {
                        glovesItem.Initialize(glovesData);
                    }
                    else
                    {

                    }
                    break;
            }
        }
        
    }


}
