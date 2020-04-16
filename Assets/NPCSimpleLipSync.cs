using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSimpleLipSync : MonoBehaviour
{
    private UMAExpressions expressions;
    private AudioSource npcAudio;

    private float[] fdata;
    private int fmax; //24000

    [Range(64, 8192)]
    public int nSamples = 256;
    [Range(20, 20000)]
    public int flow =300;
    [Range(200, 20000)]
    public int fHigh = 800;

    public float mouthDamper = 0.7f;

    [Header("Smooth Talk")]
    public bool smoothOn = false;
    public int sampleCount=10; // we can play with this values
    private float[] sampleValue;
    private int frameIdx = 0;
    private float jawStart=0.0f;
    private float jawEnd = 0.0f;
    private float tongueStart = 0.0f;
    private float tongueEnd = 0.0f;
    [Range(0, 50)]
    public float speedMouth = 25;

    [Range(0, 50)]
    public float speedTongue = 25;






    //use this for initialization 
    private void Start()
    {
        sampleValue = new float[sampleCount];
        fdata = new float[nSamples];
        expressions = GetComponent<UMAExpressions>();
        npcAudio = GetComponent<AudioSource>();
        npcAudio.Play();

        //
        fmax = AudioSettings.outputSampleRate / 2;

    }
    private void Update()
    {
        //audio range healthy range 0.....0.7
        float value = BandVol();
        Debug.Log("the band value is "+ value);
        float multiplier = 100;
        value = Mathf.Clamp01(value * multiplier);
        float normVal = (value * 2 ) * mouthDamper; //(value * 2 - 1), -1 here results in too large mouth movement
        Debug.Log("the normal value before is " + normVal);
        //smooth out values 
        if (smoothOn)
        {
            //Count Upto xframes
            //-->collect audiodata
            //-->smoothout jaw/tongue movement
            if (frameIdx < sampleCount)
            {
                sampleValue[frameIdx] = normVal;
                float perc = (float)frameIdx / sampleCount;
                normVal = Mathf.Lerp(jawStart, jawEnd, perc);

                Debug.Log("the normal value After is " + normVal);

                //tongue movement
                if (expressions.ready)
                    expressions.expressionPlayer.tongueUp_Down = Mathf.Lerp(tongueStart, tongueEnd, perc);

                frameIdx++;
            }
            //On tenth frame
            //-->average out collect audiodata
            if(frameIdx == sampleCount)
            {
                float sum = 0.0f;
                for(int i =0; i<sampleCount; i++)
                {
                    sum += sampleValue[i];

                }

                float average = sum / sampleCount;
                if (average < 0.1f)
                {
                    average = -0.7f;    
                }

                frameIdx = 0;
                jawStart = jawEnd;
                jawEnd = average;

                tongueStart = tongueEnd;
                tongueEnd = Random.Range(-1.0f, 1.0f);


            }
        }

        Debug.Log(normVal);
        if (expressions.ready)
        {
            //MouthMovement(normVal);
            expressions.expressionPlayer.jawOpen_Close = normVal;
        }
    }


    public float BandVol()
    {
        npcAudio.GetSpectrumData(fdata, 0, FFTWindow.BlackmanHarris);// 0 is for mono channel for stereo can be done later.
        int n1 = Mathf.FloorToInt(flow * nSamples / fmax);//first value
        int n2 = Mathf.FloorToInt(fHigh * nSamples / fmax);//last value

        float sum = 0.0f;

        for(int i= n1; i<n2; i++)
        {
            sum += fdata[1];
        }

        Debug.Log("band Value is "+ sum / (n2 - n1 + 1));
        return sum / (n2 - n1 + 1);

    }
}

