using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VinePicker.Models;

namespace VinePicker.ViewModels
{
    public enum Position
    {
        Left, Right, None
    }
    public class VineViewModel
    {
        [Required]
        public Vine Vine { get; set; }
        public Position Position { get; set; } 
        public string Id { get; set; } // unique id to allow for multiple vine views in 1 page

        public VineViewModel()
        {
            Vine = null;
            Position = Position.None;
            Id = "0";
        }
    }
}
