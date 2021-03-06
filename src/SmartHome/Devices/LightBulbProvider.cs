﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SmartHome.Devices
{
    public class LightBulbProvider : DeviceTypeProvider
    {
        private const string DEVICE_TYPE = "IOT.SMARTBULB";
        private static LightBulbProvider s_instance;

        public override string DeviceType => DEVICE_TYPE;

        public override Task<Device> CreateDevice(RequestContext requestContext)
        {
            var device = new LightBulb();

            SetCommonDeviceProperties(device, requestContext);
            SetBulbProperties(device, requestContext.Data);

            UpdateBulbState(device as LightBulb, requestContext.Data.Value<JObject>("light_state"));

            return Task.FromResult<Device>(device);
        }

        public override Task<bool> UpdateDevice(Device device, RequestContext requestContext)
        {
            SetCommonDeviceProperties(device, requestContext);
            SetBulbProperties(device as LightBulb, requestContext.Data);

            UpdateBulbState(device as LightBulb, requestContext.Data.Value<JObject>("light_state"));

            return Task.FromResult(true);
        }

        private static bool SetBulbProperties(LightBulb device, JObject obj)
        {
            device.IsDimmable = Convert.ToBoolean(obj.Value<int>("is_dimmable"));
            device.IsColor = Convert.ToBoolean(obj.Value<int>("is_color"));
            device.IsVariableColorTemp = Convert.ToBoolean(obj.Value<int>("is_variable_color_temp"));

            return true;
        }

        private static void UpdateBulbState(LightBulb device, JObject obj)
        {
            LightBulbState p = device.State ?? new LightBulbState();

            p.Mode = obj.Value<string>("mode");
            p.PowerState = (SwitchState)obj.Value<int>("on_off");
            p.Hue = obj.Value<int>("hue");
            p.Saturation = obj.Value<int>("saturation");
            p.ColorTemp = obj.Value<int>("color_temp");
            p.Brightness = obj.Value<int>("brightness");

            device.State = p;
        }

        public static DeviceTypeProvider Instance => s_instance ?? (s_instance = new LightBulbProvider());
    }
}
