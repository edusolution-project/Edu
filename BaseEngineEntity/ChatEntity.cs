using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseEngineEntity
{

    public class MessageEntity : EntityBase
    {
        public User Sender { get; set; }
        public string Receiver { get; set; } // chat id , user id
        public EMessageType Type { get; set; }
        public string Text { get; set; }
        public List<Attachment> Attachments { get; set; }
        public double Created { get; set; } = new UnixTime().Now;
    }
    public class ChatEntity : EntityBase
    {
        public List<string> Admins { get; set; }
        public string Title { get; set; }
        public ECHAT Type { get; set; }
        public MessageEntity LastMessage { get; set; }
        public double Created { get; set; } = new UnixTime().Now;

    }

    public class ChatDetailEntity : EntityBase
    {
        public string ChatID { get; set; }
        public List<Attachment> Attachments { get; set; } 
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        public string ID { get; set; }
        public string LastRead { get; set; }
    }

    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Type { get; set; }
    }
    public class Attachment
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Exts { get; set; } //doc, image,video, orther ...
        public string Path { get; set; }
        public double Created { get; set; } = new UnixTime().Now;
    }

    public class UnixTime
    {
        public UnixTime()
        {

        }

        public double Now
        {
            get { return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds; }
        }
        public double Date(DateTime dateTime)
        {
            return (dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
    public enum EMessageType
    {
        SYSTEM, SUPPORT, GROUP, USER
    }
    public enum EFileType
    {
        DOC,IMAGE,VIDEO,AUDIO,LINK,ORTHER
    }
    public enum ECHAT
    {
        SYSTEM,SUPPORT,GROUP,USER
    }
}
