// GameAnalytics.cs
// Thin, dependency-free Unity analytics client for Unity 6000.6 and newer.

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class AnalyticsOptions
{
    public string Endpoint;
    public string GameId;
    public string IngestToken;
    public string Environment = "production";
    public string BuildVersion;
    public int RequestTimeoutSeconds = 5;
    public bool LogPayloadsInDebugBuild;
}

public static class GameAnalytics
{
    public const string SdkVersion = "1.0.0";

    [Serializable]
    private sealed class EmptyData { }

    [Serializable]
    private class EventBase
    {
        public int schemaVersion = 1;
        public string sdkVersion = SdkVersion;
        public string sessionId;
        public string clientEventId;
        public string buildVersion;
        public string platform;
        public string engineVersion;
        public string environment;
        public string eventType;
        public string timestamp;
        public int sequence;
        public float elapsedSeconds;
    }

    [Serializable]
    private sealed class DataEvent<T> : EventBase
    {
        public T data;
    }

    [Serializable]
    private sealed class ProgressEvent<T> : EventBase
    {
        public float progress;
        public float progressGoal;
        public T data;
    }

    [Serializable]
    private sealed class Outcome
    {
        public string result;
        public string reason;
    }

    [Serializable]
    private sealed class EndEvent<T> : EventBase
    {
        public float progress;
        public float progressGoal;
        public Outcome outcome;
        public T data;
    }

    [Serializable]
    private sealed class IssueReport<T>
    {
        public int schemaVersion = 1;
        public string sdkVersion = SdkVersion;
        public string id;
        public string sessionId;
        public string signature;
        public string severity;
        public string message;
        public string stackTrace;
        public string timestamp;
        public string buildVersion;
        public string environment;
        public string platform;
        public T context;
        public string logTail;
    }

    [Serializable]
    private sealed class SessionStartResponse
    {
        public string sessionId;
    }

    private sealed class AnalyticsRunner : MonoBehaviour { }

    private static AnalyticsOptions options;
    private static AnalyticsRunner runner;
    private static string sessionId;
    private static float sessionStartedAt;
    private static int nextSequence;
    private static bool startingSession;
    private static int sessionStartGeneration;

    public static bool IsInitialized { get { return options != null && runner != null; } }
    public static bool IsSessionActive { get { return !string.IsNullOrEmpty(sessionId); } }
    public static bool IsSessionStarting { get { return startingSession; } }
    public static string SessionId { get { return sessionId; } }

    public static void Initialize(AnalyticsOptions analyticsOptions)
    {
        if (analyticsOptions == null) throw new ArgumentNullException("analyticsOptions");
        if (string.IsNullOrEmpty(analyticsOptions.Endpoint)) throw new ArgumentException("Analytics Endpoint is required.");
        if (string.IsNullOrEmpty(analyticsOptions.GameId)) throw new ArgumentException("Analytics GameId is required.");

        options = analyticsOptions;
        if (string.IsNullOrEmpty(options.Environment)) options.Environment = "production";
        if (string.IsNullOrEmpty(options.BuildVersion)) options.BuildVersion = Application.version;
        options.RequestTimeoutSeconds = Mathf.Max(1, options.RequestTimeoutSeconds);

        if (runner == null)
        {
            GameObject host = new GameObject("GameAnalytics");
            UnityEngine.Object.DontDestroyOnLoad(host);
            runner = host.AddComponent<AnalyticsRunner>();
        }
    }

    public static bool StartSession(Action<string> onStarted = null)
    {
        return StartSession(new EmptyData(), onStarted);
    }

    public static bool StartSession<T>(T initialData, Action<string> onStarted = null)
    {
        if (!IsInitialized)
        {
            Warn("Analytics is not initialized; this run will continue without analytics.");
            if (onStarted != null) onStarted(null);
            return false;
        }
        if (startingSession || IsSessionActive)
        {
            Warn("An analytics session is already starting or active.");
            if (onStarted != null) onStarted(null);
            return false;
        }

        sessionStartedAt = Time.realtimeSinceStartup;
        nextSequence = 0;

        DataEvent<T> analyticsEvent = new DataEvent<T>();
        Populate(analyticsEvent, "game_start");
        analyticsEvent.data = initialData;

        string payload;
        if (!TryToJson(analyticsEvent, out payload))
        {
            if (onStarted != null) onStarted(null);
            return false;
        }

        LogPayload("session start", payload);
        startingSession = true;
        int generation = ++sessionStartGeneration;
        runner.StartCoroutine(SendSessionStart(payload, generation, onStarted));
        return true;
    }

    public static bool Track<T>(string eventType, T data)
    {
        if (!IsSessionActive || string.IsNullOrEmpty(eventType)) return false;

        DataEvent<T> analyticsEvent = new DataEvent<T>();
        Populate(analyticsEvent, eventType);
        analyticsEvent.data = data;
        return SendEvent(analyticsEvent, eventType);
    }

    public static bool TrackProgress<T>(string eventType, float progress, float progressGoal, T data)
    {
        if (!IsSessionActive || string.IsNullOrEmpty(eventType)) return false;

        ProgressEvent<T> analyticsEvent = new ProgressEvent<T>();
        Populate(analyticsEvent, eventType);
        analyticsEvent.progress = progress;
        analyticsEvent.progressGoal = progressGoal;
        analyticsEvent.data = data;
        return SendEvent(analyticsEvent, eventType);
    }

    public static bool EndSession<T>(string result, string reason, float progress, float progressGoal, T finalData)
    {
        if (startingSession)
        {
            sessionStartGeneration += 1;
            startingSession = false;
            return false;
        }
        if (!IsSessionActive || string.IsNullOrEmpty(result)) return false;

        EndEvent<T> analyticsEvent = new EndEvent<T>();
        Populate(analyticsEvent, "game_end");
        analyticsEvent.progress = progress;
        analyticsEvent.progressGoal = progressGoal;
        analyticsEvent.outcome = new Outcome { result = result, reason = reason };
        analyticsEvent.data = finalData;

        bool sent = SendEvent(analyticsEvent, "game_end");
        sessionId = null;
        return sent;
    }

    public static bool ReportIssue<T>(string severity, string message, string stackTrace, T context, string logTail = null, string signature = null)
    {
        if (!IsInitialized || string.IsNullOrEmpty(severity) || string.IsNullOrEmpty(message)) return false;

        IssueReport<T> issue = new IssueReport<T>();
        issue.id = Guid.NewGuid().ToString("D");
        issue.sessionId = sessionId;
        issue.signature = signature;
        issue.severity = severity.ToLowerInvariant();
        issue.message = message;
        issue.stackTrace = stackTrace;
        issue.timestamp = UtcNow();
        issue.buildVersion = options.BuildVersion;
        issue.environment = options.Environment;
        issue.platform = Application.platform.ToString();
        issue.context = context;
        issue.logTail = logTail;

        string payload;
        if (!TryToJson(issue, out payload)) return false;
        LogPayload("issue", payload);
        runner.StartCoroutine(Post("issues", payload, "issue"));
        return true;
    }

    public static bool ReportException<T>(Exception exception, T context, string logTail = null)
    {
        if (exception == null) return false;
        return ReportIssue("exception", exception.Message, exception.ToString(), context, logTail);
    }

    private static bool SendEvent(object analyticsEvent, string label)
    {
        string payload;
        if (!TryToJson(analyticsEvent, out payload)) return false;
        LogPayload(label, payload);
        runner.StartCoroutine(Post("events", payload, label));
        return true;
    }

    private static void Populate(EventBase analyticsEvent, string eventType)
    {
        analyticsEvent.sessionId = sessionId;
        analyticsEvent.clientEventId = Guid.NewGuid().ToString("D");
        analyticsEvent.buildVersion = options.BuildVersion;
        analyticsEvent.platform = Application.platform.ToString();
        analyticsEvent.engineVersion = Application.unityVersion;
        analyticsEvent.environment = options.Environment;
        analyticsEvent.eventType = eventType;
        analyticsEvent.timestamp = UtcNow();
        analyticsEvent.sequence = ++nextSequence;
        analyticsEvent.elapsedSeconds = Mathf.Max(0f, Time.realtimeSinceStartup - sessionStartedAt);
    }

    private static IEnumerator SendSessionStart(string payload, int generation, Action<string> onStarted)
    {
        using (UnityWebRequest request = CreateRequest("sessions", payload))
        {
            yield return request.SendWebRequest();
            if (generation != sessionStartGeneration || !startingSession)
            {
                if (onStarted != null) onStarted(null);
                yield break;
            }

            startingSession = false;
            if (request.result == UnityWebRequest.Result.Success)
            {
                SessionStartResponse response = ParseSessionStartResponse(request.downloadHandler.text);
                if (response != null && !string.IsNullOrEmpty(response.sessionId))
                {
                    sessionId = response.sessionId;
                    if (onStarted != null) onStarted(sessionId);
                    yield break;
                }
            }

            Warn("Analytics session could not be started; this run will continue without analytics. HTTP " + request.responseCode + ": " + request.error);
            if (onStarted != null) onStarted(null);
        }
    }

    private static IEnumerator Post(string route, string payload, string label)
    {
        using (UnityWebRequest request = CreateRequest(route, payload))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
                Warn("Analytics " + label + " was not delivered. HTTP " + request.responseCode + ": " + request.error);
        }
    }

    private static UnityWebRequest CreateRequest(string route, string payload)
    {
        string url = options.Endpoint.TrimEnd('/') + "/api/games/" + Uri.EscapeDataString(options.GameId) + "/" + route;
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = options.RequestTimeoutSeconds;
        request.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(options.IngestToken))
            request.SetRequestHeader("Authorization", "Bearer " + options.IngestToken);
        return request;
    }

    private static bool TryToJson(object value, out string json)
    {
        try
        {
            json = JsonUtility.ToJson(value);
            return true;
        }
        catch (Exception exception)
        {
            json = null;
            Warn("Analytics payload could not be serialized: " + exception.Message);
            return false;
        }
    }

    private static SessionStartResponse ParseSessionStartResponse(string json)
    {
        try
        {
            return JsonUtility.FromJson<SessionStartResponse>(json);
        }
        catch (Exception exception)
        {
            Warn("Analytics returned an invalid session response: " + exception.Message);
            return null;
        }
    }

    private static string UtcNow()
    {
        return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
    }

    private static void LogPayload(string label, string payload)
    {
        if (options.LogPayloadsInDebugBuild && Debug.isDebugBuild)
            Debug.Log("Analytics " + label + " payload: " + payload);
    }

    private static void Warn(string message)
    {
        if (Debug.isDebugBuild) Debug.LogWarning(message);
    }
}
