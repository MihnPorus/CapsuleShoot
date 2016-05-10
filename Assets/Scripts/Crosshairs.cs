using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour {

    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighLightColour;
    Color originalDotColour;

    void Start()
    {
        Cursor.visible = false;
        originalDotColour = dot.color;
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
	}

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighLightColour;
        }
        else
        {
            dot.color = originalDotColour;
        }
    }
}
