using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PartInfo
{
    public List<string> names = new List<string>();
    public int counter;
    public string allNameInOne;

    public PartInfo(Transform partRoot)
    {
        ColletPartNames(partRoot);
    }

    void ColletPartNames(Transform partRoot)
    {
        names.Add(partRoot.name);
        allNameInOne += partRoot.name + "\n";

        for (int i = 0; i < partRoot.childCount; ++i)
        {
            ColletPartNames(partRoot.GetChild(i));
        }
    }
}

public class RadianceManager
{
    public bool mIsPrepaed = false;

    public int[,,] mVisibleGrid;
    public string[,,] mVisibleNames;

    //public PartInfo[] mVisibleAccumulator;

    int mVisibleAccumCount = 0;
    public int[] mVisibleAccumulator = null;

    public RadianceManager()
    {
        mVisibleGrid = new int[256 , 256 , 256];
        mVisibleNames = new string[256 , 256 , 256];
    }

    public void SetupAccumulator(int accumCount)
    {
        mVisibleAccumCount = accumCount;
        mVisibleAccumulator = new int[mVisibleAccumCount];

        Debug.Log("Child count: " + accumCount);
    }

    public void ResetAccumulator()
    {
        mVisibleAccumulator = new int[mVisibleAccumCount];
    }

}
