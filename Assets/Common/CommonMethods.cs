using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerMovement;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using System;

public static class CommonMethods
{
    public static bool GetQuadrantsBooleanFromTwoPoints(float point, float firstAnchor, float secondAnchor)
    {
        float firstMergeAnchor = (firstAnchor + secondAnchor) / 2f;
        float secondMergeAnchor = firstMergeAnchor + 0.5f;

        bool returnBool;

        if(secondMergeAnchor < 1)
        {
            if((point >= firstAnchor && point < firstMergeAnchor) || (point >= secondAnchor && point < secondMergeAnchor))
            {
                returnBool = true;
            }
            else
            {
                returnBool = false;
            }
        }
        else
        {
            if( (point >= firstAnchor && point < firstMergeAnchor) || (point >= secondAnchor || point < (secondMergeAnchor % 1)))
            {
                returnBool = true;
            }
            else
            {
                returnBool = false;
            }
        }

        return returnBool;
    }

    public static void AudioPlayOneShot(AudioSource audioSource, AudioClip audioClip, float volume)
    {
        audioSource.PlayOneShot(audioClip, volume);

        if(TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordSoundData(audioSource, audioClip, volume, 0f, RecordSoundDataType.PlayOneShot);
            }
        }
    }
    public static void AudioPlay(AudioSource audioSource, AudioClip audioClip, float volume, float startTime)
    {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.time = startTime;
        audioSource.Play();

        if(TestScene_ReplayManager.instance != null)
        {
            if (TestScene_ReplayManager.instance.isRecording)
            {
                TestScene_ReplayManager.instance.RecordSoundData(audioSource, audioClip, volume, startTime, RecordSoundDataType.Play);
            }
        }
    }


    public static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = (lineEnd - lineStart).normalized;
        float lineLength = Vector3.Distance(lineStart, lineEnd);

        float projectLength = Vector3.Dot(point - lineStart, lineDirection); // 증명은 수리증명노트 참조 (현재 완전히 못했지만..)

        projectLength = Mathf.Clamp(projectLength, 0, lineLength); // 피격시, 투영된 벡터 외부에 closestPoint가 할당될 수 있기 때문에 Clamp로 상하한선 조정

        return lineStart + lineDirection * projectLength;

    }

    public static LayerMask GetStringsToLayerMask(List<String> listOfStrings)
    {
        LayerMask combinedLayerMask = 0;

        foreach (String string1 in listOfStrings)
        {
            int layer = LayerMask.NameToLayer(string1);
            if (layer != -1)
            {
                combinedLayerMask |= (1 << layer);
            }
        }

        return combinedLayerMask;
    }
}
