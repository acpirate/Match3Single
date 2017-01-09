using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimationScript : MonoBehaviour {

    Vector3 moveTarget;
    Vector3 startPosition;

    float t;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime / Constants.MOVEANIMATIONTIME;
        transform.position = Vector3.Lerp(startPosition, moveTarget, t);
        DestinationCheck();
	}

    public void SetMoveTarget(Vector3 inTarget)
    {
        moveTarget = inTarget;
    }

    //unparent the tile and kill the animation
    void DestinationCheck()
    {
        if ((transform.position - moveTarget).sqrMagnitude<.01f) 
        {
            foreach(Transform childTransform in GetComponentsInChildren<Transform>())
            {
                if (childTransform.gameObject.CompareTag("Tile"))
                {
                    childTransform.SetParent(null);
                }
            }
            Destroy(gameObject,.1f);
        }
    }
}
