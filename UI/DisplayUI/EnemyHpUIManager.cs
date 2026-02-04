using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyHpUIManager : MonoBehaviour
{
    public GameObject hpBarPrefab; // 프리팹 직접 할당
    private List<GameObject> hpBars = new List<GameObject>();
    private HashSet<GameObject> activeEnemies = new HashSet<GameObject>(); // Contains 체크 최적화

    [SerializeField]
    private GameObject damagedTextPrefab;
    [SerializeField]
    private int poolSize = 15;

    [SerializeField]
    private int textPoolSize = 30;
    private Queue<GameObject> textPool = new Queue<GameObject>();
    private List<DamagedText> activeTexts = new List<DamagedText>();

    struct DamagedText
    {
        public GameObject obj;
        public TextMeshProUGUI textMesh;
        public float startTime;
        public Vector3 startPos;
    }

    void Start()
    {   
        for(int i=0;  i<poolSize; i++)
        {
            var hpBar=Instantiate(hpBarPrefab,this.transform);
            hpBars.Add(hpBar);
            hpBar.SetActive(false);
        }

        for (int i = 0; i < textPoolSize; i++)
        {
            var damagePrefab = Instantiate(damagedTextPrefab, transform);
            damagePrefab.SetActive(false);
            textPool.Enqueue(damagePrefab);
        }

    }

    private void Update()
    {
        float duration = 1.0f;
        for (int i = activeTexts.Count - 1; i >= 0; i--)
        {
            var item = activeTexts[i];
            float elapsed = Time.time - item.startTime;
            float percent = elapsed / duration;

            if (percent >= 1.0f)
            {
                item.obj.SetActive(false);
                textPool.Enqueue(item.obj);
                activeTexts.RemoveAt(i);
                continue;
            }

            item.obj.transform.position = item.startPos + new Vector3(0, percent * 1.5f, 0);

            Color c = item.textMesh.color;
            c.a = 1.0f - percent;
            item.textMesh.color = c;

            if (Camera.main != null)
                item.obj.transform.rotation = Camera.main.transform.rotation;
        }
    }
    public void updateEnemyList(List<GameObject> enemies)
    {
        foreach (var enemy in enemies)
        {
            // HashSet을 사용해 이미 관리 중인 적은 빠르게 스킵 (O(1))
            if (activeEnemies.Contains(enemy))
                continue;

            var hpBar = assignHpBar();
            if (hpBar == null) continue;

            var enemyComp = enemy.GetComponent<Enemy>();

            // 중복 구독 방지 및 이벤트 연결
            enemyComp.OnEnemyDied -= OnEnemyDied;
            enemyComp.OnEnemyDied += OnEnemyDied;

            activeEnemies.Add(enemy); // 활성 목록에 추가
            hpBar.SetActive(true);
            hpBar.GetComponent<EnemyHpUI>().Initialize(enemy);
        }
    }

    public void ShowDamageText(Vector3 worldPos, int damage)
    {
        if (textPool.Count == 0) return; // 풀이 모자라면 스킵

        GameObject obj = textPool.Dequeue();
        obj.transform.position = worldPos + Vector3.left * 1.7f; // 캐릭터 약간 위
        obj.SetActive(true);

        var tmpro = obj.GetComponent<TextMeshProUGUI>();
        tmpro.text = damage.ToString();

        activeTexts.Add(new DamagedText
        {
            obj = obj,
            textMesh = tmpro,
            startTime = Time.time,
            startPos = obj.transform.position
        });
    }

    private GameObject assignHpBar()
    {
        // 1. 비활성화된 바 찾기
        foreach (var hpBar in hpBars)
        {
            if (!hpBar.activeSelf)
                return hpBar;
        }

        // 2. 만약 풀이 모자라면 새로 생성 (자동 확장)
        GameObject newBar = Instantiate(hpBarPrefab, transform);
        hpBars.Add(newBar);
        return newBar;
    }

    private void OnEnemyDied(Enemy enemy)
    {
        activeEnemies.Remove(enemy.gameObject); // 관리 목록에서 제거

        foreach (var bar in hpBars)
        {
            var ui = bar.GetComponent<EnemyHpUI>();
            if (ui.targetEnemy == enemy.gameObject)
            {
                ui.targetEnemy = null;
                bar.SetActive(false);
                break;
            }
        }
    }
}