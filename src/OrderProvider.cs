using LedaoOrder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LedaoOrder
{
    public class OrderProvider
    {
        public static List<OrderItem> GetPagedOrders(OrderStatus status, int currentPageIndex, int countEachPage, out int totalCount)
        {
            totalCount = 0;
            return null;
        }

        public static bool SendOrder(int orderID)
        {
            return true;
        }
    }
}
