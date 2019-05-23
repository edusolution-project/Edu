using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public class MeasureTime : IDisposable
    {
        public string name;
        public DateTime start;
        public DateTime end;

        public MeasureTime(string name)
        {
            this.name = name;
            this.start = DateTime.Now;
        }
        public void Dispose()
        {
            this.end = DateTime.Now;
            Log();
        }

        public virtual void Log()
        {
            throw new NotImplementedException();
        }
    }
}
