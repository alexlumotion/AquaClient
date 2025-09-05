using UnityEngine;

public class FishController : MonoBehaviour
{

    public GameObject[] fishs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowFish(int id)
    {
        for (int cnt = 0; cnt < fishs.Length; cnt++)
        {
            fishs[cnt].SetActive(false);
        }
        fishs[id].SetActive(true);
    }

}
