namespace GunMobile.Net
{
    [System.Serializable]
    public sealed class GodCardSlot
    {
        public int Id;
        public int Count = 1;
    }

    [System.Serializable]
    public sealed class StockSlot
    {
        public int StockId;
        public int Shares;
        public int AvgPrice;
    }
}
