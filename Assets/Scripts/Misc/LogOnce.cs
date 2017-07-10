using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A simple useful class to log a message, warning etc once only, rather than spamming 
 * the Debug console when logging in the Update call */
public class LogOnce
{
    public enum LogType { Log, Assertion, Error, Warning };

    private bool _hasFired = false;
    private string _s;
    private LogType _type;
    private GameObject _sender = null;

    /// <summary>
    /// Creates a new instance with message s, using the Debug Log type as specified.
    /// </summary>
    public LogOnce(string s, LogType type, GameObject sender = null)
    {
        _s = s;
        _type = type;
        _sender = sender;
    }

    /// <summary>
    /// Set the reference to the object sending the message.
    /// </summary>
    public void SetSender(GameObject sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Set the log message
    /// </summary>
    public void SetMessage(string s)
    {
        _s = s;
    }

    /// <summary>
    /// Set the log type
    /// </summary>
    public void SetType(LogType type)
    {
        _type = type;
    }

    /// <summary>
    /// Logs the message in the Debug Logger. If this message has already been logged, it will be ignored
    /// </summary>
    public void Log()
    {
        if (!_hasFired)
        {
            string logString = (_sender) ? "Sender: " + _sender.name + "\nMessage:" + _s : _s;

            switch (_type)
            {
                case LogType.Log:
                    Debug.Log(logString);
                    _hasFired = true;
                    break;

                case LogType.Assertion:
                    Debug.LogAssertion(logString);
                    _hasFired = true;
                    break;

                case LogType.Error:
                    Debug.LogError(logString);
                    _hasFired = true;
                    break;

                case LogType.Warning:
                    Debug.LogWarning(logString);
                    _hasFired = true;
                    break;

                default:
                    Debug.Log(logString);
                    _hasFired = true;
                    break;
            }
        }
    }
}
