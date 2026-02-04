using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextRenderManager : MonoBehaviour
{
    public static TextRenderManager Instance { get; private set; }

    public TextMeshProUGUI textMeshProUGUI;
    public GameObject dialoguePanel;
    // 페이드 인/아웃에 사용할 공용 시간
    public float fadeDuration = 0.5f;

    // 대화 데이터를 저장하는 딕셔너리
    private Dictionary<string, string[]> textData = new Dictionary<string, string[]>();

    // --- 현재 상태 변수 ---
    private string currentTextId = string.Empty;     // 현재 표시 중인 텍스트 그룹의 ID
    private int currentIndex = -1;                   // 현재 그룹에서 표시 중인 텍스트의 인덱스
    private Coroutine activeTextCoroutine;           // 현재 활성화된 코루틴 (ShowTextSequence 또는 AutoShowSequence)
    private Color baseColor;                        // 텍스트의 기본 색상 (알파값 제외)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 기본 색상 및 초기 투명도 설정
        baseColor = textMeshProUGUI.color;
        textMeshProUGUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        // 튜토리얼 텍스트 데이터 초기화 (유지)
        textData.Add("CameraT", new string[]
        {
            "안녕하세요. 여긴 튜토리얼 필드입니다. ",
            "마우스를 우클릭을 누른 상태로 자유롭게 드래그 해보세요.",
            "잘했습니다.",
            "아무 키를 누르시면 다음 튜토리얼로 넘어갑니다."
        });

        textData.Add("MoveT", new string[]
        {
            "이동 튜토리얼을 시작하겠습니다.",
            "마우스 우클릭으로 카메라를 제어하고,W,A,S,D를 이용하여 빨간색원으로 이동해보세요.",
            "잘했습니다.",
            "아무 키를 누르시면 다음 튜토리얼로 넘어갑니다."

        });

        textData.Add("CombatT", new string[]
        {
            "전투 튜토리얼을 시작하겠습니다.",
            "마우스를 좌클릭하고 1번 또는 2번을 눌러보세요.",
            "우클릭을 하면 전투상태에 돌입하고 공격할 수 있습니다.",
            "전투 상태에서만 공격할 수 있다는 점을 기억하세요!",
            "이제 스켈레톤과 스파이더를 공격하여 쓰려트려 보세요!",
            "참고로 E키를 누르면 현재 타겟팅을 변경할 수 있습니다!",
            "잘했습니다",
            "아무 키를 누르시면 다음 튜토리얼로 넘어갑니다."


        });

        textData.Add("TargetingT", new string[]
        {
            "타겟팅 튜토리얼을 시작하겠습니다.",
            "적들 아래의 빨간 원은 현재 타겟팅 상태인 적을 표시합니다.",
            "E키를 눌러 타겟팅을 변경하고 공격도 해보세요.",
            "잘했습니다",
            "아무 키를 누르시면 다음 튜토리얼로 넘어갑니다."

        });

        textData.Add("InteractT", new string[] 
        { 
            "채집 튜토리얼을 시작하겠습니다.",
            "스페이스 바를 눌러 근처의 채집물을 자동으로 탐색하고 채집할 수 있습니다.",
            "채집물 근처에서 스페이스바를 눌러보세요",
            "잘했습니다.",
            "여러 채집물이 있을 시 가장 가까운 채집물부터 채집되는 점을 기억하세요!",
            "주변의 채집물도 채집해보세요!",
            "아무 키를 누르시면 다음 튜토리얼로 넘어갑니다"
        
        });
        textData.Add("CraftT", new string[]
        {
             "제작 튜토리얼을 시작하겠습니다.",
             "C키(예시)를 눌러 제작 UI를 열어보세요.",
             "원하는 아이템을 선택하고 재료가 충분하다면 제작이 가능합니다.",
             "잘했습니다!",
             "부족한 재료는 빨간색으로 표시됩니다.",
             "필요한 만큼 제작해보세요!",
             "아무 키나 누르면 다음으로 넘어갑니다."
        });
        textData.Add("EquipT", new string[]
       {
             "장비 튜토리얼을 시작하겠습니다.",
             "제작을 통하여 방어구를 하나 만들어보세요.",
             "그 후에 I를 눌러 장비아이템을 클릭하여 보세요.",
             "잘했습니다!",
             "장비 아이템을 장착하여 캐릭터를 성장시킬 수 있습니다.",
             "튜토리얼을 종료합니다."
       });





    }

    // 외부에서 특정 ID의 텍스트 그룹을 시작할 때 사용
    public void StartShow(string textId)
    {
        // 이전 코루틴 중지
        if (activeTextCoroutine != null)
        {
            StopCoroutine(activeTextCoroutine);
        }

        if (!textData.ContainsKey(textId))
        {
            Debug.LogError($"[TextRenderManager] 텍스트 ID '{textId}'를 찾을 수 없습니다.");
            return;
        }

        currentTextId = textId;
        currentIndex = 0;

        // 첫 번째 텍스트를 바로 표시
        activeTextCoroutine = StartCoroutine(ShowText(textData[textId][currentIndex]));
    }

    // 다음 텍스트를 표시 (주로 사용자 입력에 의해 호출)
    public bool NextShow()
    {
        if (string.IsNullOrEmpty(currentTextId) || !textData.ContainsKey(currentTextId))
            return false;

        string[] texts = textData[currentTextId];
        currentIndex++; // 다음 인덱스로 이동

        if (currentIndex < texts.Length)
        {
            // 다음 텍스트가 있으면 표시
            if (activeTextCoroutine != null)
            {
                StopCoroutine(activeTextCoroutine);
            }
            activeTextCoroutine = StartCoroutine(ShowText(texts[currentIndex]));
            return true;
        }
        else
        {
            // 모든 텍스트 표시 완료
            currentTextId = string.Empty;
            currentIndex = -1;

            // 텍스트를 완전히 숨깁니다 (페이드 아웃)
            if (activeTextCoroutine != null)
            {
                StopCoroutine(activeTextCoroutine);
            }
            activeTextCoroutine = StartCoroutine(Fade(textMeshProUGUI.color.a, 0f, fadeDuration));
            return false;
        }
    }

    private IEnumerator ShowText(string textToDisplay)
    {
        // 1. 이전 텍스트가 있다면 페이드 아웃
        if (textMeshProUGUI.color.a > 0)
        {
            // In/Out 시간을 짧게 조정하여 전환 속도 개선
            yield return StartCoroutine(Fade(textMeshProUGUI.color.a, 0f, fadeDuration * 0.4f));
        }

        // 2. 텍스트 내용 설정
        textMeshProUGUI.text = textToDisplay;

        // 3. 페이드 인 (알파 0 -> 1)
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // 4. 입력 대기 상태 (코루틴 종료. NextShow 호출될 때까지 텍스트 유지)
        activeTextCoroutine = null;
    }

    public Coroutine AutoShow(int fromIndex, int toIndex)
    {
        if (string.IsNullOrEmpty(currentTextId) || !textData.ContainsKey(currentTextId))
        {
            Debug.LogError("[TextRenderManager] 현재 텍스트 ID가 설정되지 않았거나 유효하지 않습니다.");
            return null;
        }

        string[] texts = textData[currentTextId];

        // 인덱스 유효성 검사
        fromIndex = Mathf.Clamp(fromIndex, 0, texts.Length - 1);
        toIndex = Mathf.Clamp(toIndex, fromIndex, texts.Length - 1);

        // 이전 코루틴 중지
        if (activeTextCoroutine != null)
        {
            StopCoroutine(activeTextCoroutine);
        }

        // 현재 인덱스 업데이트 (AutoShowSequence 내부에서 다시 업데이트되지만, 시작점 명시)
        currentIndex = fromIndex;

        // 자동 표시 코루틴 시작
        float timeToDisplay = 2.0f;
        activeTextCoroutine = StartCoroutine(AutoShowSequence(texts, fromIndex, toIndex, timeToDisplay));
        return activeTextCoroutine;
    }


    private IEnumerator AutoShowSequence(string[] texts, int start, int end, float displayTime)
    {
        // 이전 텍스트가 남아있다면 먼저 페이드 아웃
        if (textMeshProUGUI.color.a > 0)
        {
            yield return StartCoroutine(Fade(textMeshProUGUI.color.a, 0f, fadeDuration * 0.5f));
        }

        for (int i = start; i <= end; i++)
        {
            currentIndex = i; // 현재 인덱스 업데이트

            // 1. 텍스트 내용 설정 및 페이드 인
            textMeshProUGUI.text = texts[i];
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            // 2. 일정 시간 유지
            yield return new WaitForSecondsRealtime(displayTime);

            // 3. 페이드 아웃
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

            // 4. 내용 초기화 (화면에서 텍스트가 완전히 사라지게)
            textMeshProUGUI.text = "";
        }
        activeTextCoroutine = null;
    }

    // Lerp를 사용하여 알파값을 부드럽게 변경하는 범용 페이드 코루틴 (유지)
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        Color currentColor = textMeshProUGUI.color;
        CanvasGroup canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);

            // 텍스트 알파 변경
            textMeshProUGUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);

            canvasGroup.alpha = newAlpha;

            yield return null;
        }

        // 최종 값
        textMeshProUGUI.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);
        canvasGroup.alpha = endAlpha;

        if (endAlpha <= 0.01f)
        {
            textMeshProUGUI.text = "";
        }
    }
    public bool isDoneShowingText()
    {
        if (textMeshProUGUI.text == "" && activeTextCoroutine == null)
            return true;
        else return false;
    }
}