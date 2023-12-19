using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events; //애니매이션 구현
using UnityStandardAssets.Utility;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class Simple_Char_Move : MonoBehaviourPunCallbacks
{
    public SPUM_Prefabs _prefabs;

    [SerializeField]
    private Rigidbody2D rigidbody2d;
    private PhotonView pv;
    private SpriteRenderer characterSprite;

    public Transform Bullet_Pos;
    public GameObject bulletPrefab;
    private Animator _ani;
    private string temp_item_name;
    private int Player_Assigment;
    private string _currentState;
    private string _lastAttack;
    
    //public int ran;
    private PlayerUI _playerUI;
    //stat 관련
    private float speed = 6.0f;
    private int MAX_HP;
    private int HP = 3;
    private int MAX_Bullet = 5;
    public static int Bullet;
    private float timer = 240.0f;
    private float firelate = 1;
    private float firedelay;
    private float _fireLateCheck;
    private float _regenerativeTime = 30f;
    
    public Skill_base Skill2;
    public Stat_base stat;

    // bool 관련
    private bool isText = false;
    private bool isItem = false;
    public bool isGameDone = false;
    private bool _isDie = false;
    private bool isExit = false;
    private bool gameDoneCheck = true;
    public static bool hasKey = false; // 열쇠 조작 관련
    public static bool hasLock = false;
    public static bool isDoingQTE = false;
    public static bool LockBox = false;
    public static bool KeyBox = false;
    public bool inDoor = false;
    public bool isHit = false;
    public bool hasClass = false;
    public bool isUI = false;
    public bool isChatting = false;
    private bool isSkill1Delay;
    private bool isSkill2Delay;
    private bool isSkill2On = false;
    private bool _hasExitKey = false;
    private bool canMove = true;

    public TextMeshProUGUI interaction_text;
    public TextMeshProUGUI cant_interaction_text;

    GameObject nearObject;
    TextMeshProUGUI Text;

    public GameObject MiniGame;
    private void Awake()
    {
        characterSprite = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();
        Player_Assigment = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        _ani = GetComponentInChildren<Animator>();
        //직업에 따라 스탯 설정

        set_initStat();

        if (_prefabs == null)
        {
            _prefabs = transform.GetComponent<SPUM_Prefabs>();
        }
    //_stateChanged.AddListener(PlayStateAnimation);
    }
    private void Update()
    {
        if ((HP <= 0 || timer <= 0) && !isGameDone) // check death
        {
            if (timer <= 0)
            {
                _lastAttack = "폭탄목걸이";
                SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Boom);
            }
            timer = 0;
            isGameDone = true;
            _isDie = true;
        }

        if (!isGameDone && _playerUI != null)
        {
            timer_count();
            Regenerative_HP();
            if (canMove)
            {
                Skill();
                Shot();
            }
        }
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        MiniGame = GameObject.Find("MiniGame");
        GameObject playerUI = GameObject.Find("InGame UI");
        
        if (playerUI != null)
        {
            _playerUI = playerUI.GetComponent<PlayerUI>();
            _playerUI.init_UI();
        }
        if (pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = transform.Find("CamPivot").transform;
        }
        else
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
        }

    }

    // Update is called once per frame
    //update를 fixedupdate로 바꾸니 바로 해결되네;; 뭐지
    void FixedUpdate()
    {
        if (pv.IsMine && canMove && !isHit && !isGameDone)
        {
            if(!isDoingQTE)
                MoveCharacter();
            Interation();
            FlipSpriteToMovement();
        }
        
        if(isGameDone)
        {
            if (pv.IsMine && gameDoneCheck)
            {
                gameDoneCheck = false;
                SetIdle();
                SoundManager.instance.healthSound.Stop();
                if (_isDie)
                {
                    Debug.Log("isExit : "+isExit);
                    if (isExit)
                    {
                        _lastAttack = "누군가 탈출함";
                        SetDie();
                    }
                    else
                    {
                        SetDie();
                    }
                }
                else
                {
                    if (isExit)
                    {
                        SetWin();
                        _playerUI.NoticeResult(PlayerUI.Result.Exit);   
                    }
                    else
                    {
                        SetWin();
                        _playerUI.NoticeResult(PlayerUI.Result.Win);   
                    }
                }
            }
            
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // 아이템 트리거 시
        if (other.gameObject.CompareTag("Item") && !isText && !hasClass && pv.IsMine)
        {
            Text = Instantiate(interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = other.GetComponent<ItemPickUp>().item.itemName;
            isText = true;
            isItem = true;
        }
        else if (other.gameObject.CompareTag("Item") && !isText && hasClass && pv.IsMine)
        {
            Text = Instantiate(cant_interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = other.GetComponent<ItemPickUp>().item.itemName;
            isText = true;
            isItem = true;
        }
        
        // bullet box 트리거 시
        if (other.gameObject.CompareTag("BulletBox") && !isText && !isDoingQTE&& pv.IsMine)
        {
            Text = Instantiate(interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "BulletBox";
            isText = true;
        }
        else if (other.gameObject.CompareTag("BulletBox") && !isText && isDoingQTE && pv.IsMine)
        {
            Text = Instantiate(cant_interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "Bullet";
            isText = true;
        }
        
        // trapdoor 트리거 시
        if (other.gameObject.CompareTag("Trapdoor") && !isText && pv.IsMine)
        {
            Text = Instantiate(interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "Trapdoor";
            nearObject = other.gameObject;
            isText = true;
        }
        else if (other.gameObject.CompareTag("Trapdoor") && !isText && !_hasExitKey && pv.IsMine)
        {
            Text = Instantiate(cant_interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "Trapdoor";
            isText = true;
        }
        
        // trapdoor 트리거 시
        if (other.gameObject.CompareTag("ExitKey") && !isText && pv.IsMine)
        {
            Text = Instantiate(interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "ExitKey";
            nearObject = other.gameObject;
            isText = true;
        }
        else if (other.gameObject.CompareTag("ExitKey") && !isText && pv.IsMine)
        {
            Text = Instantiate(cant_interaction_text, new Vector3(960, 500, 0), Quaternion.identity, GameObject.Find("Canvas").transform);
            temp_item_name = "ExitKey";
            isText = true;
        }

    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && pv.IsMine)
        {
            Destroy(GameObject.FindWithTag("Text"));
            temp_item_name = "";
            isText = false;
            isItem = false;
        }
        if (other.gameObject.CompareTag("BulletBox") && pv.IsMine)
        {
            Destroy(GameObject.FindWithTag("Text"));
            temp_item_name = "";
            isText = false;
        }
        if (other.gameObject.CompareTag("Trapdoor") && pv.IsMine)
        {
            Destroy(GameObject.FindWithTag("Text"));
            temp_item_name = "";
            nearObject = null;
            isText = false;
        }
        if (other.gameObject.CompareTag("ExitKey") && pv.IsMine)
        {
            Destroy(GameObject.FindWithTag("Text"));
            temp_item_name = "";
            isText = false;
        }
    }
    void MoveCharacter()
    {
        if (!isChatting)
        {
            float diagonalFactor = 0.875f; // 대각선 이동 속도 감소 비율
            // 키보드 입력을 받음
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            // 입력한 값을 Vector2로 변환
            Vector2 direction = new Vector2(x, y).normalized;

            // 대각선 이동 속도 조절
            float factor = (Mathf.Abs(x) + Mathf.Abs(y) > 1f) ? diagonalFactor : 1f;

            // 세로 축 속도 감소
            if (direction.y != 0)
            {
                direction.y *= 0.6f;
            }

            // 이동 처리
            Vector2 velocity = direction * (speed * factor * Time.fixedDeltaTime);
            rigidbody2d.MovePosition(rigidbody2d.position + velocity);
            
            //
            // 애니매이션 구현
            if ((x != 0 || y != 0) && _currentState != "Run")
            {
                SetRun();
            }
            if (x == 0 && y == 0 && _currentState != "Idle")
            {
                SetIdle();
            }

        }
    }

    //if the player moves left, flip the sprite, if he moves right, flip it back, stay if no input is made
    void FlipSpriteToMovement()
    {
        if (characterSprite != null && !isChatting)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                _ani.SetFloat("Flip", 0);
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                _ani.SetFloat("Flip", 1);
            }
        }
    }
    void Interation()
    {
        if (isText == true && isDoingQTE == false && temp_item_name == "BulletBox")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isDoingQTE = true;
                SetIdle();
                SoundManager.instance.ContinuousSoundPlay((int)SoundManager.EffectType.SearchBox);
                MiniGame.GetComponent<MiniGameManager>().ActiveGame();

                LockBox = true;
                isText = false;

                Destroy(GameObject.FindWithTag("Text"));
            }

        }
        if (isText && hasClass == false && isItem)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                GameObject.Find("P_Class_info_Manager").GetComponent<Player_Class_Info>().p_class = GameObject.Find(temp_item_name).GetComponent<ItemPickUp>().item.class_type;
                Destroy(Text);
                isText = false;
                hasClass = true;
                photonView.RPC("DestroyItem", RpcTarget.All, temp_item_name);
                temp_item_name = "";
            }
        }
        if (isText && _hasExitKey && temp_item_name == "Trapdoor")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isGameDone = true;
                nearObject.GetComponent<TrapDoor>().Exit(photonView.OwnerActorNr);
                
                isText = false;
                temp_item_name = "";
            }
        }
        if (isText && temp_item_name == "ExitKey")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _hasExitKey = true;
                
                isText = false;
                //photonView.RPC("DestroyItem", RpcTarget.All, temp_item_name);
                temp_item_name = "";
                SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.GetKey);
            }
        }
    }
    void Skill()
    {
        //스킬 1 - 유체화
        if (Input.GetKeyDown(KeyCode.Q) && pv.IsMine)
        {
            if (isSkill1Delay == false)
            {
                isSkill1Delay = true;
                Debug.Log("유체화 사용");
                SoundManager.instance.SkillSoundPlay(stat.ghost);
                StartCoroutine(Ghost());
            }
            else
            {
                Debug.Log("유체화 쿨타임");
            }
        }
        // 스킬 2
        if (Input.GetKeyDown(KeyCode.E) && pv.IsMine)
        {
            Debug.Log("Skill2 실행 : " + Skill2.skill_name);

            if (isSkill2Delay == false)
            {
                if (Skill2.characterName == "Mutant")
                {
                    isSkill2Delay = true;
                    SoundManager.instance.SkillSoundPlay(stat.skill2Sound[0]);
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(Steroid());
                }
                else if (Skill2.characterName == "Collector")
                {
                    isSkill2Delay = true;
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(Find_Bullet());
                }
                else if (Skill2.characterName == "Agent" &&  _playerUI.nightCheck) // PlayerUI는 있는거 같은데 왜 안될까? 생성 우선순위의 차이 때문에 에러?
                {
                    isSkill2Delay = true;
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(Night_War());
                }
                else if (Skill2.characterName == "Juggernaut")
                {
                    isSkill2Delay = true;
                    SoundManager.instance.SkillSoundPlay(stat.skill2Sound[0]);
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(Body_Armor());
                }
                else if (Skill2.characterName == "DualShot")
                {
                    isSkill2Delay = true;
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(RapidShot());
                }
                else if (Skill2.characterName == "Sniper")
                {
                    isSkill2Delay = true;
                    //SoundManager.instance.SkillSoundPlay(stat.skill2Sound[0]);
                    Debug.Log(Skill2.skill_name + " 사용");
                    StartCoroutine(FinalBlow());
                }
            }
            else
            {
                Debug.Log(Skill2.skill_name + " 쿨타임");
            }
        }
        // 테스트용 스페이스바 커맨드
        if (pv.IsMine && Input.GetKeyDown(KeyCode.K))
        {
            //Get_Bullet();
            Set_HP(1, 0.3f);
            //Set_Timer(10);
        }
    }
    void Shot()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > _fireLateCheck && Bullet > 0 && pv.IsMine)
        {
            if (Skill2.characterName == "Sniper")
            {
                Debug.Log("hi");
                StartCoroutine(DelayShot(firedelay));
            }
            else if (Skill2.characterName == "DualShot")
            {
                StartCoroutine(DoubleShot());
            }
            else
            {
                Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float angle = Mathf.Atan2(mousepos.y - Bullet_Pos.position.y, mousepos.x - Bullet_Pos.position.x) * Mathf.Rad2Deg / 2;
                _fireLateCheck = Time.time + firelate;
                Bullet--;
                _playerUI.ShowBullet();
                pv.RPC(nameof(CreateBullet), RpcTarget.All,Bullet_Pos.position, angle);
                SetIdle();
                SetAttack(0);
            }
        }
    }
    
    
    IEnumerator DelayShot(float delay)
    {
        _fireLateCheck = Time.time + firelate;
        canMove = false;
        SetIdle();
        SetAttack(0.3f);
        yield return new WaitForSeconds(delay);
        canMove = true;
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousepos.y - Bullet_Pos.position.y, mousepos.x - Bullet_Pos.position.x) * Mathf.Rad2Deg / 2;
        Bullet--;
        _playerUI.ShowBullet();
            
        pv.RPC(nameof(CreateBullet), RpcTarget.All,Bullet_Pos.position, angle);
    }
    
    IEnumerator DoubleShot()
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousepos.y - Bullet_Pos.position.y, mousepos.x - Bullet_Pos.position.x) * Mathf.Rad2Deg / 2;
        _fireLateCheck = Time.time + firelate;
        
        Bullet--;
        _playerUI.ShowBullet();
        pv.RPC(nameof(CreateBullet), RpcTarget.All,Bullet_Pos.position, angle);
        SetIdle();
        SetAttack(0);

        if (Bullet > 0)
        {
            Transform temp = transform.Find("BulletPosition2").transform;
            yield return new WaitForSeconds(0.3f);
            Vector2 mousepos1 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle1 = Mathf.Atan2(mousepos1.y - temp.position.y, mousepos1.x - temp.position.x) * Mathf.Rad2Deg / 2;
            Bullet--;
            _playerUI.ShowBullet();
            pv.RPC(nameof(CreateBullet), RpcTarget.All,temp.position, angle1);
            SetIdle();
            SetAttack(0);
        }
    }
    
    //스킬 함수
    IEnumerator Ghost()
    {
        speed += 1.5f;
        yield return new WaitForSeconds(3.0f);
        speed -= 1.5f;
        StartCoroutine(_playerUI.Skill_CoolTime(10, 1));
        yield return new WaitForSeconds(9.0f);
        isSkill1Delay = false;
    }

    IEnumerator Steroid()
    {
        // 스킬 부분
        speed += 5f;
        isSkill2On = true;
        yield return new WaitForSeconds(5.0f);
        isSkill2On = false;
        speed -= 5f;
        //스킬 쿨타임 + ui 쿨타임 변경 부분
        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }

    IEnumerator Find_Bullet()
    {

        if (Random.Range(0, 2) == 1 && Bullet < MAX_Bullet)
        {
            Get_Bullet();
            _playerUI.SkillFeedBack(true);
            SoundManager.instance.SkillSoundPlay(stat.skill2Sound[0]);
        }
        else
        {
            _playerUI.SkillFeedBack(false);
            SoundManager.instance.SkillSoundPlay(stat.skill2Sound[1]);
        }
        
        //스킬 쿨타임 + ui 쿨타임 변경 부분
        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }

    IEnumerator Night_War()
    {
        speed += 3f;
        isSkill2On = true;
        yield return new WaitForSeconds(Skill2.cool);
        speed -= 3f;
        isSkill2On = false;

        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }

    IEnumerator Body_Armor()
    {
        isSkill2On = true;
        yield return new WaitForSeconds(3.0f);
        SoundManager.instance.SkillSoundPlay(stat.skill2Sound[1]);
        isSkill2On = false;

        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }

    IEnumerator IceBullet()
    {
        speed -= 3f;
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.IceBullet);
        Set_HP(1, 0.3f); 
        Set_Timer(10);
        yield return new WaitForSeconds(3.0f);
        speed += 3f;
    }

    IEnumerator RapidShot()
    {
        firelate = 0.5f;
        SoundManager.instance.SkillSoundPlay(stat.skill2Sound[0]);
        yield return new WaitForSeconds(3.0f);
        firelate = 1f;
        
        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }

    IEnumerator FinalBlow()
    {
        canMove = false;
        SetIdle();
        SetAttack(0.15f);
        yield return new WaitForSeconds(2f);
        canMove = true;
        
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousepos.y - Bullet_Pos.position.y, mousepos.x - Bullet_Pos.position.x) * Mathf.Rad2Deg / 2;

        pv.RPC(nameof(CreateBulletSkill), RpcTarget.AllViaServer,Bullet_Pos.position, angle);
        //transform.rotation = Quaternion.Euler(temp);
        
        StartCoroutine(_playerUI.Skill_CoolTime(Skill2.cool, 2));
        yield return new WaitForSeconds(Skill2.cool);
        isSkill2Delay = false;
    }
    
    //피격
    public void Attacked(String attackedNickname, String attackedBullet)
    {
        if (pv.IsMine)
        {
            Debug.Log("On hit" + attackedNickname);
            _lastAttack = attackedNickname;
            float stunTime = 0.3f;

            if (_lastAttack == "박쥐")
                stunTime = 5f;
            
            if (attackedBullet == "iceBullet")
            {
                Debug.Log("ice");
                StartCoroutine(IceBullet());
                return;
            }
            else if (attackedBullet == "SniperBullet")
            {
                Set_HP(2, stunTime); 
                Set_Timer(10);
                return;
            }
            else if (attackedBullet == "SniperSkillBullet")
            {
                Set_HP(3, stunTime); 
                Set_Timer(10);
                return;
            }
            
            if (isSkill2On)
            {
                if (Skill2.characterName == "Juggernaut")
                {
                    Debug.Log("무적입니다.");
                } 
                else if (Skill2.characterName == "Mutant") 
                { 
                    Set_HP(2, stunTime); 
                    Set_Timer(10);
                }
                else
                {
                    Set_HP(1, stunTime);
                    Set_Timer(10);
                }
            }
            else
            { 
                Set_HP(1, stunTime);
                Set_Timer(10);
            }
            
        }
    }
    //스탯 초기화
    public void set_initStat()
    {
        HP = stat.MAX_HP_set;
        MAX_HP = stat.MAX_HP_set;
        MAX_Bullet = stat.Max_Bullet_set;
        firelate = stat.firelate_set;
        firedelay = stat.firedelay_set;
        speed = stat.speed_set;
        Bullet = 5;
        _regenerativeTime = stat.regenerativeTime;
    }
    public int Get_HP()
    {
        return HP;
    }
    public float Get_Timer()
    {
        return timer;
    }
    public void Get_Bullet()
    {
        Bullet++;
        _playerUI.ShowBullet();
    }
    public int Get_BulletText()
    {
        return Bullet;
    }
    public void Set_HP(int Damage, float stunTime)
    {
        if (HP > 0)
        {
            if (HP - Damage < 0)
            {
                HP = 0;
                _playerUI.ShowHP(HP, Damage);
                StartCoroutine(hit_anim(stunTime));

            }
            else
            {
                HP = HP - Damage;
                _playerUI.ShowHP(HP, Damage);
                StartCoroutine(hit_anim(stunTime));
            }
            
            if (HP == 1)
            {
                SoundManager.instance.healthSound.Play();
            }
            
            if (_lastAttack == "박쥐")
            {
                SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.EnemyHit);
            }
            else
            {
                SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Hit);
            }
        }
    }
    public void Set_Timer(float Damage)
    {
        if (Damage == 0)
            timer = 0;
        
        if (timer > 0)
            timer = timer - Damage;
    }
    public int Get_PA()
    {
        return Player_Assigment;
    }
    void timer_count()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    void Regenerative_HP()
    {
        if (_regenerativeTime > 0 && HP < MAX_HP)
        {
            _regenerativeTime -= Time.deltaTime;   
        }
        else if(_regenerativeTime <= 0 && HP < MAX_HP)
        { 
            Debug.Log("자연치유");
            HP++;
            if (SoundManager.instance.healthSound.isPlaying)
            {
                SoundManager.instance.healthSound.Stop();
            }
            _playerUI.ShowHP(HP, -1);
            _regenerativeTime = stat.regenerativeTime;
        }
    }

    public void SetIdle()
    {
        _currentState = "Idle";
        _ani.SetBool("isRun", false);
        SoundManager.instance.walkSound.Stop();
    }
    
    private void SetRun()
    {
        _currentState = "Run";
        _ani.SetBool("isRun", true);
        SoundManager.instance.walkSound.loop = true;
        SoundManager.instance.walkSound.Play();
    }
    
    private void SetWin()
    {
        _ani.SetBool("isRun", false);
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Win);
        SoundManager.instance.bgSound.Stop();
    }
    
    private void SetDie()
    {
        _ani.SetBool("isDie", true); 
        SoundManager.instance.bgSound.Stop();
        SoundManager.instance.EffectSoundPlay((int)SoundManager.EffectType.Die);
        if (isExit)
        {
            _playerUI.NoticeResult(PlayerUI.Result.OtherExit);
        }
        else
        {
            _playerUI.NoticeResult(PlayerUI.Result.Dead);
        }
        GameObject.FindWithTag("PlayerCount").GetComponent<PlayerCountManager>().WhoDie(pv.OwnerActorNr, pv.Owner.NickName, _lastAttack);
    }

    private void SetAttack(float attackSpeed)
    {
        if(attackSpeed != 0)
            _ani.SetFloat("attackSpeed", attackSpeed);
        _ani.SetTrigger("isAttack");
    }
    
    public void Set_Exit(bool exited)
    {
        isExit = exited;
    }

    IEnumerator hit_anim(float stunTime)
    {
        isHit = true;
        _ani.SetBool("isHit", true);
        yield return new WaitForSeconds(stunTime);
        isHit = false;
        _ani.SetBool("isHit", false);
    }
    [PunRPC]
    public void CreateBullet(Vector3 bulletPosition, float angle, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        bulletPosition.z = 0;
        
        GameObject bullet = Instantiate(bulletPrefab, bulletPosition,Quaternion.Euler(0, 0, angle));
        bullet.GetComponent<Bullet_Test>().InitBullet(pv.OwnerActorNr, pv.Owner.NickName, Mathf.Abs(lag));
    }
    
    [PunRPC]
    public void CreateBulletSkill(Vector3 bulletPosition, float angle, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        bulletPosition.z = 0;
        
        GameObject bullet = Instantiate(Skill2.Prefab, bulletPosition,Quaternion.Euler(0, 0, angle));
        bullet.GetComponent<Bullet_Test>().InitBullet(pv.OwnerActorNr, pv.Owner.NickName, Mathf.Abs(lag));
    }

    [PunRPC]
    void DestroyItem(string item)
    {
        Destroy(GameObject.Find(item));
    }
    
}
