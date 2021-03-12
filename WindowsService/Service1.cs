using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Timer variable
        /// </summary>
        Timer timer;

        /// <summary>
        /// Initilizing the class
        /// </summary>
        public Service1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starting the service
        /// </summary>
        /// <param name="args">Parameteres optional</param>
        protected override void OnStart(string[] args)
        {
            WriteEventLogEntry("Process Start Generator Climate CSV");

            this.timer = new Timer();
            this.timer.Interval = 3000000; //5 min 
            this.timer.Elapsed += this.timerExecute;
            this.timer.AutoReset = true;
            this.timer.Enabled = true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        protected override void OnStop()
        {
            this.timer.Stop();
            this.timer = null;
            WriteEventLogEntry("Process Stop Generator Climate CSV");
        }

        /// <summary>
        /// Executing the process to obtain de CSV file
        /// </summary>
        /// <param name="sender">Optional parameter</param>
        /// <param name="e">Optional parameter</param>
        private void timerExecute(object sender, ElapsedEventArgs e)
        {
            WriteEventLogEntry("Process Execute Generator Climate CSV");
            data objData = new data();

            objData = process.Instance.obtainData();

            process.Instance.writeFile(objData);
        }

        /// <summary>
        /// Starting the service
        /// </summary>
        public void OnDebug()
        {
            OnStart(null);
        }

        /// <summary>
        /// Writing on the log
        /// </summary>
        /// <param name="message">Message to write</param>
        static void WriteEventLogEntry(string message)
        {
            EventLog eventLog = new EventLog();

            if (!EventLog.SourceExists("Generator Climate CSV"))
            {
                EventLog.CreateEventSource("Generator Climate CSV", "Application");
            }

            eventLog.Source = "Climate CSV Generator";

            int eventID = 8;
            eventLog.Log = "Application";

            eventLog.WriteEntry(message, EventLogEntryType.Information, eventID);

            eventLog.Close();
        }
    }
}
