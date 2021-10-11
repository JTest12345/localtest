using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oskas
{
    public class OskasMqttClient
    {
        public void Mqtt_ServerStatus(uPLibrary.Networking.M2Mqtt.MqttClient client, string servername, bool serverOn)
        {
            //var client = new uPLibrary.Networking.M2Mqtt.MqttClient(mosquittoHost);
            //var ret = client.Connect(Guid.NewGuid().ToString());

            if (client.IsConnected)
            {
                string msg;
                if (serverOn)
                    msg = "On-line [" + DateTime.Now + "]";
                else
                    msg = "Off-line [" + DateTime.Now + "]";
                client.Publish(servername, Encoding.UTF8.GetBytes(msg), 0, true);
            }
        }

        public void Mqtt_Publog(uPLibrary.Networking.M2Mqtt.MqttClient client, string topicName, string msg)
        {
            //var client = new uPLibrary.Networking.M2Mqtt.MqttClient(mosquittoHost);
            //var ret = client.Connect(Guid.NewGuid().ToString());

            if (client.IsConnected)
            {
                client.Publish(topicName, Encoding.UTF8.GetBytes(msg), 0, true);
            }
        }
    }
}
