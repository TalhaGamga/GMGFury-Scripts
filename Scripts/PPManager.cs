using UnityEngine;

public class PPManager : MonoBehaviour
{

    [SerializeField] GameObject postProcess;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            postProcess.SetActive(!postProcess.activeInHierarchy);
        }
    }
}
