using MyHomeAutomation.JSON;
using MyHomeAutomation.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace MyHomeAutomation.Common
{
    public class UpdateDeviceStore
    {
        private ObservableCollection<Device> devices;
        private Device device;
        private DeviceStore store;

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

        //JSON
        private const string statusKey = "status";
        private const string sunriseKey = "Sunrise";
        private const string sunsetKey = "Sunset";
        private const string resultKey = "result";

        private const string typeKey = "Type";

        private ObservableCollection<Sensor> sensors;
        private ObservableCollection<Switch> switches;
        //JSON End

        /// <summary>
        /// Construct the device view, passing in the persistent device store. Sets up
        /// a command to handle invoking the Add button.
        /// </summary>
        /// <param name="store"></param>
        public UpdateDeviceStore(DeviceStore store)
        {
            this.store = store;
            Devices = store.Devices;
        }

        /// <summary>
        /// The list of devices to display on the UI.
        /// </summary>
        public ObservableCollection<Device> Devices
        {
            get
            {
                return devices;
            }
            private set
            {
                devices = value;
                //NotifyPropertyChanged("Devices");
            }
        }

        /// <summary>
        /// The Device this view model represents.
        /// </summary>
        private Device Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
                //NotifyPropertyChanged("Device");
            }
        }

        private void ProcessDeviceOverview(string jsonString)
        {
            // Prevent Memory Leak
            sensors = new ObservableCollection<Sensor>();
            switches = new ObservableCollection<Switch>();

            JsonObject jsonObject = JsonObject.Parse(jsonString);
            localSettings.Values["domoticzStatus"] = jsonObject.GetNamedString(statusKey, "");
            localSettings.Values["domoticzSunrise"] = jsonObject.GetNamedString(sunriseKey, "");
            localSettings.Values["domoticzSunset"] = jsonObject.GetNamedString(sunsetKey, "");

            foreach (IJsonValue jsonValue in jsonObject.GetNamedArray(resultKey, new JsonArray()))
            {
                if (jsonValue.ValueType == JsonValueType.Object)
                {
                    var tempJsonObject = jsonValue.GetObject();
                    if (tempJsonObject.GetNamedString(typeKey, "") == "Temp" || tempJsonObject.GetNamedString(typeKey, "") == "Temp + Humidity"
                        || tempJsonObject.GetNamedString(typeKey, "") == "Humidity" || tempJsonObject.GetNamedString(typeKey, "") == "General"
                        || tempJsonObject.GetNamedString(typeKey, "") == "Current" || tempJsonObject.GetNamedString(typeKey, "") == "RFXMeter"
                        || tempJsonObject.GetNamedString(typeKey, "") == "Rain" || tempJsonObject.GetNamedString(typeKey, "") == "UV"
                        || tempJsonObject.GetNamedString(typeKey, "") == "Wind" || tempJsonObject.GetNamedString(typeKey, "") == "Usage")
                    {
                        Sensors.Add(new Sensor(jsonValue.GetObject()));
                    }
                    else if (tempJsonObject.GetNamedString(typeKey, "") == "Lighting 1" || tempJsonObject.GetNamedString(typeKey, "") == "Lighting 2"
                             || tempJsonObject.GetNamedString(typeKey, "") == "Scene" || tempJsonObject.GetNamedString(typeKey, "") == "Group"
                             || tempJsonObject.GetNamedString(typeKey, "") == "Lighting Limitless/ Applamp"
                             || tempJsonObject.GetNamedString(typeKey, "") == "Lighting Limitless/Applamp" || tempJsonObject.GetNamedString(typeKey, "") == "Light/Switch")
                    {
                        Switches.Add(new Switch(jsonValue.GetObject()));
                    }
                }
            }
        }

        // Update Devices, updateType decides what to update.
        // 0 = All
        // 1 = Switches and Sensors
        // 2 = Rooms
        public async Task UpdateDevices(int updateType = 0)
        {
            if (!(bool)localSettings.Values["refreshRunning"])
            {
                localSettings.Values["refreshRunning"] = true;
                if (updateType == 0 || updateType == 1)
                {
                    URLActions domoticzData = new URLActions();
                    string result = await domoticzData.LoadAllUsedDevices();

                    if (result != null && result != "false")
                    {
                        ProcessDeviceOverview(result);
                        localSettings.Values["hostSettingsLoadedSuccesfully"] = true;
                    }
                    else
                    {
                        if (localSettings.Values["connectionFailedAmount"] == null && (int)localSettings.Values["connectionFailedAmount"] < 3)
                        {
                            bool connectionStatus = await domoticzData.TestConnection();
                            if (connectionStatus)
                            {
                                result = await domoticzData.LoadAllUsedDevices();
                                if (result != null && result != "false")
                                {
                                    ProcessDeviceOverview(result);
                                    localSettings.Values["hostSettingsLoadedSuccesfully"] = true;
                                }
                                else
                                {
                                    localSettings.Values["connectionFailedAmount"] = (int)localSettings.Values["connectionFailedAmount"] + 1;
                                    localSettings.Values["refreshRunning"] = false;
                                    return;
                                }
                            }
                            else
                            {
                                localSettings.Values["refreshRunning"] = false;
                                return;
                            }
                        }
                        else
                        {
                            localSettings.Values["refreshRunning"] = false;
                            return;
                        }
                    }

                    int deviceOrder = 0;

                    // Update Switches
                    foreach (Switch Switch in Switches)
                    {
                        deviceOrder++;
                        bool valid = true;

                        if (string.IsNullOrEmpty(Switch.Name))
                        {
                            valid = false;
                        }
                        else
                        {
                            Device t = store.Devices.Where(p => p.Idx == Switch.Idx.Trim()).FirstOrDefault();

                            if (t == null)
                            {
                                Device = new Device();
                                Device.Idx = Switch.Idx.Trim();
                                Device.Name = Switch.Name.Trim();
                                Device.Description = Switch.Name.Trim();
                                Device.DeviceType = Switch.Type.Trim();
                                Device.AddDate = DateTime.Now;
                                Device.LastSeenDate = Convert.ToDateTime(Switch.LastUpdate);

                                Device.Used = Switch.Used;
                                Device.Favorite = Switch.Favorite;

                                Device.PlanId = Switch.PlanId;
                                Device.Image = Switch.Image;
                                Device.Order = deviceOrder;

                                Device.Status = Switch.Status;
                                Device.Unit = Switch.Unit;

                                Device.SwitchType = Switch.SwitchType;
                                Device.SwitchTypeVal = Switch.SwitchTypeVal;

                                if (Switch.SwitchTypeVal == 7)
                                {
                                    Device.Level = Switch.Level;
                                    Device.LevelInt = Switch.LevelInt;
                                    Device.MaxDimLevel = Switch.MaxDimLevel;
                                }

                                Device.SwitchProtected = Switch.IsProtected;

                                if (valid)
                                {
                                    await store.SaveDevice(Device);
                                }
                            }
                            else
                            {
                                t.Name = Switch.Name.Trim();
                                t.Description = Switch.Name.Trim();

                                t.LastSeenDate = Convert.ToDateTime(Switch.LastUpdate);

                                t.Used = Switch.Used;
                                t.Favorite = Switch.Favorite;

                                t.PlanId = Switch.PlanId;
                                t.Image = Switch.Image;
                                t.Order = deviceOrder;

                                t.Status = Switch.Status;
                                t.Unit = Switch.Unit;

                                t.SwitchType = Switch.SwitchType;
                                t.SwitchTypeVal = Switch.SwitchTypeVal;

                                if (Switch.SwitchTypeVal == 7)
                                {
                                    t.Level = Switch.Level;
                                    t.LevelInt = Switch.LevelInt;
                                    t.MaxDimLevel = Switch.MaxDimLevel;
                                }

                                t.SwitchProtected = Switch.IsProtected;

                                await store.SaveDevice(t);
                            }

                        }
                    }
                    switches = null;

                    // Update Temperature + Humidity Devices
                    foreach (Sensor Sensor in Sensors)
                    {
                        deviceOrder++;
                        bool valid = true;

                        if (string.IsNullOrEmpty(Sensor.Name))
                        {
                            valid = false;
                        }
                        else
                        {
                            Device t = store.Devices.Where(p => p.Idx == Sensor.Idx.Trim()).FirstOrDefault();

                            if (t == null)
                            {
                                Device = new Device();
                                Device.Idx = Sensor.Idx.Trim();
                                Device.Name = Sensor.Name.Trim();
                                Device.Description = Sensor.Name.Trim();
                                Device.DeviceType = Sensor.Type.Trim();
                                Device.AddDate = DateTime.Now;
                                Device.LastSeenDate = Convert.ToDateTime(Sensor.LastUpdate);
                                Device.Unit = Sensor.Unit;
                                Device.Used = Sensor.Used;
                                Device.Favorite = Sensor.Favorite;
                                Device.Order = deviceOrder;

                                if (Sensor.Type == "Temp")
                                {
                                    Device.Temp = Sensor.Temp;
                                }
                                if (Sensor.Type == "Humidity")
                                {
                                    Device.Humidity = Sensor.Humidity;
                                }
                                else if (Sensor.Type == "Temp + Humidity")
                                {
                                    Device.Temp = Sensor.Temp;
                                    Device.Humidity = Sensor.Humidity;
                                }
                                else if (Sensor.Type == "Rain" || Sensor.Type == "UV" || Sensor.Type == "Wind")
                                {
                                    Device.Data = Sensor.Data;
                                }
                                else if (Sensor.Type == "General" || Sensor.Type == "Current" || Sensor.Type == "RFXMeter" || Sensor.Type == "Usage")
                                {
                                    Device.SubType = Sensor.SubType;
                                    Device.Data = Sensor.Data;
                                }

                                if (valid)
                                {
                                    await store.SaveDevice(Device);
                                }
                            }
                            else
                            {
                                t.Name = Sensor.Name.Trim();
                                t.Description = Sensor.Name.Trim();

                                t.LastSeenDate = Convert.ToDateTime(Sensor.LastUpdate);

                                t.Used = Sensor.Used;
                                t.Favorite = Sensor.Favorite;
                                t.Order = deviceOrder;
                                t.Unit = Sensor.Unit;

                                if (Sensor.Type == "Temp")
                                {
                                    t.Temp = Sensor.Temp;
                                }
                                if (Sensor.Type == "Humidity")
                                {
                                    t.Humidity = Sensor.Humidity;
                                }
                                else if (Sensor.Type == "Temp + Humidity")
                                {
                                    t.Temp = Sensor.Temp;
                                    t.Humidity = Sensor.Humidity;
                                }
                                else if (Sensor.Type == "Rain" || Sensor.Type == "UV" || Sensor.Type == "Wind")
                                {
                                    t.Data = Sensor.Data;
                                }
                                else if (Sensor.Type == "General" || Sensor.Type == "Current" || Sensor.Type == "RFXMeter" || Sensor.Type == "Usage")
                                {
                                    t.SubType = Sensor.SubType;
                                    t.Data = Sensor.Data;
                                }

                                await store.SaveDevice(t);
                            }

                        }
                    }
                    sensors = null;
                }

                if (updateType == 0 || updateType == 2)
                {
                    //
                    // Load Rooms
                    //
                    URLActions domoticzData = new URLActions();
                    string result = await domoticzData.LoadRooms();

                    if (result != null && result != "false")
                    {
                        localSettings.Values["availableRooms"] = result;
                    }

                    result = null;
                }

                localSettings.Values["refreshRunning"] = false;
            }
        }

        private ObservableCollection<Switch> Switches
        {
            get
            {
                return switches;
            }
        }

        private ObservableCollection<Sensor> Sensors
        {
            get
            {
                return sensors;
            }
        }

    }
}