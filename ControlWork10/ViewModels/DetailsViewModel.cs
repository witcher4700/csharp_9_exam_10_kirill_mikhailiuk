using ControlWork10.Entyties;
using System.Collections.Generic;

namespace ControlWork10.ViewModels
{
    public class DetailsViewModel
    {
        public Place Place { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Photo> Photos { get; set; }
    }
}
