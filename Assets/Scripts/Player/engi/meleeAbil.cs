using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Melee Ability", menuName = "Abilities/Melee")]
public class meleeAbil : offensiveMelee
{
    public float range = 1f;
    public float animationSpeed = 1f;
    public int swingCount = 2;
    public float swingDelay = 0.2f;

    public Vector2 upOffset = new Vector2(0, 0.5f);
    public Vector2 downOffset = new Vector2(0, -0.5f);
    public Vector2 leftOffset = new Vector2(-0.5f, 0);
    public Vector2 rightOffset = new Vector2(0.5f, 0);

    public bool isSwinging = false;
    private bool isInitialized = false;
    private WooshPool wooshPool;
    public int stunDamageMultiplier;


    private Vector2 aimDir;
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        isInitialized = false;
    }

    public void Initialize(GameObject user)
    {
        if (isInitialized == true) return;

        OwnerPlayer = user;
        wooshPool = GameObject.FindAnyObjectByType<WooshPool>();

        if (wooshPool != null)
        {
            wooshPool.InitializePool(this);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        isInitialized = true;
    }

    protected override void ExecuteAbility(GameObject user, Vector2 _)
    {
        manualSoundControl = true;
        TriggerCooldown();

        if (isInitialized == false)
        {
            Initialize(user);
        }

        Animator playerAnim = user.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetFloat("AttackSpeed", animationSpeed);
            playerAnim.SetTrigger("Attack");
        }

        if (isSwinging == false)
        {
            MonoBehaviour mb = user.GetComponent<MonoBehaviour>();
            mb.StartCoroutine(SwingCoroutine(user));
        }
    }

    IEnumerator SwingCoroutine(GameObject user)
    {
        isSwinging = true;
        float actualDelay = swingDelay / animationSpeed;

        for (int i = 0; i < swingCount; i++)
        {
            PlayerAim aimingScript = GameObject.FindAnyObjectByType<PlayerAim>();

            if (aimingScript != null)
            {
                aimDir = aimingScript.GetAimDir();
            }

            SpawnWoosh(user, aimDir);
            SpawnWoosh(user, aimDir);
            PlaySound(user.transform.position);

            yield return new WaitForSeconds(actualDelay);
        }

        isSwinging = false;
    }

    void SpawnWoosh(GameObject user, Vector2 aimDir)
    {
        if (wooshPool == null)
        {
            wooshPool = GameObject.FindAnyObjectByType<WooshPool>();
        }

        GameObject woosh = wooshPool.GetWoosh();

        Vector2 snappedDir = SnapToCardinal(aimDir);
        Vector2 offset = GetDirectionOffset(snappedDir);

        woosh.transform.position = (Vector2)user.transform.position + offset;

        wooshScript ws = woosh.GetComponent<wooshScript>();
        if (ws != null)
        {
            ws.ownerAbility = this;
            ws.owner = user;
            ws.range = range;
            ws.offset = offset;
            ws.SetDirection(snappedDir);
        }

        Animator wooshAnim = woosh.GetComponent<Animator>();
        if (wooshAnim != null)
        {
            wooshAnim.SetFloat("DirX", snappedDir.x);
            wooshAnim.SetFloat("DirY", snappedDir.y);
            wooshAnim.SetTrigger("PlayWoosh");
        }

        woosh.SetActive(true);
    }

    Vector2 SnapToCardinal(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return new Vector2(Mathf.Sign(dir.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(dir.y));
        }
    }

    Vector2 GetDirectionOffset(Vector2 dir)
    {
        if (dir.x > 0) return rightOffset;
        if (dir.x < 0) return leftOffset;
        if (dir.y > 0) return upOffset;
        if (dir.y < 0) return downOffset;
        return Vector2.zero;
    }

    public void ReturnWoosh(GameObject woosh)
    {
        if (wooshPool != null)
        {
            wooshPool.ReturnWoosh(woosh);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isSwinging = false;
    }
}