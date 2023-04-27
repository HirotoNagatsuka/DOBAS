using UnityEngine;

public class CanvasUIRotate : MonoBehaviour
{
    void LateUpdate()
    {
        //@ƒJƒƒ‰‚Æ“¯‚¶Œü‚«‚Éİ’è
        transform.rotation = Camera.main.transform.rotation;
    }
}
