using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicArt.ViewModels
{
    public abstract class BindableModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Property changed event for observer pattern.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises event when a property is changed.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Sets the value of the given property if different from its current value. If the property's value does change, 
        /// the NotifyPropertyChanged event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the given property.</typeparam>
        /// <param name="storage">The backing field.</param>
        /// <param name="value">A new value, of Type T, to change the property to.</param>
        /// <param name="propertyName">The name of the Property whose value will change.</param>
        /// <returns>true if the value of the Property specified by propertyName was changed, false if not.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
    }
}
