using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHandler : MonoBehaviour
{
    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private Image healthBar;
    void Awake()
    {
        if (!healthBar)
        {
            healthBar = transform.Find("Canvas").Find("HealthBarImg").GetComponent<Image>();
            if (!healthBarRect)
            {
                healthBarRect = healthBar.GetComponent<RectTransform>();
            }
        }
    }

    public void LowerHp(int max, int cur)
    {
        if (gameObject.activeSelf)
        {
            float value = (float)cur / max;

            healthBarRect.localScale = new Vector3(value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        }

    }
}
