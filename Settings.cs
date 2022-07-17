namespace Net.DJDole.Settings
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;
    using System.Timers;

    public static class Defaults
    {
        public static double RefreshMs = 10000;
    }

    public abstract class Base
    {
        protected ILogger logger;
        protected dynamic values;
        protected readonly System.Timers.Timer watch;

        protected string hash { get; set; }
        protected string path { get; set; }

        protected Base(string path, string name)
        {
            this.path = System.IO.Path.GetFullPath(path);
            this.watch = new System.Timers.Timer();
            this.watch.Interval = double.Parse(((String.IsNullOrWhiteSpace(values) || String.IsNullOrWhiteSpace(values.RefreshMs)) ? Defaults.RefreshMs : values.RefreshMs).ToString());
            this.watch.Elapsed += new ElapsedEventHandler(LoadSettings);
            this.logger = LoggerFactory.Create(builder => {
                    builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter(name, LogLevel.Debug);
                }
            ).CreateLogger("Errors");
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            this.watch.Start();
            Load();
        }

        public void Stop() => this.watch.Stop();

        public void Load()
        {
            try
            {
                using (FileStream stream = new FileStream(this.path, FileMode.Open, FileAccess.Read))
                {
                    string loaded = new StreamReader(stream).ReadToEnd();
                    string hash = Net.DJDole.Utils.ComputeHash(loaded);
                    if (hash != this.hash)
                    {
                        Update(JObject.Parse(loaded), hash);
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.ToString());
            }
        }

        private void LoadSettings(object sender, System.Timers.ElapsedEventArgs args) => Load();

        private void Update(dynamic values, string hash)
        {
            this.hash = hash;
            this.values = values;
            try
            {
                double interval = double.Parse((values.RefreshMs).ToString());
                if ((this.watch != null) && (this.watch.Interval != interval))
                {
                    this.watch.Interval = interval;
                    Restart();
                }
            }
            catch (Exception e)
            {
                this.logger.LogDebug(e.ToString());
            }
        }
    }

    public class ServiceSettings : Base
    {
        public dynamic Values => values;
        public dynamic Path => this.path;
        public dynamic Hash => this.hash;
        public ServiceSettings(string path, string name) : base (path, name) {}
    }
}
