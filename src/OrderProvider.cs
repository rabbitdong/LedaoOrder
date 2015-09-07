using LedaoOrder.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LedaoOrder
{
    public class OrderProvider
    {
        /// <summary>
        /// 获取订单列表信息
        /// </summary>
        /// <param name="status"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="countEachPage"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<OrderItem> GetPagedOrders(OrderStatus status, int currentPageIndex, int countEachPage, out int totalCount)
        {
            totalCount = GetResouces<int>("http://gladtao.sinaapp.com/views/a/order.php?r=jsonGetCount&status=" + (int)status);

            string getUrl = string.Format("http://gladtao.sinaapp.com/views/a/order.php?r=jsonOrdered&pi={0}&pc={1}&status={2}", currentPageIndex, countEachPage, (int)status);
            List<jsonorder> jsonorders = GetResouces<List<jsonorder>>(getUrl);

            List<OrderItem> orders = new List<OrderItem>();
            foreach (jsonorder order in jsonorders)
            {
                orders.Add(new OrderItem
                {
                    OrderID = order.orderID,
                    Address = order.address,
                    OrderTime = order.orderTime,
                    Phone = order.phone,
                    Receiver = order.receiver,
                    Remark = order.remark,
                    Status = order.status,
                    TotalPrice = order.totalPrice,
                    OrderDetails = null,
                    UserName = order.userName
                });
            }

            return orders;
        }

        /// <summary>
        /// 获取订单的详细信息
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static OrderItem GetOrder(int orderID)
        {
            jsonorder jsonorder = GetResouces<jsonorder>("http://gladtao.sinaapp.com/views/a/order.php?r=jsonOrderDetail&oid=" + orderID);

            OrderItem order = new OrderItem
            {
                OrderID = jsonorder.orderID,
                Address = jsonorder.address,
                OrderTime = jsonorder.orderTime,
                Phone = jsonorder.phone,
                Receiver = jsonorder.receiver,
                Remark = jsonorder.remark,
                Status = jsonorder.status,
                TotalPrice = jsonorder.totalPrice,
                UserName = jsonorder.userName
            };

            order.OrderDetails = new List<OrderDetailItem>();
            foreach (jsonorderdetail detail in jsonorder.detailItems)
            {
                order.OrderDetails.Add(new OrderDetailItem
                {
                     ProductName = detail.ProductName,
                     ProducingArea = detail.ProducingArea,
                     Amount = detail.Amount,
                     UnitPrice = detail.UnitPrice,
                     Unit = detail.Unit
                });
            }

            return order;
        }

        /// <summary>
        /// 对订单进行发货处理
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static bool SendOrder(int orderID)
        {
            bool result = GetResouces<bool>("http://gladtao.sinaapp.com/views/a/order.php?r=jsonSend&oid=" + orderID);
            return result;
        }

        public static T GetResouces<T>(string url) where T : new()
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string result = client.DownloadString(new Uri(url));
            return JsonConvert.DeserializeAnonymousType<T>(result, new T()) ;
        }


        class jsonorder
        {
            public int orderID { get; set; }
            public string userName { get; set; }
            public OrderStatus status { get; set; }
            public string address { get; set; }
            public string receiver { get; set; }
            public DateTime orderTime { get; set; }
            public string phone { get; set; }
            public decimal totalPrice { get; set; }
            public string remark { get; set; }

            public List<jsonorderdetail> detailItems;
        }

        class jsonorderdetail
        {
            public string ProductName { get; set; }
            public string ProducingArea { get; set; }
            public string Unit { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
