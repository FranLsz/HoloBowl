using Microsoft.WindowsAzure.Storage.Table;

namespace HoloBowlFn.Models
{
    public class Score : TableEntity
    {
        [IgnoreProperty]
        public string PlatformId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        [IgnoreProperty]
        public string PlayerName
        {
            get => RowKey;
            set => RowKey = value;
        }

        public int PlayerScore { get; set; }
    }
}
