// SpriteAnimator.cs 수정

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Lean.Pool;

public class SpriteAnimator : MonoBehaviour, IPoolable
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] float frameRate = 12f;
    [SerializeField] bool isUI = false;
    [SerializeField] bool loopAnimation = false;
    [SerializeField] float initialDelay = 0.1f;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    private int currentFrame = 0;
    private bool isPlaying = false;
    private bool canPlayAnimation = false;
    private Coroutine animationCoroutine;
    private float spawnTime;

    void Awake()
    {
        if (isUI)
        {
            uiImage = GetComponent<Image>();
            if (uiImage != null)
            {
                defaultSprite = uiImage.sprite;
            }
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                defaultSprite = spriteRenderer.sprite;
            }
        }
    }

    void OnEnable()
    {
        spawnTime = Time.time;
        canPlayAnimation = false;
        StartCoroutine(EnableAnimationAfterDelay());
    }

    IEnumerator EnableAnimationAfterDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        canPlayAnimation = true;
    }

    public void OnSpawn()
    {
        currentFrame = 0;
        isPlaying = false;
        canPlayAnimation = false;
        spawnTime = Time.time;
        ResetToDefaultSprite();
        StartCoroutine(EnableAnimationAfterDelay());
    }

    public void OnDespawn()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isPlaying = false;
        canPlayAnimation = false;
        ResetToDefaultSprite();
    }

    void ResetToDefaultSprite()
    {
        if (defaultSprite != null)
        {
            UpdateSprite(defaultSprite);
        }
    }

    public bool CanPlayAnimation() => canPlayAnimation;

    public void PlayAnimation(System.Action onComplete = null)
    {
        if (isPlaying) return;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimationCoroutine(onComplete));
    }

    IEnumerator AnimationCoroutine(System.Action onComplete)
    {
        isPlaying = true;
        currentFrame = 0;

        float frameDuration = 1f / frameRate;
        WaitForSeconds wait = new WaitForSeconds(frameDuration);

        do
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                currentFrame = i;
                UpdateSprite(sprites[i]);
                yield return wait;
            }
        } while (loopAnimation);

        isPlaying = false;
        onComplete?.Invoke();
    }

    void UpdateSprite(Sprite sprite)
    {
        if (isUI && uiImage != null)
        {
            uiImage.sprite = sprite;
        }
        else if (!isUI && spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isPlaying = false;
        ResetToDefaultSprite();
    }
}