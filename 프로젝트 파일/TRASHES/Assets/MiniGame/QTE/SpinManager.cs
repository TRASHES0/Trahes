using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    [SerializeField]
    float rotateSpeed; //1�ʴ� ȸ���� �ӵ�

    [SerializeField]
    GameObject text;

    public GameObject MiniGame;

    private float zone_check = 0; // 2 -> perfect, 1 -> Good, 0 -> None;
    bool stop_check = false;
    bool print_check = false;
    float t;


    private void Start()
    {
        MiniGame = GameObject.Find("MiniGame");
    }
    void Update()
    {
        keydown_check();

        if (stop_check == false)
        {
            Spin();
        }
        else if (stop_check == true && print_check == false)
        {
            Check_print();
            print_check = true;
            Invoke("DestroyObj", 0.5f);
        }
    }


    private void OnTriggerEnter2D(Collider2D check)
    {
        if (check.CompareTag("PerfectCheck"))
        {
            zone_check = 2.0f;
        }
        else if (check.CompareTag("GoodCheck"))
        {
            zone_check = 1.0f;
        }
        else if (check.CompareTag("NoneCheck_Left"))
        {
            zone_check = 0.0f;
            stop_check = true;
        }
        else
        {
            zone_check = 0.0f;
        }
    }

    void Check_print()
    {
        if (zone_check > 1.0f)
        {
            GameObject.Find("TextManager").GetComponent<TextManager>().Text_Spawn("Perfect!!!");
            Simple_Char_Move.Bullet++;
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.QteSuccess);
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<PlayerUI>().ShowBullet();
        }
        else if (zone_check < 1.0f)
        {
            GameObject.Find("TextManager").GetComponent<TextManager>().Text_Spawn("Wrong...");
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.QteFail);
        }
        else
        {
            GameObject.Find("TextManager").GetComponent<TextManager>().Text_Spawn("Good!");
            Simple_Char_Move.Bullet++;
            SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.QteSuccess);
            GameObject.FindGameObjectWithTag("MainUI").GetComponent<PlayerUI>().ShowBullet();
        }
    }
    void Spin()
    {
        t -= Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, t * rotateSpeed);
        if (t * rotateSpeed <= -360) stop_check = true; //t���� �ʹ� Ŀ���� �ʰ� ����
    }
    void keydown_check()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            stop_check = true;
        }
    }

    void DestroyObj()
    {
        MiniGame.GetComponent<MiniGameManager>().ExitGame();
    }
}
