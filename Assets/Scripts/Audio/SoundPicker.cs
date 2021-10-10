using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPicker : MonoBehaviour
{
    [SerializeField] AudioClip[] GravelSteps = null;
    [SerializeField] AudioClip[] BushSteps = null;


    //Return an audioclip determined by the random index value and ground type passed through
    public AudioClip Walk(GroundEnum.GroundType ground)
    {
        //assign random index between 0 and 9 (Upper bound is excluded)
        int walkIndex = Random.Range(0, 9);
        //Part1: Based on ground value passed through, determine whether to pass gravel or bush steps across
        //Part2: Return audio clip to be played to calling script
        switch (ground)
        {
            case GroundEnum.GroundType.Bush:
                return BushSteps[walkIndex];

            case GroundEnum.GroundType.Gravel:
                return GravelSteps[walkIndex];
        }
        return null;
    }
}