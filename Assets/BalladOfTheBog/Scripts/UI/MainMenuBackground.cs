using UnityEngine;
using UnityEngine.UI;

public class MainMenuBackground : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private float x, y;
    //[SerializeField] private RawImage starRect;

    private void Update()
    {

        image.uvRect = new Rect(image.uvRect.position + new Vector2(x, y) * Time.deltaTime, image.uvRect.size );
        //starRect = new Rect(starRect.position + new Vector2(x, y) * Time.deltaTime, starRect.position);
    }

}
