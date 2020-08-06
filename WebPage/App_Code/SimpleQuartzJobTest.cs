using System;
using Common.Logging;
using Quartz;
using System.IO;

/// <summary>
/// SimpleQuartzJobTest 的摘要描述
/// </summary>
public class SimpleQuartzJobTest:IJob
{
    private static ILog _log = LogManager.GetLogger(typeof(SimpleQuartzJobTest));

    /// <summary>
    /// Called by the <see cref="IScheduler" /> when a
    /// <see cref="Trigger" /> fires that is associated with
    /// the <see cref="IJob" />.
    /// </summary>
    public virtual void Execute(JobExecutionContext context)
    {
        try
        {
            // This job simply prints out its job name and the
            // date and time that it is running
            string jobName = context.JobDetail.FullName;
            string jobDetail = "Executing job: " + jobName + " executing at " + DateTime.Now.ToString("r");

            //write log
            WriteLog(jobDetail);
        }
        catch (Exception e)
        {
            //* ------------------ Error in job!-------------------
            JobExecutionException e2 = new JobExecutionException(e);
            // this job will refire immediately
            e2.RefireImmediately = true;
            throw e2;
        }
    }

    public static void WriteLog(string strMsgContext)
    {
        //* Application Path
        string strApplicationPath = AppDomain.CurrentDomain.BaseDirectory + "Log/log.txt";
        System.IO.StreamWriter swFile = File.AppendText(strApplicationPath);

        //* Write Msg to text file
        swFile.WriteLine(strMsgContext);
        swFile.Flush();
        swFile.Close();
    }
}
