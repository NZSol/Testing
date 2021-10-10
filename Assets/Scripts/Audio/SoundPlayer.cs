using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] movement movementScript = null;
    [SerializeField] AudioSource playerAudioSource = null;
    [SerializeField] GroundEnum.GroundType terrainType = GroundEnum.GroundType.Gravel;
    [SerializeField] SoundPicker soundSelect = null;

    [HideInInspector]public bool moving = false;

    public float walkTimer, sprintTimer, crouchTimer;

    // Start is called before the first frame update
    void Start()
    {
        playerAudioSource = GetComponent<AudioSource>();
        movementScript = GetComponent<movement>();
        soundSelect = GetComponent<SoundPicker>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down * 5, out hit))
        {
            print("hitting object: " + hit.transform.gameObject.name);
            terrainType = hit.transform.gameObject.GetComponent<GroundEnum>().myGround;
        }
        if(moving)
            playWalkAudio(soundSelect.Walk(terrainType), 0.28f);
        //if (movementScript.MovementInputVals != Vector2.zero)
        //{
        //    moving = true;

        //    float timerVal = 0f;
        //switch (movementScript.move)
        //    {
        //        case movement.moveForm.baseMove:
        //            timerVal = 0.4f;
        //            playerAudioSource.volume = 0.5f;
        //            break;
        //        case movement.moveForm.sprint:
        //            timerVal = 0.28f;
        //            playerAudioSource.volume = 0.9f;
        //            break;
        //        case movement.moveForm.crouch:
        //            timerVal =  0.65f;
        //            playerAudioSource.volume = 0.2f;
        //            break;
        //    }

        //    playWalkAudio(soundSelect.Walk(terrainType), timerVal);
        //}
        //else
        //{
        //    moving = false;
        //}

    }

    float timer = 0;
    void playWalkAudio(AudioClip clip, float timeForPlay)
    {
        timer += Time.deltaTime;
        if (timer > timeForPlay)
        {
            timer = 0;
            playerAudioSource.PlayOneShot(clip);
        }
    }
}
