using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.ResponseCode
{
    public static class ImportResponseCode
    {
        public const int SUCCESS = 0;
        public const int VALIDATION_FAIL = 1;
        public const int FILE_NOT_VALID = 2;
        public const int FILE_EMPTY = 3;
        public const int DATA_EMPTY = 4;
        public const int OTHER_FAIL = 5;
        public const int CANNOT_READ_FILE = 6;
        public const int FILE_SAMPLE_FAIL = 8;
        public const int DATA_NOT_EXIT = 9;
        public const int CANNOT_IMPORT = 10;
    }
}
