using UnityEngine;
using System.Collections;

public class WorldLoader : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        World.instance.LoadSurfaceData(Application.streamingAssetsPath + "/tile.dat");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
