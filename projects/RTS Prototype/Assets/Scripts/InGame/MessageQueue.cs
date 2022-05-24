using UnityEngine;
using System;
using System.Collections.Generic;

public class Message<S>
{
    public GameObject sender { private set; get;}
    public S type { private set; get;}
    public int data { private set; get;}

    public Message(GameObject sender, S type, int data)
    {
        this.sender = sender;
        this.type = type;
        this.data = data;
    }
}

public class MessageQueue<S>
{
    public delegate void MessageCallback(GameObject callee);
    Dictionary<S, MessageCallback> callbacks;
    List<Message<S>> messages;

    public MessageQueue()
    {
        callbacks = new Dictionary<S, MessageCallback>();
        messages = new List<Message<S>>();
    }

    public void AddMessage(Message<S> message)
    {
        if (message == null)
        {
            Debug.LogError("MessageQueue.AddMessage(), parameter message is null");
            return;
        }

        messages.Add(message);
    }

    public void DispatchMessages()
    {
        while (messages.Count > 0)
        {
            Message<S> currMessage = messages[0];
            MessageCallback callback = null;
            if (callbacks.TryGetValue(currMessage.type, out callback))
            {
                callback(currMessage.sender);
            }
            messages.Remove(currMessage);
        }
    }

    public void AddCallback(S type, MessageCallback c)
    {
        if (type == null)
        {
            Debug.LogError("MessageQueue.AddCallback(), parameter type is null");
            return;
        }
        if (c == null)
        {
            Debug.LogError("MessageQueue.AddCallback(), parameter c is null");
            return;
        }

        callbacks.Add(type, c);
    }
}
