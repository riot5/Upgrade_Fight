using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reinforce : MonoBehaviour
{
    //public RectTransform uiGroup; // ui �׷� ȣ��
    public Animation anim; // ��ȭ ���ϸ��̼� �Ƹ� ���� or ���� 

    Player enterPlayer; // �÷��̾ ��ȭ ���� ���Դ��� �˼��հ� �Լ� ���� 

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
