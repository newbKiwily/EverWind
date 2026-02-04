using AYellowpaper.SerializedCollections;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class DisplayUIManager : MonoBehaviour
{

    public enum ProfileState
    {
        Normal,
        Hit,
        Success,
        Hurt
    }

    [SerializeField]
    private CombatManager combatManager;
    [SerializeField]
    private Player player;

    public Image[] skillSlot;
    public Image hpBar;

    public Image profileImage;

    private float temp_hp_ratio;

    [SerializeField]
    private SerializedDictionary<ProfileState, Sprite> profileTable;

    [SerializeField]
    private Camera minimapCamera;

    public static DisplayUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        if (PlayerLoader.Instance != null)
        {
            PlayerLoader.Instance.OnLocalPlayerSpawned += BindToPlayer;

            // 이미 생성돼있으면 바로 바인딩
            if (PlayerLoader.Instance.InstancedPlayer != null)
                BindToPlayer(PlayerLoader.Instance.InstancedPlayer);
        }
    }
    private void Start()
    {
        profileImage.sprite = profileTable[ProfileState.Normal];
        temp_hp_ratio = 1.0f;
    }
    private void OnDisable()
    {
        if (PlayerLoader.Instance != null)
            PlayerLoader.Instance.OnLocalPlayerSpawned -= BindToPlayer;

        // 이벤트 해제(중복 호출/메모리 누수 방지)
        if (player != null)
            player.OnTakeDamage -= UpdateHpBar;
    }

    private void BindToPlayer(GameObject playerObj)
    {
        if (playerObj == null) return;

        // 이전 바인딩 해제
        if (player != null)
            player.OnTakeDamage -= UpdateHpBar;

        player = playerObj.GetComponent<Player>();
        combatManager = playerObj.GetComponent<CombatManager>();

        if (player == null || combatManager == null)
        {
            Debug.LogError("[DisplayUIManager] Player/CombatManager component missing on player object.");
            return;
        }

        player.OnTakeDamage += UpdateHpBar;

    }

    private void Update()
    {
        if (combatManager == null) return;

        for (int i = 0; i < skillSlot.Length; i++)
        {
            float remain = combatManager.skillCooldownRemain[i];
            float max = combatManager.skillCooldownMax[i];

            if (max > 0f)
            {
                float progress = (max - remain) / max;
                progress = Mathf.Clamp01(progress);
                skillSlot[i].fillAmount = progress;
            }
        }
    }

    private void LateUpdate()
    {
        if (minimapCamera == null || player == null)
            return;

        var playerPos = player.transform.position;

 
        var cameraPos = minimapCamera.transform.position;

        minimapCamera.transform.position = new Vector3(playerPos.x, cameraPos.y, playerPos.z);
    }
    private void UpdateHpBar(float hp)
    {
        float ratio = hp / player.statManager.get_max_hp();
        temp_hp_ratio = ratio;
        Vector3 scale = hpBar.transform.localScale;
        scale.x = ratio;
        hpBar.transform.localScale = scale;
    }


    public void ChangeProfile(ProfileState state, float time)
    {
        StartCoroutine(ChangeProfileCoroutine(state,time));
    }

    IEnumerator ChangeProfileCoroutine(ProfileState state, float time)
    {
        profileImage.sprite = profileTable[state];
    
        yield return new WaitForSeconds(time);


        if (temp_hp_ratio <= 0.3f)
            profileImage.sprite = profileTable[ProfileState.Hurt];
        else
            profileImage.sprite = profileTable[ProfileState.Normal];

    }
}
