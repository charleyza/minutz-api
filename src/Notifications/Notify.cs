using System;
using System.Text;
using Interface.Services;

namespace Notifications
{
    public class Notify : INotify
    {
        public string NotifyUser{ get; private set;}
        public string NotifyKey { get; private set; }
        public string NotifyInvitationAddress { get; private set; }    
        public string NotifyDefaultTemplateKey { get; private set; }
        public string DestinationBaseAddress { get; private set; }
        public Notify ()
        {
            if (string.IsNullOrEmpty (Environment.GetEnvironmentVariable ("NOTIFY-KEY")))
            {
                throw new ArgumentNullException ("The environment variable: NOTIFY-KEY, was not supplied, please set this variable and try again.");
            }
            this.NotifyKey = Environment.GetEnvironmentVariable ("NOTIFY-KEY");

            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NOTIFY-DEFAULT-TEMPLATE-KEY")))
            {
                throw new ArgumentException("The environment variable: NOTIFY-DEFAULT-TEMPLATE-KEY, was not supplied, please set this variable and try again.");
            }
            this.NotifyDefaultTemplateKey = Environment.GetEnvironmentVariable("NOTIFY-DEFAULT-TEMPLATE-KEY");

            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NOTIFY-USER")))
            {
                throw new ArgumentException("The environment variable: NOTIFY-USER, was not supplied, please set this variable and try again.");
            }
            this.NotifyUser = Environment.GetEnvironmentVariable("NOTIFY-USER");

            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UI-BASE-URL")))
            {
                throw new ArgumentException("The environment variable: UI-BASE-URL, was not supplied, please set this variable and try again.");
            }
            this.DestinationBaseAddress = Environment.GetEnvironmentVariable("UI-BASE-URL");

            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NOTIFY-INVITATION-ADDRESS")))
            {
                throw new ArgumentException("The environment variable: NOTIFY-INVITATION-ADDRESS, was not supplied, please set this variable and try again.");
            }
            this.NotifyInvitationAddress = Environment.GetEnvironmentVariable("NOTIFY-INVITATION-ADDRESS");
        }
    }
}