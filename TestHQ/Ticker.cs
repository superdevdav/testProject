using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHQ
{
    public class Ticker
    {
        /// <summary>
        /// Валютная пара
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Цена последнего наивысшего ордера на покупку
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// Сумма 25 наибольших ордеров на покупку
        /// </summary>
        public decimal BidSize { get; set; }

        /// <summary>
        /// Цена последнего наименьшего ордера на продажу
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// Сумма объемов 25 ордеров на продажу с наименьшей ценой
        /// </summary>
        public decimal AskSize { get; set; }

        /// <summary>
        /// Сумма, на которую изменилась последняя цена со вчерашнего дня
        /// </summary>
        public decimal DailyChange { get; set; }

        /// <summary>
        /// Относительное изменение цен со вчерашнего дня (*100 для изменения в процентах)
        /// </summary>
        public decimal DailyChangeRelative { get; set; }

        /// <summary>
        /// Цена последней сделки
        /// </summary>
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Дневной объем
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Дневной пик цены
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// Дневное дно цены
        /// </summary>
        public decimal Low { get; set; }
    }
}
