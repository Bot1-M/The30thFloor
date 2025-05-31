using System.Collections;
using UnityEngine;

public class MenuTransitionUI : MonoBehaviour
{
    public RectTransform mainMenuUI;
    public RectTransform inputUI;

    public RectTransform configUI;

    public RectTransform tutorialUI;

    public float slideDuration = 0.5f;
    public Vector2 offScreenPosition = new Vector2(0, -1080);

    private Vector2 centerPosition;

    private CanvasGroup mainCanvasGroup;
    private CanvasGroup inputCanvasGroup;
    private CanvasGroup configCanvasGroup;
    private CanvasGroup tutorialCanvasGroup;


    private void Start()
    {
        centerPosition = mainMenuUI.anchoredPosition;

        mainCanvasGroup = mainMenuUI.GetComponent<CanvasGroup>();
        inputCanvasGroup = inputUI.GetComponent<CanvasGroup>();
        configCanvasGroup = configUI.GetComponent<CanvasGroup>();
        tutorialCanvasGroup = tutorialUI.GetComponent<CanvasGroup>();

        if (mainCanvasGroup == null || inputCanvasGroup == null)
        {
            Debug.LogError("Faltan CanvasGroup en uno o ambos objetos UI.");
            return;
        }

        inputUI.anchoredPosition = offScreenPosition;
        inputCanvasGroup.alpha = 0;
        inputCanvasGroup.interactable = false;
        inputCanvasGroup.blocksRaycasts = false;

        mainCanvasGroup.alpha = 1;
        mainCanvasGroup.interactable = true;
        mainCanvasGroup.blocksRaycasts = true;

        configUI.anchoredPosition = offScreenPosition;
        configCanvasGroup.alpha = 0;
        configCanvasGroup.interactable = false;
        configCanvasGroup.blocksRaycasts = false;

        tutorialUI.anchoredPosition = offScreenPosition;
        tutorialCanvasGroup.alpha = 0;
        tutorialCanvasGroup.interactable = false;
        tutorialCanvasGroup.blocksRaycasts = false;


        configUI.gameObject.SetActive(false);
        inputUI.gameObject.SetActive(false);
        tutorialUI.gameObject.SetActive(false);
    }

    public void StartGameTransition()
    {
        StartCoroutine(SlideAndFade(mainMenuUI, inputUI, mainCanvasGroup, inputCanvasGroup, false));
    }

    public void GameBackToMenuTransition()
    {
        StartCoroutine(SlideAndFade(inputUI, mainMenuUI, inputCanvasGroup, mainCanvasGroup, true));
    }

    public void ConfigTransition()
    {
        StartCoroutine(SlideAndFade(mainMenuUI, configUI, mainCanvasGroup, configCanvasGroup, false));
    }
    public void ConfigBackToMenuTransition()
    {
        StartCoroutine(SlideAndFade(configUI, mainMenuUI, configCanvasGroup, mainCanvasGroup, true));
    }


    public void TutorialTransition()
    {
        StartCoroutine(SlideAndFade(mainMenuUI, tutorialUI, mainCanvasGroup, tutorialCanvasGroup, false));
    }

    public void TutorialBackToMenuTransition()
    {
        StartCoroutine(SlideAndFade(tutorialUI, mainMenuUI, configCanvasGroup, mainCanvasGroup, true));
    }

    private IEnumerator SlideAndFade(RectTransform fromUI, RectTransform toUI, CanvasGroup fromGroup, CanvasGroup toGroup, bool isBack)
    {
        float elapsed = 0f;

        Vector2 fromStart = centerPosition;
        Vector2 fromEnd = offScreenPosition;
        Vector2 toStart = offScreenPosition;
        Vector2 toEnd = centerPosition;

        fromGroup.interactable = false;
        fromGroup.blocksRaycasts = false;

        toUI.gameObject.SetActive(true);
        toGroup.alpha = 0;
        toGroup.interactable = false;
        toGroup.blocksRaycasts = false;

        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;

            fromUI.anchoredPosition = Vector2.Lerp(fromStart, fromEnd, t);
            toUI.anchoredPosition = Vector2.Lerp(toStart, toEnd, t);

            fromGroup.alpha = 1 - t;
            toGroup.alpha = t;

            elapsed += Time.deltaTime;
            yield return null;
        }

        fromUI.anchoredPosition = fromEnd;
        toUI.anchoredPosition = toEnd;

        fromGroup.alpha = 0;
        fromUI.gameObject.SetActive(false);

        toGroup.alpha = 1;
        toGroup.interactable = true;
        toGroup.blocksRaycasts = true;
    }
}
