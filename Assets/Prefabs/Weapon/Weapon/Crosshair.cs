using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField][Range(0, 100)] int defaultValue;
    [SerializeField][Range(0, 100)] int targetValue;
    [SerializeField] int speed;
    [SerializeField] int margin;

    int currentDefault; // game manager?

    public RectTransform Top, Bottom, Left, Right, Center;

    // Start is called before the first frame update
    void Start()
    {
        currentDefault = defaultValue; //game manager?
    }

    // Update is called once per frame
    void Update()
    {
        float topValue, bottomValue, leftValue, rightValue;

        topValue    = Mathf.Lerp(Top.position.y, Center.position.y + margin + defaultValue, speed * Time.deltaTime);
        bottomValue = Mathf.Lerp(Bottom.position.y, Center.position.y - margin - defaultValue, speed * Time.deltaTime);
        leftValue   = Mathf.Lerp(Left.position.x, Center.position.x - margin - defaultValue, speed * Time.deltaTime);
        rightValue  = Mathf.Lerp(Right.position.x, Center.position.x + margin + defaultValue, speed * Time.deltaTime);

        Top.position = new Vector2(Top.position.x, topValue);
        Bottom.position = new Vector2(Bottom.position.x, bottomValue);
        Left.position = new Vector2(leftValue, Center.position.y);
        Right.position = new Vector2(rightValue, Center.position.y);

    }

    //Get crosshair value
    public int GetDefaultValue()
    {
        return currentDefault;
    }
    public int GetTargetValue()
    {
        return targetValue;
    }

    //Set crosshair value
    public void SetDefaultValue(int crossValue)
    {
        defaultValue = crossValue;
    }
    public void SetTargetValue(int crossValue)
    {
        targetValue = crossValue;
    }
}
