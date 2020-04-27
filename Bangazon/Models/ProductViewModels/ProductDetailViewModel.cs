using Bangazon.Models;
using Bangazon.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductDetailViewModel
  {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
        public Product Product { get; set; }
  }
}