using Chaat.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chaat.Models.Users
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(64)]
        public string AspUserID { get; set; }

        [Required]
        [StringLength(32, ErrorMessage = "Imię musi składać się z 2 do 32 znaków.", MinimumLength = 2)]
        [Display(Name = "Imię*:")]
        public string Name { get; set; }

        [StringLength(32, ErrorMessage = "Nazwisko musi składać się z 2 do 32 znaków.", MinimumLength = 2)]
        [Display(Name = "Nazwisko:")]
        public string Surname { get; set; }

        [StringLength(32, ErrorMessage = "Miasto musi składać się z 2 do 32 znaków.", MinimumLength = 2)]
        [Display(Name = "Nazwisko:")]
        public string Town { get; set; }

        [StringLength(32, ErrorMessage = "Państwo musi składać się z 2 do 100 znaków.", MinimumLength = 2)]
        [Display(Name = "Państwo:")]
        public string Country { get; set; }

        [Display(Name = "Data urodzin:")]
        [DataType(DataType.Date, ErrorMessage = "Nie prawidłowy format rrrr-mm-dd.")]
        public string Birth { get; set; }

        public string PhotoLink { get; set; }


        public List<User> friends = new List<User>();
    }
}
