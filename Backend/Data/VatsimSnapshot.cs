namespace ZoaIdsBackend.Data
{
    public class VatsimSnapshot
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public string RawJson { get; set; }
    }
}
