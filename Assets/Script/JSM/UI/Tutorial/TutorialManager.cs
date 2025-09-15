using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("설정")]
    public List<TutorialData> tutorialDataList;
    public Canvas tutorialCanvas;
    public AssetReferenceGameObject dialogueBoxReference;

    [Header("마스크")]
    public GameObject maskPanel;
    public GameObject blackImage;
    public GameObject blockImage;

    private GameObject dialogueBoxPrefab;
    private DialogPanel dialogPanel;
    private GameObject dialogueBoxInstance;

    private GameObject panel;
    private TMP_Text npcNameText;
    private TMP_Text dialogueText;
    private Button nextButton;
    private Button nextButton2;

    private readonly Dictionary<string, Action> triggerActions = new();

    private CameraController cameraController;
    private int currentStepIndex;

    public bool isPlaying = false;
    public bool isTutoring = false;
    private bool hasPlayedNpcIntro = false;
    private TutorialData tutorialData;
    private int tutoNum;
    private AsyncOperationHandle<GameObject> handle;

    private AsyncOperationHandle<GameObject> dialogueBoxHandle;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartTuto(int i)
    {
        Debug.Log("튜토리얼 실행");
        if (PlayerDataManager.Instance.player.tutorialDone[i]) return;
        if (isPlaying)
        {
            Debug.LogWarning("튜토리얼이 이미 진행 중입니다.");
            return;
        }

        isPlaying = true;
        isTutoring = true;
        tutorialData = tutorialDataList[i];
        tutoNum = i;

        if (i == 0)
        {
            var tutorialDeck = new DeckData();
            tutorialDeck.AddNormalUnit(1001);
            tutorialDeck.AddNormalUnit(1002);
            tutorialDeck.SetLeaderUnit(3001);

            var clonedDeck = DeckManager.Instance.CloneDeck(tutorialDeck);

            PlayerDataManager.Instance.player.currentDeck = clonedDeck;
            SceneManager.LoadScene("MainScene");
            SceneManager.sceneLoaded += OnBattleSceneLoaded;
            SceneManager.LoadScene("BattleScene");
            Debug.Log($"튜토리얼 입장");
        }

        StartCoroutine(InitTutorial());
    }


    private void OnBattleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            var normalDeck = PlayerDataManager.Instance.player.currentDeck.GetAllNormalUnit();
            var leaderDeck = PlayerDataManager.Instance.player.currentDeck.GetLeaderUnitInDeck();
            SceneManager.sceneLoaded -= OnBattleSceneLoaded;
            BattleManager.Instance.StartBattle(110000, normalDeck, leaderDeck);
        }
    }


    private IEnumerator InitTutorial()
    {
        if (dialogueBoxInstance != null)
        {
            Destroy(dialogueBoxInstance); // 혹시 남아 있으면 제거
            Addressables.ReleaseInstance(dialogueBoxInstance);
            dialogueBoxInstance = null;
        }

        dialogueBoxHandle = dialogueBoxReference.InstantiateAsync(tutorialCanvas.transform);
        yield return dialogueBoxHandle;

        if (dialogueBoxHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("튜토리얼 프리팹 로드 실패");
            yield break;
        }

        dialogueBoxInstance = dialogueBoxHandle.Result;
        dialogueBoxInstance.SetActive(false); // 처음엔 숨김
        dialogPanel = dialogueBoxInstance.GetComponent<DialogPanel>();

        AssignCamera();
        StartTutorial();
    }



    private void AssignCamera()
    {
        cameraController = FindObjectOfType<CameraController>();
        Debug.Log(cameraController);
    }

    public void StartTutorial()
    {
        Debug.Log("튜토리얼 시작!");
        BackHandlerManager.Instance.SetBackEnabled(false);
        currentStepIndex = 0;
        hasPlayedNpcIntro = false;

        blackImage.SetActive(true);
        ShowCurrentStep();
        RegisterTriggerActions();
    }

    private void ShowCurrentStep()
    {
        if (tutorialData.steps.Count <= 0)
        {
            EndTutorial();
            return;
        }
        var step = tutorialData.steps[currentStepIndex];

        // 🎯 triggerEventName은 effectID가 6이 아닐 때만 적용
        if (!string.IsNullOrEmpty(step.triggerEventName) && step.effectID != 6)
        {
            tutorialCanvas.gameObject.SetActive(false);
            Debug.Log($"[튜토리얼] 트리거 '{step.triggerEventName}' 대기 중...");
            return;
        }

        tutorialCanvas.gameObject.SetActive(true);
        StartCoroutine(PlayStep(step));
    }

    private IEnumerator PlayStep(TutorialStep step)
    {
        if (dialogueBoxInstance == null)
        {
            Debug.LogError("dialogueBoxInstance is null");
            yield break;
        }

        dialogueBoxInstance.SetActive(true); // 재활용

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.gameObject.SetActive(true);
            nextButton.interactable = false;
        }

        panel = dialogueBoxInstance.transform.Find("Panel").gameObject;
        npcNameText = dialogueBoxInstance.transform.Find("Panel/Name/NameText")?.GetComponent<TextMeshProUGUI>();
        dialogueText = dialogueBoxInstance.transform.Find("Panel/Dialog/DialogText")?.GetComponent<TextMeshProUGUI>();
        nextButton = dialogueBoxInstance.transform.Find("NextButton")?.GetComponent<Button>();
        nextButton2 = maskPanel.GetComponent<Button>();

        if (step.dialogUp)
            MoveToTopCenter(panel, 200);
        else
            MoveToBottomCenter(panel, 100);

        npcNameText.text = step.npcName;
        dialogueText.text = step.dialogue;

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextStep);
        nextButton.interactable = false;
        nextButton2?.onClick.RemoveAllListeners();

        var dialog = dialogueBoxInstance.GetComponent<DialogPanel>();
        if (dialog != null)
        {
            if (!hasPlayedNpcIntro)
            {
                hasPlayedNpcIntro = true;
                yield return dialog.PlayNpcIntroAnimationWithYield();
            }
            else
            {
                dialog.ShowDialogInstant();
            }
        }

        yield return HandleStepEffects(step);

        if (step.effectID != 6 && string.IsNullOrEmpty(step.triggerEventName))
        {
            nextButton.interactable = true;
        }
    }


    public void MoveToTopCenter(GameObject uiObject, float offsetY = 0f)
    {
        RectTransform targetRect = uiObject.GetComponent<RectTransform>();
        if (targetRect == null) return;

        targetRect.pivot = new Vector2(0.5f, 1f);
        targetRect.anchorMin = new Vector2(0.5f, 1f);
        targetRect.anchorMax = new Vector2(0.5f, 1f);
        targetRect.anchoredPosition = new Vector2(0f, -offsetY);
    }

    public void MoveToBottomCenter(GameObject uiObject, float offsetY = 0f)
    {
        RectTransform targetRect = uiObject.GetComponent<RectTransform>();
        if (targetRect == null) return;

        targetRect.pivot = new Vector2(0.5f, 0f);
        targetRect.anchorMin = new Vector2(0.5f, 0f);
        targetRect.anchorMax = new Vector2(0.5f, 0f);
        targetRect.anchoredPosition = new Vector2(0f, offsetY);
    }

    private IEnumerator HandleStepEffects(TutorialStep step)
    {
        int effectID = step.effectID;
        string target = step.highlightTarget;
        maskPanel.SetActive(false);
        blackImage.SetActive(false);
        blockImage.SetActive(false);

        switch (effectID)
        {
            case 0://연출 없음
                blockImage.SetActive(true);
                var cam = FindObjectOfType<BattleCameraController>();
                if (target == "EnemyHealth") cam?.FocusRightMax();
                else if (target == "AllyHealth") cam?.FocusLeftMax();
                break;

            case 1://target 하이라이트
                maskPanel.SetActive(true);
                GameObject button = GameObject.Find(target);
                if (button != null) HighlightUI(button);
                break;

            case 2://배틀신 이동
                SceneManager.LoadScene("BattleScene");
                break;

            case 3://검은 화면
                blackImage.SetActive(true);
                break;

            case 4://배틀 카메라 되돌리기(1번만씀)
                blockImage.SetActive(true);
                FindObjectOfType<BattleCameraController>()?.RestoreCameraState();
                break;

            case 5://메인신 이동
                SceneManager.LoadScene("MainScene");
                break;

            case 6:
                blockImage.SetActive(true);
                maskPanel.SetActive(true);
                GameObject highlightTarget = GameObject.Find(target);
                if (highlightTarget != null)
                    HighlightUI(highlightTarget);

                nextButton2.onClick.RemoveAllListeners();

                nextButton2.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(step.triggerEventName) && triggerActions.TryGetValue(step.triggerEventName, out var action))
                    {
                        Debug.Log($"[튜토리얼] triggerEventName 실행: {step.triggerEventName}");
                        action.Invoke();
                    }

                    NextStep();
                });

                nextButton2.gameObject.SetActive(true);
                if (nextButton != null) nextButton.gameObject.SetActive(false);
                break;
            case 7:
                UIController.Instance.OnExitBtn();
                break;
            case 8:
                var ui = GameObject.Find("PurchaseUIBox");
                ui.SetActive(false);
                break;

            case 20:
                GameObject GosCloseBtn = GameObject.Find("GosCloseBtn");
                Button gosCloseBtn = GosCloseBtn.GetComponent<Button>();
                gosCloseBtn.onClick.Invoke();
                GameObject ShowCancleBtn = GameObject.Find("ShowCancleBtn");
                Button showCancleBtn = ShowCancleBtn.GetComponent<Button>();
                showCancleBtn.onClick.Invoke();

                break;
            
        }

        yield return null;
    }

    public void NextStep()
    {
        var dialogPanel = dialogueBoxInstance?.GetComponent<DialogPanel>();
        if (dialogPanel != null && dialogPanel.IsNpcAnimating())
        {
            dialogPanel.ForceFinishAnimation();
            return;
        }

        currentStepIndex++;
        if (currentStepIndex < tutorialData.steps.Count)
            ShowCurrentStep();
        else
            EndTutorial();
    }

    public void OnEventTriggered(string eventName)
    {
        if (tutorialData == null) return;
        var step = tutorialData.steps[currentStepIndex];
        if (step.effectID == 6 && step.highlightTarget == eventName)
        {
            Debug.Log($"[튜토리얼] 이벤트 '{eventName}' 수신됨, 다음 단계로");
            NextStep();
        }
        else if (step.triggerEventName == eventName)
        {
            Debug.Log($"[튜토리얼] 트리거 '{eventName}' 수신됨, 다음 단계로");
            NextStep();
        }
    }

    public void EndTutorial()
    {
        maskPanel.SetActive(false);
        blackImage.SetActive(false);
        blockImage.SetActive(false);
        isPlaying = false;
        isTutoring = false;

        if (dialogueBoxInstance != null)
        {
            Addressables.ReleaseInstance(dialogueBoxInstance);
            dialogueBoxInstance = null;
        }
        Debug.Log(tutoNum);
        PlayerDataManager.Instance.player.tutorialDone[tutoNum] = true;
        Debug.Log(PlayerDataManager.Instance.player.tutorialDone[0] + "" + PlayerDataManager.Instance.player.tutorialDone[1] + "" + PlayerDataManager.Instance.player.tutorialDone[2] + "" + PlayerDataManager.Instance.player.tutorialDone[3]);
        Debug.Log("튜토리얼 완료");
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "BattleScene")
        {
            SceneManager.LoadScene("MainScene");
        }
        else
            UIController.Instance.OnExitBtn();
        switch (tutoNum)
        {
            case 1:
                UIController.Instance.OpenShop();
                break;
            case 2:
                UIController.Instance.OpenSacredPlace();
                break;
            case 3:
                UIController.Instance.OpenStage();
                break;
        }
        BackHandlerManager.Instance.SetBackEnabled(true);
    }



    public void HighlightUI(GameObject targetUI)
    {
        RectTransform targetRect = targetUI.GetComponent<RectTransform>();
        RectTransform maskRect = maskPanel.GetComponent<RectTransform>();
        Canvas maskCanvas = maskPanel.GetComponentInParent<Canvas>();
        Canvas targetCanvas = targetUI.GetComponentInParent<Canvas>();

        if (targetRect == null || maskRect == null || targetCanvas == null || maskCanvas == null) return;

        Camera targetCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
        Camera maskCamera = maskCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : maskCanvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        targetRect.GetWorldCorners(corners);

        Vector3 worldMin = corners[0];
        Vector3 worldMax = corners[2];

        RectTransform maskCanvasRect = maskCanvas.GetComponent<RectTransform>();

        // 월드 좌표를 마스크 캔버스의 로컬 좌표로 변환
        Vector2 localMin, localMax;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskCanvasRect,
            RectTransformUtility.WorldToScreenPoint(targetCamera, worldMin),
            maskCamera,
            out localMin
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskCanvasRect,
            RectTransformUtility.WorldToScreenPoint(targetCamera, worldMax),
            maskCamera,
            out localMax
        );

        Vector2 localCenter = (localMin + localMax) * 0.5f;
        Vector2 localSize = localMax - localMin;

        float margin = 5f;
        maskRect.anchoredPosition = localCenter;
        maskRect.sizeDelta = localSize + new Vector2(margin * 2f, margin * 2f);
    }

    private void RegisterTriggerActions()
    {
        triggerActions.Clear();

        triggerActions["Gottcha"] = () => UIController.Instance.OpenGottcha();
        triggerActions["Gottcha10"] = () =>
        {
            GameObject gottchaObj = GameObject.Find("Gottcha");
            if (gottchaObj == null)
            {
                Debug.LogError("[튜토리얼] 'Gottcha' 오브젝트를 찾을 수 없습니다.");
                return;
            }

            Pick pickComponent = gottchaObj.GetComponent<Pick>();
            if (pickComponent == null)
            {
                Debug.LogError("[튜토리얼] 'Gottcha' 오브젝트에 Pick 컴포넌트가 없습니다.");
                return;
            }
            //PlayerDataManager.Instance.player.ticket += 10;
            Debug.Log("[튜토리얼] Pick.PickTenTimes() 실행");
            pickComponent.PickTenTimes();
        };
        triggerActions["Gottcha10Exit"] = () =>
        {
            GameObject pickTenPage = GameObject.Find("PickTenPage");
            if (pickTenPage != null)
            {
                pickTenPage.SetActive(false);
                Debug.Log("[튜토리얼] PickTenPage 오브젝트 비활성화됨");
            }
            else
            {
                Debug.LogWarning("PickTenPage 오브젝트를 찾을 수 없습니다.");
            }
            UIController.Instance.OnExitBtn();
        };
        triggerActions["UnitManage"] = () => UIController.Instance.UnitManageActive();
        triggerActions["ClickNorm"] = () =>
        {
            GameObject gottchaObj = GameObject.Find("ItemSlot(Clone)");
            if (gottchaObj == null)
            {
                Debug.LogError("[튜토리얼] 'ItemSlot(Clone)' 오브젝트를 찾을 수 없습니다.");
                return;
            }

            ItemSlot pickComponent = gottchaObj.GetComponent<ItemSlot>();
            if (pickComponent == null)
            {
                Debug.LogError("[튜토리얼] 'ItemSlot(Clone)' 오브젝트에 ItemSlot 컴포넌트가 없습니다.");
                return;
            }
            Debug.Log("[튜토리얼] Pick.PickTenTimes() 실행");
            pickComponent.OnButtonClicked();
        };
        triggerActions["CertificateStoreBtn"] = () =>
        {
            GameObject gottchaObj = GameObject.Find("BookMark");
            if (gottchaObj == null)
            {
                Debug.LogError("[튜토리얼] 'BookMark' 오브젝트를 찾을 수 없습니다.");
                return;
            }

            var pickComponent = gottchaObj.GetComponent<BookMarkSet>();
            if (pickComponent == null)
            {
                Debug.LogError("[튜토리얼] 'BookMark' 오브젝트에 BookMarkSet 컴포넌트가 없습니다.");
                return;
            }
            Debug.Log("[튜토리얼] Pick.PickTenTimes() 실행");
            pickComponent.CertificateStoreSet();
        };

        triggerActions["SelectBtn"] = () =>
        {
            GameObject stageBtn = GameObject.Find("StageNode(Clone)");
            if (stageBtn == null)
            {
                Debug.Log("selectBtn을 찾을 수 없습니다.");
            }
            StageNode node = stageBtn.GetComponent<StageNode>();
            node.OnClickNode();
        };

        triggerActions["exitBtn"] = () =>
        {
            GameObject exitBtn = GameObject.Find("StageInfo");
            if (exitBtn == null)
            {
                Debug.Log("exitBtn을 찾을 수 없습니다.");
            }
            exitBtn.SetActive(false);
        };

        triggerActions["GoldDungeonBtn"] = () =>
        {
            GameObject gd = GameObject.Find("Stage");
            if (gd == null)
            {
                Debug.Log("gd를 찾을 수 없습니다.");
            }
            StageInit init = gd.GetComponentInChildren<StageInit>();
            init.OnGoldBtn();
        };

        triggerActions["TowerBtn"] = () =>
        {
            GameObject tower = GameObject.Find("Stage");
            if (tower == null)
            {
                Debug.Log("tower를 찾을 수 없습니다.");
            }
            StageInit init = tower.GetComponentInChildren<StageInit>();
            init.OnTowerBtn();
        };

        triggerActions["EnterTowerBtn"] = () =>
        {
            GameObject enterTower = GameObject.Find("TowerPrefab(Clone)");
            if (enterTower == null)
            {
                Debug.Log("enterTower를 찾을 수 없습니다.");
            }
            UITowerSlot info = enterTower.GetComponentInChildren<UITowerSlot>();
            info.OnClickTower();
        };

        triggerActions["ExitTowerInfo"] = () =>
        {
            GameObject towerInfo = GameObject.Find("TowerInfo");
            if (towerInfo == null)
            {
                Debug.Log("towerInfo를 찾을 수 없습니다.");
            }
            towerInfo.SetActive(false);
        };
        triggerActions["BuildSlot"] = () =>
        {
            GospelManager.Instance.LoadGospels();
            GameObject BuildSlot = GameObject.Find("BuildSlot_0");
            if (BuildSlot == null)
            {
                Debug.LogError("[튜토리얼] 'BuildSlot_0' 오브젝트를 찾을 수 없습니다.");
                return;
            }
            BuildSlotUI buildSlot = BuildSlot.GetComponent<BuildSlotUI>();
            if (buildSlot == null)
            {
                Debug.LogError("[튜토리얼] 'buildSlot' 오브젝트를 찾을 수 없습니다.");
                return;
            }
            buildSlot.Select();
        };
        triggerActions["BuildGrave"] = () =>

        {
            GameObject BuildGrave = GameObject.Find("BuildMenuSlot_0");
            BuildSelectButton buildGrave = BuildGrave.GetComponent<BuildSelectButton>();
            buildGrave.OnClick();
        };
        triggerActions["BuildIcons"] = () =>
        {
            GameObject BuildIcons = GameObject.Find("BuildMenuSlot_0");
            BuildSelectButton buildIcons = BuildIcons.GetComponent<BuildSelectButton>();
            buildIcons.ShowGospel();
        };
        triggerActions["TouchSpell"] = () =>
        {
            GameObject TouchSpell = GameObject.Find("GospelSlot_2");
            GospelSlotUI touchspell = TouchSpell.GetComponentInChildren<GospelSlotUI>();
            touchspell.OnClick();
        };
        triggerActions["ClosePanel1"] = () =>
        {
            GameObject ShowCancleBtn = GameObject.Find("ShowCancleBtn");
            Button showCancleBtn = ShowCancleBtn.GetComponent<Button>();
            showCancleBtn.onClick.Invoke();
        };
        triggerActions["ClosePanel2"] = () =>
        {
            GameObject GosCloseBtn = GameObject.Find("GosCloseBtn");
            Button gosCloseBtn = GosCloseBtn.GetComponent<Button>();
            gosCloseBtn.onClick.Invoke();
        };
        triggerActions["Confirm"] = () =>
        {
            GameObject GosCloseBtn = GameObject.Find("BuildConfirmBtn");
            Button gosCloseBtn = GosCloseBtn.GetComponent<Button>();
            gosCloseBtn.onClick.Invoke();
        };
        triggerActions["LvlPanel"] = () =>
        {
            GameObject build = GameObject.Find("BuildSlot_0/lvlButton");
            BuildingLevelUpUI lvBtn = build.GetComponentInChildren<BuildingLevelUpUI>();
            lvBtn.OnClick();
        };
    }
}