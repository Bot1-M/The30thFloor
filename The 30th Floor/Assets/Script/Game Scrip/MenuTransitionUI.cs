using System.Collections;
using UnityEngine;

public class MenuTransitionUI : MonoBehaviour
{
    public RectTransform mainMenuUI;
    public RectTransform inputUI;

    public RectTransform Config;

    public float slideDuration = 0.5f;
    public Vector2 offScreenPosition = new Vector2(0, -1080);

    private Vector2 centerPosition;

    private CanvasGroup mainCanvasGroup;
    private CanvasGroup inputCanvasGroup;

    private void Start()
    {
        centerPosition = mainMenuUI.anchoredPosition;

        mainCanvasGroup = mainMenuUI.GetComponent<CanvasGroup>();
        inputCanvasGroup = inputUI.GetComponent<CanvasGroup>();

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

        inputUI.gameObject.SetActive(false);
    }

    public void StartGameTransition()
    {
        StartCoroutine(SlideAndFade(mainMenuUI, inputUI, mainCanvasGroup, inputCanvasGroup, false));
    }

    public void BackToMenuTransition()
    {
        StartCoroutine(SlideAndFade(inputUI, mainMenuUI, inputCanvasGroup, mainCanvasGroup, true));
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
