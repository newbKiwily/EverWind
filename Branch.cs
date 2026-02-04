using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : FieldItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        obtainTime = 5.0f;
        id = "Branch";
        isRespawnsible = true;
    }

    protected override IEnumerator Obtaining()
    {
        // 채집 완료까지 대기
        while (obtainTime > 0)
        {
            obtainTime -= Time.deltaTime;

            yield return null;
        }

        Debug.Log("채집이 완료되었습니다!");


        ItemMediator.Instance.mediation(id);

        if (isRespawnsible)
        {
            ItemMediator.Instance.itemRespawn(this.gameObject, 5.0f);
        }
        else
        {
            initialize();
        }
        // 채집 완료 이벤트 호출
        EndRooting();

    }

    public override void initialize()
    {
        obtainTime = 5.0f;
    }
}
