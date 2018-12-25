using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VinePicker.Models
{
    public class Vine
    {
        public string VineId { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public DateTime Created { get; set; }
        public string Permalink { get; set; }
        public string Loops { get; set; }
        public string Likes { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public int Submitter { get; set; }
    }
}
