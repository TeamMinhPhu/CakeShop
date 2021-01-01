using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakeShopProject.Classes
{
	class CakeViewModel
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string CoverImage { get; set; }
		public int Price { get; set; }
	}


	class ReceiptViewModel
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string Note { get; set; }
		public int Payment { get; set; }
		public string PaymentType { get; set; }
		public string Status { get; set; }
	}
}
