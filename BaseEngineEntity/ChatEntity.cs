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
        public long Created { get; set; }
    }
    public class ChatEntity : EntityBase
    {
        public string Title { get; set; }
        public ECHAT Type { get; set; }
        public MessageEntity LastMessage { get; set; }
        public long Created { get; set; }

    }

    public class ChatDetailEntity : EntityBase
    {
        public string ChatID { get; set; }
        public List<Attachment> Attachments { get; set; } 
        public List<Member> Members { get; set; }
    }

    public class Member : User
    {
        public string LastRead { get; set; }
    }

    public class User
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
    public class Attachment
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public EFileType Exts { get; set; } //doc, image,video, orther ...
        public string Path { get; set; }
        public long Created { get; set; }
    }
    public enum EMessageType
    {
        GROUP,PRIVATE
    }
    public enum EFileType
    {
        DOC,IMAGE,VIDEO,AUDIO,ORTHER
    }
    public enum ECHAT
    {
        SYSTEM,SUPPORT,GROUP,TYPE
    }
}
