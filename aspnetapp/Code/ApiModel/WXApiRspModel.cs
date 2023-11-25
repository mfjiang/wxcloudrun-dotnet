namespace aspnetapp.Code.ApiModel
{
    [Serializable]
    public class WXApiRspModel
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }
        public List<WXRspData> data_list { get; set; }
    }

    [Serializable]
    public class WXRspData 
    {
        public string cloud_id { get; set; }
        public string json { get; set; }
        public WXRspInnerJsonObj JsonObj { get; set; }
        public dynamic data { get; set; }
    }

    public class WXRspInnerJsonObj
    { 
        public string cloudID { get; set; }
        public WXRspInnerJsonObj_Data data { get; set; }
    }

    public class WXRspInnerJsonObj_Data
    { 
        public string opengid { get; set; }
    }
}
