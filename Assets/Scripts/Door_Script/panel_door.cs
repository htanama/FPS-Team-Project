using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class panel_door : MonoBehaviour
{
    public float interactionDistance;
    public GameObject inText;
    public string doorOpenAnimName, doorCloseAnimName;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance));
        {
            if(hit.collider.gameObject.tag == "door")
            {
                GameObject doorParent = hit.collider.transform.root.gameObject;
                Animator doorAnim = doorParent.GetComponent<Animator>();
                inText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                    {
                        doorAnim.ResetTrigger("open");
                        doorAnim.SetTrigger("close");
                    }
                    if (doorAnim.GetCurrentAnimatorStateInfo(0).IsName(doorOpenAnimName))
                    {
                        doorAnim.ResetTrigger("open");
                        doorAnim.SetTrigger("close");
                    }
                }
            }
            else
            {
                inText.SetActive(false);
            }
        }
    }
}
