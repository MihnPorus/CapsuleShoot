using UnityEngine;
using System.Collections;

public class MuzzleFlash : MonoBehaviour {

    public GameObject flashHover;
    public Sprite[] flashSprite;
    public SpriteRenderer[] spriteRenderers;

    public float flashTime;
    
    void Start()
    {
        Deactivate();
    }

	public void Activate()
    {
        flashHover.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprite.Length);
        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprite[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    void Deactivate()
    {
        flashHover.SetActive(false);
    }
}
