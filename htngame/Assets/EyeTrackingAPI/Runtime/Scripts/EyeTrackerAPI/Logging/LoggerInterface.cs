// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#pragma warning disable CS0414

/*
Deprecated
*/

namespace AdhawkApi.Logging
{
    internal static class LogId
    {
        private static int _nextId = 1;
        public static int Next {
            get { return _nextId++; }
        }
    }

    public sealed class LogSession
    {
        internal bool Published;
        internal int Id {get; private set; }
        internal LogSession()
        {
            Id = LogId.Next;
            Published = false;
        }
    }

    public sealed class Annotation
    {
        private static int _nextId = 0;
        internal int Id { get; private set; }
        internal string Name { get; private set; }
        internal Annotation Parent { get; private set; }
        internal LogSession Session { get; private set; }
        internal string JsonContent { get; private set; }
        internal Annotation(string name, Annotation parent, LogSession session, string json_content = "{}")
        {
            Id = LogId.Next;
            Name = name;
            Parent = parent;
            Session = session;
            JsonContent = json_content;
        }
    }

    public class LoggerInterface
    {
        private static LoggerInterface _instance;
        private readonly UDPBehaviour udpClient;
        private LogSession defaultSession;

        /// <summary>
        /// Get the singleton instance
        /// </summary>
        public static LoggerInterface Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoggerInterface();
                return _instance;
            }
        }

        private LoggerInterface()
        {
            udpClient = GameObject.FindObjectOfType<UDPBehaviour>();
            defaultSession = new LogSession();
        }

        /// <summary>
        /// Get a new logging session handle
        /// </summary>
        /// <returns>The handle</returns>
        public LogSession StartSession()
        {
            LogSession session = new LogSession();
            PublishSession(session);
            return session;
        }

        /// <summary>
        /// Log the current frame of gaze data
        /// </summary>
        /// <param name="headOrientation">current head orientation Euler angles</param>
        /// <param name="session">the logging session to update</param>
        public void LogGazeData(Vector3? headOrientation = null, LogSession session = null) { }

        /// <summary>
        /// Log an annotation in the data stream
        /// </summary>
        /// <param name="name">The text associated with the annotation</param>
        /// <param name="parent">A parent annotation the annotation is tied to</param>
        /// <param name="session">the logging session to update (defaults to session of the parent, or the default session)</param>
        /// <param name="json_content">Additional Json content to associate with the annotation</param>
        /// <returns>A handle to an annotation</returns>
        /// <exception cref="ArgumentException">annotation parent did not have matching session</exception>
        public Annotation LogAnnotation(string name = "", Annotation parent = null, LogSession session = null, string json_content="{}") {
            if (parent != null && parent.Session != (session??defaultSession))
            {
                throw new ArgumentException("annotation parent must be in the same session");
            }
            Annotation annotation = new Annotation(parent != null ? parent.Name + "." + name : name, parent, session ?? defaultSession);
            PublishAnnotation(annotation);
            return annotation;
        }

        /// <summary>
        /// Mark an annotation as ended as of the current time
        /// </summary>
        /// <param name="annotation">The annotation, as returned from LogAnnotation</param>
        public void CloseAnnotation(Annotation annotation) {
            Annotation close_annotation = new Annotation("/" + annotation.Name, annotation, annotation.Session);
            PublishAnnotation(close_annotation);
        }

        /// <summary>
        /// Send data to DB using specified route
        /// </summary>
        /// <param name="configLabel">unused</param>
        /// <param name="route">unused</param>
        /// <param name="tags">unused</param>
        /// <param name="extraData">unused</param>
        /// <param name="session">the logging session to update</param>
        public void PublishToDB(string configLabel, string route, List<String> tags = null,
                                Dictionary<string, object> extraData = null, LogSession session = null)
        {
            Annotation close_session = new Annotation("/LogSession", null, session ?? defaultSession);
            PublishAnnotation(close_session);
            if (session == null)
            {
                defaultSession = new LogSession();
            }
        }

        private void PublishSession(LogSession session)
        {
            if (!session.Published)
            {
                byte[] id_data = BitConverter.GetBytes(session.Id);
                byte[] parent_data = BitConverter.GetBytes(0);
                var name_data = Encoding.UTF8.GetBytes("LogSession").Concat(new byte[] { 0, 0 });
                var payload = id_data.Concat(parent_data).Concat(name_data);
                if (udpClient)
                {
                    udpClient.WriteCommand(udpInfo.LOG_ANNOTATION, payload);
                }
                session.Published = true;
            }
        }

        private void PublishAnnotation(Annotation annotation)
        {
            PublishSession(annotation.Session);
            byte[] id_data = BitConverter.GetBytes(annotation.Id);
            byte[] parent_data = BitConverter.GetBytes(annotation.Parent != null ? annotation.Parent.Id : annotation.Session.Id);
            var name_data = Encoding.UTF8.GetBytes(annotation.Name).Concat(new byte[] { 0 });
            var json_data = Encoding.UTF8.GetBytes(annotation.JsonContent).Concat(new byte[] { 0 });
            var payload = id_data.Concat(parent_data).Concat(name_data).Concat(json_data);
            udpClient.WriteCommand(udpInfo.LOG_ANNOTATION, payload);
        }
    }
}

#pragma warning restore CS0414