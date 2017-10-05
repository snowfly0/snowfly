using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 事件收发机，用于注册和触发事件
/// </summary>
public class EventDispatcher
{
    /// <summary>
    /// 事件处理类。
    /// </summary>
    public class EventController
    {
        private Dictionary<uint, Delegate> m_theRouter = new Dictionary<uint, Delegate>();

        public Dictionary<uint, Delegate> TheRouter
        {
            get { return m_theRouter; }
        }
        /// <summary>
        /// 处理增加监听器前的事项， 检查 参数等
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(uint eventType, Delegate listenerBeingAdded)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                m_theRouter.Add(eventType, null);
            }

            Delegate d = m_theRouter[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new Exception(string.Format(
                       "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                       eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// 移除监听器之前的检查
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(uint eventType, Delegate listenerBeingRemoved)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                return false;
            }

            Delegate d = m_theRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new Exception(string.Format(
                    "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                    eventType, d.GetType(), listenerBeingRemoved.GetType()));
            }
            else
                return true;
        }

        /// <summary>
        /// 移除监听器之后的处理。删掉事件
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(uint eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
            {
                m_theRouter.Remove(eventType);
            }
        }

        #region 增加监听器
        /// <summary>
        ///  增加监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(uint eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(uint eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(uint eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(uint eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  增加监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(uint eventType, Action<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] + handler;
        }
        #endregion

        #region 移除监听器

        /// <summary>
        ///  移除监听器， 不带参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(uint eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 1个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(uint eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 2个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(uint eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 3个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(uint eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  移除监听器， 4个参数
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(uint eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        ///  触发事件， 不带参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent(uint eventType)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            Action callback = d as Action;
            if (callback != null)
            {
                callback();
            }
        }

        /// <summary>
        ///  触发事件， 带1个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(uint eventType, T arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            Action<T> callback = d as Action<T>;
            if (callback != null)
            {
                callback(arg1);
            }
        }

        /// <summary>
        ///  触发事件， 带2个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(uint eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            Action<T, U> callback = d as Action<T, U>;
            if (callback != null)
            {
                callback(arg1,arg2);
            }
        }

        /// <summary>
        ///  触发事件， 带3个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(uint eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            Action<T, U, V> callback = d as Action<T, U, V>;
            if (callback != null)
            {
                callback(arg1, arg2,arg3);
            }
        }

        /// <summary>
        ///  触发事件， 带4个参数触发
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(uint eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            Action<T, U, V,W> callback = d as Action<T, U, V,W>;
            if (callback != null)
            {
                callback(arg1, arg2, arg3,arg4);
            }
        }

        #endregion
    }



    private static EventController m_eventController = new EventController();

    public static Dictionary<uint, Delegate> TheRouter
    {
        get { return m_eventController.TheRouter; }
    }




    #region 增加监听器
    /// <summary>
    ///  增加监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener(EventInfo eventType, Action handler)
    {
        m_eventController.AddEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  增加监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T>(EventInfo eventType, Action<T> handler)
    {
        m_eventController.AddEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  增加监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U>(EventInfo eventType, Action<T, U> handler)
    {
        m_eventController.AddEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  增加监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U, V>(EventInfo eventType, Action<T, U, V> handler)
    {
        m_eventController.AddEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  增加监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U, V, W>(EventInfo eventType, Action<T, U, V, W> handler)
    {
        m_eventController.AddEventListener(eventType.GetId(), handler);
    }
    #endregion

    #region 移除监听器
    /// <summary>
    ///  移除监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener(EventInfo eventType, Action handler)
    {
        m_eventController.RemoveEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  移除监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T>(EventInfo eventType, Action<T> handler)
    {
        m_eventController.RemoveEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  移除监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U>(EventInfo eventType, Action<T, U> handler)
    {
        m_eventController.RemoveEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  移除监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U, V>(EventInfo eventType, Action<T, U, V> handler)
    {
        m_eventController.RemoveEventListener(eventType.GetId(), handler);
    }

    /// <summary>
    ///  移除监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U, V, W>(EventInfo eventType, Action<T, U, V, W> handler)
    {
        m_eventController.RemoveEventListener(eventType.GetId(), handler);
    }
    #endregion

    #region 触发事件
    /// <summary>
    ///  触发事件， 不带参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent(EventInfo eventType)
    {
        m_eventController.TriggerEvent(eventType.GetId());
    }

    /// <summary>
    ///  触发事件， 带1个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T>(EventInfo eventType, T arg1)
    {
        m_eventController.TriggerEvent(eventType.GetId(), arg1);
    }

    /// <summary>
    ///  触发事件， 带2个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U>(EventInfo eventType, T arg1, U arg2)
    {
        m_eventController.TriggerEvent(eventType.GetId(), arg1, arg2);
    }

    /// <summary>
    ///  触发事件， 带3个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U, V>(EventInfo eventType, T arg1, U arg2, V arg3)
    {
        m_eventController.TriggerEvent(eventType.GetId(), arg1, arg2, arg3);
    }

    /// <summary>
    ///  触发事件， 带4个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U, V, W>(EventInfo eventType, T arg1, U arg2, V arg3, W arg4)
    {
        m_eventController.TriggerEvent(eventType.GetId(), arg1, arg2, arg3, arg4);
    }

    #endregion
}




public class EventInfo
{
    static uint _currentId = 1;
    uint _eventId;
    public uint GetId()
    {
        if (_eventId == 0)
        {
            _eventId = _currentId++;
            return _eventId;
        }
        return _eventId;
    } 
}



