using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;

namespace BaseMongoDB.Database
{
    public class SysTemplateEntity : EntityBase
    {
        public string Name { get; set; }
        public int LangID { get; set; }
        public string File { get; set; } // ten layout
        public string Html { get; set; }
    }
    public class SysTemplateService : ServiceBase<SysTemplateEntity>
    {
        public SysTemplateService(IConfiguration config) : base(config, "SysTemplates")
        {

        }
    }
}
