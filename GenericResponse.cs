using System.Globalization;

namespace Bev.Instruments.Heidenhain
{
    public class GenericResponse
    {
        public GenericResponse(string textLine)
        {
            ParseInstrumentResponse(textLine);
        }

        public GenericResponse()
        {
            InvalidateProperties();
        }

        public double Value => ConvertToMm();
        public double NumericalValue { get; private set; }
        public string ResponseLine { get; private set; }
        public Unit MeasurementUnit { get; private set; }
        public Sign Sign { get; private set; }
        public Sorting SortingStatus { get; private set; }
        public MeasurementSeries SeriesStatus { get; private set; }

        private void ParseInstrumentResponse(string response)
        {
            InvalidateProperties();
            ResponseLine = StripCrLfFromString(response);
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (response.Length < 15)
                return;
            // parse the sign character
            string sSign = ResponseLine.Substring(0, 1);
            if (sSign == "-") Sign = Sign.Negative;
            if (sSign == "+") Sign = Sign.Positive;
            // "  -19.28081    "
            if (sSign == " ") Sign = Sign.Positive; // this is a hack for an old ND281B from BEV/E33
            // parse the unit character
            string sUnit = ResponseLine.Substring(12, 1);
            if (sUnit == " ") MeasurementUnit = Unit.MM;
            if (sUnit == "\"") MeasurementUnit = Unit.Inch;
            if (sUnit == "G") MeasurementUnit = Unit.Degree;
            if (sUnit == "M") MeasurementUnit = Unit.DMS;
            if (sUnit == "R") MeasurementUnit = Unit.Rad;
            if (sUnit == "?") MeasurementUnit = Unit.Fault;
            // parse numerical value
            string sValue = ResponseLine.Substring(1, 10);
            if (!double.TryParse(sValue, out double value))
                value = double.NaN;
            NumericalValue = value * ConvertSignToDouble(Sign);
            // parse sorting status (Klassierzustand)
            string sSort = ResponseLine.Substring(13, 1);
            if (sSort == "<") SortingStatus = Sorting.Below;
            if (sSort == ">") SortingStatus = Sorting.Above;
            if (sSort == "=") SortingStatus = Sorting.Inside;
            if (sSort == "?") SortingStatus = Sorting.Fault;
            // parse series status (Messreihe)
            string sSeries = ResponseLine.Substring(14, 1);
            if (sSeries == "A") SeriesStatus = MeasurementSeries.Actual;
            if (sSeries == "S") SeriesStatus = MeasurementSeries.Min; 
            if (sSeries == "G") SeriesStatus = MeasurementSeries.Max;
            if (sSeries == "D") SeriesStatus = MeasurementSeries.Diff;
            if (sSeries == " ") SeriesStatus = MeasurementSeries.Actual;
        }

        private double ConvertSignToDouble(Sign sign)
        {
            if (sign == Sign.Positive) return 1;
            if (sign == Sign.Negative) return -1;
            return double.NaN;
        }

        // TODO include conversion for angle values
        private double ConvertToMm()
        {
            if (MeasurementUnit == Unit.Inch)
                return NumericalValue * 25.4;
            return NumericalValue;
        }

        private string StripCrLfFromString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return str.Replace("\n", "").Replace("\r", "");
        }

        private void InvalidateProperties()
        {
            NumericalValue = double.NaN;
            ResponseLine = string.Empty;
            MeasurementUnit = Unit.None;
            Sign = Sign.None;
            SortingStatus = Sorting.None;
            SeriesStatus = MeasurementSeries.None;
        }

        public override string ToString()
        {
            return $"[{GetType().Name}: ResponseLine=\"{ResponseLine}\" NumericalValue={NumericalValue} MeasurementUnit={MeasurementUnit}]";
        }

    }

    public enum Unit
    {
        None,
        MM,
        Inch,
        Degree,
        DMS,
        Rad,
        Fault
    }

    public enum Sign
    {
        None,
        Positive,
        Negative,
        Fault
    }

    public enum Sorting // not for ND280 ?
    {
        None,
        Above,
        Below,
        Inside,
        Fault
    }

    public enum MeasurementSeries // not for ND280 ?
    {
        None,
        Min,
        Actual,
        Max,
        Diff,
        Fault
    }
}
