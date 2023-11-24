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
        public WXRspInnerData data { get; set; }
    }

    [Serializable]
    public class WXRspInnerData 
    {
        public string cloudID { get; set; }
        public Dictionary<string, string> data { get; set; }
    }
}
