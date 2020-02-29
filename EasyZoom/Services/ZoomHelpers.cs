using AndcultureCode.ZoomClient;
using AndcultureCode.ZoomClient.Interfaces;
using AndcultureCode.ZoomClient.Models;
using AndcultureCode.ZoomClient.Models.Meetings;
using AndcultureCode.ZoomClient.Models.Users;
using EasyZoom.Interfaces;
using EasyZoom.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyZoom.Services
{
    public class ZoomHelpers : IZoomHelpers
    {
        private readonly IOptions<ZoomConfig> _options;
        private readonly ZoomConfig _zoomConfig;
        private readonly IZoomClient _zoomClient;
        private readonly ListUsers _listUsers;
        private readonly User _user;
        private bool _zoomReady = false;
        public ZoomHelpers(IOptions<ZoomConfig> options)
        {
            _options = options;
            _zoomConfig = _options.Value;
            _zoomClient = new ZoomClient(new ZoomClientOptions
            {
                ZoomApiKey = _zoomConfig.ZoomApiKey,
                ZoomApiSecret = _zoomConfig.ZoomApiSecret
            });
            if (_zoomClient != null)
            {
                _listUsers = _zoomClient.Users.GetUsers();
                _user = _listUsers != null && _listUsers.Users.Count > 0 ? _listUsers.Users[0] : null;
                _zoomReady = true;
            }
        }
        public bool RemoveSchedule(string meetingId)
        {
            return _zoomClient.Meetings.DeleteMeeting(meetingId);
        }
        public Meeting CreateScheduled(string topic, DateTime startTime,int duration)
        {
            var settings = new MeetingSettings
            {
                EnableHostVideo = true,
                EnableParticipantVideo = true,
                EnableJoinBeforeHost = false,
                ApprovalType = MeetingApprovalTypes.Automatic,
                AutoRecording = MeetingAutoRecordingOptions.Local,
                EnableEnforceLogin = true
            };
            var meeting = new Meeting()
            {
                Topic = topic,
                Type = MeetingTypes.Scheduled,
                StartTime = startTime,
                Duration = duration,
                Settings = settings
            };
            var currentMeeting = _zoomClient.Meetings.CreateMeeting(_user.Id, meeting);
            return currentMeeting;
        }

    }
}
