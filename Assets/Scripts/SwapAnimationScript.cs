using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapAnimationScript : MonoBehaviour {

    float timer;
    public SWAPDIRECTION myDirection { get; set; }

    float startRotation = 0;
    float endRotation = 180;


	// Use this for initialization
	void Start () {
        timer = 0;
        Destroy(gameObject, Constants.SWAPANIMATIONTIME + .5f);
	}
	
    void OnDestroy()
    {
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.CompareTag("Tile")) child.parent = null;
        }
    }

	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (timer < Constants.SWAPANIMATIONTIME) SwapAnimation();
	}

    void SwapAnimation()
    {
        
        float rotateAmount = 180-endRotation*(Constants.SWAPANIMATIONTIME-timer)/Constants.SWAPANIMATIONTIME;
        Vector3 rotationVector = Vector3.zero;

        if (myDirection == SWAPDIRECTION.RIGHT) rotationVector = new Vector3(0, 0, -rotateAmount);
        if (myDirection == SWAPDIRECTION.LEFT) rotationVector = new Vector3(0, 0, +rotateAmount);
        if (myDirection == SWAPDIRECTION.UP) rotationVector = new Vector3(-rotateAmount, 0, 0);
        if (myDirection == SWAPDIRECTION.DOWN) rotationVector = new Vector3(+rotateAmount, 0, 0);

        transform.localEulerAngles = rotationVector;
    }

}
