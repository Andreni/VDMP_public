using System;
using Windows.UI.Notifications;

namespace VDMP.App.Helpers
{
    public sealed class NotificationToUser
    {
        /// <summary>Shows the toast of added movies notification.</summary>
        /// <param name="title">Library the movie(is) was added to.</param>
        /// <param name="stringContent">Amount of titles added.</param>
        public static void ShowToastOfAddedMoviesNotification(string title, string stringContent)
        {
            var toastNotifier = ToastNotificationManager.CreateToastNotifier();
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toastNodeList = toastXml.GetElementsByTagName("text");

            toastNodeList.Item(0)?.AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1)?.AppendChild(toastXml.CreateTextNode(stringContent));

            var audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            var toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            toastNotifier.Show(toast);
        }
    }
}