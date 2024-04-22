namespace Bev.Instruments.Heidenhain
{
    public interface IHeidenhain
    {
        string InstrumentManufacturer { get; }
        string InstrumentType { get; }
        string InstrumentID { get; }
        double GetValue();
    }
}
