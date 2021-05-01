using UnityEngine;
using System;
using System.Collections;

namespace WJF_CodeLibrary.CommonUtility
{
    public static class CallbackUtility
    {
        /// <summary>
        /// 间隔执行模式：无限循环间隔执行、持续时间内间隔执行、次数间隔执行
        /// </summary>
        public enum InvervalMode { Loop, Duration, Count }

        /// <summary>
        /// 延迟执行事件
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="onExecute">执行事件</param>
        /// <returns>执行事件的行为物体</returns>
        public static GameObject ExecuteDelay(float delay, Action onExecute)
        {
            DelayBehaviour behaviour = new GameObject("DelayBehaviour").AddComponent<DelayBehaviour>();
            behaviour.delay = delay;
            behaviour.onExecute = onExecute;
            behaviour.ActiveCallback();
            return behaviour.gameObject;
        }

        /// <summary>
        /// 持续时间内执行事件
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="duration">持续时间</param>
        /// <param name="onStart">开始事件</param>
        /// <param name="onUpdate">持续时间内每帧执行的事件</param>
        /// <param name="onComplete">结束事件</param>
        /// <returns>执行事件的行为物体</returns>
        public static GameObject ExecuteDuration(float delay, float duration, Action onStart = null, Action onUpdate = null, Action onComplete = null)
        {
            DurationBehaviour behaviour = new GameObject("DurationBehaviour").AddComponent<DurationBehaviour>();
            behaviour.delay = delay;
            behaviour.duration = duration;
            behaviour.onStart = onStart;
            behaviour.onUpdate = onUpdate;
            behaviour.onComplete = onComplete;
            behaviour.StartCoroutine(behaviour.WaitForExecute());
            return behaviour.gameObject;
        }

        /// <summary>
        /// 无限循环间隔执行事件
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="onStart">开始事件</param>
        /// <param name="onUpdate">间隔执行事件</param>
        /// <param name="onComplete">结束事件</param>
        /// <returns>执行事件的行为物体</returns>
        public static GameObject ExecuteInterval(float delay, float interval, Action onStart = null, Action onUpdate = null, Action onComplete = null)
        {
            IntervalBehaviour behaviour = new GameObject("IntervalBehaviour").AddComponent<IntervalBehaviour>();
            behaviour.delay = delay;
            behaviour.interval = interval;
            behaviour.onStart = onStart;
            behaviour.onUpdate = onUpdate;
            behaviour.onComplete = onComplete;
            behaviour.Execute(InvervalMode.Loop);
            return behaviour.gameObject;
        }

        /// <summary>
        /// 持续时间内间隔执行事件
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="duration">持续时间</param>
        /// <param name="onStart">开始事件</param>
        /// <param name="onUpdate">间隔执行事件</param>
        /// <param name="onComplete">结束事件</param>
        /// <returns>执行事件的行为物体</returns>
        public static GameObject ExecuteInterval(float delay, float interval, float duration, Action onStart = null, Action onUpdate = null, Action onComplete = null)
        {
            IntervalBehaviour behaviour = new GameObject("IntervalBehaviour").AddComponent<IntervalBehaviour>();
            behaviour.delay = delay;
            behaviour.interval = interval;
            behaviour.duration = duration;
            behaviour.onStart = onStart;
            behaviour.onUpdate = onUpdate;
            behaviour.onComplete = onComplete;
            behaviour.Execute(InvervalMode.Duration);
            return behaviour.gameObject;
        }

        /// <summary>
        /// 间隔执行事件
        /// </summary>
        /// <param name="delay">延迟</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="count">执行次数</param>
        /// <param name="onStart">开始事件</param>
        /// <param name="onUpdate">间隔执行事件</param>
        /// <param name="onComplete">结束事件</param>
        /// <returns>执行事件的行为物体</returns>
        public static GameObject ExecuteInterval(float delay, float interval, int count, Action onStart = null, Action onUpdate = null, Action onComplete = null)
        {
            IntervalBehaviour behaviour = new GameObject("IntervalBehaviour").AddComponent<IntervalBehaviour>();
            behaviour.delay = delay;
            behaviour.interval = interval;
            behaviour.totalCount = count;
            behaviour.onStart = onStart;
            behaviour.onUpdate = onUpdate;
            behaviour.onComplete = onComplete;
            behaviour.Execute(InvervalMode.Count);
            return behaviour.gameObject;
        }

        /// <summary>
        /// 延迟执行事件的行为类
        /// </summary>
        private class DelayBehaviour : MonoBehaviour
        {
            public float delay = 0f;
            public Action onExecute = null;

            public void ActiveCallback()
            {
                Invoke("Execute", delay);
            }

            public void Execute()
            {
                if (onExecute != null)
                    onExecute.Invoke();

                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 持续时间执行事件的行为类
        /// </summary>
        public class DurationBehaviour : MonoBehaviour
        {
            public float delay = 0f;
            public float duration = 1f;
            public Action onStart = null;
            public Action onUpdate = null;
            public Action onComplete = null;

            public IEnumerator WaitForExecute()
            {
                yield return new WaitForSeconds(delay);

                if (onStart != null)
                    onStart.Invoke();

                float sTime = Time.time;
                float iTime = Time.time;

                do
                {
                    if (onUpdate != null)
                        onUpdate.Invoke();
                    yield return null;
                } while ((Time.time - sTime) <= duration);

                if (onComplete != null)
                    onComplete.Invoke();

                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 间隔执行事件的行为类
        /// </summary>
        public class IntervalBehaviour : MonoBehaviour
        {
            public float delay = 0f;
            public float duration = 1f;
            public float interval = 0.5f;
            public int totalCount = 1;
            public Action onStart = null;
            public Action onUpdate = null;
            public Action onComplete = null;

            public void Execute(InvervalMode mode)
            {
                switch (mode)
                {
                    case InvervalMode.Loop:
                        StartCoroutine(Loop());
                        break;
                    case InvervalMode.Duration:
                        StartCoroutine(Duration());
                        break;
                    case InvervalMode.Count:
                        StartCoroutine(Count());
                        break;
                }
            }

            private IEnumerator Loop()
            {
                yield return new WaitForSeconds(delay);

                if (onStart != null)
                    onStart.Invoke();

                float iTime = Time.time;

                do
                {
                    if (Time.time - iTime >= interval)
                    {
                        if (onUpdate != null)
                            onUpdate.Invoke();
                    }
                        
                    yield return null;
                } while (true);
            }

            private IEnumerator Duration()
            {
                yield return new WaitForSeconds(delay);

                if (onStart != null)
                    onStart.Invoke();

                float sTime = Time.time;
                float iTime = sTime;

                do
                {
                    if (Time.time - iTime >= interval)
                    {
                        if (onUpdate != null)
                            onUpdate.Invoke();
                        iTime = Time.time;
                    }
                        
                    yield return null;
                } while (Time.time - sTime <= duration);

                if (onComplete != null)
                    onComplete.Invoke();

                Destroy(gameObject);
            }

            private IEnumerator Count()
            {
                yield return new WaitForSeconds(delay);

                if (onStart != null)
                    onStart.Invoke();

                int count = 0;
                float iTime = Time.time;

                do
                {
                    if (Time.time - iTime >= interval)
                    {
                        if (onUpdate != null)
                            onUpdate.Invoke();
                        count++;
                        iTime = Time.time;
                    }
                        
                    yield return null;
                } while (count < totalCount);

                if (onComplete != null)
                    onComplete.Invoke();

                Destroy(gameObject);
            }
        }
    }
}