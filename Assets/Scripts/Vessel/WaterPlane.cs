using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour
{
    [SerializeField]
    private Vector2 MinMaxHeight;
    [SerializeField]
    private Vector2 MinMaxHealth;

    [SerializeField]
    private float Speed = 1;

    // Update is called once per frame
    void Update()
    {

        float t = (PlayerSubHealth.Instance.Health - MinMaxHealth.x)/(MinMaxHealth.y - MinMaxHealth.x);
        t = Mathf.Clamp01(t);
        t = 1- t;
        float targY = Mathf.Lerp(MinMaxHeight.x, MinMaxHeight.y, t);

        Vector3 pos = transform.position;
        pos.y += ((targY - pos.y) * 0.1f) * Time.deltaTime * Speed;

        transform.position = pos;

    }
}