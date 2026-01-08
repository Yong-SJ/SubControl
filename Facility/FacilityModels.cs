using System;
using System.Drawing;

namespace SubControl.Facility
{
    public enum DeviceType { Printer, Furnace, Polisher, Cleaner }

    [Serializable]
    public class DeviceModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; } = "New Device";
        public DeviceType Type { get; set; } = DeviceType.Printer;

        public Point Location { get; set; } = new Point(40, 40);
        public Size Size { get; set; } = new Size(140, 60);

        public string ServerIp { get; set; }
        public bool Connected { get; set; }
        public string Status { get; set; } = "Idle";
        public int ServerPort { get; set; } = 5000;
    }
}