using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace WebsiteBanHang.Hubs
{
    public class EchoHub : Hub
    {

        public void Echo(string message)
        {
            Clients.All.SendAsync("Send", message);
            
        }
        public void Print()
        {
            Clients.All.SendAsync("Send", "hello");
        }
        
    }
    public class NotifyChange
    {
        private readonly IHubContext<EchoHub> _hubContext;

        public NotifyChange(IHubContext<EchoHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public int GetStock(int id)
        {
            using (var conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=SaleDB;Trusted_Connection=True;MultipleActiveResultSets=true"))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT Stock FROM [dbo].Products WHERE ProductId ="+id, conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);

                    void handler(object sender, SqlNotificationEventArgs e)
                    {
                        if (e.Type == SqlNotificationType.Change)
                        {
                            SqlDependency de = (SqlDependency)sender;
                            de.OnChange -= handler;
                            _hubContext.Clients.All.SendAsync("stockproduct"+id.ToString(), GetStock(id));
                        }
                    }
                    
                    dependency.OnChange += handler;

                    //dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (conn.State == System.Data.ConnectionState.Closed)
                        conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        //private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        //{
        //    if (e.Type == SqlNotificationType.Change)
        //    {
        //        _hubContext.Clients.All.SendAsync("Send", "babab");
                
        //    }

        //}
    }
}
