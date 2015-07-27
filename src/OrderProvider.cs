﻿using LedaoOrder.Models;
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
        public static List<OrderItem> GetPagedOrders(OrderStatus status, int currentPageIndex, int countEachPage, out int totalCount)
        {
            List<jsonorder> jsonorders = GetResouces<List<jsonorder>>("http://yiimod.sinaapp.com/views/a/order.php?r=jsonOrdered&status="+(int)status);

            List<OrderItem> orders = new List<OrderItem>();
            totalCount = orders.Count;
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
            jsonorder jsonorder = GetResouces<jsonorder>("http://yiimod.sinaapp.com/views/a/order.php?r=jsonOrderDetail&oid="+orderID);

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
        public static bool SendOrder(int orderID)
        {
            return true;
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
