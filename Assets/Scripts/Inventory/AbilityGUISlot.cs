using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class AbilityGUISlot : MonoBehaviour {

	public float RightPadding;
    public Image icon;

    private RectTransform rectTransform;
	private CanvasRenderer canvasRenderer;
    private readonly Vector3 minScale = Vector3.one;
    private readonly Vector3 maxScale = new Vector3(1.5f, 1.5f, 1.5f);
    private Timer shrinkTimer;
    private bool active;

    private void Awake()
    {
        shrinkTimer = new Timer(0.5f);
    }

    private void Start()
    {
        active = false;
    }

    private void Update ()
    {
        if (!active) return;

        if (shrinkTimer.Update(Time.deltaTime))
        {
            active = false;
            transform.localScale = minScale;
        }
        else
        {
            transform.localScale = Vector3.Lerp(maxScale, minScale, shrinkTimer.PercentDone);
        }
    }

    public void Use()
    {
        transform.localScale = maxScale;
        active = true;
        shrinkTimer.Reset();
    }
}
