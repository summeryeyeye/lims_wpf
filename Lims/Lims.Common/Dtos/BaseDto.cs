using System.ComponentModel;

namespace Lims.Common.Dtos
{
    public class BaseDto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertiesChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public BaseDto? Clone()
        {
            return this.MemberwiseClone() as BaseDto;
        }
    }
}
