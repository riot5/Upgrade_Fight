using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float hAxis;
    float vAxis;
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public GameObject grenadeObject;
    public Camera followCamera;

    public int ammo;
    public int coin;
    public int curHealth;
    public int regeneration;
    public int defense;
    public int avoid;
    public int playerDamage;
    public float playerAtkSpeed;
    public int playerCriticalRate;
    public int playerCriticalDamage;
    public int playerAtkRange;
    public int totalDamage;

    public int weaponIndex;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool gDown;
    bool rDown;

    bool sDown1;
    bool sDown2;
    bool sDown3;


    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;
    bool isBorder;
    bool isDamage;
    bool isDead;
    bool isStun;
    bool isWearWeapon = false;

    //재생
    public float spwanTime = 3f;
    public float curTime;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Vector3 randVec;
    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;
    // 피격시스템 




    GameObject nearObject;

    Weapon equipWeapon;
    float fireDelay;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Interation();
        //Swap();
        Warp();
        Regeneration();

    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButton("Fire2");
        rDown = Input.GetButtonDown("Reload");

        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");

    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if (isDodge)
            moveVec = dodgeVec;
        if (isSwap || !isFireReady || isReload || isDead || isStun)
            moveVec = Vector3.zero;
        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);



    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 키보드에 의한 회전
        if (fDown && !isDead && !isStun)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스에 의한 회전
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }

    }
    /*void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isDead)
        {
            rigid.AddForce(Vector3.up * 20 , ForceMode.Impulse );
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }*/
    void Grenade()
    {
        if (hasGrenades == 0)
            return;
        if (gDown && !isReload && !isSwap && !isDead && !isStun)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 마우스에 의한 회전
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;
                GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
            return;



        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isDead && !isStun)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            if (equipWeapon.type == Weapon.Type.Melee)
            {

                anim.SetFloat("AtkSpeed", playerAtkSpeed);
                anim.SetFloat("BulletSpeed", playerAtkSpeed);
            }

            fireDelay = 0;
        }
    }
    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2.5f);
        }

    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isDead)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    /*void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;


        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
       
        

        if ((sDown1 || sDown2 || sDown3 ) && !isJump && !isDodge && !isSwap && !isDead)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut" , 0.4f);
        }
        
    }*/
    /*void SwapOut()
    {
        isSwap = false;
    }*/
    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                weaponIndex = item.value;
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                if (isWearWeapon == false)
                {
                    hasWeapons[weaponIndex] = true;
                    equipWeapon.gameObject.SetActive(true);
                    isWearWeapon = true;
                }
                Destroy(nearObject);
            }

        }

    }
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void Warp()
    {
        if (nearObject != null && !isDead && !isStun)
        {
            if (nearObject.tag == "Reinforce")
            {
                Reinforce reinforce = nearObject.GetComponent<Reinforce>();
                reinforce.Enter(this);

                randVec = new Vector3(Random.Range(-40, 40), 3, Random.Range(-40, 40));
                transform.position = randVec;

               
            }
        }

    }
    void UpgradeWeapon()
    {


        equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
        if (isWearWeapon == false)
        {

            hasWeapons[weaponIndex] = true;
            equipWeapon.gameObject.SetActive(true);
            isWearWeapon = true;

        }


    }
    void Regeneration()
    {
        if (curTime >= spwanTime)
        {
            curHealth = curHealth + regeneration;
            if (maxHealth < curHealth)
                curHealth = maxHealth;
            curTime = 0;

        }
        curTime += Time.deltaTime;
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
                case Item.Type.Heart:
                    curHealth += item.value;
                    if (curHealth > maxHealth)
                        curHealth = maxHealth;
                    break;
            }
            Destroy(other.gameObject);
        }
        //피격시스템 
        else if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            totalDamage = weapon.weaponDamage + playerDamage;
            int a = Random.Range(0, 100);
            if (a + weapon.weaponCriticalRate + playerCriticalRate > 95)
            {
                totalDamage += (weapon.weaponCriticalDamage + playerCriticalDamage);
            }
            if (a + avoid < 5)
            {
                totalDamage = 0;
            }
            if (a > 70)
                isStun = true;
            curHealth -= (totalDamage - defense);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec.normalized, false));
            Debug.Log("Range :" + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            totalDamage = bullet.bulletDamage + playerDamage;
            int a = Random.Range(0, 100);
            int b = Random.Range(0, 10000);
            if (a + bullet.bulletCriticalRate + playerCriticalRate > 95)
            {
                totalDamage += (bullet.bulletCriticalDamage + playerCriticalDamage);
            }
            if (a + avoid < 5)
            {
                totalDamage = 0;
            }
            if (b < 2)
            {
                totalDamage = 1000;

            }
            curHealth -= (totalDamage - defense);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec.normalized, false));
            Debug.Log("Range :" + curHealth);
        }

    }


    // 밑에 두개 피격시스템 
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {


        if (isGrenade)
        {
            yield return new WaitForSeconds(0.5f);
            /*foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;*/                                     //피격시 레드
            yield return new WaitForSeconds(1f);
            reactVec = reactVec.normalized;                       ///  피격시 넉백 시스템
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * -5, ForceMode.Impulse);
            rigid.AddTorque(reactVec * -15, ForceMode.Impulse);

        }
        else
        {
            // foreach (MeshRenderer mesh in meshs)
            //   mesh.material.color = Color.red;                                         //피격시 레드
            if (isStun)
            {
                yield return new WaitForSeconds(5f);
                isStun = false;
            }
            yield return new WaitForSeconds(0.1f);
            reactVec = reactVec.normalized;                       ///  피격시 넉백 시스템
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);           //  요까지

        }
        if (curHealth > 0)
        {
            //foreach (MeshRenderer mesh in meshs)
            {
                //  mesh.material.color = Color.white;   // 살아잇을시 다시 색이 돌아옴.
            }
        }
        else
        {
            // foreach (MeshRenderer mesh in meshs)
            {
                OnDie();
            }
        }

    }
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
        if (other.tag == "Reinforce")
        {
            
            nearObject = other.gameObject;
            wait();
        }
       

    }

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Reinforce")
        {
            if (equipWeapon != null)
            {
                equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
                if (isWearWeapon == true && weaponIndex % 5 != 0)
                {
                    hasWeapons[weaponIndex] = false;
                    equipWeapon.gameObject.SetActive(false);
                    isWearWeapon = false;

                    weaponIndex++;
                    UpgradeWeapon();
                }

                wait();

            }
            Reinforce reinforce = nearObject.GetComponent<Reinforce>();
            nearObject = null;
        }
    }

}
