using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace WJF_CodeLibrary.UIFramework
{   
	/// <summary>
    /// 
    /// </summary>
	public class KeepPressLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
        public float pressInterval = 0.2f;//触发间隔
        public Action pressEvent;

        private float startTime;
        private bool isPressed = false;

        private void Update()
        {
            if (isPressed)
            {
                if (Time.time - startTime < pressInterval)
                {
                    if (pressEvent != null)
                        pressEvent.Invoke();
                    startTime = Time.time;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startTime = Time.time;
            isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            startTime = Time.time;
            isPressed = false;
        }
    }
}