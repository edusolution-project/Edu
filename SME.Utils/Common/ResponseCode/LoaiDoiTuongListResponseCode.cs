using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common.ResponseCode
{
    public static class LoaiDoiTuongListResponseCode
    {
        public const int SUCCESS = 0;
        public const int NOT_FOUND = 1;
        public const int ADD_ERROR = 2;
        public const int UPDATE_ERROR = 3;
        public const int LOCK_ERROR = 4;
        public const int UPLOAD_ERROR = 5;
        public const int UPLOAD_DATA_NULL = 6;
        public const int CHECK_LENGTH = 7;
        public const int CHECK_CHARACTER_SPECIAL = 8;
    }
}
