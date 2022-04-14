﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } //primary key

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;


        [ForeignKey("CityId")]
        public City? City { get; set; } //relate point of interest with city

        public int CityId { get; set; }    //foreign key

        //public string? Description { get; set; }

        //constructor
        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
