using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheGreatFinChallenge.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string ImageData { get; set; }

        public Image(int userId, string imageData)
        {
            UserId = userId;
            ImageData = imageData;
        }
    }
}
