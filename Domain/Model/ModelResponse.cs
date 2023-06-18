using System;

namespace Nvenger.Common.BaseModel
{
    public class ModelResponse 
    {
        public string status { get; set; } = "fail";
        public string message { get; set; } = "Error";
        public Object data { get; set; } = null;
        public MetaData _metadata { get; set; } = new MetaData();


        public class MetaData
        {
            public long totalRecords { get; set; } = 0;
            public long totalPages { get; set; } = 0;
        }

        public bool GetStatus()
        {
            if (status == "success")
                return true;

            if (status == "error")
                return false;

            if (status == "fail")
                return false;
            
            return Convert.ToBoolean(status);
        }

        public void Success()
        {
            status = "success";
            message = "OK";
        }

        public void Error()
        {
            status = "fail";

            if (string.IsNullOrEmpty(message))
                message = "error some thing.";

        }
    }
}