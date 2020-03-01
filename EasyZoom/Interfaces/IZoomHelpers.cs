using AndcultureCode.ZoomClient.Models.Meetings;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyZoom.Interfaces
{
    public interface IZoomHelpers
    {
        bool RemoveSchedule(string meetingId);
        Meeting CreateScheduled(string topic, DateTime startTime, int duration);
    }
}
