using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforce : MonoBehaviour
{
    //public RectTransform uiGroup; // ui 그룹 호출
    public Animation anim; // 강화 에니메이션 아마 실패 or 성공 

    Player enterPlayer; // 플레이어가 강화 존에 들어왔는지 알수잇게 함수 선언 

    // Start is called before the first frame update
    public void Enter(Player player)
    {
        enterPlayer = player;
        
        //uiGroup.anchoredPosition = Vector3.zero;
    }

   
    // Update is called once per frame
    /*& public void Exit()
     {
         // anim.SetTrigger("doHello");
        // uiGroup.anchoredPosition = Vector3.down * 1000;
     }*/
}
