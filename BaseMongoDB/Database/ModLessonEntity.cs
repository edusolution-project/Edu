﻿using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BaseMongoDB.Database
{
    public class ModLessonEntity : EntityBase
    {
        public string CourseID { get; set; }
        public string ChapterID { get; set; }
        public bool IsParentCourse { get; set; } // có phải là course hay không ?
        public int TemplateType { get; set; }
        public int Point { get; set; }
        public int Timer { get; set; }
        public string CreateUser { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public int Order { get; set; }
        public Media Media { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class ModLessonService : ServiceBase<ModLessonEntity>
    {
        public ModLessonService(IConfiguration config) : base(config, "ModLessons")
        {

        }
        public ModLessonService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }

        public List<ModLessonEntity> getListByCourseIdNoChapter(string courseId)
        {
            var result = new List<ModLessonEntity>();
            var query = CreateQuery().Find(o => o.CourseID == courseId );
            result = query.ToList();
            return result;
        }

        public List<ModLessonEntity> getListByCourseIdHaveChapter(string courseId)
        {
            var result = new List<ModLessonEntity>();
            var query = CreateQuery().Find(o => o.CourseID == courseId && o.ChapterID != "");
            result = query.ToList();
            return result;
        }
    }
}
