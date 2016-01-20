using System;
using System.ComponentModel;

namespace MyHomeAutomation.Model
{
    /// <summary>
    /// Device model. Stores basic details about individual devices, retreived from the DeviceStore.
    /// Implements INotifyPropertyChanged to allow two-way data-binding at the UI layer.
    /// </summary>
    public class Device : INotifyPropertyChanged
    {
        private string idx;
        private string name;
        private string description;
        private string deviceType;
        private DateTime? addDate;
        private DateTime? lastSeenDate;

        private int used;
        private int favorite;
        private string planId;
        private string image;
        private int order;

        private string status;
        private double temp;
        private int humidity;
        private int unit;

        // Switch Type
        private string switchType;
        private int switchTypeVal;
        // Switch details
        private bool switchProtected;

        // For a Dimmer
        private int level;
        private int levelInt;
        private int maxDimLevel;

        private bool showOnDash;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Unique identifier of a Device.
        /// </summary>
        public string Idx
        {
            get
            {
                return idx;
            }
            set
            {
                idx = value;
                NotifyPropertyChanged("Idx");
            }
        }

        /// <summary>
        /// Name of a device. Should not be an empty string.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Description of a device. Shown to users as a basic description in the UI.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                NotifyPropertyChanged("Description");
            }
        }

        /// <summary>
        /// Device Type. Should not be an empty string.
        /// </summary>
        public string DeviceType
        {
            get
            {
                return deviceType;
            }
            set
            {
                deviceType = value;
                NotifyPropertyChanged("DeviceType");
            }
        }

        /// <summary>
        /// Date the device was added.
        /// </summary>
        public DateTime? AddDate
        {
            get
            {
                return addDate;
            }
            set
            {
                addDate = value;
                NotifyPropertyChanged("AddDate");
            }
        }

        /// <summary>
        /// Date the device was last seen, also known as lastUpdate.
        /// </summary>
        public DateTime? LastSeenDate
        {
            get
            {
                return lastSeenDate;
            }
            set
            {
                lastSeenDate = value;
                NotifyPropertyChanged("LastSeenDate");
            }
        }

        /// <summary>
        /// Is the device used by domoticz
        /// </summary>
        public int Used
        {
            get
            {
                return used;
            }
            set
            {
                used = value;
                NotifyPropertyChanged("Used");
            }
        }

        /// <summary>
        /// Plan Id of the device
        /// </summary>
        public string PlanId
        {
            get
            {
                return planId;
            }
            set
            {
                planId = value;
                NotifyPropertyChanged("PlanId");
            }
        }

        /// <summary>
        /// Image used by the device
        /// </summary>
        public string Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                NotifyPropertyChanged("Image");
            }
        }

        /// <summary>
        /// The order Domoticz shows devices in
        /// </summary>
        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
                NotifyPropertyChanged("Order");
            }
        }

        /// <summary>
        /// Is the device marked as a favorite in domotizc
        /// </summary>
        public int Favorite
        {
            get
            {
                return favorite;
            }
            set
            {
                favorite = value;
                NotifyPropertyChanged("Favorite");
            }
        }

        /// <summary>
        /// Status of the device.
        /// </summary>
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        /// <summary>
        /// Temperature of the device
        /// </summary>
        public double Temp
        {
            get
            {
                return temp;
            }
            set
            {
                temp = value;
                NotifyPropertyChanged("Temp");
            }
        }

        /// <summary>
        /// Humidity of the device
        /// </summary>
        public int Humidity
        {
            get
            {
                return humidity;
            }
            set
            {
                humidity = value;
                NotifyPropertyChanged("Humidity");
            }
        }

        /// <summary>
        /// Unit used by the device
        /// </summary>
        public int Unit
        {
            get
            {
                return unit;
            }
            set
            {
                unit = value;
                NotifyPropertyChanged("Unit");
            }
        }

        /// <summary>
        /// SwitchType of the device
        /// </summary>
        public string SwitchType
        {
            get
            {
                return switchType;
            }
            set
            {
                switchType = value;
                NotifyPropertyChanged("SwitchType");
            }
        }

        /// <summary>
        /// SwitchTypeVal of the device
        /// </summary>
        public int SwitchTypeVal
        {
            get
            {
                return switchTypeVal;
            }
            set
            {
                switchTypeVal = value;
                NotifyPropertyChanged("SwitchTypeVal");
            }
        }

        /// <summary>
        /// Is the switch protected (If so it requires a pin to be flipped.)
        /// </summary>
        public bool SwitchProtected
        {
            get
            {
                return switchProtected;
            }
            set
            {
                switchProtected = value;
                NotifyPropertyChanged("SwitchProtected");
            }
        }

        /// <summary>
        /// Level (60% for a dimmer that is at 60%)of the device
        /// </summary>
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                NotifyPropertyChanged("Level");
            }
        }

        /// <summary>
        /// LevelInt (60 for a dimmer that is at 60% and a maxDimLevel of 100 and 9 (60 devided by (100/15)) for a maxDimLevel of 15 ) of the device
        /// </summary>
        public int LevelInt
        {
            get
            {
                return levelInt;
            }
            set
            {
                levelInt = value;
                NotifyPropertyChanged("LevelInt");
            }
        }

        /// <summary>
        /// MaxDimLevel (Mostly 100 or 15) of the device
        /// </summary>
        public int MaxDimLevel
        {
            get
            {
                return maxDimLevel;
            }
            set
            {
                maxDimLevel = value;
                NotifyPropertyChanged("MaxDimLevel");
            }
        }

        /// <summary>
        /// Show the device on the Dashboard (this overrules favorites).
        /// </summary>
        public bool ShowOnDash
        {
            get
            {
                return showOnDash;
            }
            set
            {
                showOnDash = value;
                NotifyPropertyChanged("ShowOnDash");
            }
        }

        /// <summary>
        /// Notify any subscribers to the INotifyPropertyChanged interface that a property
        /// was updated. This allows the UI to automatically update (for instance, if Cortana
        /// triggers an update to a device, or removal of a device).
        /// </summary>
        /// <param name="propertyName">The case-sensitive name of the property that was updated.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                handler(this, args);
            }
        }
    }
}