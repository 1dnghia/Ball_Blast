using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Event Bus - Hệ thống giao tiếp giữa các component không cần reference trực tiếp
/// Sử dụng: EventBus.Subscribe<EventType>(handler); EventBus.Publish(new EventType());
/// </summary>
public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> eventHandlers = new Dictionary<Type, List<Delegate>>();
    
    /// <summary>
    /// Đăng ký lắng nghe event
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (!eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType] = new List<Delegate>();
        }
        
        if (!eventHandlers[eventType].Contains(handler))
        {
            eventHandlers[eventType].Add(handler);
        }
    }
    
    /// <summary>
    /// Hủy đăng ký lắng nghe event
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Remove(handler);
        }
    }
    
    /// <summary>
    /// Phát event đến tất cả listeners
    /// </summary>
    public static void Publish<T>(T eventData) where T : IGameEvent
    {
        Type eventType = typeof(T);
        
        if (eventHandlers.ContainsKey(eventType))
        {
            foreach (var handler in eventHandlers[eventType])
            {
                try
                {
                    ((Action<T>)handler)?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error invoking event {eventType.Name}: {e.Message}");
                }
            }
        }
    }
    
    /// <summary>
    /// Xóa tất cả event handlers (dùng khi restart game)
    /// </summary>
    public static void Clear()
    {
        eventHandlers.Clear();
    }
    
    /// <summary>
    /// Xóa tất cả handlers của một event type cụ thể
    /// </summary>
    public static void Clear<T>() where T : IGameEvent
    {
        Type eventType = typeof(T);
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Clear();
        }
    }
}

/// <summary>
/// Interface đánh dấu cho tất cả game events
/// </summary>
public interface IGameEvent { }
