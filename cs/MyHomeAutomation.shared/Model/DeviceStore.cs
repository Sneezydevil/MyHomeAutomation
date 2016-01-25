using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;

namespace MyHomeAutomation.Model
{
    /// <summary>
    /// Persistance layer for Devices created with MyHomeAutomation. Writes out and retreives devices
    /// from a simple xml format like
    /// <root>
    ///  <Device>
    ///   <Idx></Idx>
    ///   <Name></Name>
    ///   <Description></Description>
    ///   <DeviceType></DeviceType>
    ///   <AddDate></AddDate>
    ///   <LastSeenDate></LastSeenDate>
    ///   <Used></Used>
    ///   <Favorite></Favorite>
    ///   <PlanId></PlanId>
    ///   <Image></Image>
    ///   <Order></Order>
    ///   <Status></Status>
    ///   <Temp></Temp>
    ///   <Humidity></Humidity>
    ///   <Unit></Unit>
    ///   <SwitchType></SwitchType>
    ///   <SwitchTypeVal></SwitchTypeVal>
    ///   <SwitchProtected></SwitchProtected>
    ///   <Level></Level>
    ///   <LevelInt></LevelInt>
    ///   <MaxDimLevel></MaxDimLevel>
    ///   <SubType></SubType>
    ///   <Data></Data>
    ///   <ShowOnDash></ShowOnDash>
    ///  </Device>
    ///  <Device>
    ///   ....
    ///  </Device>
    /// </root>
    /// </summary>
    public class DeviceStore
    {
        private bool loaded;

        /// <summary>
        /// Persist the loaded devices in memory for use in other parts of the application.
        /// </summary>
        private ObservableCollection<Device> devices;

        public DeviceStore()
        {
            loaded = false;
            Devices = new ObservableCollection<Device>();
        }

        /// <summary>
        /// Persisted devices, reloaded across executions of the application
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
            }
        }

        /// <summary>
        /// Load devices from a file on first-launch of the app. If the file does not yet exist,
        /// pre-seed it with a device, in order to give the app demonstration data.
        /// </summary>
        public async Task LoadDevices()
        {
            // Ensure that we don't load device data more than once.
            if (loaded)
            {
                return;
            }
            loaded = true;

            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            this.devices.Clear();

            var item = await folder.TryGetItemAsync("devices.xml");
            if (item == null)
            {
                // Add some 'starter' devices
                devices.Add(
                    new Device()
                    {
                        Idx = "1",
                        Name = "Example Device",
                        Description = "Light above TV",
                        DeviceType = "Lighting 2",
                        AddDate = new DateTime(2015, 12, 12),
                        LastSeenDate = new DateTime(2015, 12, 12)
                    });
                await WriteDevices();
                return;
            }

            // Load devices out of a simple XML format.
            if (item.IsOfType(StorageItemTypes.File))
            {
                StorageFile deviceFile = item as StorageFile;

                string deviceXmlText = await FileIO.ReadTextAsync(deviceFile);

                try
                {
                    XElement xmldoc = XElement.Parse(deviceXmlText);

                    var deviceElements = xmldoc.Descendants("Device");
                    foreach (var deviceElement in deviceElements)
                    {
                        Device device = new Device();

                        var idxElement = deviceElement.Descendants("Idx").FirstOrDefault();
                        if (idxElement != null)
                        {
                            device.Idx = idxElement.Value;
                        }

                        var nameElement = deviceElement.Descendants("Name").FirstOrDefault();
                        if (nameElement != null)
                        {
                            device.Name = nameElement.Value;
                        }

                        var descElement = deviceElement.Descendants("Description").FirstOrDefault();
                        if (descElement != null)
                        {
                            device.Description = descElement.Value;
                        }

                        var devTypeElement = deviceElement.Descendants("DeviceType").FirstOrDefault();
                        if (devTypeElement != null)
                        {
                            device.DeviceType = devTypeElement.Value;
                        }

                        var addElement = deviceElement.Descendants("AddDate").FirstOrDefault();
                        if (addElement != null)
                        {
                            DateTime addDate;
                            if (DateTime.TryParse(addElement.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out addDate))
                            {
                                device.AddDate = addDate;
                            }
                            else
                            {
                                device.AddDate = null;
                            }
                        }

                        var lastSeenElement = deviceElement.Descendants("LastSeenDate").FirstOrDefault();
                        if (lastSeenElement != null)
                        {
                            DateTime lastSeenDate;
                            if (DateTime.TryParse(lastSeenElement.Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out lastSeenDate))
                            {
                                device.LastSeenDate = lastSeenDate;
                            }
                            else
                            {
                                device.LastSeenDate = null;
                            }
                        }

                        var usedElement = deviceElement.Descendants("Used").FirstOrDefault();
                        if (usedElement != null)
                        {
                            device.Used = Int32.Parse(usedElement.Value);
                        }

                        var favoriteElement = deviceElement.Descendants("Favorite").FirstOrDefault();
                        if (favoriteElement != null)
                        {
                            device.Favorite = Int32.Parse(favoriteElement.Value);
                        }

                        var planIdElement = deviceElement.Descendants("PlanId").FirstOrDefault();
                        if (planIdElement != null)
                        {
                            device.PlanId = planIdElement.Value;
                        }

                        var imageElement = deviceElement.Descendants("Image").FirstOrDefault();
                        if (imageElement != null)
                        {
                            device.Image = imageElement.Value;
                        }

                        var orderElement = deviceElement.Descendants("Order").FirstOrDefault();
                        if (orderElement != null)
                        {
                            device.Order = Int32.Parse(orderElement.Value);
                        }

                        var statusElement = deviceElement.Descendants("Status").FirstOrDefault();
                        if (statusElement != null)
                        {
                            device.Status = statusElement.Value;
                        }

                        var tempElement = deviceElement.Descendants("Temp").FirstOrDefault();
                        if (tempElement != null)
                        {
                            device.Temp = Double.Parse(tempElement.Value);
                        }

                        var humidityElement = deviceElement.Descendants("Humidity").FirstOrDefault();
                        if (humidityElement != null)
                        {
                            device.Humidity = Int32.Parse(humidityElement.Value);
                        }

                        var unitElement = deviceElement.Descendants("Unit").FirstOrDefault();
                        if (unitElement != null)
                        {
                            device.Unit = Int32.Parse(unitElement.Value);
                        }

                        var switchTypeElement = deviceElement.Descendants("SwitchType").FirstOrDefault();
                        if (switchTypeElement != null)
                        {
                            device.SwitchType = switchTypeElement.Value;
                        }

                        var switchProtectedElement = deviceElement.Descendants("SwitchProtected").FirstOrDefault();
                        if (switchProtectedElement != null)
                        {
                            device.SwitchProtected = bool.Parse(switchProtectedElement.Value);
                        }

                        var switchTypeValElement = deviceElement.Descendants("SwitchTypeVal").FirstOrDefault();
                        if (switchTypeValElement != null)
                        {
                            device.SwitchTypeVal = Int32.Parse(switchTypeValElement.Value);
                        }

                        var levelElement = deviceElement.Descendants("Level").FirstOrDefault();
                        if (levelElement != null)
                        {
                            device.Level = Int32.Parse(levelElement.Value);
                        }

                        var levelIntElement = deviceElement.Descendants("LevelInt").FirstOrDefault();
                        if (levelIntElement != null)
                        {
                            device.LevelInt = Int32.Parse(levelIntElement.Value);
                        }

                        var maxDimLevelElement = deviceElement.Descendants("MaxDimLevel").FirstOrDefault();
                        if (maxDimLevelElement != null)
                        {
                            device.MaxDimLevel = Int32.Parse(maxDimLevelElement.Value);
                        }

                        var subTypeElement = deviceElement.Descendants("SubType").FirstOrDefault();
                        if (subTypeElement != null)
                        {
                            device.SubType = subTypeElement.Value;
                        }

                        var dataElement = deviceElement.Descendants("Data").FirstOrDefault();
                        if (dataElement != null)
                        {
                            device.Data = dataElement.Value;
                        }

                        var showOnDashElement = deviceElement.Descendants("ShowOnDash").FirstOrDefault();
                        if (showOnDashElement != null)
                        {
                            device.ShowOnDash = bool.Parse(showOnDashElement.Value);
                        }

                        Devices.Add(device);
                    }
                }
                catch (XmlException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    return;
                }

            }
        }

        /// <summary>
        /// Delete a device from the persistent device store, and save the devices file.
        /// </summary>
        /// <param name="device">The device to delete. If the device is not an existing device in the store,
        /// will not have an effect.</param>
        public async Task DeleteDevice(Device device)
        {
            Devices.Remove(device);
            await WriteDevices();
        }

        /// <summary>
        /// Add a device to the persistent device store, and saves the devices data file.
        /// </summary>
        /// <param name="device">The device to save or update in the data file.</param>
        public async Task SaveDevice(Device device)
        {
            if (!Devices.Contains(device))
            {
                Devices.Add(device);
            }

            await WriteDevices();
        }

        /// <summary>
        /// Write out a new XML file, overwriting the existing one if it already exists
        /// with the currently persisted devices. See class comment for basic format.
        /// </summary>
        private async Task WriteDevices()
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            XElement xmldoc = new XElement("Root");

            StorageFile devicesFile;

            var item = await folder.TryGetItemAsync("devices.xml");
            if (item == null)
            {
                devicesFile = await folder.CreateFileAsync("devices.xml");
            }
            else
            {
                devicesFile = await folder.GetFileAsync("devices.xml");
            }

            foreach (var device in Devices)
            {
                string addDateField = null;
                if (device.AddDate.HasValue)
                {
                    addDateField = device.AddDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                string lastSeenDateField = null;
                if (device.LastSeenDate.HasValue)
                {
                    lastSeenDateField = device.LastSeenDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                xmldoc.Add(
                    new XElement("Device",
                    new XElement("Idx", device.Idx),
                    new XElement("Name", device.Name),
                    new XElement("Description", device.Description),
                    new XElement("DeviceType", device.DeviceType),
                    new XElement("AddDate", addDateField),
                    new XElement("LastSeenDate", lastSeenDateField),
                    new XElement("Used", device.Used),
                    new XElement("Favorite", device.Favorite),
                    new XElement("PlanId", device.PlanId),
                    new XElement("Image", device.Image),
                    new XElement("Order", device.Order),
                    new XElement("Status", device.Status),
                    new XElement("Temp", device.Temp),
                    new XElement("Humidity", device.Humidity),
                    new XElement("Unit", device.Unit),
                    new XElement("SwitchType", device.SwitchType),
                    new XElement("SwitchTypeVal", device.SwitchTypeVal),
                    new XElement("SwitchProtected", device.SwitchProtected),
                    new XElement("Level", device.Level),
                    new XElement("LevelInt", device.LevelInt),
                    new XElement("MaxDimLevel", device.MaxDimLevel),
                    new XElement("SubType", device.SubType),
                    new XElement("Data", device.Data),
                    new XElement("ShowOnDash", device.ShowOnDash)
                    ));
            }

            await FileIO.WriteTextAsync(devicesFile, xmldoc.ToString());
        }
    }
}