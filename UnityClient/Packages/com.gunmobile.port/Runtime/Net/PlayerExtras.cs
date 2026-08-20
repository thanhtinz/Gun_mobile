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

    [System.Serializable]
    public sealed class FightSpiritSlot
    {
        public int SpiritId;
        public int Level;
    }

    [System.Serializable]
    public sealed class MagicStoneSlot
    {
        public int TemplateId;
        public int Level;
    }

    [System.Serializable]
    public sealed class AuctionListing
    {
        public int Id;
        public int SellerId;
        public string SellerNick = "";
        public int TemplateId;
        public int Count = 1;
        public int Price;
        public int Strengthen;
    }
}
