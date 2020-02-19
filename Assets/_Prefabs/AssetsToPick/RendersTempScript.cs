using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendersTempScript : MonoBehaviour
{
    public List<Transform> items;
    private int index = 0;
    void Start()
    {
        if (items != null)
        {
            foreach (Transform t in items)
            {
                t.gameObject.SetActive(false);

            }
                }
    }

    // Update is called once per frame
    void Update()
    {
       if( Input.GetKeyDown(KeyCode.N))
            {
            Next();
        }
    }

    void Next()
    {
        items[index].gameObject.SetActive(false);
        index = (index + 1) % items.Count;
        items[index].gameObject.SetActive(true);

    }
}
