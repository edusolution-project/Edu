using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExamples")]
    public class ModExampleEntity : EntityBase
    {
        public string Name { get; set; }
        public int MenuID { get; set; }
        public bool IsAdmin { get; set; } // giáo viên tao . hoặc admin tao
        public int CreateUser { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class ModExampleService : ServiceBase<ModExampleEntity>
    {
        #region Private
        public ModExampleService() : base("ModExamples")
        {

        }
        #endregion

        
    }
}
