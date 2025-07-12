namespace Lims.WPF.Resources.Converters
{
    public interface IEventArgsConverter
    {
        object Convert(object sender, object args);
    }

    public abstract class EventArgsConverterBase<TArgs> : IEventArgsConverter
    {
        private EventArgsConverterBase()
        {
        }

        public abstract object Convert(object sender, object args);
    }
}