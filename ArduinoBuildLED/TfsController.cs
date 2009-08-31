using System;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

namespace ArduinoBuildLED
{
    class TfsController
    {
        private Arduino _arduino;

        //private WallboardForm form;
        private IQueuedBuildsView buildQueue;

        /// <summary>
        ///   Create initial connection to the TFS 2008 Build Server.
        /// </summary>
        public void Connect()
        {
            TeamFoundationServer tfs = new TeamFoundationServer(TfsUrl);

            IBuildServer buildServer = (IBuildServer)tfs.GetService(typeof(IBuildServer));

            buildQueue = buildServer.CreateQueuedBuildsView(TeamProject);
            
            // We are only interested in builds when they are finished or as they are in progress
            buildQueue.StatusFilter = QueueStatus.Completed | QueueStatus.InProgress;

            // Hook up our build queue listener.
            buildQueue.StatusChanged += new StatusChangedEventHandler(buildQueue_StatusChanged);

            try
            {
                buildQueue.Connect();
            }
            catch (Exception ex)
            {
            }
        }

        void buildQueue_StatusChanged(object sender, StatusChangedEventArgs e)
        {            
            // We have to explicitly check to see if the build has changed.
            if (e.Changed && buildQueue.QueuedBuilds.Length > 0)
            {
                // Get the last item (TFS sorts them as we need)
                int lastItem = buildQueue.QueuedBuilds.Length - 1;
                
                // In the session, we showed the results of the last build for the team project.
                // here you may want to filter
                UpdateStatus(buildQueue.QueuedBuilds[lastItem]);
            }
        }

        private void UpdateStatus(IQueuedBuild queuedBuild)
        {
            if (queuedBuild.Build != null)
            {
                UpdateStatus(queuedBuild.Build);
            }
        }

        private void UpdateStatus(IBuildDetail detail)
        {
            // We used the build server later to convert the enums into the localized display values.
            IBuildServer buildServer = detail.BuildServer;

            //lblBuildNumber.Text = detail.BuildNumber;

            //  The TFS API's always return DOMAIN\username, however the convention is that we only
            //  display the domain portion if it is different from our current users domain.
            //lblRequestedFor.Text = UserNameFormatter.GetFriendlyName(detail.RequestedFor, null);

            // If the build has finished then display the finish time.
            //lblFinishTime.Text = detail.FinishTime.Equals(DateTime.MinValue) ?
            //  "" : detail.FinishTime.ToString();

            // Convert the build status into a localized display text.
            String statusText = buildServer.GetDisplayText(detail.Status);

            // Now we want to show the fancy images.
            switch (detail.Status)
            {
                case BuildStatus.Failed:
                    _arduino.ChangeColor(255, 0, 0);
                    break;
                case BuildStatus.InProgress:
                    _arduino.ChangeColor(0, 0, 255);
                    break;
                case BuildStatus.NotStarted:
                    _arduino.ChangeColor(0, 0, 255);
                    break;
                case BuildStatus.PartiallySucceeded:
                    _arduino.ChangeColor(255, 0, 0);
                    break;
                case BuildStatus.Stopped:
                    _arduino.ChangeColor(255, 0, 0);
                    break;
                case BuildStatus.Succeeded:
                    _arduino.ChangeColor(0, 255, 0);
                    break;
                default:
                    break;
            }

            // And finally set the text.
            //lblStatus.Text = statusText;
        }

        public TfsController (Arduino arduino)
	    {
            _arduino = arduino;
	    }

        public string TfsUrl { get; set; }

        public string TeamProject { get; set; }

        public string BuildDefinitionName { get; set; }
    }
}
