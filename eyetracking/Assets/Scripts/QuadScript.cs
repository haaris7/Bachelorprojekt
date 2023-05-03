using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScript : MonoBehaviour
{
    Material mMaterial;
    MeshRenderer mMeshRenderer;

    float[] mPoints;
    float[] mFixationDurations;

    int mHitCount;

    float mDelay;

    void Start()
    {

        mMeshRenderer = GetComponent<MeshRenderer>();
        mMaterial = mMeshRenderer.material;

        mPoints = new float[32 * 3]; //32 point 
        mFixationDurations = new float[32];


    }


    public void addHitPoint(float xp, float yp, float fixationDuration)
    {
        mPoints[mHitCount * 3] = xp;
        mPoints[mHitCount * 3 + 1] = yp;
        mPoints[mHitCount * 3 + 2] = Random.Range(1f, 3f);
        mFixationDurations[mHitCount] = fixationDuration;

        mHitCount++;
        mHitCount %= 32;

        mMaterial.SetFloatArray("_Hits", mPoints);
        mMaterial.SetFloatArray("_FixationDurations", mFixationDurations);
        mMaterial.SetInt("_HitCount", mHitCount);
    }


}