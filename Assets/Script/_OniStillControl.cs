using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _OniStillControl : MonoBehaviour {


    public AudioClip emitAudioClip;     // 坠落音频
    public AudioClip hitAudioClip;      // 碰撞音频
	
	void Start () {

	}
	

	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "OniYama")
        {
            GetComponent<AudioSource>().clip = hitAudioClip;
            GetComponent<AudioSource>().Play();
        }
        else if (collision.gameObject.tag == "Floor")
        {
            Destroy(GetComponent<Rigidbody>());
        }
    }
}
