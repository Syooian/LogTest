using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenLog : MonoBehaviour
{
    ///// <summary>
    ///// 畫面Log文字
    ///// </summary>
    //[SerializeField, Header("畫面Log文字")]
    //TMPro.TextMeshProUGUI Text;

    /// <summary>
    /// 是否啟用寫入Log
    /// </summary>
    [SerializeField, Header("是否啟用寫入Log")]
    bool WriteLog;

    private void Awake()
    {
        Application.logMessageReceived += SetScreenLog1;
        Application.logMessageReceivedThreaded += SetScreenLog2;

        if (WriteLog)
            StartWriteLog();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShowLogQueue.Count > 0)
        {
            SetScreenLog(ShowLogQueue.Dequeue());
        }
    }

    private void OnApplicationQuit()
    {
        Application.logMessageReceived -= SetScreenLog1;
        Application.logMessageReceivedThreaded -= SetScreenLog2;
    }

    /// <summary>
    /// 
    /// </summary>
    Queue<LogStruct> ShowLogQueue = new Queue<LogStruct>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="StackTrace"></param>
    /// <param name="MsgType"></param>
    void SetScreenLog1(string Msg, string StackTrace, LogType MsgType)
    {
        var NewLog = new LogStruct(Msg, StackTrace, MsgType, true);

        ShowLogQueue.Enqueue(NewLog);

        if (WriteLog)
            WriteLogQueue.Enqueue(NewLog);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Msg"></param>
    /// <param name="StackTrace"></param>
    /// <param name="MsgType"></param>
    void SetScreenLog2(string Msg, string StackTrace, LogType MsgType)
    {
        var NewLog = new LogStruct(Msg, StackTrace, MsgType, false);

        ShowLogQueue.Enqueue(NewLog);

        if (WriteLog)
            WriteLogQueue.Enqueue(NewLog);
    }

    /// <summary>
    /// 
    /// </summary>
    DateTime CleanDateTime;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Log"></param>
    void SetScreenLog(LogStruct Log)
    {
        try
        {
            var TS = DateTime.Now - CleanDateTime;

            //if (Text.text.Length >= 2000000000 || TS.TotalHours >= 1)
            //{
            //    Text.text = "";
            //    CleanDateTime = DateTime.Now;
            //}

            //if (Text.text != "")
            //    Text.text += "\n";

            string ColorString = "";
            switch (Log.MsgType)
            {
                case LogType.Error:
                    ColorString = ColorUtility.ToHtmlStringRGB(Color.red);
                    break;
                case LogType.Warning:
                    ColorString = "FFA000";
                    break;
                default:
                    ColorString = ColorUtility.ToHtmlStringRGB(Color.blue);
                    break;
            }

            //Text.text += "<color=#" + ColorString + ">" + Log.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + "	" + Log.Msg + "</color>";
        }
        catch (Exception ex)
        {
            //Text.text = "";
            CleanDateTime = DateTime.Now;
        }
    }

    #region 寫Log
    /// <summary>
    /// 
    /// </summary>
    const string LogPath = @"LogFiles\";
    /// <summary>
    /// 
    /// </summary>
    Queue<LogStruct> WriteLogQueue = new Queue<LogStruct>();
    /// <summary>
    /// 開始寫入Log
    /// </summary>
    void StartWriteLog()
    {
        //路徑檢查
        if (!Directory.Exists(LogPath))
        {
            Directory.CreateDirectory(LogPath);
        }

        StartCoroutine(IE_WriteLog());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator IE_WriteLog()
    {
        while (true)
        {
            if (WriteLogQueue.Count > 0)
            {
                using (StreamWriter SW = new StreamWriter(LogPath + DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt", true, System.Text.Encoding.UTF8))
                {
                    while (WriteLogQueue.TryDequeue(out LogStruct Result))
                    {
                        SW.WriteLine(Result.DateTime.ToString("yyyy-MM-dd HH:mm:ss") + "	" + Result.MainThread + "	"
                            + Result.MsgType + "	" + Result.Msg);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    struct LogStruct
    {
        /// <summary>
        /// Log訊息
        /// </summary>
        public string Msg;
        /// <summary>
        /// 
        /// </summary>
        public string StackTrace;
        /// <summary>
        /// Log種類
        /// </summary>
        public LogType MsgType;
        /// <summary>
        /// 時間
        /// </summary>
        public DateTime DateTime { get; private set; }
        /// <summary>
        /// 主執行續
        /// </summary>
        public bool MainThread { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="StackTrace"></param>
        /// <param name="MsgType"></param>
        public LogStruct(string Msg, string StackTrace, LogType MsgType, bool MainThread)
        {
            this.Msg = Msg;
            this.StackTrace = StackTrace;
            this.MsgType = MsgType;
            DateTime = DateTime.Now;
            this.MainThread = MainThread;
        }
    }
}
